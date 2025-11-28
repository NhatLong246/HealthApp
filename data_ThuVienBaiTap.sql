
-- DỮ LIỆU MẪU CHO BẢNG ThuVienBaiTap
-- 10 loại mục tiêu: Cơ Ngực, Cơ Lưng, Cơ Vai, Cơ Tay, Cơ Bụng,
--                  Cơ Mông, Cơ Đùi, Cơ Cổ, Tăng Cân, Giảm Cân
-- Mỗi loại mục tiêu: 5 bài tập (tổng 50 bài tập)

USE WF_HealthTracker;
GO


INSERT INTO ThuVienBaiTap
    (BaiTapID, TenBaiTap, LoaiMucTieu, NhomCoChinhNhat, CapDo,
     DungCu, CaloriesMoiRep, ThoiLuongDeNghi, SoRep, SoSet, ThoiGianNghi)
VALUES
-- Cơ Ngực (5)
('exercise_lib_0001', N'Hít đất cơ bản',           N'Cơ Ngực', N'Ngực', 'Beginner',      N'Thảm tập',               0.8, 30,  N'10-15', N'3', 60),
('exercise_lib_0002', N'Chest Press với tạ đôi',    N'Cơ Ngực', N'Ngực', 'Intermediate',  N'Dumbbell, ghế tập',      1.2, 40,  N'8-12',  N'4', 90),
('exercise_lib_0003', N'Incline Dumbbell Press',    N'Cơ Ngực', N'Ngực', 'Intermediate',  N'Dumbbell, ghế dốc',      1.3, 40,  N'8-12',  N'4', 90),
('exercise_lib_0004', N'Cable Crossover',           N'Cơ Ngực', N'Ngực', 'Advanced',      N'Máy cable',              1.0, 45,  N'12-15', N'4', 75),
('exercise_lib_0005', N'Push-up hẹp tay',           N'Cơ Ngực', N'Ngực', 'Advanced',      N'Thảm tập',               0.9, 35,  N'15-20', N'4', 60),

-- Cơ Lưng (5)
('exercise_lib_0006', N'Lat Pulldown',              N'Cơ Lưng', N'Lưng', 'Beginner',      N'Máy kéo xô',             1.0, 40,  N'10-12', N'3', 75),
('exercise_lib_0007', N'Pull-up rộng tay',          N'Cơ Lưng', N'Lưng', 'Advanced',      N'Xà đơn',                 1.5, 30,  N'6-10',  N'4', 90),
('exercise_lib_0008', N'Bent-over Row với tạ đòn',  N'Cơ Lưng', N'Lưng', 'Intermediate',  N'Tạ đòn',                 1.4, 40,  N'8-12',  N'4', 90),
('exercise_lib_0009', N'Seated Cable Row',          N'Cơ Lưng', N'Lưng', 'Intermediate',  N'Máy kéo cáp',            1.1, 45,  N'10-12', N'4', 75),
('exercise_lib_0010', N'Deadlift cơ bản',           N'Cơ Lưng', N'Lưng', 'Advanced',      N'Tạ đòn',                 1.8, 45,  N'5-8',   N'4', 120),

-- Cơ Vai (5)
('exercise_lib_0011', N'Overhead Press với tạ đòn', N'Cơ Vai',  N'Vai',  'Intermediate',  N'Tạ đòn',                 1.3, 40,  N'8-10',  N'4', 90),
('exercise_lib_0012', N'Lateral Raise',             N'Cơ Vai',  N'Vai',  'Beginner',      N'Dumbbell',              0.7, 35,  N'12-15', N'3', 60),
('exercise_lib_0013', N'Front Raise',               N'Cơ Vai',  N'Vai',  'Beginner',      N'Dumbbell',              0.7, 35,  N'12-15', N'3', 60),
('exercise_lib_0014', N'Arnold Press',              N'Cơ Vai',  N'Vai',  'Intermediate',  N'Dumbbell',              1.1, 40,  N'8-12',  N'4', 75),
('exercise_lib_0015', N'Face Pull',                 N'Cơ Vai',  N'Vai',  'Intermediate',  N'Máy cable',             0.9, 40,  N'12-15', N'4', 60),

-- Cơ Tay (5)
('exercise_lib_0016', N'Bicep Curl với tạ đôi',     N'Cơ Tay',  N'Tay',  'Beginner',      N'Dumbbell',              0.8, 30,  N'10-15', N'3', 60),
('exercise_lib_0017', N'Hammer Curl',               N'Cơ Tay',  N'Tay',  'Intermediate',  N'Dumbbell',              0.9, 35,  N'10-12', N'4', 60),
('exercise_lib_0018', N'Tricep Pushdown',           N'Cơ Tay',  N'Tay',  'Intermediate',  N'Máy cable',             0.9, 35,  N'10-12', N'4', 60),
('exercise_lib_0019', N'Dips trên ghế',             N'Cơ Tay',  N'Tay',  'Intermediate',  N'Ghế tập',               1.0, 30,  N'8-12',  N'4', 75),
('exercise_lib_0020', N'Close-grip Bench Press',    N'Cơ Tay',  N'Tay',  'Advanced',      N'Tạ đòn, ghế phẳng',     1.4, 40,  N'6-10',  N'4', 90),

-- Cơ Bụng (5)
('exercise_lib_0021', N'Crunch cơ bản',             N'Cơ Bụng', N'Bụng', 'Beginner',      N'Thảm tập',               0.6, 30,  N'15-20', N'3', 45),
('exercise_lib_0022', N'Plank giữ thân người',      N'Cơ Bụng', N'Bụng', 'Beginner',      N'Thảm tập',               0.5, 45,  N'30-45s',N'3', 45),
('exercise_lib_0023', N'Leg Raise treo người',      N'Cơ Bụng', N'Bụng', 'Intermediate',  N'Xà đơn',                 0.9, 40,  N'10-15', N'4', 60),
('exercise_lib_0024', N'Russian Twist',             N'Cơ Bụng', N'Bụng', 'Intermediate',  N'Tạ tay hoặc không tạ',  0.8, 40,  N'20-30', N'4', 60),
('exercise_lib_0025', N'Ab Wheel Rollout',          N'Cơ Bụng', N'Bụng', 'Advanced',      N'Con lăn bụng',          1.1, 35,  N'8-12',  N'4', 75),

