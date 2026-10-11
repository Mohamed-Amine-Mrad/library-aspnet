<h1 align="center">📚 Library Plus</h1>

<h3 align="center">
Library Management Web Application
</h3>

<p align="center">
  ASP.NET Core MVC • C# • Entity Framework Core • SQL Server
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET_9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white">
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white">
  <img src="https://img.shields.io/badge/Bootstrap-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white">
</p>

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=rect&color=00BFFF&height=2&section=header" width="80%">
</p>

## 📌 Overview

A web application to manage a library: books, authors, categories, students, loans and reservations. It has two sides, an **administration area** to run the library and a **student area** to search for books, reserve them and follow loans.

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=rect&color=00BFFF&height=2&section=header" width="80%">
</p>

## ✨ Features

### 🎓 Student area
- 🔐 Sign up and log in with ASP.NET Core Identity
- 🏠 Personal dashboard
- 🔎 Search books and open a detailed page for each one
- 📅 Reserve a book
- 📖 Follow current loans and reservations

### 🛡️ Administration area
- 📊 Dashboard with library statistics
- 📚 Manage books (create, view, edit, delete)
- ✍️ Manage authors and categories
- 👥 Manage students
- 🔁 Manage loans and reservations
- ⏰ Return alerts, per-book statistics and student scoring

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=rect&color=00BFFF&height=2&section=header" width="80%">
</p>

## 🔐 Roles & Security

Access is role-based, with **Administrator** and **Student** roles managed through ASP.NET Core Identity.

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=rect&color=00BFFF&height=2&section=header" width="80%">
</p>

## 🛠️ Tech Stack

**Architecture**

`Razor Views` → `MVC Controllers` → `Entity Framework Core` → `SQL Server`

**Tech Stack**

`ASP.NET Core MVC` · `.NET 9` · `C#` · `Entity Framework Core (Code First migrations)` · `ASP.NET Core Identity` · `SQL Server (LocalDB)` · `Razor` · `Bootstrap` · `jQuery Validation`

**Data model**

`Livre` · `Auteur` · `AuteurLivre` · `Categorie` · `Etudiant` · `Emprunt` · `Reservation`

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=rect&color=00BFFF&height=2&section=header" width="80%">
</p>

## 📁 Project Structure

```
LIBRARY_PLUS/
├── Controllers/   # admin and student controllers
├── Models/        # entities and view models
├── Views/         # Razor views (admin and student layouts)
├── Migrations/    # Entity Framework Core migrations
├── data/          # AppDbContext
├── wwwroot/       # static files (CSS, JS, images)
├── Program.cs     # app configuration
└── appsettings.json
```

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=rect&color=00BFFF&height=2&section=header" width="80%">
</p>

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (installed with Visual Studio) or another SQL Server instance
- The EF Core tools: `dotnet tool install --global dotnet-ef`

### 1. Clone

```bash
git clone https://github.com/Mohamed-Amine-Mrad/library-aspnet.git
cd library-aspnet
```

### 2. Check the connection string

The default connection string in `LIBRARY_PLUS/appsettings.json` points to LocalDB. Change it if you use another SQL Server instance.

### 3. Create the database

```bash
dotnet ef database update --project LIBRARY_PLUS
```

### 4. Run

```bash
dotnet run --project LIBRARY_PLUS
```

Open the address printed in the terminal.

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=rect&color=00BFFF&height=2&section=header" width="80%">
</p>

## 👨‍💻 Author

**Med Amine Mrad**, Software Engineering student at ITBS, Tunisia.

<p align="center">
  <a href="https://www.linkedin.com/in/mrad-mohamed-amine">
    <img src="https://img.shields.io/badge/LinkedIn-Connect-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white">
  </a>
  <a href="mailto:mohamedamine.mrad@itbs.tn">
    <img src="https://img.shields.io/badge/Email-Contact-EA4335?style=for-the-badge&logo=gmail&logoColor=white">
  </a>
</p>
