-- =============================================================
-- THU VIEN QUAN LY - DU LIEU MAU (52 SACH)
-- Ngay tao: 2026-07-16
-- Chay toan bo script nay trong 1 lan (khong dung GO de tach batch)
-- =============================================================
SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    -- =============================================================
    -- 1. NHA XUAT BAN (Publishers) - 10 NXB
    -- =============================================================
    INSERT INTO Publishers (PublisherName, Address, Phone, Email) VALUES
    (N'NXB Tr?',                  N'161B Lý Chính Th?ng, Q.3, TP.HCM',       N'028-39316289', N'nxbtre@nxbtre.com.vn'),
    (N'NXB Kim D?ng',             N'55 Quang Trung, Hai Bà Tr?ng, Hà N?i',    N'024-39434730', N'kimdong@kimdong.com.vn'),
    (N'NXB Giáo D?c Vi?t Nam',    N'81 Tr?n H?ng D?o, Hoàn Ki?m, Hà N?i',    N'024-38220801', N'giaoduc@nxbgd.vn'),
    (N'NXB T?ng H?p TP.HCM',     N'62 Nguy?n Th? Minh Khai, Q.1, TP.HCM',   N'028-38225340', N'tonghop@nxbhcm.com.vn'),
    (N'NXB Lao D?ng',             N'175 Gi?ng Võ, D?ng Da, Hà N?i',           N'024-38512780', N'laodong@nxbld.vn'),
    (N'NXB Van H?c',              N'18 Nguy?n Tr??ng T?, Ba Dình, Hà N?i',    N'024-37165464', N'vanhoc@nxbvanhoc.vn'),
    (N'NXB H?i Nhà Van',          N'65 Nguy?n Du, Hai Bà Tr?ng, Hà N?i',     N'024-39448573', N'hoinhavan@vanhocviet.vn'),
    (N'NXB D?i H?c Qu?c Gia HN',  N'144 Xuân Th?y, C?u Gi?y, Hà N?i',        N'024-37547684', N'dhqg@vnu.edu.vn'),
    (N'NXB Th? Gi?i',             N'46 Tr?n H?ng D?o, Hoàn Ki?m, Hà N?i',    N'024-38253841', N'thegioi@thegioi.vn'),
    (N'Alpha Books',              N'12 Khu?t Duy Ti?n, Thanh Xuân, Hà N?i',  N'024-62622012', N'info@alphabooks.vn');

    -- =============================================================
    -- 2. TAC GIA (Authors) - 34 tac gia
    -- =============================================================
    INSERT INTO Authors (FullName, Biography) VALUES
    (N'Nguy?n Nh?t Ánh',          N'Nhà van Vi?t Nam n?i ti?ng v?i các tác ph?m dành cho thi?u nhi và tu?i tr? nh? Tôi Th?y Hoa Vàng Trên C? Xanh, M?t Bi?c.'),
    (N'Tô Hoài',                  N'Nhà van Vi?t Nam, tác gi? c?a D? Mèn Phiêu L?u Ký và nhi?u tác ph?m van xuôi n?i ti?ng.'),
    (N'Nam Cao',                  N'Nhà van hi?n th?c xu?t s?c c?a van h?c Vi?t Nam hi?n d?i, tác gi? Chí Phèo, Lão H?c.'),
    (N'V? Tr?ng Ph?ng',           N'Nhà van trào phúng n?i ti?ng c?a van h?c Vi?t Nam d?u th? k? XX, tác gi? S? D?.'),
    (N'Nguy?n Du',                N'D?i thi hào dân t?c Vi?t Nam, tác gi? c?a Truy?n Ki?u - ki?t tác van h?c ch? Nôm.'),
    (N'Paulo Coelho',             N'Nhà van ng??i Brazil n?i ti?ng th? gi?i, tác gi? c?a Nhà Gi? Kim.'),
    (N'Dale Carnegie',            N'Tác gi? ng??i M? n?i ti?ng v?i các cu?n sách k? nang giao ti?p và phát tri?n b?n thân.'),
    (N'Leo Tolstoy',              N'Nhà van Nga vi d?i th? k? XIX, tác gi? c?a Chi?n Tranh Và Hòa Bình.'),
    (N'Khaled Hosseini',          N'Nhà van g?c Afghanistan, tác gi? c?a Ng??i Dua Di?u - ti?u thuy?t bán ch?y toàn c?u.'),
    (N'Charles Duhigg',           N'Nhà báo và tác gi? ng??i M?, chuyên gia nghiên c?u v? thói quen và nang su?t làm vi?c.'),
    (N'Daniel Kahneman',          N'Nhà tâm lý h?c do?t gi?i Nobel Kinh t? 2002, chuyên gia v? hành vi và ra quy?t d?nh.'),
    (N'Ichiro Kishimi',           N'Tri?t gia và nhà tâm lý h?c ng??i Nh?t, d?ng tác gi? cu?n Dám B? Ghét.'),
    (N'Napoleon Hill',            N'Tác gi? ng??i M?, tác gi? Ngh? Giàu Làm Giàu - m?t trong nh?ng cu?n sách t? l?c bán ch?y nh?t l?ch s?.'),
    (N'Stephen Covey',            N'Nhà lãnh d?o và tác gi? ng??i M?, n?i ti?ng v?i cu?n 7 Thói Quen C?a Ng??i Thành D?t.'),
    (N'James Clear',              N'Tác gi? ng??i M? chuyên v? thói quen và hi?u su?t cá nhân, tác gi? c?a Atomic Habits.'),
    (N'Yuval Noah Harari',        N'Nhà s? h?c và tri?t h?c ng??i Israel, tác gi? b? ba Sapiens, Homo Deus và 21 Bài H?c.'),
    (N'Stanley Karnow',           N'Nhà báo và s? gia ng??i M?, chuyên gia v? l?ch s? chi?n tranh Vi?t Nam.'),
    (N'Richard Feynman',          N'Nhà v?t lý lý thuy?t ng??i M? do?t gi?i Nobel V?t Lý 1965.'),
    (N'Stephen Hawking',          N'Nhà v?t lý lý thuy?t và vu? tr? h?c ng??i Anh, tác gi? c?a L??c S? Th?i Gian.'),
    (N'Thomas Friedman',          N'Nhà báo và tác gi? ng??i M? do?t ba gi?i Pulitzer, tác gi? Th? Gi?i Ph?ng.'),
    (N'Eric Ries',                N'Doanh nhân và tác gi? ng??i M?, ng??i sáng l?p phong trào Lean Startup.'),
    (N'Peter Thiel',              N'Doanh nhân công ngh? và nhà d?u t? ng??i M?, d?ng sáng l?p PayPal.'),
    (N'J.K. Rowling',             N'Nhà van ng??i Anh n?i ti?ng v?i b? truy?n Harry Potter.'),
    (N'Antoine de Saint-Exupéry', N'Nhà van và phi công ng??i Pháp, tác gi? c?a Hoàng T? Bé.'),
    (N'Luis Sepúlveda',           N'Nhà van ng??i Chile, tác gi? c?a Chuy?n Con Mèo D?y H?i Âu Bay.'),
    (N'Charles Darwin',           N'Nhà t? nhiên h?c ng??i Anh, cha d? c?a h?c thuy?t ti?n hóa qua ch?n l?c t? nhiên.'),
    (N'George Orwell',            N'Nhà van và nhà báo ng??i Anh, tác gi? 1984 và Tr?i Súc V?t.'),
    (N'Mario Puzo',               N'Nhà van ng??i M? g?c Ý, n?i ti?ng v?i ti?u thuy?t B? Già.'),
    (N'Victor Hugo',              N'Nhà van và nhà th? ng??i Pháp vi d?i, tác gi? c?a Nh?ng Ng??i Kh?n Kh?.'),
    (N'Alexandre Dumas',          N'Nhà van ng??i Pháp n?i ti?ng v?i Bá T??c Monte Cristo.'),
    (N'Phan Huy Lê',              N'Giáo s? s? h?c Vi?t Nam, nhà nghiên c?u l?ch s? và d?ch thu?t tri?t h?c n?i ti?ng.'),
    (N'Emil Ludwig',              N'Nhà van và nhà báo ng??i D?c-Th?y S?, n?i ti?ng v?i các cu?n ti?u s? l?ch s?.'),
    (N'Eric Matthes',             N'Tác gi? và l?p trình viên ng??i M?, n?i ti?ng v?i cu?n Python Crash Course.'),
    (N'Karl Marx',                N'Tri?t h?c gia, kinh t? h?c gia ng??i D?c, tác gi? T? B?n Lu?n và Tuyên Ngôn D?ng C?ng S?n.');

    -- =============================================================
    -- 3. THE LOAI (Categories) - 10 the loai
    -- =============================================================
    INSERT INTO Categories (CategoryName, Description) VALUES
    (N'Van h?c',              N'Sách van h?c trong và ngoài n??c, ti?u thuy?t, truy?n ng?n, tho ca.'),
    (N'Khoa h?c k? thu?t',    N'Sách v? khoa h?c, công ngh? thông tin, l?p trình và k? thu?t.'),
    (N'L?ch s?',              N'Sách l?ch s? Vi?t Nam và th? gi?i, ti?u s? nhân v?t l?ch s?.'),
    (N'Tâm lý h?c',           N'Sách v? tâm lý con ng??i, hành vi, c?m xúc và phân tích tâm lý.'),
    (N'K? nang s?ng',         N'Sách phát tri?n b?n thân, k? nang giao ti?p, lãnh d?o và thành công.'),
    (N'Thi?u nhi',            N'Sách dành cho thi?u nhi và thanh thi?u niên, truy?n phiêu l?u và c? tích.'),
    (N'Tri?t h?c',            N'Sách tri?t h?c Dông và Tây, d?o d?c h?c, siêu hình h?c và t? t??ng.'),
    (N'Kinh t? - Kinh doanh', N'Sách v? kinh t? h?c, qu?n tr? kinh doanh, tài chính và kh?i nghi?p.'),
    (N'Y h?c - S?c kh?e',     N'Sách v? y h?c, sinh h?c, gi?i ph?u và cham sóc s?c kh?e.'),
    (N'Khoa h?c t? nhiên',    N'Sách v? v?t lý, hóa h?c, sinh h?c, thiên van h?c và khoa h?c t? nhiên.');

    -- =============================================================
    -- 4. SACH (Books) - 52 cuon
    -- =============================================================
    DECLARE @b1  UNIQUEIDENTIFIER = NEWID(); DECLARE @b2  UNIQUEIDENTIFIER = NEWID();
    DECLARE @b3  UNIQUEIDENTIFIER = NEWID(); DECLARE @b4  UNIQUEIDENTIFIER = NEWID();
    DECLARE @b5  UNIQUEIDENTIFIER = NEWID(); DECLARE @b6  UNIQUEIDENTIFIER = NEWID();
    DECLARE @b7  UNIQUEIDENTIFIER = NEWID(); DECLARE @b8  UNIQUEIDENTIFIER = NEWID();
    DECLARE @b9  UNIQUEIDENTIFIER = NEWID(); DECLARE @b10 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b11 UNIQUEIDENTIFIER = NEWID(); DECLARE @b12 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b13 UNIQUEIDENTIFIER = NEWID(); DECLARE @b14 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b15 UNIQUEIDENTIFIER = NEWID(); DECLARE @b16 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b17 UNIQUEIDENTIFIER = NEWID(); DECLARE @b18 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b19 UNIQUEIDENTIFIER = NEWID(); DECLARE @b20 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b21 UNIQUEIDENTIFIER = NEWID(); DECLARE @b22 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b23 UNIQUEIDENTIFIER = NEWID(); DECLARE @b24 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b25 UNIQUEIDENTIFIER = NEWID(); DECLARE @b26 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b27 UNIQUEIDENTIFIER = NEWID(); DECLARE @b28 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b29 UNIQUEIDENTIFIER = NEWID(); DECLARE @b30 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b31 UNIQUEIDENTIFIER = NEWID(); DECLARE @b32 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b33 UNIQUEIDENTIFIER = NEWID(); DECLARE @b34 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b35 UNIQUEIDENTIFIER = NEWID(); DECLARE @b36 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b37 UNIQUEIDENTIFIER = NEWID(); DECLARE @b38 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b39 UNIQUEIDENTIFIER = NEWID(); DECLARE @b40 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b41 UNIQUEIDENTIFIER = NEWID(); DECLARE @b42 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b43 UNIQUEIDENTIFIER = NEWID(); DECLARE @b44 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b45 UNIQUEIDENTIFIER = NEWID(); DECLARE @b46 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b47 UNIQUEIDENTIFIER = NEWID(); DECLARE @b48 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b49 UNIQUEIDENTIFIER = NEWID(); DECLARE @b50 UNIQUEIDENTIFIER = NEWID();
    DECLARE @b51 UNIQUEIDENTIFIER = NEWID(); DECLARE @b52 UNIQUEIDENTIFIER = NEWID();

    -- Publisher ID variables
    DECLARE @pNXBTre    INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'NXB Tr?');
    DECLARE @pKimDong   INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'NXB Kim D?ng');
    DECLARE @pGiaoDuc   INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'NXB Giáo D?c Vi?t Nam');
    DECLARE @pTongHop   INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'NXB T?ng H?p TP.HCM');
    DECLARE @pLaoDong   INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'NXB Lao D?ng');
    DECLARE @pVanHoc    INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'NXB Van H?c');
    DECLARE @pHoiNhaVan INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'NXB H?i Nhà Van');
    DECLARE @pDHQGHN    INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'NXB D?i H?c Qu?c Gia HN');
    DECLARE @pTheGioi   INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'NXB Th? Gi?i');
    DECLARE @pAlpha     INT = (SELECT PublisherId FROM Publishers WHERE PublisherName = N'Alpha Books');

    INSERT INTO Books (BookId, Title, ISBN, PublisherId, PublicationYear, Language, Edition, Description, CoverImageUrl, CreatedAt, IsHidden) VALUES
    (@b1,  N'Nhà Gi? Kim',                         '978-604-1-10001-1', @pNXBTre,    2017, N'Ti?ng Vi?t', N'L?n 1', N'Hành trình c?a chàng chan c?u Santiago di tìm kho báu và khám phá ý nghia cu?c d?i.',                      '/images/1.jpg',  GETDATE(), 0),
    (@b2,  N'D?c Nhân Tâm',                        '978-604-1-10002-2', @pAlpha,     2016, N'Ti?ng Vi?t', N'L?n 1', N'Cu?n sách kinh di?n v? ngh? thu?t giao ti?p, t?o thi?n c?m và ?nh h??ng d?n ng??i khác.',              '/images/2.jpg',  GETDATE(), 0),
    (@b3,  N'Chi?n Tranh Và Hòa Bình',             '978-604-1-10003-3', @pVanHoc,    2018, N'Ti?ng Vi?t', N'L?n 1', N'Ti?u thuy?t s? thi vi d?i v? n??c Nga th?i k? Napoleon xâm l??c và cu?c s?ng gi?i quý t?c.',            '/images/3.jpg',  GETDATE(), 0),
    (@b4,  N'Ng??i Dua Di?u',                      '978-604-1-10004-4', @pHoiNhaVan, 2019, N'Ti?ng Vi?t', N'L?n 1', N'Câu chuy?n xúc d?ng v? tình b?n, ph?n b?i và hành trình chu?c l?i ? d?t n??c Afghanistan.',            '/images/4.jpg',  GETDATE(), 0),
    (@b5,  N'Tôi Th?y Hoa Vàng Trên C? Xanh',     '978-604-1-10005-5', @pNXBTre,    2010, N'Ti?ng Vi?t', N'L?n 1', N'Truy?n v? tu?i tho ? vùng quê Vi?t Nam v?i nh?ng k? ni?m trong sáng, c?m d?ng và tình anh em.',        '/images/5.jpg',  GETDATE(), 0),
    (@b6,  N'D? Mèn Phiêu L?u Ký',               '978-604-1-10006-6', @pKimDong,   2015, N'Ti?ng Vi?t', N'L?n 1', N'Cu?c phiêu l?u k? thú c?a chú D? Mèn và bài h?c v? tình b?n, d?ng c?m và khám phá th? gi?i.',          '/images/6.jpg',  GETDATE(), 0),
    (@b7,  N'S? D?',                               '978-604-1-10007-7', @pVanHoc,    2014, N'Ti?ng Vi?t', N'L?n 1', N'Ti?u thuy?t trào phúng s?c bén v? xã h?i Vi?t Nam th?i Pháp thu?c qua nhân v?t Xuân Tóc D?.',           '/images/7.jpg',  GETDATE(), 0),
    (@b8,  N'Truy?n Ki?u',                         '978-604-1-10008-8', @pGiaoDuc,   2012, N'Ti?ng Vi?t', N'L?n 1', N'Ki?t tác van h?c ch? Nôm, truy?n tho k? v? s? ph?n bi th??ng c?a ng??i con gái tài s?c Thúy Ki?u.',    '/images/8.jpg',  GETDATE(), 0),
    (@b9,  N'Lão H?c - Tuy?n T?p Nam Cao',        '978-604-1-10009-9', @pGiaoDuc,   2016, N'Ti?ng Vi?t', N'L?n 1', N'Tuy?n t?p các truy?n ng?n hi?n th?c xu?t s?c c?a nhà van Nam Cao v? ng??i nông dân Vi?t Nam.',            '/images/9.jpg',  GETDATE(), 0),
    (@b10, N'Chí Phèo Và Nh?ng Truy?n Ng?n',      '978-604-1-10010-5', @pVanHoc,    2015, N'Ti?ng Vi?t', N'L?n 1', N'Chí Phèo và các tác ph?m tiêu bi?u c?a Nam Cao v? bi k?ch ng??i nông dân trong xã h?i cu.',              '/images/10.jpg', GETDATE(), 0),
    (@b11, N'S?c M?nh C?a Thói Quen',             '978-604-1-10011-2', @pAlpha,     2019, N'Ti?ng Vi?t', N'L?n 1', N'Phân tích khoa h?c v? cách thói quen hình thành và cách thay d?i chúng d? c?i thi?n cu?c s?ng.',          '/images/11.jpg', GETDATE(), 0),
    (@b12, N'T? Duy Nhanh Và Ch?m',               '978-604-1-10012-9', @pAlpha,     2020, N'Ti?ng Vi?t', N'L?n 1', N'Khám phá hai h? th?ng t? duy: t? duy nhanh theo b?n nang và t? duy ch?m theo logic và lý trí.',           '/images/12.jpg', GETDATE(), 0),
    (@b13, N'Dám B? Ghét',                         '978-604-1-10013-6', @pLaoDong,   2018, N'Ti?ng Vi?t', N'L?n 1', N'Tri?t h?c Adler v? t? do, h?nh phúc và d?ng khí d? s?ng cu?c d?i mình mu?n b?t ch?p s? phán xét.',      '/images/13.jpg', GETDATE(), 0),
    (@b14, N'Ngh? Giàu Làm Giàu',                 '978-604-1-10014-3', @pLaoDong,   2017, N'Ti?ng Vi?t', N'L?n 1', N'Bí quy?t thành công và làm giàu t? vi?c ki?m soát t? duy, thi?t l?p m?c tiêu và kiên trì theo du?i.', '/images/14.jpg', GETDATE(), 0),
    (@b15, N'7 Thói Quen C?a Ng??i Thành D?t',   '978-604-1-10015-0', @pAlpha,     2018, N'Ti?ng Vi?t', N'L?n 1', N'B?y nguyên t?c s?ng giúp b?n tr? nên hi?u qu? h?n và xây d?ng m?i quan h? t?t d?p.',                    '/images/15.jpg', GETDATE(), 0),
    (@b16, N'Thói Quen Nguyên T?',                '978-604-1-10016-7', @pLaoDong,   2020, N'Ti?ng Vi?t', N'L?n 1', N'Ph??ng pháp xây d?ng thói quen t?t và lo?i b? thói quen x?u thông qua nh?ng thay d?i nh? nh?ng hi?u qu?.', '/images/16.jpg', GETDATE(), 0),
    (@b17, N'Sapiens: L??c S? Loài Ng??i',        '978-604-1-10017-4', @pTheGioi,   2018, N'Ti?ng Vi?t', N'L?n 1', N'L?ch s? tóm l??c c?a loài ng??i t? khi xu?t hi?n d?n hi?n d?i qua các cu?c cách m?ng l?n.',              '/images/17.jpg', GETDATE(), 0),
    (@b18, N'Homo Deus: L??c S? T??ng Lai',       '978-604-1-10018-1', @pTheGioi,   2019, N'Ti?ng Vi?t', N'L?n 1', N'T??ng lai c?a loài ng??i trong th?i d?i công ngh? sinh h?c và trí tu? nhân t?o.',                        '/images/18.jpg', GETDATE(), 0),
    (@b19, N'D?i C??ng L?ch S? Vi?t Nam T?p 1',   '978-604-1-10019-8', @pGiaoDuc,   2020, N'Ti?ng Vi?t', N'L?n 4', N'L?ch s? Vi?t Nam t? th?i nguyên th?y d?n cu?i th? k? XIX, dùng làm giáo trình d?i h?c.',                '/images/19.jpg', GETDATE(), 0),
    (@b20, N'L?ch S? Th? Gi?i C? Trung D?i',      '978-604-1-10020-4', @pGiaoDuc,   2018, N'Ti?ng Vi?t', N'L?n 3', N'T?ng quan l?ch s? th? gi?i t? th?i k? c? d?i d?n h?t th?i trung d?i, tài li?u tham kh?o.',               '/images/20.jpg', GETDATE(), 0),
    (@b21, N'Chi?n Tranh Vi?t Nam',               '978-604-1-10021-1', @pTheGioi,   2016, N'Ti?ng Vi?t', N'L?n 1', N'L?ch s? chi?n tranh Vi?t Nam toàn di?n qua góc nhìn c?a nhà báo ng??i M? Stanley Karnow.',               '/images/21.jpg', GETDATE(), 0),
    (@b22, N'Napoleon Bonaparte: Ti?u S?',         '978-604-1-10022-8', @pHoiNhaVan, 2017, N'Ti?ng Vi?t', N'L?n 1', N'Ti?u s? toàn di?n v? Hoàng d? Napoleon - thiên tài quân s? và nhà lãnh d?o vi d?i c?a n??c Pháp.',      '/images/22.jpg', GETDATE(), 0),
    (@b23, N'Bài Gi?ng V?t Lý Feynman T?p 1',    '978-604-1-10023-5', @pGiaoDuc,   2019, N'Ti?ng Vi?t', N'L?n 1', N'B? bài gi?ng v?t lý kinh di?n c?a Richard Feynman, m?t trong nh?ng b? sách v?t lý hay nh?t th? k? XX.', '/images/23.jpg', GETDATE(), 0),
    (@b24, N'L??c S? Th?i Gian',                  '978-604-1-10024-2', @pNXBTre,    2018, N'Ti?ng Vi?t', N'L?n 1', N'Hành trình khám phá vu? tr? t? Big Bang d?n l? den và các lý thuy?t v?t lý hi?n d?i, dành cho ng??i không chuyên.', '/images/24.jpg', GETDATE(), 0),
    (@b25, N'Vu? Tr? Trong V? H?t D?',            '978-604-1-10025-9', @pNXBTre,    2017, N'Ti?ng Vi?t', N'L?n 1', N'Khám phá các lý thuy?t v?t lý hi?n d?i v? vu? tr? du?c Stephen Hawking trình bày d? hi?u và sinh d?ng.','/images/25.jpg', GETDATE(), 0),
    (@b26, N'Th? Gi?i Ph?ng',                     '978-604-1-10026-6', @pLaoDong,   2016, N'Ti?ng Vi?t', N'L?n 1', N'Phân tích toàn c?u hóa và cách internet và công ngh? dã san ph?ng th? gi?i kinh doanh.',                 '/images/26.jpg', GETDATE(), 0),
    (@b27, N'Python Crash Course',                '978-604-1-10027-3', @pDHQGHN,    2021, N'Ti?ng Vi?t', N'L?n 2', N'H??ng d?n h?c l?p trình Python toàn di?n t? co b?n d?n nâng cao, có nhi?u bài t?p và d? án th?c t?.',    '/images/27.jpg', GETDATE(), 0),
    (@b28, N'Kh?i Nghi?p Tinh G?n',              '978-604-1-10028-0', @pAlpha,     2019, N'Ti?ng Vi?t', N'L?n 1', N'Ph??ng pháp Lean Startup giúp các doanh nghi?p xây d?ng s?n ph?m nhanh, ti?t ki?m và hi?u qu?.',          '/images/28.jpg', GETDATE(), 0),
    (@b29, N'T? 0 D?n 1',                         '978-604-1-10029-7', @pLaoDong,   2018, N'Ti?ng Vi?t', N'L?n 1', N'Bài h?c v? kh?i nghi?p, d?i m?i sáng t?o và xây d?ng công ty d?c quy?n t? nhà d?u t? Peter Thiel.',     '/images/29.jpg', GETDATE(), 0),
    (@b30, N'T? B?n Lu?n T?p 1',                  '978-604-1-10030-3', @pLaoDong,   2020, N'Ti?ng Vi?t', N'L?n 1', N'Phân tích ph??ng th?c s?n xu?t t? b?n ch? nghia, quá trình tích lu? v?n và b?c l?t lao d?ng.',            '/images/30.jpg', GETDATE(), 0),
    (@b31, N'D?o D?c Kinh',                       '978-604-1-10031-0', @pTongHop,   2017, N'Ti?ng Vi?t', N'L?n 1', N'Tác ph?m tri?t h?c kinh di?n c?a Lão T? v? D?o và D?c, b?n d?ch và chú gi?i d?y d? ti?ng Vi?t.',        '/images/31.jpg', GETDATE(), 0),
    (@b32, N'1984',                               '978-604-1-10032-7', @pHoiNhaVan, 2019, N'Ti?ng Vi?t', N'L?n 1', N'Ti?u thuy?t dystopia v? xã h?i toàn tr? do D?ng ki?m soát tuy?t d?i t? t??ng và cu?c s?ng công dân.',    '/images/32.jpg', GETDATE(), 0),
    (@b33, N'Tr?i Súc V?t',                       '978-604-1-10033-4', @pHoiNhaVan, 2018, N'Ti?ng Vi?t', N'L?n 1', N'Ng? ngôn chính tr? v? cu?c cách m?ng c?a nh?ng con v?t và s? tha hóa t?t y?u c?a quy?n l?c.',           '/images/33.jpg', GETDATE(), 0),
    (@b34, N'B? Già',                             '978-604-1-10034-1', @pVanHoc,    2018, N'Ti?ng Vi?t', N'L?n 1', N'Ti?u thuy?t v? gia dình mafia Corleone, du?c coi là ki?t tác van h?c t?i ph?m c?a th? k? XX.',            '/images/34.jpg', GETDATE(), 0),
    (@b35, N'Nh?ng Ng??i Kh?n Kh?',             '978-604-1-10035-8', @pVanHoc,    2017, N'Ti?ng Vi?t', N'L?n 1', N'Ki?t tác van h?c Pháp v? công lý, tình yêu, s? c?u r?i và ph?m giá con ng??i trong xã h?i Pháp th? k? XIX.', '/images/35.jpg', GETDATE(), 0),
    (@b36, N'Bá T??c Monte Cristo',               '978-604-1-10036-5', @pVanHoc,    2019, N'Ti?ng Vi?t', N'L?n 1', N'Ti?u thuy?t phiêu l?u h?p d?n v? s? tr? thù hoàn h?o và công lý c?a Edmond Dantès.',                     '/images/36.jpg', GETDATE(), 0),
    (@b37, N'Harry Potter Và Hòn Dá Phù Th?y',   '978-604-1-10037-2', @pNXBTre,    2020, N'Ti?ng Vi?t', N'L?n 1', N'T?p d?u tiên c?a b? truy?n Harry Potter, kh?i d?u chuy?n phiêu l?u c?a c?u bé phù th?y t?i tr??ng Hogwarts.', '/images/37.jpg', GETDATE(), 0),
    (@b38, N'Harry Potter Và Phòng Ch?a Bí M?t', '978-604-1-10038-9', @pNXBTre,    2020, N'Ti?ng Vi?t', N'L?n 1', N'T?p th? hai c?a b? truy?n Harry Potter v?i bí ?n v? Phòng Ch?a Bí M?t và k? th?a k? c?a Slytherin.',    '/images/38.jpg', GETDATE(), 0),
    (@b39, N'Hoàng T? Bé',                       '978-604-1-10039-6', @pKimDong,   2016, N'Ti?ng Vi?t', N'L?n 1', N'Câu chuy?n tri?t h?c dành cho m?i l?a tu?i v? tình yêu, s? thu?n khi?t và ý nghia th?c s? c?a cu?c s?ng.','/images/39.jpg', GETDATE(), 0),
    (@b40, N'Chuy?n Con Mèo D?y H?i Âu Bay',    '978-604-1-10040-2', @pHoiNhaVan, 2017, N'Ti?ng Vi?t', N'L?n 1', N'Câu chuy?n c?m d?ng v? tình b?n gi?a con mèo và chú h?i âu, bài h?c v? lòng d?ng c?m và s? khác bi?t.', '/images/40.jpg', GETDATE(), 0),
    (@b41, N'Kính V?n Hoa T?p 1',               '978-604-1-10041-9', @pKimDong,   2015, N'Ti?ng Vi?t', N'L?n 1', N'T?p d?u c?a b? truy?n Kính V?n Hoa k? v? nh?ng chuy?n vui bu?n trong cu?c s?ng h?c trò.',               '/images/41.jpg', GETDATE(), 0),
    (@b42, N'Cho Tôi Xin M?t Vé Di Tu?i Tho',   '978-604-1-10042-6', @pNXBTre,    2008, N'Ti?ng Vi?t', N'L?n 1', N'Hành trình tr? v? tu?i tho trong sáng v?i nh?ng k? ni?m ng?t ngào và dáng nh? c?a Nguy?n Nh?t Ánh.',    '/images/42.jpg', GETDATE(), 0),
    (@b43, N'M?t Bi?c',                          '978-604-1-10043-3', @pNXBTre,    2016, N'Ti?ng Vi?t', N'L?n 1', N'Câu chuy?n tình yêu h?c trò don ph??ng trong sáng và day d?t gi?a Ng?n và cô b?n Hà Lan m?t bi?c.',       '/images/43.jpg', GETDATE(), 0),
    (@b44, N'Ngu?n G?c Các Loài',               '978-604-1-10044-0', @pGiaoDuc,   2018, N'Ti?ng Vi?t', N'L?n 1', N'Tác ph?m khoa h?c cách m?ng c?a Darwin trình bày h?c thuy?t ti?n hóa qua ch?n l?c t? nhiên.',             '/images/44.jpg', GETDATE(), 0),
    (@b45, N'Ng?n G?n V? M?i Th?',              '978-604-1-10045-7', @pNXBTre,    2021, N'Ti?ng Vi?t', N'L?n 1', N'M??i câu h?i l?n v? vu? tr? du?c Stephen Hawking tr? l?i m?t cách ng?n g?n và d?y c?m h?ng.',              '/images/45.jpg', GETDATE(), 0),
    (@b46, N'L?p Trình H??ng D?i T??ng V?i Java','978-604-1-10046-4', @pDHQGHN,    2020, N'Ti?ng Vi?t', N'L?n 2', N'Giáo trình l?p trình h??ng d?i t??ng v?i Java t? co b?n d?n nâng cao, phù h?p sinh viên CNTT.',           '/images/46.jpg', GETDATE(), 0),
    (@b47, N'Tâm Lý H?c Dám Dông',              '978-604-1-10047-1', @pTongHop,   2019, N'Ti?ng Vi?t', N'L?n 1', N'Phân tích tâm lý và hành vi dám dông, cách t?p th? ?nh h??ng d?n hành d?ng và quy?t d?nh cá nhân.',       '/images/47.jpg', GETDATE(), 0),
    (@b48, N'Ngh? Thu?t T? Duy Rõ Ràng',        '978-604-1-10048-8', @pTongHop,   2020, N'Ti?ng Vi?t', N'L?n 1', N'52 l?i t? duy ph? bi?n nh?t trong cu?c s?ng hàng ngày và cách tránh chúng d? ra quy?t d?nh sáng su?t.', '/images/48.jpg', GETDATE(), 0),
    (@b49, N'C?ng Hòa (Plato)',                  '978-604-1-10049-5', @pTheGioi,   2019, N'Ti?ng Vi?t', N'L?n 1', N'Tác ph?m tri?t h?c vi d?i c?a Plato v? công lý, nhà n??c lý t??ng và b?n ch?t con ng??i.',               '/images/49.jpg', GETDATE(), 0),
    (@b50, N'Vu? Tr? Không C?n Th??ng D?',      '978-604-1-10050-1', @pNXBTre,    2022, N'Ti?ng Vi?t', N'L?n 1', N'Gi?i thích khoa h?c v? ngu?n g?c vu? tr? và s? s?ng mà không c?n d?n gi? thuy?t v? d?ng t?o hóa.',        '/images/50.jpg', GETDATE(), 0),
    (@b51, N'M?i Sáng Th?c D?y',                '978-604-1-10051-8', @pLaoDong,   2021, N'Ti?ng Vi?t', N'L?n 1', N'H??ng d?n xây d?ng thói quen bu?i sáng lành m?nh d? có m?t ngày hi?u qu? và tràn d?y nang l??ng.',        '/images/51.jpg', GETDATE(), 0),
    (@b52, N'D?c Nhân Tâm Cho Tu?i Tr?',        '978-604-1-10052-5', @pAlpha,     2020, N'Ti?ng Vi?t', N'L?n 1', N'Phiên b?n dành riêng cho gi?i tr? v?i ngôn ng? hi?n d?i, ví d? g?n gui và bài h?c giao ti?p thi?t th?c.',  '/images/52.jpg', GETDATE(), 0);

    -- =============================================================
    -- 5. TAC GIA - SACH (BookAuthors)
    -- =============================================================
    DECLARE @aNNA       INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Nguy?n Nh?t Ánh');
    DECLARE @aToHoai    INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Tô Hoài');
    DECLARE @aNamCao    INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Nam Cao');
    DECLARE @aVTP       INT = (SELECT AuthorId FROM Authors WHERE FullName = N'V? Tr?ng Ph?ng');
    DECLARE @aNgDu      INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Nguy?n Du');
    DECLARE @aCoelho    INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Paulo Coelho');
    DECLARE @aCarnegie  INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Dale Carnegie');
    DECLARE @aTolstoy   INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Leo Tolstoy');
    DECLARE @aHosseini  INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Khaled Hosseini');
    DECLARE @aDuhigg    INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Charles Duhigg');
    DECLARE @aKahneman  INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Daniel Kahneman');
    DECLARE @aKishimi   INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Ichiro Kishimi');
    DECLARE @aHill      INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Napoleon Hill');
    DECLARE @aCovey     INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Stephen Covey');
    DECLARE @aClear     INT = (SELECT AuthorId FROM Authors WHERE FullName = N'James Clear');
    DECLARE @aHarari    INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Yuval Noah Harari');
    DECLARE @aKarnow    INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Stanley Karnow');
    DECLARE @aFeynman   INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Richard Feynman');
    DECLARE @aHawking   INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Stephen Hawking');
    DECLARE @aFriedman  INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Thomas Friedman');
    DECLARE @aRies      INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Eric Ries');
    DECLARE @aThiel     INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Peter Thiel');
    DECLARE @aRowling   INT = (SELECT AuthorId FROM Authors WHERE FullName = N'J.K. Rowling');
    DECLARE @aSaintEx   INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Antoine de Saint-Exupéry');
    DECLARE @aSepulveda INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Luis Sepúlveda');
    DECLARE @aDarwin    INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Charles Darwin');
    DECLARE @aOrwell    INT = (SELECT AuthorId FROM Authors WHERE FullName = N'George Orwell');
    DECLARE @aPuzo      INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Mario Puzo');
    DECLARE @aHugo      INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Victor Hugo');
    DECLARE @aDumas     INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Alexandre Dumas');
    DECLARE @aPhanHuyLe INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Phan Huy Lê');
    DECLARE @aLudwig    INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Emil Ludwig');
    DECLARE @aMatthes   INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Eric Matthes');
    DECLARE @aMarx      INT = (SELECT AuthorId FROM Authors WHERE FullName = N'Karl Marx');

    INSERT INTO BookAuthors (BookId, AuthorId) VALUES
    (@b1,@aCoelho),    (@b2,@aCarnegie),  (@b3,@aTolstoy),    (@b4,@aHosseini),
    (@b5,@aNNA),       (@b6,@aToHoai),    (@b7,@aVTP),        (@b8,@aNgDu),
    (@b9,@aNamCao),    (@b10,@aNamCao),   (@b11,@aDuhigg),    (@b12,@aKahneman),
    (@b13,@aKishimi),  (@b14,@aHill),     (@b15,@aCovey),     (@b16,@aClear),
    (@b17,@aHarari),   (@b18,@aHarari),   (@b19,@aPhanHuyLe), (@b20,@aPhanHuyLe),
    (@b21,@aKarnow),   (@b22,@aLudwig),   (@b23,@aFeynman),   (@b24,@aHawking),
    (@b25,@aHawking),  (@b26,@aFriedman), (@b27,@aMatthes),   (@b28,@aRies),
    (@b29,@aThiel),    (@b30,@aMarx),     (@b31,@aPhanHuyLe), (@b32,@aOrwell),
    (@b33,@aOrwell),   (@b34,@aPuzo),     (@b35,@aHugo),      (@b36,@aDumas),
    (@b37,@aRowling),  (@b38,@aRowling),  (@b39,@aSaintEx),   (@b40,@aSepulveda),
    (@b41,@aNNA),      (@b42,@aNNA),      (@b43,@aNNA),       (@b44,@aDarwin),
    (@b45,@aHawking),  (@b46,@aMatthes),  (@b47,@aKahneman),  (@b48,@aKahneman),
    (@b49,@aPhanHuyLe),(@b50,@aHawking),  (@b51,@aClear),     (@b52,@aCarnegie);

    -- =============================================================
    -- 6. THE LOAI - SACH (BookCategories)
    -- =============================================================
    DECLARE @cVanHoc   INT = (SELECT CategoryId FROM Categories WHERE CategoryName = N'Van h?c');
    DECLARE @cKHKT     INT = (SELECT CategoryId FROM Categories WHERE CategoryName = N'Khoa h?c k? thu?t');
    DECLARE @cLichSu   INT = (SELECT CategoryId FROM Categories WHERE CategoryName = N'L?ch s?');
    DECLARE @cTamLy    INT = (SELECT CategoryId FROM Categories WHERE CategoryName = N'Tâm lý h?c');
    DECLARE @cKyNang   INT = (SELECT CategoryId FROM Categories WHERE CategoryName = N'K? nang s?ng');
    DECLARE @cThieuNhi INT = (SELECT CategoryId FROM Categories WHERE CategoryName = N'Thi?u nhi');
    DECLARE @cTrietHoc INT = (SELECT CategoryId FROM Categories WHERE CategoryName = N'Tri?t h?c');
    DECLARE @cKinhTe   INT = (SELECT CategoryId FROM Categories WHERE CategoryName = N'Kinh t? - Kinh doanh');
    DECLARE @cKHTN     INT = (SELECT CategoryId FROM Categories WHERE CategoryName = N'Khoa h?c t? nhiên');

    INSERT INTO BookCategories (BookId, CategoryId) VALUES
    -- Van hoc
    (@b1,@cVanHoc),  (@b3,@cVanHoc),  (@b4,@cVanHoc),  (@b5,@cVanHoc),
    (@b6,@cVanHoc),  (@b7,@cVanHoc),  (@b8,@cVanHoc),  (@b9,@cVanHoc),
    (@b10,@cVanHoc), (@b32,@cVanHoc), (@b33,@cVanHoc), (@b34,@cVanHoc),
    (@b35,@cVanHoc), (@b36,@cVanHoc), (@b37,@cVanHoc), (@b38,@cVanHoc),
    (@b39,@cVanHoc), (@b40,@cVanHoc), (@b42,@cVanHoc), (@b43,@cVanHoc),
    -- Ky nang song
    (@b2,@cKyNang),  (@b11,@cKyNang), (@b13,@cKyNang), (@b14,@cKyNang),
    (@b15,@cKyNang), (@b16,@cKyNang), (@b48,@cKyNang), (@b51,@cKyNang), (@b52,@cKyNang),
    -- Tam ly hoc
    (@b11,@cTamLy),  (@b12,@cTamLy),  (@b13,@cTamLy),  (@b16,@cTamLy),
    (@b47,@cTamLy),  (@b48,@cTamLy),
    -- Lich su
    (@b17,@cLichSu), (@b18,@cLichSu), (@b19,@cLichSu), (@b20,@cLichSu),
    (@b21,@cLichSu), (@b22,@cLichSu),
    -- Thieu nhi
    (@b6,@cThieuNhi),  (@b37,@cThieuNhi), (@b38,@cThieuNhi), (@b39,@cThieuNhi),
    (@b40,@cThieuNhi), (@b41,@cThieuNhi), (@b52,@cThieuNhi),
    -- Khoa hoc tu nhien
    (@b23,@cKHTN), (@b24,@cKHTN), (@b25,@cKHTN), (@b44,@cKHTN), (@b45,@cKHTN), (@b50,@cKHTN),
    -- Khoa hoc ky thuat
    (@b23,@cKHKT), (@b27,@cKHKT), (@b46,@cKHKT),
    -- Kinh te - Kinh doanh
    (@b26,@cKinhTe), (@b28,@cKinhTe), (@b29,@cKinhTe), (@b30,@cKinhTe),
    -- Triet hoc
    (@b30,@cTrietHoc), (@b31,@cTrietHoc), (@b49,@cTrietHoc);

    -- =============================================================
    -- 7. BAN SAO SACH (BookCopies) - 2~5 ban/sach | Tong: 183 ban
    --    Format barcode: BC-XXXXXX
    -- =============================================================
    -- B1: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b1,'BC-000001','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b1,'BC-000002','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b1,'BC-000003','Available',CAST(GETDATE() AS DATE));
    -- B2: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b2,'BC-000004','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b2,'BC-000005','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b2,'BC-000006','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b2,'BC-000007','Available',CAST(GETDATE() AS DATE));
    -- B3: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b3,'BC-000008','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b3,'BC-000009','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b3,'BC-000010','Available',CAST(GETDATE() AS DATE));
    -- B4: 2 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b4,'BC-000011','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b4,'BC-000012','Available',CAST(GETDATE() AS DATE));
    -- B5: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b5,'BC-000013','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b5,'BC-000014','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b5,'BC-000015','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b5,'BC-000016','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b5,'BC-000017','Available',CAST(GETDATE() AS DATE));
    -- B6: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b6,'BC-000018','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b6,'BC-000019','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b6,'BC-000020','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b6,'BC-000021','Available',CAST(GETDATE() AS DATE));
    -- B7: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b7,'BC-000022','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b7,'BC-000023','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b7,'BC-000024','Available',CAST(GETDATE() AS DATE));
    -- B8: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b8,'BC-000025','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b8,'BC-000026','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b8,'BC-000027','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b8,'BC-000028','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b8,'BC-000029','Available',CAST(GETDATE() AS DATE));
    -- B9: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b9,'BC-000030','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b9,'BC-000031','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b9,'BC-000032','Available',CAST(GETDATE() AS DATE));
    -- B10: 2 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b10,'BC-000033','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b10,'BC-000034','Available',CAST(GETDATE() AS DATE));
    -- B11: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b11,'BC-000035','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b11,'BC-000036','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b11,'BC-000037','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b11,'BC-000038','Available',CAST(GETDATE() AS DATE));
    -- B12: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b12,'BC-000039','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b12,'BC-000040','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b12,'BC-000041','Available',CAST(GETDATE() AS DATE));
    -- B13: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b13,'BC-000042','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b13,'BC-000043','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b13,'BC-000044','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b13,'BC-000045','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b13,'BC-000046','Available',CAST(GETDATE() AS DATE));
    -- B14: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b14,'BC-000047','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b14,'BC-000048','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b14,'BC-000049','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b14,'BC-000050','Available',CAST(GETDATE() AS DATE));
    -- B15: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b15,'BC-000051','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b15,'BC-000052','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b15,'BC-000053','Available',CAST(GETDATE() AS DATE));
    -- B16: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b16,'BC-000054','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b16,'BC-000055','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b16,'BC-000056','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b16,'BC-000057','Available',CAST(GETDATE() AS DATE));
    -- B17: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b17,'BC-000058','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b17,'BC-000059','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b17,'BC-000060','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b17,'BC-000061','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b17,'BC-000062','Available',CAST(GETDATE() AS DATE));
    -- B18: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b18,'BC-000063','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b18,'BC-000064','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b18,'BC-000065','Available',CAST(GETDATE() AS DATE));
    -- B19: 2 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b19,'BC-000066','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b19,'BC-000067','Available',CAST(GETDATE() AS DATE));
    -- B20: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b20,'BC-000068','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b20,'BC-000069','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b20,'BC-000070','Available',CAST(GETDATE() AS DATE));
    -- B21: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b21,'BC-000071','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b21,'BC-000072','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b21,'BC-000073','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b21,'BC-000074','Available',CAST(GETDATE() AS DATE));
    -- B22: 2 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b22,'BC-000075','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b22,'BC-000076','Available',CAST(GETDATE() AS DATE));
    -- B23: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b23,'BC-000077','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b23,'BC-000078','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b23,'BC-000079','Available',CAST(GETDATE() AS DATE));
    -- B24: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b24,'BC-000080','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b24,'BC-000081','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b24,'BC-000082','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b24,'BC-000083','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b24,'BC-000084','Available',CAST(GETDATE() AS DATE));
    -- B25: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b25,'BC-000085','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b25,'BC-000086','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b25,'BC-000087','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b25,'BC-000088','Available',CAST(GETDATE() AS DATE));
    -- B26: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b26,'BC-000089','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b26,'BC-000090','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b26,'BC-000091','Available',CAST(GETDATE() AS DATE));
    -- B27: 2 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b27,'BC-000092','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b27,'BC-000093','Available',CAST(GETDATE() AS DATE));
    -- B28: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b28,'BC-000094','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b28,'BC-000095','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b28,'BC-000096','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b28,'BC-000097','Available',CAST(GETDATE() AS DATE));
    -- B29: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b29,'BC-000098','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b29,'BC-000099','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b29,'BC-000100','Available',CAST(GETDATE() AS DATE));
    -- B30: 2 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b30,'BC-000101','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b30,'BC-000102','Available',CAST(GETDATE() AS DATE));
    -- B31: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b31,'BC-000103','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b31,'BC-000104','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b31,'BC-000105','Available',CAST(GETDATE() AS DATE));
    -- B32: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b32,'BC-000106','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b32,'BC-000107','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b32,'BC-000108','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b32,'BC-000109','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b32,'BC-000110','Available',CAST(GETDATE() AS DATE));
    -- B33: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b33,'BC-000111','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b33,'BC-000112','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b33,'BC-000113','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b33,'BC-000114','Available',CAST(GETDATE() AS DATE));
    -- B34: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b34,'BC-000115','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b34,'BC-000116','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b34,'BC-000117','Available',CAST(GETDATE() AS DATE));
    -- B35: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b35,'BC-000118','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b35,'BC-000119','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b35,'BC-000120','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b35,'BC-000121','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b35,'BC-000122','Available',CAST(GETDATE() AS DATE));
    -- B36: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b36,'BC-000123','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b36,'BC-000124','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b36,'BC-000125','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b36,'BC-000126','Available',CAST(GETDATE() AS DATE));
    -- B37: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b37,'BC-000127','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b37,'BC-000128','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b37,'BC-000129','Available',CAST(GETDATE() AS DATE));
    -- B38: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b38,'BC-000130','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b38,'BC-000131','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b38,'BC-000132','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b38,'BC-000133','Available',CAST(GETDATE() AS DATE));
    -- B39: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b39,'BC-000134','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b39,'BC-000135','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b39,'BC-000136','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b39,'BC-000137','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b39,'BC-000138','Available',CAST(GETDATE() AS DATE));
    -- B40: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b40,'BC-000139','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b40,'BC-000140','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b40,'BC-000141','Available',CAST(GETDATE() AS DATE));
    -- B41: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b41,'BC-000142','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b41,'BC-000143','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b41,'BC-000144','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b41,'BC-000145','Available',CAST(GETDATE() AS DATE));
    -- B42: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b42,'BC-000146','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b42,'BC-000147','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b42,'BC-000148','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b42,'BC-000149','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b42,'BC-000150','Available',CAST(GETDATE() AS DATE));
    -- B43: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b43,'BC-000151','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b43,'BC-000152','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b43,'BC-000153','Available',CAST(GETDATE() AS DATE));
    -- B44: 2 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b44,'BC-000154','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b44,'BC-000155','Available',CAST(GETDATE() AS DATE));
    -- B45: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b45,'BC-000156','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b45,'BC-000157','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b45,'BC-000158','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b45,'BC-000159','Available',CAST(GETDATE() AS DATE));
    -- B46: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b46,'BC-000160','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b46,'BC-000161','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b46,'BC-000162','Available',CAST(GETDATE() AS DATE));
    -- B47: 5 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b47,'BC-000163','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b47,'BC-000164','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b47,'BC-000165','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b47,'BC-000166','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b47,'BC-000167','Available',CAST(GETDATE() AS DATE));
    -- B48: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b48,'BC-000168','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b48,'BC-000169','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b48,'BC-000170','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b48,'BC-000171','Available',CAST(GETDATE() AS DATE));
    -- B49: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b49,'BC-000172','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b49,'BC-000173','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b49,'BC-000174','Available',CAST(GETDATE() AS DATE));
    -- B50: 2 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b50,'BC-000175','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b50,'BC-000176','Available',CAST(GETDATE() AS DATE));
    -- B51: 4 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b51,'BC-000177','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b51,'BC-000178','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b51,'BC-000179','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b51,'BC-000180','Available',CAST(GETDATE() AS DATE));
    -- B52: 3 ban
    INSERT INTO BookCopies (CopyId,BookId,Barcode,Status,AddedDate) VALUES
    (NEWID(),@b52,'BC-000181','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b52,'BC-000182','Available',CAST(GETDATE() AS DATE)),
    (NEWID(),@b52,'BC-000183','Available',CAST(GETDATE() AS DATE));

    COMMIT TRANSACTION;

    PRINT '=== DU LIEU MAU DA THEM THANH CONG ===';
    PRINT '  10 Nha Xuat Ban';
    PRINT '  34 Tac Gia';
    PRINT '  10 The Loai';
    PRINT '  52 Sach (anh 1.jpg -> 52.jpg)';
    PRINT '  183 Ban Sao (BC-000001 -> BC-000183)';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'LOI: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
