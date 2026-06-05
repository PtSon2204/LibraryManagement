using System;
using System.Collections.Generic;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models.Context;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<BookCategory> BookCategories { get; set; }

    public virtual DbSet<BookCopy> BookCopies { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Fine> Fines { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<LoanDetail> LoanDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    // --- Bảng mới thay thế Users + Roles ---
    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Authors ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC34CC5F8E3C");

            entity.Property(e => e.FullName).HasMaxLength(255);
        });

        // ── Books ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C2070B8D05FB");

            entity.HasIndex(e => e.Title, "IX_Books_Title");

            entity.HasIndex(e => e.ISBN, "UQ__Books__447D36EAF0D6485A").IsUnique();

            entity.Property(e => e.BookId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CoverImageUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Edition).HasMaxLength(100);
            entity.Property(e => e.ISBN)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK_Books_Publishers");
        });

        // ── BookAuthors ───────────────────────────────────────────────────────
        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.AuthorId });

            entity.HasOne(e => e.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(e => e.BookId);

            entity.HasOne(e => e.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(e => e.AuthorId);
        });

        // ── BookCategories ────────────────────────────────────────────────────
        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.CategoryId });

            entity.HasOne(e => e.Book)
                .WithMany(b => b.BookCategories)
                .HasForeignKey(e => e.BookId);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(e => e.CategoryId);
        });

        // ── BookCopies ────────────────────────────────────────────────────────
        modelBuilder.Entity<BookCopy>(entity =>
        {
            entity.HasKey(e => e.CopyId).HasName("PK__BookCopi__C26CCCC5205CE687");

            entity.HasIndex(e => e.BookId, "IX_BookCopies_BookId");
            entity.HasIndex(e => e.Status, "IX_BookCopies_Status");
            entity.HasIndex(e => e.Barcode, "UQ__BookCopi__177800D33A29574E").IsUnique();

            entity.Property(e => e.CopyId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddedDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.Barcode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Book).WithMany(p => p.BookCopies)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookCopies_Books");
        });

        // ── Categories ────────────────────────────────────────────────────────
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B3AD84380");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E0882B8F80").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // ── Publishers ────────────────────────────────────────────────────────
        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657FABD9BFF99F");

            entity.HasIndex(e => e.PublisherName, "UQ__Publishe__5F0E22495C071047").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PublisherName).HasMaxLength(255);
        });

        // ── Readers ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.ReaderId);

            entity.HasIndex(e => e.Email, "UQ_Readers_Email").IsUnique();
            entity.HasIndex(e => e.Status, "IX_Readers_Status");

            entity.Property(e => e.ReaderId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
        });

        // ── Accounts ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId);

            entity.HasIndex(e => e.Email, "UQ_Accounts_Email").IsUnique();
            entity.HasIndex(e => e.Role, "IX_Accounts_Role");

            entity.Property(e => e.AccountId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(30);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
        });

        // ── UserProfiles ──────────────────────────────────────────────────────
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserProfileId);

            entity.Property(e => e.UserProfileId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(500);

            // Unique: mỗi Reader chỉ có 1 profile
            entity.HasIndex(e => e.ReaderId, "UQ_UserProfiles_ReaderId")
                .IsUnique()
                .HasFilter("[ReaderId] IS NOT NULL");

            // Unique: mỗi Account chỉ có 1 profile
            entity.HasIndex(e => e.AccountId, "UQ_UserProfiles_AccountId")
                .IsUnique()
                .HasFilter("[AccountId] IS NOT NULL");

            // Check constraint: đúng 1 trong 2 FK phải có giá trị
            entity.ToTable(tb => tb.HasCheckConstraint(
                "CK_UserProfiles_OneOwner",
                "([ReaderId] IS NOT NULL AND [AccountId] IS NULL) OR ([ReaderId] IS NULL AND [AccountId] IS NOT NULL)"));

            // FK → Readers (1:1)
            entity.HasOne(p => p.Reader)
                .WithOne(r => r.Profile)
                .HasForeignKey<UserProfile>(p => p.ReaderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserProfiles_Readers");

            // FK → Accounts (1:1)
            entity.HasOne(p => p.Account)
                .WithOne(a => a.Profile)
                .HasForeignKey<UserProfile>(p => p.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserProfiles_Accounts");
        });

        // ── Fines ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Fine>(entity =>
        {
            entity.HasKey(e => e.FineId).HasName("PK__Fines__9D4A9B2CA6D51367");

            entity.HasIndex(e => e.LoanDetailId, "IX_Fines_LoanDetailId");
            entity.HasIndex(e => e.Status, "IX_Fines_Status");

            entity.Property(e => e.FineId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Unpaid");

            // FK → LoanDetails (NOT NULL: mọi khoản phạt phải gắn với 1 lần mượn)
            entity.HasOne(d => d.LoanDetail).WithMany(p => p.Fines)
                .HasForeignKey(d => d.LoanDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fines_LoanDetails");

            // FK → Payments (nullable: NULL = chưa thanh toán)
            entity.HasOne(d => d.Payment).WithMany(p => p.Fines)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Fines_Payments");
        });

        // ── Payments ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);

            entity.HasIndex(e => e.ReaderId, "IX_Payments_ReaderId");
            entity.HasIndex(e => e.ProcessedByAccountId, "IX_Payments_ProcessedByAccountId");
            entity.HasIndex(e => e.PaidAt, "IX_Payments_PaidAt");

            entity.Property(e => e.PaymentId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Method)
                .HasMaxLength(30);
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .IsRequired(false);
            entity.Property(e => e.PaidAt).HasDefaultValueSql("(sysdatetime())");

            // FK → Readers (Reader này nộp tiền)
            entity.HasOne(d => d.Reader).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Readers");

            // FK → Accounts (thủ thư xác nhận nhận tiền, nullable)
            entity.HasOne(d => d.ProcessedByAccount).WithMany(p => p.ProcessedPayments)
                .HasForeignKey(d => d.ProcessedByAccountId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Payments_ProcessedByAccount");
        });

        // ── Loans ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId).HasName("PK__Loans__4F5AD457CBAEC671");

            entity.HasIndex(e => e.BorrowerReaderId, "IX_Loans_BorrowerReaderId");
            entity.HasIndex(e => e.ProcessedByAccountId, "IX_Loans_ProcessedByAccountId");
            entity.HasIndex(e => e.Status, "IX_Loans_Status");

            entity.Property(e => e.LoanId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BorrowedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Borrowed");

            // FK → Readers (chỉ reader mới mượn được)
            entity.HasOne(d => d.BorrowerReader).WithMany(p => p.Loans)
                .HasForeignKey(d => d.BorrowerReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loans_BorrowerReader");

            // FK → Accounts (librarian/admin duyệt phiếu mượn)
            entity.HasOne(d => d.ProcessedByAccount).WithMany(p => p.ProcessedLoans)
                .HasForeignKey(d => d.ProcessedByAccountId)
                .HasConstraintName("FK_Loans_ProcessedByAccount");
        });

        // ── LoanDetails ───────────────────────────────────────────────────────
        modelBuilder.Entity<LoanDetail>(entity =>
        {
            entity.HasKey(e => e.LoanDetailId).HasName("PK__LoanDeta__760C10C83689AB7B");

            entity.HasIndex(e => e.CopyId, "IX_LoanDetails_CopyId");
            entity.HasIndex(e => e.LoanId, "IX_LoanDetails_LoanId");

            entity.HasIndex(e => e.CopyId, "UX_LoanDetails_OneActiveLoanPerCopy")
                .IsUnique()
                .HasFilter("([Status]='Borrowed')");

            entity.Property(e => e.LoanDetailId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Borrowed");

            entity.HasOne(d => d.Copy).WithOne(p => p.LoanDetail)
                .HasForeignKey<LoanDetail>(d => d.CopyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoanDetails_BookCopies");

            entity.HasOne(d => d.Loan).WithMany(p => p.LoanDetails)
                .HasForeignKey(d => d.LoanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoanDetails_Loans");
        });

        // ── Rooms ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId);

            entity.HasIndex(e => e.RoomName, "UQ_Rooms_RoomName").IsUnique();
            entity.HasIndex(e => e.Status, "IX_Rooms_Status");

            entity.Property(e => e.RoomId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.RoomName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Available");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
        });

        // ── Reservations (đặt phòng) ──────────────────────────────────────────
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId);

            entity.HasIndex(e => e.ReaderId, "IX_Reservations_ReaderId");
            entity.HasIndex(e => e.RoomId, "IX_Reservations_RoomId");
            entity.HasIndex(e => e.Status, "IX_Reservations_Status");

            // Mỗi Reader chỉ được có 1 đặt phòng đang active (Pending hoặc Confirmed)
            entity.HasIndex(e => e.ReaderId, "UQ_Reservations_OneActivePerReader")
                .IsUnique()
                .HasFilter("([Status] IN ('Pending', 'Confirmed'))");

            entity.Property(e => e.ReservationId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ReservationDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");

            // FK → Readers
            entity.HasOne(d => d.Reader).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservations_Readers");

            // FK → Rooms
            entity.HasOne(d => d.Room).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservations_Rooms");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
