using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Models;

namespace CoursePlatform.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // 1. Rolleri Tanımlama
        string[] roleNames = { "Teacher", "Student" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Varsayılan Kategorileri Yükleme
        var defaultCategories = new List<Category>
        {
            new Category { Name = "Web Geliştirme", Description = "ASP.NET Core, React, Angular, HTML/CSS Kursları" },
            new Category { Name = "Mobil Uygulama", Description = "Flutter, React Native, iOS, Android Kursları" },
            new Category { Name = "Veritabanı & SQL", Description = "SQL Server, PostgreSQL, MongoDB Kursları" },
            new Category { Name = "Oyun Geliştirme", Description = "Unity, Unreal Engine, C# Oyun Kursları" },
            new Category { Name = "Yapay Zeka & Veri Bilimi", Description = "Python, Machine Learning, Deep Learning Kursları" },
            new Category { Name = "Siber Güvenlik", Description = "Ağ Güvenliği, Sızma Testleri, Ethical Hacking" },
            new Category { Name = "Genel Yazılım", Description = "Algoritma, Temel Programlama ve Nesne Yönelimli Programlama" }
        };

        foreach (var category in defaultCategories)
        {
            var exists = await dbContext.Categories.AnyAsync(c => c.Name == category.Name);
            if (!exists)
            {
                dbContext.Categories.Add(category);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}