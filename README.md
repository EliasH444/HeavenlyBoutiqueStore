# 📘 Learning Platform (ASP.NET Core MVC)

A full-stack web application built using **ASP.NET Core MVC**, demonstrating authentication, role-based access control, database integration, and third-party service integration.

---

## 🚀 Overview

This project is a learning platform built to explore and implement real-world backend and full-stack engineering concepts using the .NET ecosystem.

It includes:
- User authentication & authorization (ASP.NET Identity)
- Role-based access control (Admin/User)
- PostgreSQL database integration
- Email services
- Image handling service abstraction
- Stripe payment integration
- Admin user seeding on startup

---

## 🧱 Tech Stack

- **Backend:** ASP.NET Core MVC (.NET)
- **Database:** PostgreSQL (Entity Framework Core)
- **Authentication:** ASP.NET Identity
- **Payments:** Stripe API
- **Email Service:** SMTP (Gmail)
- **Architecture:** Service-based layered architecture (DI-driven)

---

## 🔐 Features

### 👤 Authentication & Authorization
- User registration and login
- Secure password handling via ASP.NET Identity
- Role-based access control (Admin role included)

### 🧑‍💼 Admin System
- Automatic admin user creation on startup
- Role seeding during application boot
- Admin-only protected functionality

### 🗄️ Database
- Entity Framework Core ORM
- Automatic migrations applied at startup
- PostgreSQL backend integration

### 📧 Email Service
- SMTP-based email sending service abstraction
- Configurable via application settings

### 💳 Payments
- Stripe integration for payment processing
- Configured via secure app settings

### 🖼️ Image Handling
- Local image service abstraction
- Designed for extensibility (cloud storage ready)

---

## 🏗️ Architecture

The project follows a service-based architecture:

- **Controllers** → Handle HTTP requests  
- **Services** → Business logic layer (dependency injection)  
- **Data Layer** → EF Core DbContext  
- **Models** → Domain entities  
- **Configuration** → Managed via `appsettings.json`

---

## ⚙️ Setup Instructions

### 1. Clone repository
```bash
git clone https://github.com/yourusername/learning.git
