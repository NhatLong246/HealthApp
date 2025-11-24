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
                var embedData = new Dictionary<string, object>();
                var items = new[] { new { } };
                var appTransId = DateTime.Now.ToString("yyMMdd") + "_" + orderId; // Format: yymmdd_xxx

                var param = new Dictionary<string, object>
                {
                    { "app_id", _zaloPayAppId },
                    { "app_user", _zaloPayAppUser },
                    { "app_time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                    { "amount", amount },
                    { "app_trans_id", appTransId },
                    { "embed_data", JsonConvert.SerializeObject(embedData) },
                    { "item", JsonConvert.SerializeObject(items) },
                    { "description", description },
                    { "bank_code", "zalopayapp" },
                    { "callback_url", callbackUrl }
                };

                // Tạo mac
                var data = $"{param["app_id"]}|{param["app_trans_id"]}|{param["app_user"]}|{param["amount"]}|{param["app_time"]}|{param["embed_data"]}|{param["item"]}";
                var mac = ComputeHmacSha256(data, _zaloPayKey1);

                param["mac"] = mac;

                var json = JsonConvert.SerializeObject(param);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_zaloPayApiEndpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ZaloPayPaymentResponse>(responseContent);
                    if (result.return_code == 1)
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
                        return new PaymentResult
                        {
                            Success = false,
                            Message = $"Lỗi ZaloPay: {result.return_message}"
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

