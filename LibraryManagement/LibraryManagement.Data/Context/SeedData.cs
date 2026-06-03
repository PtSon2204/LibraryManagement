using System;
using BCrypt.Net;
using LibraryManagement.Models;
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

            if (context.Roles.Any())
                return;

            // =========================
            // ROLES
            // =========================

            var adminRole = new Role
            {
                RoleName = "Admin",
                Description = "System administrator"
            };

            var staffRole = new Role 
            {
                RoleName = "Staff",
                Description = "Library staff"
            };

            var userRole = new Role
            {
                RoleName = "User",
                Description = "Library member"
            };

            context.Roles.AddRange(
                adminRole,
                staffRole,
                userRole
            );

            await context.SaveChangesAsync();

            // =========================
            // USERS
            // password: 123456
            // =========================

            var admin = new User
            {
                RoleId = adminRole.RoleId,
                Email = "admin@library.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                FullName = "System Admin",
                Phone = "0901111111",
                Address = "Hai Phong",
                Status = "Active"
            };

            var staff = new User
            {
                RoleId = staffRole.RoleId,
                Email = "staff@library.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                FullName = "Library Staff",
                Phone = "0902222222",
                Address = "Ha Noi",
                Status = "Active"
            };

            var member = new User
            {
                RoleId = userRole.RoleId,
                Email = "user@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                FullName = "Pham The Son",
                Phone = "0903333333",
                Address = "Hai Phong",
                Status = "Active"
            };

            context.Users.AddRange(
                admin,
                staff,
                member
            );

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

            context.Publishers.AddRange(
                kimDong,
                tre
            );

            await context.SaveChangesAsync();

            // =========================
            // AUTHORS
            // =========================

            var nna = new Author
            {
                FullName = "Nguyen Nhat Anh"
            };

            var jk = new Author
            {
                FullName = "J.K Rowling"
            };

            context.Authors.AddRange(
                nna,
                jk
            );

            // =========================
            // CATEGORIES
            // =========================

            var novel = new Category
            {
                CategoryName = "Novel"
            };

            var fantasy = new Category
            {
                CategoryName = "Fantasy"
            };

            context.Categories.AddRange(
                novel,
                fantasy
            );

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

            context.Books.AddRange(
                matBiec,
                hp
            );

            await context.SaveChangesAsync();

            // =========================
            // BOOK AUTHORS
            // =========================

            context.BookAuthors.AddRange(
                new BookAuthor
                {
                    BookId = matBiec.BookId,
                    AuthorId = nna.AuthorId
                },

                new BookAuthor
                {
                    BookId = hp.BookId,
                    AuthorId = jk.AuthorId
                }
            );

            // =========================
            // BOOK CATEGORIES
            // =========================

            context.BookCategories.AddRange(
                new BookCategory
                {
                    BookId = matBiec.BookId,
                    CategoryId = novel.CategoryId
                },

                new BookCategory
                {
                    BookId = hp.BookId,
                    CategoryId = fantasy.CategoryId
                }
            );

            await context.SaveChangesAsync();
        }
    }
}