-- Cơ Mông (5)
('exercise_lib_0026', N'Glute Bridge',              N'Cơ Mông', N'Mông', 'Beginner',      N'Thảm tập',               0.7, 35,  N'12-15', N'3', 60),
('exercise_lib_0027', N'Hip Thrust với tạ đòn',     N'Cơ Mông', N'Mông', 'Intermediate',  N'Tạ đòn, ghế tập',        1.3, 45,  N'8-12',  N'4', 90),
('exercise_lib_0028', N'Lunge bước tới',            N'Cơ Mông', N'Mông', 'Intermediate',  N'Dumbbell',              1.0, 40,  N'10-12', N'4', 75),
('exercise_lib_0029', N'Bulgarian Split Squat',     N'Cơ Mông', N'Mông', 'Advanced',      N'Dumbbell, ghế tập',      1.4, 45,  N'8-10',  N'4', 90),
('exercise_lib_0030', N'Cable Kickback',            N'Cơ Mông', N'Mông', 'Intermediate',  N'Máy cable',             0.9, 40,  N'12-15', N'4', 60),

-- Cơ Đùi (5)
('exercise_lib_0031', N'Squat cơ bản',              N'Cơ Đùi',  N'Chân','Beginner',      N'Tạ đòn hoặc không tạ',  1.2, 40,  N'10-12', N'3', 90),
('exercise_lib_0032', N'Leg Press',                 N'Cơ Đùi',  N'Chân','Intermediate',  N'Máy leg press',          1.3, 40,  N'10-12', N'4', 90),
('exercise_lib_0033', N'Leg Extension',             N'Cơ Đùi',  N'Chân','Intermediate',  N'Máy duỗi chân',          0.8, 35,  N'12-15', N'4', 60),
('exercise_lib_0034', N'Romanian Deadlift',         N'Cơ Đùi',  N'Chân','Intermediate',  N'Tạ đòn hoặc dumbbell',  1.4, 45,  N'8-10',  N'4', 90),
('exercise_lib_0035', N'Walking Lunge',             N'Cơ Đùi',  N'Chân','Advanced',      N'Dumbbell',              1.3, 45,  N'10-12', N'4', 90),

-- Cơ Cổ (5)
('exercise_lib_0036', N'Neck Flexion với dây kháng lực', N'Cơ Cổ', N'Cổ','Beginner',     N'Dây kháng lực',          0.4, 25,  N'15-20', N'3', 45),
('exercise_lib_0037', N'Neck Extension trên ghế',   N'Cơ Cổ', N'Cổ','Beginner',         N'Tạ đĩa, ghế tập',        0.5, 25,  N'15-20', N'3', 45),
('exercise_lib_0038', N'Lateral Neck Raise',        N'Cơ Cổ', N'Cổ','Intermediate',     N'Dây kháng lực',          0.5, 30,  N'12-15', N'4', 45),
('exercise_lib_0039', N'Isometric Neck Hold',       N'Cơ Cổ', N'Cổ','Intermediate',     N'Không dụng cụ',          0.3, 30,  N'20-30s',N'4', 45),
('exercise_lib_0040', N'Neck Harness Lifting',      N'Cơ Cổ', N'Cổ','Advanced',         N'Neck harness, tạ đĩa',   0.7, 35,  N'12-15', N'4', 60),

-- Tăng Cân (5)
('exercise_lib_0041', N'Full-body Strength A',      N'Tăng cân', N'Ngực','Beginner',    N'Tạ đòn, dumbbell',       1.5, 45,  N'8-10',  N'3', 90),
('exercise_lib_0042', N'Full-body Strength B',      N'Tăng cân', N'Lưng','Intermediate',N'Tạ đòn, dumbbell',       1.6, 50,  N'6-10',  N'4', 90),
('exercise_lib_0043', N'Upper-body Mass',           N'Tăng cân', N'Vai', 'Intermediate',N'Tạ đòn, dumbbell',       1.4, 45,  N'8-12',  N'4', 90),
('exercise_lib_0044', N'Lower-body Mass',           N'Tăng cân', N'Chân','Intermediate',N'Tạ đòn, máy',            1.7, 50,  N'6-10',  N'4', 120),
('exercise_lib_0045', N'Push Pull Legs tăng cân',   N'Tăng cân', N'Ngực','Advanced',    N'Tạ đòn, dumbbell, máy',  1.8, 60,  N'6-10',  N'5', 120),

-- Giảm Cân (5)
('exercise_lib_0046', N'Circuit Cardio đơn giản',   N'Giảm cân', N'Chân','Beginner',    N'Thảm tập',               1.0, 600, N'3-4 vòng',N'1', 60),
('exercise_lib_0047', N'HIIT chạy bộ',              N'Giảm cân', N'Chân','Intermediate',N'Máy chạy bộ',            1.4, 900, N'20 phút',N'1', 120),
('exercise_lib_0048', N'Cardio đạp xe',             N'Giảm cân', N'Chân','Beginner',    N'Xe đạp hoặc máy',        1.1, 900, N'25-30p',N'1', 90),
('exercise_lib_0049', N'Full-body HIIT 30 phút',    N'Giảm cân', N'Ngực','Intermediate',N'Thảm tập, tạ nhẹ',       1.5, 1800,N'30 phút',N'1', 120),
('exercise_lib_0050', N'EMOM đốt mỡ toàn thân',     N'Giảm cân', N'Vai', 'Advanced',    N'Thảm tập, tạ tay',       1.6, 900, N'20 phút',N'1', 90);
GO

-- ============================================================
-- CẬP NHẬT DỮ LIỆU CHO CÁC TRƯỜNG: MoTa, HuongDan, LuuY, VideoHuongDan
-- ============================================================

