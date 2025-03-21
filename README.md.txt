# Training Company API

## Overview
The Training Company API is a RESTful service built using .NET Core that manages trainers, courses, and payments. It provides CRUD operations for courses and trainers, assigns trainers to courses, processes payments, and generates reports.

## 📌 Technical Stack
- **Backend:** .NET Core
- **Database:** Entity Framework Core (SQL Server)
- **Authentication:** JWT
- **Logging:** Serilog
- **Testing:** xUnit
- **Caching:** In-Memory Cache
- **Reporting:** PDF Library (for generating reports)



## 🔥 API Endpoints

### **Authentication**
- **Register:** `POST /api/Auth/Register`
- **Login:** `POST /api/Auth/Login`

### **Trainer Management**
- **Add Trainer:** `POST /api/Trainer`
- **Update Trainer:** `PUT /api/Trainer/{id}`
- **Delete Trainer:** `DELETE /api/Trainer/{id}`
- **Get Trainer Details:** `GET /api/Trainer/{id}`
- **List All Trainers:** `GET /api/Trainer`

### **Course Management**
- **Add Course:** `POST /api/Course`
- **Update Course:** `PUT /api/Course/{id}`
- **Delete Course:** `DELETE /api/Course/{id}`
- **Get Course Details:** `GET /api/Course/{id}`
- **List All Courses:** `GET /api/Course`

### **Trainer-Course Assignment**
- **Assign Trainer to Course:** `POST /api/TrainerCourse`
- **Get Trainer’s Courses:** `GET /api/TrainerCourse/{trainerId}`
- **List All Course-Trainer Links:** `GET /api/TrainerCourse`

### **Payments**
- **Add Payment:** `POST /api/Payment`
- **Get Payments for a Trainer & Course:** `GET /api/Payment?trainerId={id}&courseId={id}`

### **Reporting**
- **Export Course-Trainer Payments Report as PDF:** `GET /api/TrainerCourse/export-pdf`



