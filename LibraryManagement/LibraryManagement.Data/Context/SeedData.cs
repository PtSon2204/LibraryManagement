using System;
using BCrypt.Net;
using LibraryManagement.Models.Context;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            // Chỉ seed nếu chưa có dữ liệu
            if (context.Accounts.Any())
                return;

            // =========================
            // ACCOUNTS (Admin + Librarian)
            // =========================

            var admin = new Account
            {
                Email = "admin@library.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "Admin",
                Status = "Active"
            };

            var librarian = new Account
            {
                Email = "librarian@library.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "Librarian",
                Status = "Active"
            };

            context.Accounts.AddRange(admin, librarian);
            await context.SaveChangesAsync();

            // =========================
            // READERS (Độc giả)
            // =========================

            var reader1 = new Reader
            {
                Email = "user@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Status = "Active"
            };

            context.Readers.Add(reader1);
            await context.SaveChangesAsync();

            // =========================
            // USER PROFILES
            // =========================

            context.UserProfiles.AddRange(
                new UserProfile
                {
                    AccountId = admin.AccountId,
                    FullName = "System Admin",
                    Phone = "0901111111",
                    Address = "Hai Phong"
                },
                new UserProfile
                {
                    AccountId = librarian.AccountId,
                    FullName = "Library Staff",
                    Phone = "0902222222",
                    Address = "Ha Noi"
                },
                new UserProfile
                {
                    ReaderId = reader1.ReaderId,
                    FullName = "Pham The Son",
                    Phone = "0903333333",
                    Address = "Hai Phong",
                    DateOfBirth = new DateOnly(2002, 4, 22)
                }
            );

            await context.SaveChangesAsync();

            // =========================
            // PUBLISHERS
            // =========================

            var kimDong = new Publisher
            {
                PublisherName = "NXB Kim Dong",
                Address = "Ha Noi",
                Email = "kimdong@gmail.com"
            };

            var tre = new Publisher
            {
                PublisherName = "NXB Tre",
                Address = "HCM"
            };

            context.Publishers.AddRange(kimDong, tre);

            // =========================
            // AUTHORS
            // =========================

            var nna = new Author { FullName = "Nguyen Nhat Anh" };
            var jk = new Author { FullName = "J.K Rowling" };

            context.Authors.AddRange(nna, jk);

            // =========================
            // CATEGORIES
            // =========================

            var novel = new Category { CategoryName = "Novel" };
            var fantasy = new Category { CategoryName = "Fantasy" };

            context.Categories.AddRange(novel, fantasy);

            await context.SaveChangesAsync();

            // =========================
            // BOOKS
            // =========================

            var matBiec = new Book
            {
                Title = "Mat Biec",
                ISBN = "978604209001",
                PublisherId = kimDong.PublisherId,
                PublicationYear = 2019,
                Language = "Vietnamese"
            };

            var hp = new Book
            {
                Title = "Harry Potter",
                ISBN = "978074753269",
                PublisherId = tre.PublisherId,
                PublicationYear = 1997,
                Language = "English"
            };

            context.Books.AddRange(matBiec, hp);
            await context.SaveChangesAsync();

            // =========================
            // BOOK AUTHORS
            // =========================

            context.BookAuthors.AddRange(
                new BookAuthor { BookId = matBiec.BookId, AuthorId = nna.AuthorId },
                new BookAuthor { BookId = hp.BookId, AuthorId = jk.AuthorId }
            );

            // =========================
            // BOOK CATEGORIES
            // =========================

            context.BookCategories.AddRange(
                new BookCategory { BookId = matBiec.BookId, CategoryId = novel.CategoryId },
                new BookCategory { BookId = hp.BookId, CategoryId = fantasy.CategoryId }
            );

            await context.SaveChangesAsync();
        }
    }
}