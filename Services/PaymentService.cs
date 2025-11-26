using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace HealthApp.Services
{
    /// <summary>
    /// Service xử lý thanh toán qua MoMo và ZaloPay
    /// </summary>
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _momoPartnerCode;
        private readonly string _momoAccessKey;
        private readonly string _momoSecretKey;
        private readonly string _momoApiEndpoint;
        
        private readonly string _zaloPayAppId;
        private readonly string _zaloPayKey1;
        private readonly string _zaloPayKey2;
        private readonly string _zaloPayAppUser;
        private readonly string _zaloPayApiEndpoint;

        public PaymentService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            // Load MoMo config
            _momoPartnerCode = ConfigurationManager.AppSettings["MoMo_PartnerCode"] ?? "MOMO";
            _momoAccessKey = ConfigurationManager.AppSettings["MoMo_AccessKey"] ?? "";
            _momoSecretKey = ConfigurationManager.AppSettings["MoMo_SecretKey"] ?? "";
            _momoApiEndpoint = ConfigurationManager.AppSettings["MoMo_ApiEndpoint"] ?? "https://test-payment.momo.vn/v2/gateway/api/create";

            // Load ZaloPay config
            _zaloPayAppId = ConfigurationManager.AppSettings["ZaloPay_AppId"] ?? "";
            _zaloPayKey1 = ConfigurationManager.AppSettings["ZaloPay_Key1"] ?? "";
            _zaloPayKey2 = ConfigurationManager.AppSettings["ZaloPay_Key2"] ?? "";
            _zaloPayAppUser = ConfigurationManager.AppSettings["ZaloPay_AppUser"] ?? "HealthWeb";
            _zaloPayApiEndpoint = ConfigurationManager.AppSettings["ZaloPay_ApiEndpoint"] ?? "https://sb-openapi.zalopay.vn/v2/create";
        }

        /// <summary>
        /// Tạo thanh toán qua MoMo
        /// </summary>
        public async Task<PaymentResult> CreateMoMoPaymentAsync(string orderId, long amount, string orderInfo, string returnUrl, string notifyUrl)
        {
            try
            {
                var requestId = Guid.NewGuid().ToString();
                var extraData = "";

                // Tạo raw signature
                var rawHash = $"partnerCode={_momoPartnerCode}&accessKey={_momoAccessKey}&requestId={requestId}&amount={amount}&orderId={orderId}&orderInfo={orderInfo}&returnUrl={returnUrl}&notifyUrl={notifyUrl}&extraData={extraData}";
                var signature = ComputeHmacSha256(rawHash, _momoSecretKey);

                var requestBody = new
                {
                    partnerCode = _momoPartnerCode,
                    accessKey = _momoAccessKey,
                    requestId = requestId,
                    amount = amount,
                    orderId = orderId,
                    orderInfo = orderInfo,
                    returnUrl = returnUrl,
                    notifyUrl = notifyUrl,
                    extraData = extraData,
                    requestType = "captureWallet",
                    signature = signature
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_momoApiEndpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MoMoPaymentResponse>(responseContent);
                    if (result.errorCode == 0)
                    {
                        return new PaymentResult
                        {
                            Success = true,
                            PaymentUrl = result.payUrl,
                            QrCodeUrl = result.qrCodeUrl, // URL để hiển thị QR code
                            OrderId = orderId,
                            TransactionId = result.requestId,
                            Message = "Tạo thanh toán MoMo thành công"
                        };
                    }
                    else
                    {
                        return new PaymentResult
                        {
                            Success = false,
                            Message = $"Lỗi MoMo: {result.localMessage}"
                        };
                    }
                }
                else
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    Success = false,
                    Message = $"Lỗi khi tạo thanh toán MoMo: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Tạo thanh toán qua ZaloPay
        /// </summary>
        public async Task<PaymentResult> CreateZaloPayPaymentAsync(string orderId, long amount, string description, string callbackUrl)
        {
            try
            {
                // Kiểm tra config
                if (string.IsNullOrEmpty(_zaloPayAppId) || string.IsNullOrEmpty(_zaloPayKey1))
                {
                    System.Diagnostics.Debug.WriteLine("ZaloPay config is missing!");
                    return new PaymentResult
                    {
                        Success = false,
                        Message = "Cấu hình ZaloPay chưa đầy đủ. Vui lòng kiểm tra App.config."
                    };
                }

                // Format app_trans_id: yymmdd_HHmmss_xxx (tối đa 40 ký tự, phải unique)
                var dateStr = DateTime.Now.ToString("yyMMdd");
                var timeStr = DateTime.Now.ToString("HHmmss");
                var shortOrderId = orderId.Length > 20 ? orderId.Substring(0, 20) : orderId;
                // Format: yymmdd_HHmmss_xxx (tối đa 40 ký tự)
                var appTransId = $"{dateStr}_{timeStr}_{shortOrderId}";
                if (appTransId.Length > 40)
                {
                    appTransId = appTransId.Substring(0, 40);
                }

                // Giới hạn description (tối đa 255 ký tự)
                var limitedDescription = description.Length > 255 ? description.Substring(0, 255) : description;

                // Embed data và items - format đúng theo ZaloPay API
                var embedData = new Dictionary<string, object>();
                var items = new[]
                {
                    new
                    {
                        itemid = "PT_Session_001",
                        itemname = limitedDescription.Length > 100 ? limitedDescription.Substring(0, 100) : limitedDescription,
                        itemprice = (long)amount,
                        itemquantity = 1
                    }
                };

                // Unix timestamp milliseconds
                var appTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // Tạo param dictionary
                var embedDataJson = JsonConvert.SerializeObject(embedData);
                var itemsJson = JsonConvert.SerializeObject(items);

                var param = new Dictionary<string, object>
                {
                    { "app_id", int.Parse(_zaloPayAppId) }, // Convert to int
                    { "app_user", _zaloPayAppUser },
                    { "app_time", appTime },
                    { "amount", amount },
                    { "app_trans_id", appTransId },
                    { "embed_data", embedDataJson },
                    { "item", itemsJson },
                    { "description", limitedDescription },
                    { "bank_code", "zalopayapp" },
                    { "callback_url", callbackUrl }
                };

                // Tạo mac (theo thứ tự: app_id|app_trans_id|app_user|amount|app_time|embed_data|item)
                var data = $"{param["app_id"]}|{param["app_trans_id"]}|{param["app_user"]}|{param["amount"]}|{param["app_time"]}|{param["embed_data"]}|{param["item"]}";
                var mac = ComputeHmacSha256(data, _zaloPayKey1);

                param["mac"] = mac;

                // Debug logging
                System.Diagnostics.Debug.WriteLine("=== ZaloPay Payment Request ===");
                System.Diagnostics.Debug.WriteLine($"App ID: {_zaloPayAppId}");
                System.Diagnostics.Debug.WriteLine($"App User: {_zaloPayAppUser}");
                System.Diagnostics.Debug.WriteLine($"Amount: {amount} (Type: {amount.GetType().Name})");
                System.Diagnostics.Debug.WriteLine($"App Trans ID: {appTransId} (Length: {appTransId.Length})");
                System.Diagnostics.Debug.WriteLine($"Description: {limitedDescription} (Length: {limitedDescription.Length})");
                System.Diagnostics.Debug.WriteLine($"Embed Data: {embedDataJson}");
                System.Diagnostics.Debug.WriteLine($"Items: {itemsJson}");
                System.Diagnostics.Debug.WriteLine($"Data for MAC: {data}");
                System.Diagnostics.Debug.WriteLine($"MAC: {mac}");

                var json = JsonConvert.SerializeObject(param);
                System.Diagnostics.Debug.WriteLine($"Request JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"Sending request to: {_zaloPayApiEndpoint}");
                var response = await _httpClient.PostAsync(_zaloPayApiEndpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ZaloPayPaymentResponse>(responseContent);
                    
                    System.Diagnostics.Debug.WriteLine($"Return Code: {result?.return_code}");
                    System.Diagnostics.Debug.WriteLine($"Return Message: {result?.return_message}");
                    System.Diagnostics.Debug.WriteLine($"Order URL: {result?.order_url}");

                    if (result != null && result.return_code == 1)
                    {
                        return new PaymentResult
                        {
                            Success = true,
                            PaymentUrl = result.order_url,
                            OrderId = orderId,
                            TransactionId = result.app_trans_id,
                            Message = "Tạo thanh toán ZaloPay thành công"
                        };
                    }
                    else
                    {
                        var errorMsg = result?.return_message ?? "Không rõ lỗi";
                        var errorCode = result?.return_code ?? -1;
                        System.Diagnostics.Debug.WriteLine($"ZaloPay Error Code: {errorCode}");
                        System.Diagnostics.Debug.WriteLine($"ZaloPay Error Message: {errorMsg}");
                        
                        // Thông báo lỗi chi tiết hơn dựa trên error code
                        string detailedMessage = errorMsg;
                        if (errorCode == 2)
                        {
                            detailedMessage = $"Giao dịch thất bại. Có thể do:\n" +
                                            "- app_trans_id đã tồn tại hoặc format sai\n" +
                                            "- Thông tin thanh toán không hợp lệ\n" +
                                            "- Sandbox environment có giới hạn\n\n" +
                                            $"Chi tiết: {errorMsg}";
                        }
                        
                        return new PaymentResult
                        {
                            Success = false,
                            Message = $"Lỗi ZaloPay (Code: {errorCode}): {detailedMessage}"
                        };
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"HTTP Error: {response.StatusCode} - {responseContent}");
                    return new PaymentResult
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in CreateZaloPayPaymentAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return new PaymentResult
                {
                    Success = false,
                    Message = $"Lỗi khi tạo thanh toán ZaloPay: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Xác thực callback từ MoMo
        /// </summary>
        public bool VerifyMoMoCallback(string orderId, long amount, string signature)
        {
            try
            {
                var rawHash = $"partnerCode={_momoPartnerCode}&accessKey={_momoAccessKey}&requestId={orderId}&amount={amount}&orderId={orderId}&orderInfo=&orderType=momo_wallet&transId=&resultCode=0&message=&payType=&responseTime=";
                var computedSignature = ComputeHmacSha256(rawHash, _momoSecretKey);
                return computedSignature == signature;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Xác thực callback từ ZaloPay
        /// </summary>
        public bool VerifyZaloPayCallback(string data, string mac)
        {
            try
            {
                var computedMac = ComputeHmacSha256(data, _zaloPayKey2);
                return computedMac == mac;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tính HMAC SHA256
        /// </summary>
        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }

    /// <summary>
    /// Kết quả thanh toán
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string PaymentUrl { get; set; }
        public string QrCodeUrl { get; set; } // URL để hiển thị QR code
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Response từ MoMo API
    /// </summary>
    internal class MoMoPaymentResponse
    {
        public int errorCode { get; set; }
        public string message { get; set; }
        public string localMessage { get; set; }
        public string requestId { get; set; }
        public string payUrl { get; set; }
        public string deeplink { get; set; }
        public string qrCodeUrl { get; set; }
    }

    /// <summary>
    /// Response từ ZaloPay API
    /// </summary>
    internal class ZaloPayPaymentResponse
    {
        public int return_code { get; set; }
        public string return_message { get; set; }
        public string app_trans_id { get; set; }
        public string zp_trans_id { get; set; }
        public string order_url { get; set; }
        public string order_token { get; set; }
    }
}