-- Cơ Ngực (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập hít đất cơ bản giúp phát triển cơ ngực, vai và tay. Phù hợp cho người mới bắt đầu.',
    HuongDan = N'1. Nằm sấp, hai tay đặt rộng bằng vai\n2. Giữ lưng thẳng, siết cơ bụng\n3. Hạ người xuống từ từ cho đến khi ngực gần chạm sàn\n4. Đẩy người lên về vị trí ban đầu\n5. Lặp lại động tác',
    LuuY = N'- Giữ lưng thẳng trong suốt động tác\n- Không để hông chùng xuống\n- Hít vào khi hạ xuống, thở ra khi đẩy lên',
    VideoHuongDan = N'https://www.youtube.com/watch?v=IODxDxX7oi4'
WHERE BaiTapID = 'exercise_lib_0001';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập ép ngực với tạ đôi giúp phát triển cơ ngực, vai và tay sau. Tăng sức mạnh và độ dày cơ ngực.',
    HuongDan = N'1. Nằm trên ghế phẳng, hai tay cầm tạ đôi\n2. Đưa tạ lên cao, lòng bàn tay hướng về phía trước\n3. Hạ tạ từ từ xuống ngang ngực\n4. Đẩy tạ lên mạnh mẽ về vị trí ban đầu\n5. Lặp lại động tác',
    LuuY = N'- Giữ lưng áp sát ghế\n- Không khóa khớp khuỷu tay khi đẩy lên\n- Kiểm soát trọng lượng tạ phù hợp',
    VideoHuongDan = N'https://www.youtube.com/watch?v=VmB1G1K7v94'
WHERE BaiTapID = 'exercise_lib_0002';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập ép ngực trên ghế dốc nhắm vào phần trên cơ ngực. Tăng độ dày và hình dạng cơ ngực.',
    HuongDan = N'1. Điều chỉnh ghế dốc 30-45 độ\n2. Nằm trên ghế, hai tay cầm tạ đôi\n3. Đưa tạ lên cao, hơi nghiêng về phía trước\n4. Hạ tạ từ từ xuống ngang ngực trên\n5. Đẩy tạ lên mạnh mẽ',
    LuuY = N'- Không điều chỉnh ghế quá dốc (tối đa 45 độ)\n- Giữ vai ổn định, không nhún vai\n- Tập trung vào phần trên ngực',
    VideoHuongDan = N'https://www.youtube.com/watch?v=8iPEnovl8hU'
WHERE BaiTapID = 'exercise_lib_0003';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập kéo cáp chéo giúp phát triển và định hình cơ ngực. Tạo độ rộng và độ sâu cho cơ ngực.',
    HuongDan = N'1. Đứng giữa máy cable, hai tay nắm tay cầm\n2. Bước một chân về phía trước, hơi nghiêng người\n3. Kéo hai tay từ hai bên về phía trước, gần chạm nhau\n4. Giữ tư thế 1 giây, cảm nhận cơ ngực co lại\n5. Từ từ mở rộng tay về vị trí ban đầu',
    LuuY = N'- Giữ lưng thẳng, không cong lưng quá mức\n- Kiểm soát tốc độ, không đung đưa\n- Tập trung cảm nhận cơ ngực',
    VideoHuongDan = N'https://www.youtube.com/watch?v=taI4XduLpTk'
WHERE BaiTapID = 'exercise_lib_0004';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập hít đất hẹp tay tập trung vào cơ ngực trong và tay sau. Tăng độ dày và sức mạnh.',
    HuongDan = N'1. Nằm sấp, hai tay đặt hẹp hơn vai (khoảng cách bằng chiều rộng ngực)\n2. Giữ lưng thẳng, siết cơ bụng\n3. Hạ người xuống từ từ, khuỷu tay sát thân\n4. Đẩy người lên mạnh mẽ về vị trí ban đầu\n5. Lặp lại động tác',
    LuuY = N'- Giữ khuỷu tay sát thân người\n- Không để hông chùng xuống\n- Bài tập khó hơn hít đất thường, cần kiên trì',
    VideoHuongDan = N'https://www.youtube.com/watch?v=J0DnG1_S92I'
WHERE BaiTapID = 'exercise_lib_0005';

-- Cơ Lưng (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập kéo xô giúp phát triển cơ lưng rộng và cơ tay trước. Phù hợp cho người mới bắt đầu.',
    HuongDan = N'1. Ngồi trên máy, đặt đùi dưới đệm\n2. Nắm thanh kéo rộng hơn vai, lòng bàn tay hướng về phía trước\n3. Kéo thanh kéo xuống ngang ngực trên\n4. Giữ tư thế 1 giây, siết cơ lưng\n5. Từ từ thả thanh kéo lên vị trí ban đầu',
    LuuY = N'- Không đung đưa người khi kéo\n- Giữ lưng thẳng, không cong lưng\n- Tập trung cảm nhận cơ lưng',
    VideoHuongDan = N'https://www.youtube.com/watch?v=CAwf7n6Luuc'
WHERE BaiTapID = 'exercise_lib_0006';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập kéo xà rộng tay là bài tập nâng cao giúp phát triển cơ lưng rộng và sức mạnh toàn thân.',
    HuongDan = N'1. Nắm xà đơn rộng hơn vai, lòng bàn tay hướng về phía trước\n2. Treo người, hai chân không chạm đất\n3. Kéo người lên cho đến khi cằm vượt qua xà\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ hạ người xuống vị trí ban đầu',
    LuuY = N'- Không đung đưa người\n- Giữ lưng thẳng, không cong lưng\n- Nếu chưa đủ sức, có thể dùng dây hỗ trợ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=eGo4IYlbE5g'
WHERE BaiTapID = 'exercise_lib_0007';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập kéo tạ đòn cúi người giúp phát triển cơ lưng giữa và cơ tay trước. Tăng độ dày cơ lưng.',
    HuongDan = N'1. Đứng, hai chân rộng bằng vai, cầm tạ đòn\n2. Cúi người về phía trước, lưng thẳng, gối hơi cong\n3. Kéo tạ lên ngang bụng, khuỷu tay sát thân\n4. Siết cơ lưng ở vị trí cao nhất\n5. Từ từ hạ tạ xuống vị trí ban đầu',
    LuuY = N'- Giữ lưng thẳng trong suốt động tác\n- Không cong lưng khi kéo tạ\n- Kiểm soát trọng lượng tạ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=9efgcAjQe7E'
WHERE BaiTapID = 'exercise_lib_0008';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập kéo cáp ngồi giúp phát triển cơ lưng giữa và cơ tay trước. Tăng độ dày và sức mạnh cơ lưng.',
    HuongDan = N'1. Ngồi trên máy, hai chân đặt trên bàn đạp\n2. Nắm tay cầm, lưng thẳng\n3. Kéo tay cầm về phía bụng, khuỷu tay sát thân\n4. Siết cơ lưng ở vị trí gần bụng nhất\n5. Từ từ duỗi tay về vị trí ban đầu',
    LuuY = N'- Giữ lưng thẳng, không cong lưng\n- Không đung đưa người khi kéo\n- Tập trung cảm nhận cơ lưng',
    VideoHuongDan = N'https://www.youtube.com/watch?v=GZbfZ033f74'
WHERE BaiTapID = 'exercise_lib_0009';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập nâng tạ chết là bài tập tổng hợp phát triển cơ lưng, mông và đùi sau. Tăng sức mạnh toàn thân.',
    HuongDan = N'1. Đứng, hai chân rộng bằng vai, tạ đòn trước mặt\n2. Cúi xuống nắm tạ, lưng thẳng, gối hơi cong\n3. Đứng thẳng lên, nâng tạ lên cao\n4. Giữ tư thế 1 giây ở vị trí đứng thẳng\n5. Từ từ hạ tạ xuống vị trí ban đầu',
    LuuY = N'- QUAN TRỌNG: Giữ lưng thẳng trong suốt động tác\n- Không cong lưng khi nâng tạ\n- Bắt đầu với trọng lượng nhẹ để học kỹ thuật\n- Nếu có vấn đề về lưng, nên tránh bài tập này',
    VideoHuongDan = N'https://www.youtube.com/watch?v=op9kVnSso6Q'
WHERE BaiTapID = 'exercise_lib_0010';

-- Cơ Vai (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập đẩy tạ đòn qua đầu giúp phát triển cơ vai trước và giữa. Tăng sức mạnh và độ rộng vai.',
    HuongDan = N'1. Đứng, hai chân rộng bằng vai, cầm tạ đòn ngang vai\n2. Giữ lưng thẳng, siết cơ bụng\n3. Đẩy tạ lên cao qua đầu, không khóa khớp khuỷu tay\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ hạ tạ xuống ngang vai',
    LuuY = N'- Giữ lưng thẳng, không cong lưng quá mức\n- Không khóa khớp khuỷu tay khi đẩy lên\n- Kiểm soát trọng lượng tạ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=2yjwXTZQDDI'
WHERE BaiTapID = 'exercise_lib_0011';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập nâng tạ đôi sang hai bên giúp phát triển cơ vai giữa. Tăng độ rộng và hình dạng vai.',
    HuongDan = N'1. Đứng, hai tay cầm tạ đôi, để hai bên thân\n2. Giữ lưng thẳng, hơi nghiêng người về phía trước\n3. Nâng hai tay sang hai bên cho đến khi ngang vai\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ hạ tạ xuống vị trí ban đầu',
    LuuY = N'- Không nâng tạ quá cao (chỉ đến ngang vai)\n- Giữ khuỷu tay hơi cong, không duỗi thẳng\n- Kiểm soát tốc độ, không đung đưa',
    VideoHuongDan = N'https://www.youtube.com/watch?v=3VcKaXpzqRo'
WHERE BaiTapID = 'exercise_lib_0012';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập nâng tạ đôi ra phía trước giúp phát triển cơ vai trước. Tăng độ dày và sức mạnh vai.',
    HuongDan = N'1. Đứng, hai tay cầm tạ đôi, để trước đùi\n2. Giữ lưng thẳng, siết cơ bụng\n3. Nâng hai tay lên phía trước cho đến khi ngang vai\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ hạ tạ xuống vị trí ban đầu',
    LuuY = N'- Không nâng tạ quá cao (chỉ đến ngang vai)\n- Giữ khuỷu tay hơi cong\n- Kiểm soát tốc độ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=-t7fuZ0KhDA'
WHERE BaiTapID = 'exercise_lib_0013';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập Arnold Press là biến thể của đẩy tạ vai, kết hợp xoay tay để tác động toàn diện cơ vai.',
    HuongDan = N'1. Ngồi trên ghế, hai tay cầm tạ đôi, lòng bàn tay hướng về mình\n2. Bắt đầu ở vị trí ngang vai, khuỷu tay sát thân\n3. Xoay cổ tay và đẩy tạ lên cao, lòng bàn tay hướng về phía trước\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ xoay và hạ tạ về vị trí ban đầu',
    LuuY = N'- Kiểm soát tốc độ xoay và đẩy\n- Giữ lưng thẳng, không cong lưng\n- Tập trung cảm nhận cơ vai',
    VideoHuongDan = N'https://www.youtube.com/watch?v=6Z15_WdXmVw'
WHERE BaiTapID = 'exercise_lib_0014';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập kéo cáp về mặt giúp phát triển cơ vai sau và cơ lưng trên. Cải thiện tư thế và sức mạnh.',
    HuongDan = N'1. Đứng trước máy cable, nắm dây cáp bằng hai tay\n2. Kéo dây cáp về phía mặt, tách hai tay ra hai bên\n3. Kéo cho đến khi tay ngang mặt, khuỷu tay cao hơn vai\n4. Giữ tư thế 1 giây, siết cơ vai sau\n5. Từ từ thả dây cáp về vị trí ban đầu',
    LuuY = N'- Giữ lưng thẳng, không cong lưng\n- Tập trung vào cơ vai sau\n- Kiểm soát tốc độ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=rep-qVOkqgk'
WHERE BaiTapID = 'exercise_lib_0015';

-- Cơ Tay (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập cuốn tạ đôi giúp phát triển cơ tay trước (biceps). Tăng độ dày và sức mạnh cơ tay.',
    HuongDan = N'1. Đứng, hai tay cầm tạ đôi, để hai bên thân\n2. Giữ khuỷu tay cố định, chỉ di chuyển cẳng tay\n3. Cuốn tạ lên cho đến khi tạ gần vai\n4. Siết cơ tay trước ở vị trí cao nhất\n5. Từ từ hạ tạ xuống vị trí ban đầu',
    LuuY = N'- Giữ khuỷu tay cố định, không đung đưa\n- Không nâng vai khi cuốn tạ\n- Kiểm soát tốc độ, không đung đưa tạ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=ykJmrZ5v0Oo'
WHERE BaiTapID = 'exercise_lib_0016';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập Hammer Curl giúp phát triển cơ tay trước và cơ cẳng tay. Tăng độ dày và sức mạnh.',
    HuongDan = N'1. Đứng, hai tay cầm tạ đôi, lòng bàn tay hướng vào thân\n2. Giữ khuỷu tay cố định\n3. Cuốn tạ lên cho đến khi tạ gần vai\n4. Siết cơ tay ở vị trí cao nhất\n5. Từ từ hạ tạ xuống vị trí ban đầu',
    LuuY = N'- Giữ khuỷu tay cố định\n- Lòng bàn tay luôn hướng vào thân (khác với bicep curl thường)\n- Tập trung cảm nhận cơ tay',
    VideoHuongDan = N'https://www.youtube.com/watch?v=zC3OxLlJx4U'
WHERE BaiTapID = 'exercise_lib_0017';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập đẩy cáp xuống giúp phát triển cơ tay sau (triceps). Tăng độ dày và sức mạnh cơ tay sau.',
    HuongDan = N'1. Đứng trước máy cable, nắm tay cầm bằng hai tay\n2. Giữ khuỷu tay sát thân, cố định\n3. Đẩy tay cầm xuống cho đến khi tay duỗi thẳng\n4. Siết cơ tay sau ở vị trí duỗi thẳng\n5. Từ từ co tay về vị trí ban đầu',
    LuuY = N'- Giữ khuỷu tay cố định, không di chuyển\n- Không đung đưa người khi đẩy\n- Tập trung cảm nhận cơ tay sau',
    VideoHuongDan = N'https://www.youtube.com/watch?v=2-LAMcpzODU'
WHERE BaiTapID = 'exercise_lib_0018';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập dips trên ghế giúp phát triển cơ tay sau và vai. Tăng sức mạnh và độ dày cơ tay sau.',
    HuongDan = N'1. Ngồi trên mép ghế, hai tay đặt sau lưng, nắm mép ghế\n2. Duỗi chân ra phía trước, gót chân chạm đất\n3. Hạ người xuống bằng cách co khuỷu tay\n4. Đẩy người lên về vị trí ban đầu\n5. Lặp lại động tác',
    LuuY = N'- Giữ lưng thẳng, không cong lưng\n- Không để vai quá thấp khi hạ xuống\n- Có thể điều chỉnh độ khó bằng cách co chân',
    VideoHuongDan = N'https://www.youtube.com/watch?v=6kALZikXxLc'
WHERE BaiTapID = 'exercise_lib_0019';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập ép ngực hẹp tay tập trung vào cơ tay sau. Tăng sức mạnh và độ dày cơ tay sau.',
    HuongDan = N'1. Nằm trên ghế phẳng, hai tay cầm tạ đòn hẹp hơn vai\n2. Hạ tạ từ từ xuống ngang ngực, khuỷu tay sát thân\n3. Đẩy tạ lên mạnh mẽ về vị trí ban đầu\n4. Siết cơ tay sau khi đẩy lên\n5. Lặp lại động tác',
    LuuY = N'- Giữ khuỷu tay sát thân trong suốt động tác\n- Không khóa khớp khuỷu tay khi đẩy lên\n- Kiểm soát trọng lượng tạ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=nEF0bv2FW94'
WHERE BaiTapID = 'exercise_lib_0020';

-- Cơ Bụng (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập gập bụng cơ bản giúp phát triển cơ bụng trên. Phù hợp cho người mới bắt đầu.',
    HuongDan = N'1. Nằm ngửa trên thảm, hai chân co, bàn chân đặt trên sàn\n2. Đặt hai tay sau đầu hoặc trước ngực\n3. Nâng đầu và vai lên khỏi sàn, siết cơ bụng\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ hạ xuống vị trí ban đầu',
    LuuY = N'- Không kéo cổ bằng tay\n- Tập trung cảm nhận cơ bụng\n- Hít vào khi hạ xuống, thở ra khi nâng lên',
    VideoHuongDan = N'https://www.youtube.com/watch?v=MKmrqcoCZ-M'
WHERE BaiTapID = 'exercise_lib_0021';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập plank giữ thân người giúp phát triển cơ bụng, lưng và vai. Tăng sức mạnh core.',
    HuongDan = N'1. Nằm sấp, chống hai tay và mũi chân\n2. Giữ lưng thẳng, tạo thành đường thẳng từ đầu đến chân\n3. Siết cơ bụng và mông\n4. Giữ tư thế trong thời gian quy định\n5. Hạ người xuống khi hết thời gian',
    LuuY = N'- QUAN TRỌNG: Giữ lưng thẳng, không cong lưng hoặc hạ hông\n- Không nín thở, thở đều\n- Bắt đầu với thời gian ngắn, tăng dần',
    VideoHuongDan = N'https://www.youtube.com/watch?v=pSHjTRCQxIw'
WHERE BaiTapID = 'exercise_lib_0022';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập nâng chân treo người giúp phát triển cơ bụng dưới và cơ hông. Tăng sức mạnh core.',
    HuongDan = N'1. Treo người trên xà đơn, hai tay nắm xà rộng bằng vai\n2. Giữ lưng thẳng, siết cơ bụng\n3. Nâng hai chân lên cho đến khi đùi song song với sàn\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ hạ chân xuống vị trí ban đầu',
    LuuY = N'- Không đung đưa người khi nâng chân\n- Giữ lưng thẳng, không cong lưng\n- Nếu chưa đủ sức, có thể co gối khi nâng',
    VideoHuongDan = N'https://www.youtube.com/watch?v=H-Xp2n5V8vo'
WHERE BaiTapID = 'exercise_lib_0023';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập xoay người Nga giúp phát triển cơ bụng xiên và core. Tăng sức mạnh và độ ổn định.',
    HuongDan = N'1. Ngồi trên thảm, hai chân co, nâng chân lên khỏi sàn\n2. Cầm tạ tay hoặc không tạ, nghiêng người về phía sau\n3. Xoay thân người sang trái và phải\n4. Giữ cơ bụng siết trong suốt động tác\n5. Lặp lại động tác',
    LuuY = N'- Giữ lưng thẳng, không cong lưng\n- Kiểm soát tốc độ xoay\n- Tập trung cảm nhận cơ bụng xiên',
    VideoHuongDan = N'https://www.youtube.com/watch?v=wkD8rjkodUI'
WHERE BaiTapID = 'exercise_lib_0024';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập lăn bụng bằng con lăn giúp phát triển toàn bộ cơ bụng và core. Tăng sức mạnh và độ ổn định.',
    HuongDan = N'1. Quỳ trên thảm, hai tay cầm con lăn bụng\n2. Đẩy con lăn về phía trước, duỗi người ra\n3. Giữ lưng thẳng, siết cơ bụng\n4. Kéo con lăn về phía mình, co người lại\n5. Lặp lại động tác',
    LuuY = N'- QUAN TRỌNG: Giữ lưng thẳng, không cong lưng quá mức\n- Không để hông chùng xuống\n- Bắt đầu với khoảng cách ngắn, tăng dần',
    VideoHuongDan = N'https://www.youtube.com/watch?v=6Er6IBDp8hE'
WHERE BaiTapID = 'exercise_lib_0025';

-- Cơ Mông (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập cầu mông giúp phát triển cơ mông và cơ đùi sau. Phù hợp cho người mới bắt đầu.',
    HuongDan = N'1. Nằm ngửa trên thảm, hai chân co, bàn chân đặt trên sàn\n2. Đặt hai tay hai bên thân\n3. Nâng hông lên cao, siết cơ mông\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ hạ hông xuống vị trí ban đầu',
    LuuY = N'- Tập trung cảm nhận cơ mông\n- Không cong lưng quá mức\n- Hít vào khi hạ xuống, thở ra khi nâng lên',
    VideoHuongDan = N'https://www.youtube.com/watch?v=wPM8icPu6H8'
WHERE BaiTapID = 'exercise_lib_0026';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập đẩy hông với tạ đòn giúp phát triển cơ mông và cơ đùi sau. Tăng sức mạnh và độ dày cơ mông.',
    HuongDan = N'1. Ngồi trên sàn, lưng tựa vào ghế, tạ đòn đặt trên hông\n2. Hai chân co, bàn chân đặt trên sàn\n3. Đẩy hông lên cao, siết cơ mông\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ hạ hông xuống vị trí ban đầu',
    LuuY = N'- Giữ lưng thẳng, không cong lưng\n- Tập trung cảm nhận cơ mông\n- Kiểm soát trọng lượng tạ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=xDmFkJxPzeM'
WHERE BaiTapID = 'exercise_lib_0027';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập lunge bước tới giúp phát triển cơ mông, đùi trước và đùi sau. Tăng sức mạnh chân.',
    HuongDan = N'1. Đứng, hai tay cầm tạ đôi, hai bên thân\n2. Bước một chân về phía trước, hạ người xuống\n3. Gối trước tạo góc 90 độ, gối sau gần chạm sàn\n4. Đẩy chân trước lên, trở về vị trí ban đầu\n5. Lặp lại với chân kia',
    LuuY = N'- Giữ lưng thẳng, không nghiêng người về phía trước\n- Gối trước không vượt quá mũi chân\n- Kiểm soát tốc độ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=QOVaHwm-Q6U'
WHERE BaiTapID = 'exercise_lib_0028';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập squat một chân Bulgaria giúp phát triển cơ mông, đùi trước và đùi sau. Tăng sức mạnh và độ ổn định.',
    HuongDan = N'1. Đứng trước ghế, đặt một chân lên ghế phía sau\n2. Hai tay cầm tạ đôi, hai bên thân\n3. Hạ người xuống bằng chân trước, gối tạo góc 90 độ\n4. Đẩy chân trước lên, trở về vị trí ban đầu\n5. Lặp lại với chân kia',
    LuuY = N'- Giữ lưng thẳng, không nghiêng người\n- Gối trước không vượt quá mũi chân\n- Tập trung cảm nhận cơ mông và đùi',
    VideoHuongDan = N'https://www.youtube.com/watch?v=2C-uNgKwPLE'
WHERE BaiTapID = 'exercise_lib_0029';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập đá chân sau bằng cáp giúp phát triển cơ mông. Tăng độ dày và hình dạng cơ mông.',
    HuongDan = N'1. Đứng trước máy cable, nắm tay cầm để giữ thăng bằng\n2. Buộc dây cáp vào cổ chân\n3. Đá chân về phía sau, siết cơ mông\n4. Giữ tư thế 1 giây ở vị trí cao nhất\n5. Từ từ hạ chân về vị trí ban đầu',
    LuuY = N'- Giữ lưng thẳng, không cong lưng\n- Tập trung cảm nhận cơ mông\n- Kiểm soát tốc độ, không đung đưa',
    VideoHuongDan = N'https://www.youtube.com/watch?v=Gl3jJ3as5w8'
WHERE BaiTapID = 'exercise_lib_0030';

-- Cơ Đùi (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập squat cơ bản giúp phát triển cơ đùi trước, mông và đùi sau. Tăng sức mạnh chân.',
    HuongDan = N'1. Đứng, hai chân rộng bằng vai, hai tay cầm tạ đòn hoặc để trước ngực\n2. Hạ người xuống bằng cách co gối, đẩy hông về phía sau\n3. Hạ xuống cho đến khi đùi song song với sàn\n4. Đẩy người lên về vị trí ban đầu\n5. Lặp lại động tác',
    LuuY = N'- Giữ lưng thẳng, không cong lưng\n- Gối không vượt quá mũi chân\n- Kiểm soát tốc độ, không khóa gối khi đứng lên',
    VideoHuongDan = N'https://www.youtube.com/watch?v=Dy28eq2PjcM'
WHERE BaiTapID = 'exercise_lib_0031';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập đẩy chân trên máy giúp phát triển cơ đùi trước và mông. Tăng sức mạnh và độ dày chân.',
    HuongDan = N'1. Ngồi trên máy leg press, đặt hai chân lên bàn đạp\n2. Điều chỉnh ghế cho phù hợp\n3. Đẩy bàn đạp ra xa, duỗi chân\n4. Co chân lại, hạ bàn đạp về vị trí ban đầu\n5. Lặp lại động tác',
    LuuY = N'- Không khóa gối khi đẩy ra\n- Giữ lưng áp sát ghế\n- Kiểm soát trọng lượng tạ',
    VideoHuongDan = N'https://www.youtube.com/watch?v=IZxyjW8MPJY'
WHERE BaiTapID = 'exercise_lib_0032';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập duỗi chân trên máy giúp phát triển cơ đùi trước. Tăng độ dày và sức mạnh cơ đùi.',
    HuongDan = N'1. Ngồi trên máy, đặt hai chân dưới đệm\n2. Nắm tay cầm hai bên ghế\n3. Duỗi chân ra, nâng đệm lên\n4. Siết cơ đùi trước ở vị trí duỗi thẳng\n5. Từ từ co chân, hạ đệm về vị trí ban đầu',
    LuuY = N'- Không khóa gối khi duỗi ra\n- Giữ lưng áp sát ghế\n- Tập trung cảm nhận cơ đùi trước',
    VideoHuongDan = N'https://www.youtube.com/watch?v=YyvSfVjQeL0'
WHERE BaiTapID = 'exercise_lib_0033';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập nâng tạ chết Romania giúp phát triển cơ đùi sau và mông. Tăng sức mạnh và độ dày.',
    HuongDan = N'1. Đứng, hai chân rộng bằng vai, cầm tạ đòn hoặc tạ đôi\n2. Cúi người về phía trước, lưng thẳng, gối hơi cong\n3. Hạ tạ xuống dọc theo chân, cảm nhận cơ đùi sau căng\n4. Đứng thẳng lên, trở về vị trí ban đầu\n5. Lặp lại động tác',
    LuuY = N'- QUAN TRỌNG: Giữ lưng thẳng trong suốt động tác\n- Không cong lưng khi hạ tạ\n- Tập trung cảm nhận cơ đùi sau',
    VideoHuongDan = N'https://www.youtube.com/watch?v=_oyxCn2iSjU'
WHERE BaiTapID = 'exercise_lib_0034';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập lunge đi bộ giúp phát triển cơ đùi trước, đùi sau và mông. Tăng sức mạnh và độ ổn định.',
    HuongDan = N'1. Đứng, hai tay cầm tạ đôi, hai bên thân\n2. Bước một chân về phía trước, hạ người xuống\n3. Gối trước tạo góc 90 độ, gối sau gần chạm sàn\n4. Đẩy chân trước lên, bước chân sau lên phía trước\n5. Lặp lại động tác, đi bộ về phía trước',
    LuuY = N'- Giữ lưng thẳng, không nghiêng người\n- Gối trước không vượt quá mũi chân\n- Kiểm soát tốc độ, giữ thăng bằng',
    VideoHuongDan = N'https://www.youtube.com/watch?v=L8fvypPrzzs'
WHERE BaiTapID = 'exercise_lib_0035';

-- Cơ Cổ (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập gập cổ với dây kháng lực giúp phát triển cơ cổ trước. Tăng sức mạnh và độ ổn định cổ.',
    HuongDan = N'1. Ngồi hoặc đứng, đặt dây kháng lực sau đầu\n2. Giữ lưng thẳng, siết cơ cổ\n3. Gập cổ về phía trước, chống lại lực căng của dây\n4. Giữ tư thế 1 giây ở vị trí gập\n5. Từ từ thả cổ về vị trí ban đầu',
    LuuY = N'- QUAN TRỌNG: Thực hiện chậm và kiểm soát\n- Không gập cổ quá mức\n- Nếu có vấn đề về cổ, nên tránh bài tập này',
    VideoHuongDan = N'https://www.youtube.com/watch?v=z1q3h5tH5o8'
WHERE BaiTapID = 'exercise_lib_0036';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập duỗi cổ trên ghế giúp phát triển cơ cổ sau. Tăng sức mạnh và độ ổn định cổ.',
    HuongDan = N'1. Ngồi trên ghế, đặt tạ đĩa nhẹ lên đầu (có thể dùng khăn lót)\n2. Giữ lưng thẳng, siết cơ cổ\n3. Duỗi cổ về phía sau, chống lại trọng lượng tạ\n4. Giữ tư thế 1 giây ở vị trí duỗi\n5. Từ từ thả cổ về vị trí ban đầu',
    LuuY = N'- QUAN TRỌNG: Bắt đầu với trọng lượng rất nhẹ\n- Thực hiện chậm và kiểm soát\n- Nếu có vấn đề về cổ, nên tránh bài tập này',
    VideoHuongDan = N'https://www.youtube.com/watch?v=KZprhFdD5vU'
WHERE BaiTapID = 'exercise_lib_0037';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập nâng cổ sang bên giúp phát triển cơ cổ bên. Tăng sức mạnh và độ ổn định cổ.',
    HuongDan = N'1. Ngồi hoặc đứng, đặt dây kháng lực bên đầu\n2. Giữ lưng thẳng, siết cơ cổ\n3. Nghiêng cổ sang bên, chống lại lực căng của dây\n4. Giữ tư thế 1 giây ở vị trí nghiêng\n5. Từ từ thả cổ về vị trí ban đầu',
    LuuY = N'- Thực hiện chậm và kiểm soát\n- Không nghiêng cổ quá mức\n- Tập cả hai bên',
    VideoHuongDan = N'https://www.youtube.com/watch?v=z1q3h5tH5o8'
WHERE BaiTapID = 'exercise_lib_0038';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập giữ cổ tĩnh giúp phát triển toàn bộ cơ cổ. Tăng sức mạnh và độ ổn định.',
    HuongDan = N'1. Ngồi hoặc đứng, giữ lưng thẳng\n2. Đặt tay lên đầu, tạo lực cản nhẹ\n3. Giữ cổ ở vị trí trung tâm, chống lại lực cản\n4. Siết cơ cổ, giữ tư thế trong thời gian quy định\n5. Thả tay, thư giãn',
    LuuY = N'- QUAN TRỌNG: Không tạo lực cản quá mạnh\n- Thực hiện chậm và kiểm soát\n- Nếu có vấn đề về cổ, nên tránh bài tập này',
    VideoHuongDan = N'https://www.youtube.com/watch?v=KZprhFdD5vU'
WHERE BaiTapID = 'exercise_lib_0039';

UPDATE ThuVienBaiTap SET
    MoTa = N'Bài tập nâng cổ với neck harness giúp phát triển toàn bộ cơ cổ. Tăng sức mạnh và độ dày.',
    HuongDan = N'1. Đeo neck harness, đặt tạ đĩa nhẹ\n2. Đứng hoặc ngồi, giữ lưng thẳng\n3. Gập và duỗi cổ, chống lại trọng lượng tạ\n4. Thực hiện động tác chậm và kiểm soát\n5. Lặp lại động tác',
    LuuY = N'- QUAN TRỌNG: Bắt đầu với trọng lượng rất nhẹ\n- Thực hiện chậm và kiểm soát\n- Nếu có vấn đề về cổ, nên tránh bài tập này\n- Chỉ dành cho người có kinh nghiệm',
    VideoHuongDan = N'https://www.youtube.com/watch?v=KZprhFdD5vU'
WHERE BaiTapID = 'exercise_lib_0040';

-- Tăng Cân (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình tập sức mạnh toàn thân A giúp tăng cân và phát triển cơ bắp. Phù hợp cho người mới bắt đầu.',
    HuongDan = N'1. Squat: 3 sets x 8-10 reps\n2. Bench Press: 3 sets x 8-10 reps\n3. Bent-over Row: 3 sets x 8-10 reps\n4. Overhead Press: 3 sets x 8-10 reps\n5. Nghỉ 90 giây giữa các sets',
    LuuY = N'- Tập trung vào kỹ thuật trước khi tăng trọng lượng\n- Ăn đủ calo và protein để hỗ trợ tăng cân\n- Nghỉ đủ giữa các buổi tập',
    VideoHuongDan = N'https://www.youtube.com/watch?v=3A4y8w0d2BU'
WHERE BaiTapID = 'exercise_lib_0041';

UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình tập sức mạnh toàn thân B giúp tăng cân và phát triển cơ bắp. Nâng cao hơn chương trình A.',
    HuongDan = N'1. Deadlift: 4 sets x 6-8 reps\n2. Incline Bench Press: 4 sets x 8-10 reps\n3. Pull-up: 4 sets x 6-10 reps\n4. Barbell Curl: 4 sets x 8-10 reps\n5. Nghỉ 90-120 giây giữa các sets',
    LuuY = N'- Tập trung vào kỹ thuật và an toàn\n- Ăn đủ calo và protein\n- Nghỉ đủ giữa các buổi tập',
    VideoHuongDan = N'https://www.youtube.com/watch?v=3A4y8w0d2BU'
WHERE BaiTapID = 'exercise_lib_0042';

UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình tập tăng khối lượng phần trên giúp phát triển cơ ngực, lưng, vai và tay.',
    HuongDan = N'1. Bench Press: 4 sets x 8-12 reps\n2. Pull-up: 4 sets x 8-12 reps\n3. Overhead Press: 4 sets x 8-12 reps\n4. Barbell Row: 4 sets x 8-12 reps\n5. Nghỉ 90 giây giữa các sets',
    LuuY = N'- Tập trung vào kỹ thuật\n- Ăn đủ calo và protein\n- Nghỉ đủ giữa các buổi tập',
    VideoHuongDan = N'https://www.youtube.com/watch?v=3A4y8w0d2BU'
WHERE BaiTapID = 'exercise_lib_0043';

UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình tập tăng khối lượng phần dưới giúp phát triển cơ đùi, mông và bắp chân.',
    HuongDan = N'1. Squat: 4 sets x 6-10 reps\n2. Deadlift: 4 sets x 6-10 reps\n3. Leg Press: 4 sets x 10-12 reps\n4. Calf Raise: 4 sets x 12-15 reps\n5. Nghỉ 120 giây giữa các sets',
    LuuY = N'- Tập trung vào kỹ thuật, đặc biệt là Squat và Deadlift\n- Ăn đủ calo và protein\n- Nghỉ đủ giữa các buổi tập',
    VideoHuongDan = N'https://www.youtube.com/watch?v=3A4y8w0d2BU'
WHERE BaiTapID = 'exercise_lib_0044';

UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình Push Pull Legs nâng cao giúp tăng cân và phát triển toàn bộ cơ thể. Dành cho người có kinh nghiệm.',
    HuongDan = N'Ngày 1 - Push (Đẩy): Bench Press, Overhead Press, Tricep Extension\nNgày 2 - Pull (Kéo): Pull-up, Barbell Row, Bicep Curl\nNgày 3 - Legs (Chân): Squat, Deadlift, Leg Press\nNghỉ 1 ngày, lặp lại chu kỳ',
    LuuY = N'- Chương trình nâng cao, cần có kinh nghiệm\n- Ăn đủ calo và protein\n- Nghỉ đủ giữa các buổi tập\n- Tập trung vào kỹ thuật và an toàn',
    VideoHuongDan = N'https://www.youtube.com/watch?v=3A4y8w0d2BU'
WHERE BaiTapID = 'exercise_lib_0045';

-- Giảm Cân (5)
UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình cardio vòng tròn đơn giản giúp đốt mỡ và tăng cường sức khỏe tim mạch. Phù hợp cho người mới bắt đầu.',
    HuongDan = N'1. Jumping Jacks: 30 giây\n2. High Knees: 30 giây\n3. Mountain Climbers: 30 giây\n4. Burpees: 30 giây\n5. Nghỉ 30 giây, lặp lại 3-4 vòng',
    LuuY = N'- Thực hiện với tốc độ vừa phải, không quá nhanh\n- Nghỉ khi cần thiết\n- Uống đủ nước\n- Khởi động trước khi tập',
    VideoHuongDan = N'https://www.youtube.com/watch?v=ml6cT4AZdqI'
WHERE BaiTapID = 'exercise_lib_0046';

UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình HIIT chạy bộ giúp đốt mỡ hiệu quả và tăng cường sức khỏe tim mạch.',
    HuongDan = N'1. Khởi động: Chạy nhẹ 5 phút\n2. Chạy nhanh: 30 giây\n3. Chạy chậm/đi bộ: 60 giây\n4. Lặp lại 10-15 lần\n5. Giãn cơ: 5 phút',
    LuuY = N'- Điều chỉnh tốc độ phù hợp với thể lực\n- Nghỉ khi cần thiết\n- Uống đủ nước\n- Khởi động kỹ trước khi tập',
    VideoHuongDan = N'https://www.youtube.com/watch?v=400B5L84y6c'
WHERE BaiTapID = 'exercise_lib_0047';

UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình cardio đạp xe giúp đốt mỡ và tăng cường sức khỏe tim mạch. Phù hợp cho người mới bắt đầu.',
    HuongDan = N'1. Khởi động: Đạp nhẹ 5 phút\n2. Đạp với tốc độ vừa phải: 20-25 phút\n3. Tăng tốc độ: 5 phút cuối\n4. Giảm tốc độ: 5 phút cuối\n5. Giãn cơ: 5 phút',
    LuuY = N'- Điều chỉnh độ kháng lực phù hợp\n- Giữ tư thế đúng\n- Uống đủ nước\n- Khởi động kỹ trước khi tập',
    VideoHuongDan = N'https://www.youtube.com/watch?v=Gc4a2wC6jLs'
WHERE BaiTapID = 'exercise_lib_0048';

UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình HIIT toàn thân 30 phút giúp đốt mỡ hiệu quả và tăng cường sức mạnh.',
    HuongDan = N'1. Khởi động: 5 phút\n2. Burpees: 45 giây, nghỉ 15 giây\n3. Mountain Climbers: 45 giây, nghỉ 15 giây\n4. Jump Squats: 45 giây, nghỉ 15 giây\n5. Plank: 45 giây, nghỉ 15 giây\n6. Lặp lại 4-5 vòng\n7. Giãn cơ: 5 phút',
    LuuY = N'- Thực hiện với tốc độ vừa phải\n- Nghỉ khi cần thiết\n- Uống đủ nước\n- Khởi động kỹ trước khi tập',
    VideoHuongDan = N'https://www.youtube.com/watch?v=ml6cT4AZdqI'
WHERE BaiTapID = 'exercise_lib_0049';

UPDATE ThuVienBaiTap SET
    MoTa = N'Chương trình EMOM đốt mỡ toàn thân giúp đốt mỡ hiệu quả trong thời gian ngắn. Dành cho người có kinh nghiệm.',
    HuongDan = N'EMOM (Every Minute On the Minute): Mỗi phút thực hiện một bài tập\n1. Phút 1: Burpees (10 reps)\n2. Phút 2: Jump Squats (15 reps)\n3. Phút 3: Mountain Climbers (20 reps)\n4. Phút 4: Plank (30 giây)\n5. Lặp lại trong 20 phút',
    LuuY = N'- Chương trình nâng cao, cần có kinh nghiệm\n- Điều chỉnh số reps phù hợp với thể lực\n- Nghỉ khi cần thiết\n- Uống đủ nước',
    VideoHuongDan = N'https://www.youtube.com/watch?v=ml6cT4AZdqI'
WHERE BaiTapID = 'exercise_lib_0050';

GO

select * from ThuVienBaiTap