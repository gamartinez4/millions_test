
## 📋 Table of Contents
- [Description](#-description)
- [Technologies](#-technologies)
- [Requirements](#-requirements)
- [Installation](#-installation)
- [Project Structure](#-project-structure)
- [Usage](#-usage)
- [Docker](#-docker)
- [Unit testing](#-development)
- [Contribution](#-contribution)

## 📝 Description
Million Project is a modern web application for real estate management that allows users to manage properties, owners, and track real estate transactions.

### ✨ Key Features
- 🏠 Property management
- 👥 Owner administration
- 📊 Transaction tracking
- 🖼️ Property image management
- 🔐 Secure authentication system

## 🛠️ Technologies

### Frontend
- ⚛️ Next.js 15.3.3
- 🎨 Material-UI
- 📱 Responsive Design
- 🔒 JWT Authentication

### Backend
- 🔷 .NET Core 8.0
- 🗄️ MongoDB
- 🔑 JWT Authentication
- 📚 Swagger Documentation

### DevOps
- 🐳 Docker & Docker Compose
- 🔄 CI/CD Ready

## 📋 Requirements

- 🐳 Docker Desktop
- 📦 Node.js (for development)
- 🔷 .NET SDK 8.0 (for development)
- 📚 Mongo DB
- 💻 VS Code or Visual Studio 2022

## 🚀 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/gamartinez4/millions_project.git
   cd millions_project
   ```

2. **Build and run with Docker**
   ```bash
   # Build and run image 
   docker-compose up --build
   ```

3. **Verify installation**
   ```bash
   # Check container status
   docker-compose ps
   ```

## 📁 Project Structure
millions_project/
├── Frontend/ # Next.js application
│ ├── src/
│ │ ├── app/ # Pages and routes
│ │ ├── components/ # React components
│ │ └── context/ # React contexts
│ └── public/ # Static files
│
├── Backened/ # .NET Core API
│ ├── src/
│ │ ├── Million.Api/ # API Controllers
│ │ ├── Million.Application/ # Business logic
│ │ ├── Million.Domain/ # Models and entities
│ │ └── Million.Infrastructure/# Data access
│ └── tests/ # Unit tests
│
└── docker-compose.yml # Docker configuration
└── mongo-init.js # MongoDB DB configuration file
└── workspace # VSCode workspace
└── README.md


## 💻 Usage

### 🌐 Access the Application
- Frontend: [http://localhost:3000](http://localhost:3000)
- Backend API: [http://localhost:5249](http://localhost:5249)
- Swagger UI: [http://localhost:5249/swagger](http://localhost:5249/swagger)

### 🔑 Default Credentials
```json
{
  "Username": "john",
  "Password": "password"
}
```

## 🐳 Docker

### Some useful Commands

```bash
# Start services
docker-compose up -d

# Stop services
docker-compose down -v

# View logs
docker-compose logs -f

# Restart specific service
docker-compose restart frontend
docker-compose restart backend
docker-compose restart mongodb
```

### 📊 Ports
| Service | Port |
|----------|--------|
| Frontend | 3000   |
| Backend  | 5249   |
| MongoDB  | 27017  |



## 👨‍💻 Unit Testing

### Frontend
```bash
cd Frontend
npm test -- --watchAll=false
```

### Backened
```bash
cd Backened
dotnet test
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

---

