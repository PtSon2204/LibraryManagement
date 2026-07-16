BEGIN TRANSACTION;

DECLARE @ReaderId UNIQUEIDENTIFIER;
DECLARE @UserProfileId UNIQUEIDENTIFIER = NEWID();
DECLARE @BookId UNIQUEIDENTIFIER;
DECLARE @CopyId UNIQUEIDENTIFIER = NEWID();
DECLARE @LoanId UNIQUEIDENTIFIER = NEWID();
DECLARE @LoanDetailId UNIQUEIDENTIFIER = NEWID();
DECLARE @PublisherId INT;
DECLARE @Email NVARCHAR(255) = 'oniichanbaka204@gmail.com';

-- 1. Xóa dữ liệu cũ (nếu có) của oniichanbaka204@gmail.com để làm lại cho sạch
SELECT @ReaderId = ReaderId FROM Readers WHERE Email = @Email;
IF @ReaderId IS NOT NULL
BEGIN
    DELETE FROM Fines WHERE LoanDetailId IN (SELECT LoanDetailId FROM LoanDetails WHERE LoanId IN (SELECT LoanId FROM Loans WHERE BorrowerReaderId = @ReaderId));
    DELETE FROM LoanDetails WHERE LoanId IN (SELECT LoanId FROM Loans WHERE BorrowerReaderId = @ReaderId);
    DELETE FROM Loans WHERE BorrowerReaderId = @ReaderId;
    DELETE FROM UserProfiles WHERE ReaderId = @ReaderId;
    DELETE FROM Reservations WHERE ReaderId = @ReaderId;
    DELETE FROM Readers WHERE ReaderId = @ReaderId;
END

-- Đặt lại ID mới cho an toàn
SET @ReaderId = NEWID();

-- Lấy tạm 1 Publisher có sẵn
SELECT TOP 1 @PublisherId = PublisherId FROM Publishers;
IF @PublisherId IS NULL
BEGIN
    INSERT INTO Publishers (PublisherName) VALUES (N'NXB Thử nghiệm');
    SET @PublisherId = SCOPE_IDENTITY();
END

-- Mật khẩu đã được mã hóa bằng BCrypt cho chuỗi '123456'
DECLARE @Password123456 NVARCHAR(MAX) = '$2a$11$2jbkjFI3gQkFKqnAEYASyO6ilZc3oh6/SyZyNbi75eondWSddPQ5i';

-- 2. Tạo Độc giả (Reader)
INSERT INTO Readers (ReaderId, Email, PasswordHash, Status, CreatedAt)
VALUES (@ReaderId, @Email, @Password123456, 'Active', GETDATE());

-- 3. Tạo Hồ sơ cá nhân (UserProfile)
INSERT INTO UserProfiles (UserProfileId, ReaderId, FullName, Phone, Address)
VALUES (@UserProfileId, @ReaderId, N'Oniichan Baka', '0988888888', N'TP.HCM');

-- 4. Tìm 1 cuốn sách bất kỳ (hoặc tạo mới nếu không có)
SELECT TOP 1 @BookId = BookId FROM Books;
IF @BookId IS NULL
BEGIN
    SET @BookId = NEWID();
    INSERT INTO Books (BookId, Title, PublisherId, IsHidden, CreatedAt)
    VALUES (@BookId, N'Sách bị quá hạn', @PublisherId, 0, GETDATE());
END

-- 5. Tạo 1 bản sao sách (BookCopy) - Trạng thái Borrowed
INSERT INTO BookCopies (CopyId, BookId, Barcode, Status, AddedDate)
VALUES (@CopyId, @BookId, 'OVERDUE-ONII', 'Borrowed', CAST(GETDATE() AS DATE));

-- 6. Tạo Phiếu mượn (Loan) - Mượn 20 ngày trước, Hạn trả là 6 ngày trước
-- QUAN TRỌNG: Status phải là 'Borrowed' để hệ thống tự quét DueAt
INSERT INTO Loans (LoanId, BorrowerReaderId, BorrowedAt, DueAt, Status, CreatedAt)
VALUES (
    @LoanId, 
    @ReaderId, 
    DATEADD(day, -20, GETDATE()), 
    DATEADD(day, -6, GETDATE()),  
    'Borrowed', 
    DATEADD(day, -20, GETDATE())
);

-- 7. Tạo Chi tiết phiếu mượn (LoanDetail) - Status cũng là 'Borrowed'
INSERT INTO LoanDetails (LoanDetailId, LoanId, CopyId, Status)
VALUES (@LoanDetailId, @LoanId, @CopyId, 'Borrowed');

COMMIT TRANSACTION;

PRINT N'Tạo dữ liệu thành công! Tài khoản: oniichanbaka204@gmail.com | Mật khẩu: 123456';
