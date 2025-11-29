/*
    Script: 2025_11_27_AddMucTieuLuyenTapToDatLichPT.sql
    Purpose: Separate training goals from general notes on PT bookings.
        - Add column MucTieuLuyenTap to DatLichPT
        - Backfill existing data using GhiChu values
        - Keep GhiChu for free-form notes
*/

IF COL_LENGTH('dbo.DatLichPT', 'MucTieuLuyenTap') IS NULL
BEGIN
    PRINT 'Adding column MucTieuLuyenTap to DatLichPT...';
    ALTER TABLE dbo.DatLichPT
    ADD MucTieuLuyenTap NVARCHAR(200) NULL;
END
ELSE
BEGIN
    PRINT 'Column MucTieuLuyenTap already exists. Skipping add.';
END;

