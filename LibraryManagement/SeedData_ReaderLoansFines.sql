-- =============================================================
-- THU VIEN QUAN LY - DU LIEU MAU DOC GIA, MUON SACH, DAT PHONG, PHAT
-- Ngay tao: 2026-07-16
-- Chay toan bo script nay trong 1 lan
-- =============================================================
SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    -- =============================================================
    -- 1. TAO TAI KHOAN DOC GIA (Readers & UserProfiles)
    -- Yeu cau:
    -- - 5 doc gia chi muon sach (R1 - R5)
    -- - 5 doc gia vua muon sach vua dat phong (R6 - R10)
    -- - 1 doc gia sap den han tra (he181997phamtheson@gmail.com) - da ton tai, se update neu can
    -- - 1 doc gia bi phat (oniichanbaka204@gmail.com) - co the nam trong nhom tren
    -- =============================================================
    
    DECLARE @r1 UNIQUEIDENTIFIER = NEWID(), @r2 UNIQUEIDENTIFIER = NEWID(), @r3 UNIQUEIDENTIFIER = NEWID(), @r4 UNIQUEIDENTIFIER = NEWID(), @r5 UNIQUEIDENTIFIER = NEWID();
    DECLARE @r6 UNIQUEIDENTIFIER = NEWID(), @r7 UNIQUEIDENTIFIER = NEWID(), @r8 UNIQUEIDENTIFIER = NEWID(), @r9 UNIQUEIDENTIFIER = NEWID(), @r10 UNIQUEIDENTIFIER = NEWID();
    
    -- Kiem tra he181997phamtheson@gmail.com, neu chua co thi tao
    DECLARE @rHe181997 UNIQUEIDENTIFIER;
    SELECT @rHe181997 = ReaderId FROM Readers WHERE Email = 'he181997phamtheson@gmail.com';
    IF @rHe181997 IS NULL
    BEGIN
        SET @rHe181997 = NEWID();
        INSERT INTO Readers (ReaderId, Email, PasswordHash, Status, CreatedAt)
        VALUES (@rHe181997, 'he181997phamtheson@gmail.com', 'hashed_pwd_123', 'Active', GETDATE());
        
        INSERT INTO UserProfiles (UserProfileId, ReaderId, FullName, Phone, Address, DateOfBirth)
        VALUES (NEWID(), @rHe181997, N'Doc gia he181997', '0900000001', N'Ha Noi', '2000-01-01');
    END
    
    -- Kiem tra oniichanbaka204@gmail.com, neu chua co thi tao
    DECLARE @rOniichan UNIQUEIDENTIFIER;
    SELECT @rOniichan = ReaderId FROM Readers WHERE Email = 'oniichanbaka204@gmail.com';
    IF @rOniichan IS NULL
    BEGIN
        SET @rOniichan = NEWID();
        INSERT INTO Readers (ReaderId, Email, PasswordHash, Status, CreatedAt)
        VALUES (@rOniichan, 'oniichanbaka204@gmail.com', 'hashed_pwd_123', 'Active', GETDATE());
        
        INSERT INTO UserProfiles (UserProfileId, ReaderId, FullName, Phone, Address, DateOfBirth)
        VALUES (NEWID(), @rOniichan, N'Doc gia Bi Phat', '0900000002', N'HCM', '2000-02-02');
    END

    -- Tao 10 doc gia moi (R1-R10)
    INSERT INTO Readers (ReaderId, Email, PasswordHash, Status, CreatedAt) VALUES
    (@r1, 'reader1@test.com', 'hash', 'Active', GETDATE()),
    (@r2, 'reader2@test.com', 'hash', 'Active', GETDATE()),
    (@r3, 'reader3@test.com', 'hash', 'Active', GETDATE()),
    (@r4, 'reader4@test.com', 'hash', 'Active', GETDATE()),
    (@r5, 'reader5@test.com', 'hash', 'Active', GETDATE()),
    (@r6, 'reader6@test.com', 'hash', 'Active', GETDATE()),
    (@r7, 'reader7@test.com', 'hash', 'Active', GETDATE()),
    (@r8, 'reader8@test.com', 'hash', 'Active', GETDATE()),
    (@r9, 'reader9@test.com', 'hash', 'Active', GETDATE()),
    (@r10, 'reader10@test.com', 'hash', 'Active', GETDATE());

    INSERT INTO UserProfiles (UserProfileId, ReaderId, FullName) VALUES
    (NEWID(), @r1, N'Doc gia 1'), (NEWID(), @r2, N'Doc gia 2'),
    (NEWID(), @r3, N'Doc gia 3'), (NEWID(), @r4, N'Doc gia 4'),
    (NEWID(), @r5, N'Doc gia 5'), (NEWID(), @r6, N'Doc gia 6'),
    (NEWID(), @r7, N'Doc gia 7'), (NEWID(), @r8, N'Doc gia 8'),
    (NEWID(), @r9, N'Doc gia 9'), (NEWID(), @r10, N'Doc gia 10');

    -- Lay mot tai khoan admin/librarian de duyet phieu
    DECLARE @libAccountId UNIQUEIDENTIFIER = (SELECT TOP 1 AccountId FROM Accounts WHERE Role IN ('Admin', 'Librarian'));

    -- Lay cac BookCopy san sang cho muon
    DECLARE @bc1 UNIQUEIDENTIFIER, @bc2 UNIQUEIDENTIFIER, @bc3 UNIQUEIDENTIFIER, @bc4 UNIQUEIDENTIFIER, @bc5 UNIQUEIDENTIFIER;
    DECLARE @bc6 UNIQUEIDENTIFIER, @bc7 UNIQUEIDENTIFIER, @bc8 UNIQUEIDENTIFIER, @bc9 UNIQUEIDENTIFIER, @bc10 UNIQUEIDENTIFIER;
    DECLARE @bcOnii1 UNIQUEIDENTIFIER, @bcOnii2 UNIQUEIDENTIFIER, @bcOnii3 UNIQUEIDENTIFIER, @bcOnii4 UNIQUEIDENTIFIER, @bcOnii5 UNIQUEIDENTIFIER;
    DECLARE @bcHe UNIQUEIDENTIFIER;
    
    -- Select random 16 book copies that are available
    SELECT TOP 16 CopyId INTO #TempCopies FROM BookCopies WHERE Status = 'Available' ORDER BY NEWID();
    
    DECLARE cur_copies CURSOR FOR SELECT CopyId FROM #TempCopies;
    OPEN cur_copies;
    FETCH NEXT FROM cur_copies INTO @bc1; FETCH NEXT FROM cur_copies INTO @bc2;
    FETCH NEXT FROM cur_copies INTO @bc3; FETCH NEXT FROM cur_copies INTO @bc4;
    FETCH NEXT FROM cur_copies INTO @bc5; FETCH NEXT FROM cur_copies INTO @bc6;
    FETCH NEXT FROM cur_copies INTO @bc7; FETCH NEXT FROM cur_copies INTO @bc8;
    FETCH NEXT FROM cur_copies INTO @bc9; FETCH NEXT FROM cur_copies INTO @bc10;
    FETCH NEXT FROM cur_copies INTO @bcOnii1; FETCH NEXT FROM cur_copies INTO @bcOnii2;
    FETCH NEXT FROM cur_copies INTO @bcOnii3; FETCH NEXT FROM cur_copies INTO @bcOnii4;
    FETCH NEXT FROM cur_copies INTO @bcOnii5; FETCH NEXT FROM cur_copies INTO @bcHe;
    CLOSE cur_copies; DEALLOCATE cur_copies;
    DROP TABLE #TempCopies;

    -- =============================================================
    -- 2. TAO PHIEU MUON SACH (Loans & LoanDetails)
    -- =============================================================
    
    -- Helper function/logic de insert Loan va LoanDetail
    -- Doc gia 1-5 (chi muon)
    DECLARE @l1 UNIQUEIDENTIFIER = NEWID(), @l2 UNIQUEIDENTIFIER = NEWID(), @l3 UNIQUEIDENTIFIER = NEWID(), @l4 UNIQUEIDENTIFIER = NEWID(), @l5 UNIQUEIDENTIFIER = NEWID();
    INSERT INTO Loans (LoanId, BorrowerReaderId, ProcessedByAccountId, BorrowedAt, DueAt, Status, CreatedAt) VALUES
    (@l1, @r1, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE()),
    (@l2, @r2, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE()),
    (@l3, @r3, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE()),
    (@l4, @r4, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE()),
    (@l5, @r5, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE());

    INSERT INTO LoanDetails (LoanDetailId, LoanId, CopyId, Status) VALUES
    (NEWID(), @l1, @bc1, 'Borrowed'), (NEWID(), @l2, @bc2, 'Borrowed'),
    (NEWID(), @l3, @bc3, 'Borrowed'), (NEWID(), @l4, @bc4, 'Borrowed'),
    (NEWID(), @l5, @bc5, 'Borrowed');

    -- Doc gia 6-10 (muon + dat phong)
    DECLARE @l6 UNIQUEIDENTIFIER = NEWID(), @l7 UNIQUEIDENTIFIER = NEWID(), @l8 UNIQUEIDENTIFIER = NEWID(), @l9 UNIQUEIDENTIFIER = NEWID(), @l10 UNIQUEIDENTIFIER = NEWID();
    INSERT INTO Loans (LoanId, BorrowerReaderId, ProcessedByAccountId, BorrowedAt, DueAt, Status, CreatedAt) VALUES
    (@l6, @r6, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE()),
    (@l7, @r7, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE()),
    (@l8, @r8, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE()),
    (@l9, @r9, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE()),
    (@l10, @r10, @libAccountId, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed', GETDATE());

    INSERT INTO LoanDetails (LoanDetailId, LoanId, CopyId, Status) VALUES
    (NEWID(), @l6, @bc6, 'Borrowed'), (NEWID(), @l7, @bc7, 'Borrowed'),
    (NEWID(), @l8, @bc8, 'Borrowed'), (NEWID(), @l9, @bc9, 'Borrowed'),
    (NEWID(), @l10, @bc10, 'Borrowed');

    -- =============================================================
    -- 3. DOC GIA oniichanbaka204@gmail.com (Qua han + Co phat)
    -- =============================================================
    -- Tao 5 khoan phat (sach muon tu cach day 20 ngay, han la 14 ngay, qua han 6 ngay)
    DECLARE @lOnii UNIQUEIDENTIFIER = NEWID();
    DECLARE @dueOnii DATETIME = DATEADD(day, -6, GETDATE()); -- Qua han 6 ngay
    
    INSERT INTO Loans (LoanId, BorrowerReaderId, ProcessedByAccountId, BorrowedAt, DueAt, Status, CreatedAt) VALUES
    (@lOnii, @rOniichan, @libAccountId, DATEADD(day, -20, GETDATE()), @dueOnii, 'Overdue', GETDATE());

    DECLARE @ldOnii1 UNIQUEIDENTIFIER = NEWID(), @ldOnii2 UNIQUEIDENTIFIER = NEWID(), @ldOnii3 UNIQUEIDENTIFIER = NEWID(), @ldOnii4 UNIQUEIDENTIFIER = NEWID(), @ldOnii5 UNIQUEIDENTIFIER = NEWID();
    INSERT INTO LoanDetails (LoanDetailId, LoanId, CopyId, Status) VALUES
    (@ldOnii1, @lOnii, @bcOnii1, 'Overdue'),
    (@ldOnii2, @lOnii, @bcOnii2, 'Overdue'),
    (@ldOnii3, @lOnii, @bcOnii3, 'Overdue'),
    (@ldOnii4, @lOnii, @bcOnii4, 'Overdue'),
    (@ldOnii5, @lOnii, @bcOnii5, 'Overdue');

    -- Add Fines
    INSERT INTO Fines (FineId, LoanDetailId, Amount, Reason, Status, CreatedAt) VALUES
    (NEWID(), @ldOnii1, 30000, N'Phat qua han 6 ngay (5k/ngay)', 'Unpaid', GETDATE()),
    (NEWID(), @ldOnii2, 30000, N'Phat qua han 6 ngay (5k/ngay)', 'Unpaid', GETDATE()),
    (NEWID(), @ldOnii3, 30000, N'Phat qua han 6 ngay (5k/ngay)', 'Unpaid', GETDATE()),
    (NEWID(), @ldOnii4, 30000, N'Phat qua han 6 ngay (5k/ngay)', 'Unpaid', GETDATE()),
    (NEWID(), @ldOnii5, 50000, N'Sach bi rach bia', 'Unpaid', GETDATE()); -- Loi khac tren cuon thu 5

    -- =============================================================
    -- 4. DOC GIA he181997phamtheson@gmail.com (Gan den han tra - Con 1 ngay)
    -- =============================================================
    DECLARE @lHe UNIQUEIDENTIFIER = NEWID();
    DECLARE @dueHe DATETIME = DATEADD(day, 1, GETDATE()); -- Ngay mai het han
    
    INSERT INTO Loans (LoanId, BorrowerReaderId, ProcessedByAccountId, BorrowedAt, DueAt, Status, CreatedAt) VALUES
    (@lHe, @rHe181997, @libAccountId, DATEADD(day, -13, GETDATE()), @dueHe, 'Borrowed', GETDATE());

    INSERT INTO LoanDetails (LoanDetailId, LoanId, CopyId, Status) VALUES
    (NEWID(), @lHe, @bcHe, 'Borrowed');

    -- Update trang thai cac cuon sach thanh Borrowed
    UPDATE BookCopies 
    SET Status = 'Borrowed' 
    WHERE CopyId IN (@bc1, @bc2, @bc3, @bc4, @bc5, @bc6, @bc7, @bc8, @bc9, @bc10, 
                     @bcOnii1, @bcOnii2, @bcOnii3, @bcOnii4, @bcOnii5, @bcHe);

    -- =============================================================
    -- 5. TAO PHONG VA DAT PHONG (Cho R6-R10)
    -- =============================================================
    -- Kiem tra co phong nao khong, neu khong thi tao
    DECLARE @roomId UNIQUEIDENTIFIER = (SELECT TOP 1 RoomId FROM Rooms);
    IF @roomId IS NULL
    BEGIN
        SET @roomId = NEWID();
        INSERT INTO Rooms (RoomId, RoomName, Capacity, Status, CreatedAt)
        VALUES (@roomId, N'Phong doc nhom 1', 10, 'Available', GETDATE());
    END

    -- Dat phong trong tuong lai gan
    INSERT INTO Reservations (ReservationId, ReaderId, RoomId, StartTime, EndTime, ReservationDate, Status, IsNoShow) VALUES
    (NEWID(), @r6, @roomId, DATEADD(hour, 1, GETDATE()), DATEADD(hour, 3, GETDATE()), GETDATE(), 'Confirmed', 0),
    (NEWID(), @r7, @roomId, DATEADD(hour, 4, GETDATE()), DATEADD(hour, 6, GETDATE()), GETDATE(), 'Confirmed', 0),
    (NEWID(), @r8, @roomId, DATEADD(hour, 7, GETDATE()), DATEADD(hour, 9, GETDATE()), GETDATE(), 'Confirmed', 0),
    (NEWID(), @r9, @roomId, DATEADD(day, 1, GETDATE()), DATEADD(hour, 2, DATEADD(day, 1, GETDATE())), GETDATE(), 'Confirmed', 0),
    (NEWID(), @r10, @roomId, DATEADD(day, 2, GETDATE()), DATEADD(hour, 2, DATEADD(day, 2, GETDATE())), GETDATE(), 'Confirmed', 0);


    COMMIT TRANSACTION;
    PRINT '=== THANH CONG: Da them du lieu mau mượn sách, phạt, đặt phòng ===';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'LOI: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
