-- Script để sửa lỗi font trong cột ThuNgay của bảng BuoiTap
-- Vấn đề: Dữ liệu có thể bị lỗi font do không sử dụng Unicode prefix khi insert
-- Giải pháp: Cập nhật lại tất cả giá trị ThuNgay với format Unicode đúng

USE WF_HealthTracker;
GO

-- Kiểm tra dữ liệu hiện tại
SELECT 
    BuoiTapID,
    ThuNgay,
    LEN(ThuNgay) AS DoDai,
    DATALENGTH(ThuNgay) AS KichThuocBytes
FROM BuoiTap
WHERE ThuNgay IS NOT NULL
ORDER BY BuoiTapID;
GO

-- Sửa lỗi font: Cập nhật lại ThuNgay với giá trị Unicode đúng
-- Các giá trị có thể bị lỗi: "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật"

BEGIN TRANSACTION;

BEGIN TRY
    -- Sửa "Thứ 2" (có thể bị lỗi thành "Th? 2" hoặc các ký tự lạ)
    UPDATE BuoiTap
    SET ThuNgay = N'Thứ 2'
    WHERE ThuNgay IS NOT NULL
        AND (
            ThuNgay LIKE '%Thứ 2%' 
            OR ThuNgay LIKE '%thứ 2%'
            OR ThuNgay LIKE '%Th? 2%'
            OR ThuNgay LIKE '%Thứ hai%'
            OR ThuNgay LIKE '%Monday%'
            OR (LEN(ThuNgay) BETWEEN 4 AND 6 AND ThuNgay LIKE '%2%')
        )
        AND NOT ThuNgay = N'Thứ 2'; -- Chỉ update nếu chưa đúng

    -- Sửa "Thứ 3"
    UPDATE BuoiTap
    SET ThuNgay = N'Thứ 3'
    WHERE ThuNgay IS NOT NULL
        AND (
            ThuNgay LIKE '%Thứ 3%' 
            OR ThuNgay LIKE '%thứ 3%'
            OR ThuNgay LIKE '%Th? 3%'
            OR ThuNgay LIKE '%Thứ ba%'
            OR ThuNgay LIKE '%Tuesday%'
            OR (LEN(ThuNgay) BETWEEN 4 AND 6 AND ThuNgay LIKE '%3%')
        )
        AND NOT ThuNgay = N'Thứ 3';

    -- Sửa "Thứ 4"
    UPDATE BuoiTap
    SET ThuNgay = N'Thứ 4'
    WHERE ThuNgay IS NOT NULL
        AND (
            ThuNgay LIKE '%Thứ 4%' 
            OR ThuNgay LIKE '%thứ 4%'
            OR ThuNgay LIKE '%Th? 4%'
            OR ThuNgay LIKE '%Thứ tư%'
            OR ThuNgay LIKE '%Wednesday%'
            OR (LEN(ThuNgay) BETWEEN 4 AND 6 AND ThuNgay LIKE '%4%')
        )
        AND NOT ThuNgay = N'Thứ 4';

    -- Sửa "Thứ 5"
    UPDATE BuoiTap
    SET ThuNgay = N'Thứ 5'
    WHERE ThuNgay IS NOT NULL
        AND (
            ThuNgay LIKE '%Thứ 5%' 
            OR ThuNgay LIKE '%thứ 5%'
            OR ThuNgay LIKE '%Th? 5%'
            OR ThuNgay LIKE '%Thứ năm%'
            OR ThuNgay LIKE '%Thursday%'
            OR (LEN(ThuNgay) BETWEEN 4 AND 6 AND ThuNgay LIKE '%5%')
        )
        AND NOT ThuNgay = N'Thứ 5';

    -- Sửa "Thứ 6"
    UPDATE BuoiTap
    SET ThuNgay = N'Thứ 6'
    WHERE ThuNgay IS NOT NULL
        AND (
            ThuNgay LIKE '%Thứ 6%' 
            OR ThuNgay LIKE '%thứ 6%'
            OR ThuNgay LIKE '%Th? 6%'
            OR ThuNgay LIKE '%Thứ sáu%'
            OR ThuNgay LIKE '%Friday%'
            OR (LEN(ThuNgay) BETWEEN 4 AND 6 AND ThuNgay LIKE '%6%')
        )
        AND NOT ThuNgay = N'Thứ 6';

    -- Sửa "Thứ 7"
    UPDATE BuoiTap
    SET ThuNgay = N'Thứ 7'
    WHERE ThuNgay IS NOT NULL
        AND (
            ThuNgay LIKE '%Thứ 7%' 
            OR ThuNgay LIKE '%thứ 7%'
            OR ThuNgay LIKE '%Th? 7%'
            OR ThuNgay LIKE '%Thứ bảy%'
            OR ThuNgay LIKE '%Saturday%'
            OR (LEN(ThuNgay) BETWEEN 4 AND 6 AND ThuNgay LIKE '%7%')
        )
        AND NOT ThuNgay = N'Thứ 7';

    -- Sửa "Chủ nhật"
    UPDATE BuoiTap
    SET ThuNgay = N'Chủ nhật'
    WHERE ThuNgay IS NOT NULL
        AND (
            ThuNgay LIKE '%Chủ nhật%' 
            OR ThuNgay LIKE '%chủ nhật%'
            OR ThuNgay LIKE '%Ch? nh?t%'
            OR ThuNgay LIKE '%Chu nhat%'
            OR ThuNgay LIKE '%Sunday%'
            OR (LEN(ThuNgay) BETWEEN 8 AND 12 AND ThuNgay LIKE '%nhật%')
        )
        AND NOT ThuNgay = N'Chủ nhật';

    -- Kiểm tra kết quả
    SELECT 
        ThuNgay,
        COUNT(*) AS SoLuong
    FROM BuoiTap
    WHERE ThuNgay IS NOT NULL
    GROUP BY ThuNgay
    ORDER BY ThuNgay;

    -- Hiển thị các record còn có vấn đề (nếu có)
    SELECT 
        BuoiTapID,
        ThuNgay,
        KeHoachTapID
    FROM BuoiTap
    WHERE ThuNgay IS NOT NULL
        AND ThuNgay NOT IN (N'Thứ 2', N'Thứ 3', N'Thứ 4', N'Thứ 5', N'Thứ 6', N'Thứ 7', N'Chủ nhật');

    COMMIT TRANSACTION;
    PRINT 'Đã sửa thành công lỗi font trong cột ThuNgay!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Lỗi khi sửa dữ liệu: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
GO
