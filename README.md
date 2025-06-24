# 🏢 Million Project

> Sistema de Gestión Inmobiliaria con Next.js y .NET Core

## 📋 Tabla de Contenidos
- [Descripción](#-descripción)
- [Tecnologías](#-tecnologías)
- [Requisitos](#-requisitos)
- [Instalación](#-instalación)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Uso](#-uso)
- [Docker](#-docker)
- [Desarrollo](#-desarrollo)
- [Contribución](#-contribución)

## 📝 Descripción
Million Project es una aplicación web moderna para la gestión inmobiliaria que permite administrar propiedades, propietarios y realizar seguimiento de transacciones inmobiliarias.

### ✨ Características Principales
- 🏠 Gestión de propiedades
- 👥 Administración de propietarios
- 📊 Seguimiento de transacciones
- 🖼️ Gestión de imágenes de propiedades
- 🔐 Sistema de autenticación seguro

## 🛠️ Tecnologías

### Frontend
- ⚛️ Next.js 15.3.3
- 🎨 Material-UI
- 📱 Diseño Responsive
- 🔒 JWT Authentication

### Backend
- 🔷 .NET Core 8.0
- 🗄️ MongoDB
- 🔑 JWT Authentication
- 📚 Swagger Documentation

### DevOps
- 🐳 Docker & Docker Compose
- 🔄 CI/CD Ready

## 📋 Requisitos

- 🐳 Docker Desktop
- 📦 Node.js (para desarrollo)
- 🔷 .NET SDK 8.0 (para desarrollo)
- 💻 VS Code o Visual Studio 2022

## 🚀 Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/gamartinez4/millions_project.git
   cd millions_project
   ```

2. **Construir y ejecutar con Docker**
   ```bash
   # Construir las imágenes
   docker-compose build

   # Iniciar los servicios
   docker-compose up -d
   ```

3. **Verificar la instalación**
   ```bash
   # Ver el estado de los contenedores
   docker-compose ps
   ```

## 📁 Estructura del Proyecto

```
millions_project/
├── Frontend/                # Aplicación Next.js
│   ├── src/
│   │   ├── app/           # Páginas y rutas
│   │   ├── components/    # Componentes React
│   │   └── context/      # Contextos de React
│   └── public/           # Archivos estáticos
│
├── Backened/              # API .NET Core
│   ├── src/
│   │   ├── Million.Api/           # API Controllers
│   │   ├── Million.Application/   # Lógica de negocio
│   │   ├── Million.Domain/        # Modelos y entidades
│   │   └── Million.Infrastructure/# Acceso a datos
│   └── tests/                     # Pruebas unitarias
│
└── docker-compose.yml     # Configuración de Docker
└── mongo-init.js     # Archivo de configuración DB en MongoDB 
└── workspace     # Workspace VSCode
└── README.md 
```

## 💻 Uso

### 🌐 Acceder a la Aplicación
- Frontend: [http://localhost:3000](http://localhost:3000)
- Backend API: [http://localhost:5249](http://localhost:5249)
- Swagger UI: [http://localhost:5249/swagger](http://localhost:5249/swagger)

### 🔑 Credenciales por Defecto
```json
{
  "Username": "john",
  "Password": "password"
}
```

## 🐳 Docker

### Comandos Útiles

```bash
# Iniciar servicios
docker-compose up -d

# Detener servicios
docker-compose down

# Ver logs
docker-compose logs -f

# Reiniciar un servicio específico
docker-compose restart frontend
docker-compose restart backend
docker-compose restart mongodb
```

### 📊 Puertos
| Servicio | Puerto |
|----------|--------|
| Frontend | 3000   |
| Backend  | 5249   |
| MongoDB  | 27017  |

## 👨‍💻 Desarrollo

### Frontend
```bash
cd Frontend
npm install
npm run dev
```

### Backend
```bash
cd Backened
dotnet restore
dotnet run --project src/Million.Api
```

## 🤝 Contribución

1. 🍴 Fork el proyecto
2. 🔨 Crea tu Feature Branch (`git checkout -b feature/AmazingFeature`)
3. 💾 Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. 📤 Push a la Branch (`git push origin feature/AmazingFeature`)
5. 🔃 Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE.md](LICENSE.md) para más detalles.

---

<div align="center">
  <p>Desarrollado con ❤️ por Tu Equipo</p>
  <p>
    <a href="https://github.com/tu-usuario">GitHub</a> •
    <a href="https://tu-sitio-web.com">Website</a>
  </p>
</div>
