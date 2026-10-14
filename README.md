# 🧾 Invoice Generator API

A full-stack invoice management system built with **.NET 8**, **Python FastAPI**, and **SQL Server** — containerized with Docker.

## ✨ Features

- 🔐 JWT Authentication (Register / Login)
- 👥 Client Management
- 🧾 Invoice Creation with line items
- 📄 PDF Generation (professional layout via ReportLab)
- 📧 Email invoices directly to clients (SMTP)
- 🔄 Invoice status tracking: Draft → Sent → Paid → Overdue
- 🐳 Fully Dockerized

## 🏗️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend API | .NET 8 (ASP.NET Core) |
| PDF Service | Python 3.11 + FastAPI + ReportLab |
| Database | SQL Server 2022 |
| Auth | JWT Bearer Tokens |
| Email | MailKit (SMTP) |
| Container | Docker + Docker Compose |

## 🚀 Getting Started

### Prerequisites
- Docker Desktop

### Run the project

1. Clone the repo
2. Create a `.env` file in the root:
```env
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASS=your-app-password
```

3. Start all services:
```bash
docker-compose up --build
```

4. Open Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)

## 📡 API Endpoints

### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login and get token |

### Clients
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/clients` | List all clients |
| POST | `/api/clients` | Add a client |
| DELETE | `/api/clients/{id}` | Delete a client |

### Invoices
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/invoices` | List all invoices |
| POST | `/api/invoices` | Create invoice |
| GET | `/api/invoices/{id}` | Get invoice details |
| PATCH | `/api/invoices/{id}/status` | Update status |
| GET | `/api/invoices/{id}/pdf` | Download PDF |
| POST | `/api/invoices/{id}/send-email` | Email invoice to client |
| DELETE | `/api/invoices/{id}` | Delete invoice |

## 📁 Project Structure

```
invoice-generator-api/
├── backend-dotnet/         # .NET 8 API
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   ├── DTOs/
│   ├── Data/
│   └── Dockerfile
├── python-pdf-service/     # FastAPI PDF generator
│   ├── routers/
│   ├── models/
│   ├── services/
│   └── Dockerfile
└── docker-compose.yml
```

## 👨‍💻 Author

Built as a freelance portfolio project.
