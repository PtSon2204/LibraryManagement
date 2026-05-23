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

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC34CC5F8E3C");

            entity.Property(e => e.FullName).HasMaxLength(255);
        });

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

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B3AD84380");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E0882B8F80").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Fine>(entity =>
        {
            entity.HasKey(e => e.FineId).HasName("PK__Fines__9D4A9B2CA6D51367");

            entity.HasIndex(e => e.UserId, "IX_Fines_UserId");

            entity.Property(e => e.FineId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Unpaid");

            entity.HasOne(d => d.LoanDetail).WithMany(p => p.Fines)
                .HasForeignKey(d => d.LoanDetailId)
                .HasConstraintName("FK_Fines_LoanDetails");

            entity.HasOne(d => d.User).WithMany(p => p.Fines)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fines_Users");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId).HasName("PK__Loans__4F5AD457CBAEC671");

            entity.HasIndex(e => e.BorrowerUserId, "IX_Loans_BorrowerUserId");

            entity.HasIndex(e => e.ProcessedByUserId, "IX_Loans_ProcessedByUserId");

            entity.HasIndex(e => e.Status, "IX_Loans_Status");

            entity.Property(e => e.LoanId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BorrowedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Borrowed");

            entity.HasOne(d => d.BorrowerUser).WithMany(p => p.LoanBorrowerUsers)
                .HasForeignKey(d => d.BorrowerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loans_BorrowerUser");

            entity.HasOne(d => d.ProcessedByUser).WithMany(p => p.LoanProcessedByUsers)
                .HasForeignKey(d => d.ProcessedByUserId)
                .HasConstraintName("FK_Loans_ProcessedByUser");
        });

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

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F24EE6920B8");

            entity.ToTable(tb => tb.HasTrigger("TRG_Reservations_CopyMatchesBook"));

            entity.HasIndex(e => e.BookId, "IX_Reservations_BookId");

            entity.HasIndex(e => e.UserId, "IX_Reservations_UserId");

            entity.HasIndex(e => e.CopyId, "UX_Reservations_OneActiveReservationPerCopy")
                .IsUnique()
                .HasFilter("([CopyId] IS NOT NULL AND ([Status] IN ('Pending', 'ReadyForPickup')))");

            entity.HasIndex(e => new { e.UserId, e.BookId }, "UX_Reservations_OneActiveReservationPerMemberBook")
                .IsUnique()
                .HasFilter("([Status] IN ('Pending', 'ReadyForPickup'))");

            entity.Property(e => e.ReservationId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ReservationDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Book).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservations_Books");

            entity.HasOne(d => d.Copy).WithOne(p => p.Reservation)
                .HasForeignKey<Reservation>(d => d.CopyId)
                .HasConstraintName("FK_Reservations_BookCopies");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservations_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AC2BA1463");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61606178B3F3").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CF1485FA5");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasIndex(e => e.Status, "IX_Users_Status");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534911F2EB8").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
