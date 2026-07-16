BEGIN TRANSACTION;

DECLARE @ReaderId UNIQUEIDENTIFIER;
DECLARE @BookId UNIQUEIDENTIFIER;
DECLARE @CopyId UNIQUEIDENTIFIER = NEWID();
DECLARE @LoanId UNIQUEIDENTIFIER = NEWID();
DECLARE @LoanDetailId UNIQUEIDENTIFIER = NEWID();
DECLARE @PublisherId INT;
DECLARE @Email NVARCHAR(255) = 'he181997phamtheson@gmail.com';

-- 1. Tìm ReaderId của he181997phamtheson@gmail.com đã tồn tại trong DB
SELECT @ReaderId = ReaderId FROM Readers WHERE Email = @Email;

IF @ReaderId IS NULL
BEGIN
    PRINT N'LỖI: Không tìm thấy tài khoản ' + @Email + N' trong hệ thống. Bạn cần đăng ký tài khoản này trước.';
    ROLLBACK TRANSACTION;
    RETURN;
END

-- 2. Dọn dẹp các phiếu mượn cũ của tài khoản này (để tránh nhiễu dữ liệu khi test mail)
DELETE FROM Fines WHERE LoanDetailId IN (SELECT LoanDetailId FROM LoanDetails WHERE LoanId IN (SELECT LoanId FROM Loans WHERE BorrowerReaderId = @ReaderId));
DELETE FROM LoanDetails WHERE LoanId IN (SELECT LoanId FROM Loans WHERE BorrowerReaderId = @ReaderId);
DELETE FROM Loans WHERE BorrowerReaderId = @ReaderId;

-- Lấy tạm 1 Publisher có sẵn
SELECT TOP 1 @PublisherId = PublisherId FROM Publishers;
IF @PublisherId IS NULL
BEGIN
    INSERT INTO Publishers (PublisherName) VALUES (N'NXB Thử nghiệm 2');
    SET @PublisherId = SCOPE_IDENTITY();
END

-- 3. Tìm 1 cuốn sách bất kỳ (hoặc tạo mới nếu không có)
SELECT TOP 1 @BookId = BookId FROM Books;
IF @BookId IS NULL
BEGIN
    SET @BookId = NEWID();
    INSERT INTO Books (BookId, Title, PublisherId, IsHidden, CreatedAt)
    VALUES (@BookId, N'Sách nhắc nhở trả', @PublisherId, 0, GETDATE());
END

-- 4. Tạo 1 bản sao sách (BookCopy) - Trạng thái Borrowed
INSERT INTO BookCopies (CopyId, BookId, Barcode, Status, AddedDate)
VALUES (@CopyId, @BookId, 'DUESOON-HE', 'Borrowed', CAST(GETDATE() AS DATE));

-- 5. Tạo Phiếu mượn (Loan)
-- QUAN TRỌNG: Mượn 13 ngày trước, Hạn trả là NGÀY MAI (DueAt = DATEADD(day, 1, GETDATE()))
INSERT INTO Loans (LoanId, BorrowerReaderId, BorrowedAt, DueAt, Status, CreatedAt)
VALUES (
    @LoanId, 
    @ReaderId, 
    DATEADD(day, -13, GETDATE()), 
    DATEADD(day, 1, GETDATE()),  
    'Borrowed', 
    DATEADD(day, -13, GETDATE())
);

-- 6. Tạo Chi tiết phiếu mượn (LoanDetail) - Status 'Borrowed'
INSERT INTO LoanDetails (LoanDetailId, LoanId, CopyId, Status)
VALUES (@LoanDetailId, @LoanId, @CopyId, 'Borrowed');

COMMIT TRANSACTION;

PRINT N'Tạo dữ liệu thành công! Tài khoản: he181997phamtheson@gmail.com ĐÃ CÓ phiếu mượn sắp đến hạn trả vào NGÀY MAI.';
