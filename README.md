# 🎓 EduStream — Modern Role-Based Online Course & Video Streaming Platform

A robust, enterprise-grade Web Application built with **ASP.NET Core 8 MVC**, **Entity Framework Core**, **SQL Server**, and **ASP.NET Core Identity**. 

Designed with modern UI/UX principles and strict **OWASP Top 10 Security Standards**, EduStream provides a seamless learning environment for students and an intuitive management dashboard for course instructors.

---

## 🌟 Key Application Features

### 👨‍🏫 Instructor / Teacher Features
- **Course Lifecycle Management:** Create, edit, and delete courses with instant category assignment and cover image processing.
- **Dynamic Lesson Management:**
  - Upload `.mp4` video lessons with max file size (500 MB) and binary header signature checks.
  - Reorder lessons numerically within the curriculum.
  - Replace existing lesson video files or update titles and descriptions seamlessly.
- **Instructor Dashboard:** View enrolled student counts, course breakdowns, and manage existing course video lists.

### 👨‍🎓 Student Features
- **Course Discovery & Smart Filtering:**
  - Browse courses across multiple dynamic categories (Web Development, Mobile App, Game Dev, AI/Data Science, Cyber Security, etc.).
  - Search by course title or keyword with an instant search & filter reset mechanism.
- **Seamless Enrollment:** Free one-click enrollment system for all published courses.
- **Interactive Video Player & Progress Tracker:**
  - Dedicated video watch page with a real-time curriculum navigation sidebar.
  - Interactive lesson completion toggle.
  - Dynamic progress bar showing live percentage (%) of overall course completion.

---

## 🛡️ OWASP Top 10 Aligned Security Architecture

- **Secure File Upload (`IFileService` Layer):**
  - **Magic Bytes Validation:** Inspects raw file binary signatures (headers) to prevent malicious executable files (`.exe`, `.php`, `.sh`) disguised as images or `.mp4` videos from being uploaded (prevents Remote Code Execution / RCE attacks).
  - **Path Traversal Protection:** All filenames are sanitized and re-assigned unique GUIDs (`Guid.NewGuid():N`).
- **Authorization & Access Control (RBAC & IDOR Prevention):**
  - Strict role-based access checks (`[Authorize(Roles = "Teacher")]`, `[Authorize(Roles = "Student")]`).
  - Ownership verification on data modification requests (`TeacherId == currentUserId`) preventing Insecure Direct Object References (IDOR).
- **Cross-Site Scripting (XSS) & CSRF Protection:**
  - Automatic Razor HTML output encoding.
  - Anti-Forgery CSRF Tokens enforced on state-changing requests (`[ValidateAntiForgeryToken]`).
- **SQL Injection Prevention:** 100% Parameterized queries via Entity Framework Core LINQ.
- **HTTPS & Security Headers:**
  - Mandatory HTTPS redirection (`https://localhost:7230`).
  - Strict Security Headers enforced on all HTTPS responses:
    - `X-Frame-Options: DENY` (Prevents Clickjacking attacks)
    - `X-Content-Type-Options: nosniff` (Prevents MIME-type sniffing)
    - `X-XSS-Protection: 1; mode=block` (Cross-Site Scripting protection)
  - Secure Cookie configuration (`HttpOnly`, `SameSite`, and `SecurePolicy.Always`).

---

## 📐 Project Structure

CoursePlatform/
├── Areas/
│   └── Identity/            # Custom Identity Razor Pages (Login, Register, Logout)
├── Controllers/
│   ├── CourseController.cs  # Public course discovery & detail actions
│   ├── HomeController.cs    # Hero section & landing page
│   ├── StudentController.cs # Enrollment & video watch page with progress tracking
│   └── TeacherController.cs # Instructor course & lesson CRUD operations
├── Data/
│   ├── ApplicationDbContext.cs # EF Core DbContext with model configurations
│   └── DbSeeder.cs          # Automatic Role & Default Categories seeding
├── Models/                  # Core Entity Models (Course, Lesson, Enrollment, LessonProgress)
├── Services/                # File validation & sanitization services (IFileService)
├── ViewModels/              # Strongly-typed Data Transfer Objects for Forms & Views
├── Views/                   # Modern, responsive Razor views
└── wwwroot/                 # Static assets (Custom CSS, Bootstrap 5, Uploads)

---

## 🛠️ Tech Stack & Dependencies

- **Framework:** .NET 8.0 (ASP.NET Core MVC)
- **Database:** SQL Server
- **ORM:** Entity Framework Core 8.0 (Code-First approach with Migrations)
- **Authentication & Authorization:** ASP.NET Core Identity & Role-Based Security
- **Frontend & UI:** Razor Views, Bootstrap 5, Bootstrap Icons, Custom Modern CSS

---

## ⚙️ Local Setup & Running Guide

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or Express instance)

### Installation Steps

1. **Clone the Repository:**
   git clone [https://github.com/YOUR_USERNAME/YOUR_REPOSITORY_NAME.git](https://github.com/YOUR_USERNAME/YOUR_REPOSITORY_NAME.git)
   cd CoursePlatform

2. **Configure Database Connection:**
   Ensure your local SQL Server instance connection string is set correctly in appsettings.json:
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CoursePlatformDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }

3. **Apply Database Migrations:**
   dotnet ef database update

4. **Trust HTTPS Development Certificate:**
   dotnet dev-certs https --trust

5. **Run the Application:**
   dotnet run
   
   Navigate to https://localhost:7230 in your browser.

---

## 📝 License

This project is created as an open-source educational portfolio project.