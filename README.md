# 🏠 Proyecto Inmobiliaria – .NET 9 MVC - ULP

Sistema de **gestión de alquileres de propiedades** para una agencia inmobiliaria.  
Desarrollado en **ASP.NET Core MVC (.NET 9)** con base de datos **MySQL** y administración vía **phpMyAdmin** en contenedores Docker.

---

## 📦 Requisitos previos

Antes de comenzar, asegurate de tener instalado:

- [Git](https://git-scm.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [SDK .NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

---

## 🚀 Instalación y ejecución del proyecto

### 1️⃣ Clonar el repositorio

```bash
git clone https://github.com/FedericoDG/proyecto-inmobiliaria.git
cd proyecto-inmobiliaria
```

### 2️⃣ Levantar base de datos y phpMyAdmin con Docker

En la raíz del proyecto encontrarás un archivo docker-compose.yml.
Ejecuta:

```bash
docker compose up -d
```

Esto creará:

- MySQL en localhost:3306
- phpMyAdmin en http://localhost:8080

Credenciales por defecto (modificables en `docker-compose.yml`):

- Usuario: root
- Password: 1234
- Base de datos: inmobiliaria

### 3️⃣ Configurar la conexión en `appsettings.Development.json`

```json
"ConnectionStrings": {
  "MySqlConnection": "Server=localhost;Port=3306;Database=inmobiliaria;User=root;Password=1234;"
}
```

### 4️⃣ Restaurar dependencias y compilar

```bash
dotnet restore
dotnet build
```

### 5️⃣ Ejecutar el proyecto

```bash
dotnet run
```
