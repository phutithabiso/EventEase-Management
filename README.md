# 🎉 EventEase Management System

**EventEase** is a web-based event management platform designed to streamline the process of organizing, booking, and managing events and venues. Built with ASP.NET Core MVC and Entity Framework, the application supports both admin and user interactions, providing robust tools for venue listing, event creation, booking, and user role-based dashboards.

---

## 📌 Table of Contents

- [Features](#-features)
- [Technologies](#-technologies)
- [Setup Instructions](#-setup-instructions)
- [Database Schema](#-database-schema)
- [User Roles](#-user-roles)
- [Screenshots](#-screenshots)
- [Future Improvements](#-future-improvements)

---

## ✅ Features

- Event and venue CRUD operations (Admins only).
- Book events with real-time availability checks.
- Upload and manage venue and event images using Azure Blob Storage.
- Display feedback messages (success or error) via TempData.
- Custom error handling for improved user experience.
- Filtering and searching for events and bookings.
- Separate dashboards for Admin and Customer views.

---

## 🧰 Technologies

- **Frontend:** Razor Pages, HTML5, CSS3, Bootstrap 5
- **Backend:** ASP.NET Core MVC (.NET 6/7)
- **Database:** Microsoft SQL Server, Entity Framework Core
- **Cloud Storage:** Azure Blob Storage
- **Others:** LINQ, AutoMapper, TempData, ViewBag, Role-based Authorization

---

## ⚙️ Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/phutithabiso/EventEase-Management.git
   cd EventEase_Management
