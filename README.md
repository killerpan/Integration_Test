# Sistema de Integración de Asistencia - Con Cliente Mock API

API de integración que se conecta al Mock API externo para obtener y procesar datos de asistencia.

## 🏗️ Arquitectura

Este proyecto incluye:
- ✅ API de integración completa (puerto 5001)
- ✅ Cliente HTTP para conectarse al Mock API (puerto 7001)
- ✅ Importación automática desde Mock API
- ✅ Importación manual (JSON/Excel)

## 🔗 Integración con Mock API

El sistema puede obtener datos automáticamente desde el Mock API usando HTTP.

### Nuevo Endpoint:
```
POST /api/attendance/import-from-mock?companyId=1
```

Este endpoint:
1. Se conecta al Mock API (localhost:7001)
2. Obtiene los datos de asistencia
3. Los importa automáticamente a la base de datos

## 🚀 Inicio Rápido

### 1. Ejecutar Mock API (Proyecto separado):
```bash
cd ../AttendanceMockAPI
dotnet run
# Corre en: https://localhost:7001
```

### 2. Ejecutar Integration API:
```bash
cd AttendanceIntegration.API
dotnet ef migrations add InitialCreate --project ../AttendanceIntegration.Infrastructure
dotnet ef database update
dotnet run
# Corre en: https://localhost:5001
```

### 3. Probar integración automática:
```bash
curl -X POST "https://localhost:5001/api/attendance/import-from-mock?companyId=1" -k
```

## 📚 Endpoints

### Importación Automática desde Mock API (NUEVO):
```
POST /api/attendance/import-from-mock?companyId=1
```

### Importación Manual (JSON):
```
POST /api/attendance/import
```

### Importación Manual (Excel):
```
POST /api/attendance/import-excel?companyId=1
```

### Consultar Asistencia:
```
GET /api/attendance/{companyId}?startDate=2025-10-01&endDate=2025-10-31
```

### Autenticación:
```
POST /api/auth/login
GET /api/auth/test-token
```

## ⚙️ Configuración

### appsettings.json:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=attendance.db"
  },
  "MockApiSettings": {
    "BaseUrl": "https://localhost:7001",
    "ExportEndpoint": "/api/attendance/export/{companyId}"
  }
}
```

## 🔐 Características

✅ **Integración automática** con Mock API vía HTTP  
✅ Autenticación JWT  
✅ Importación vía API (JSON)  
✅ Importación vía Excel (.xlsx)  
✅ Logging con Serilog  
✅ Auditoría completa  
✅ SQLite o SQL Server  
✅ Swagger/OpenAPI  

## 🧪 Casos de Uso

### 1. Importación Automática (desde Mock API):
```bash
curl -X POST "https://localhost:5001/api/attendance/import-from-mock?companyId=1" -k
```

### 2. Importación Manual (JSON):
```bash
curl -X POST "https://localhost:5001/api/attendance/import" \
  -k -H "Content-Type: application/json" \
  -d @datos.json
```

### 3. Importación Manual (Excel):
```bash
curl -X POST "https://localhost:5001/api/attendance/import-excel?companyId=1" \
  -k -F "file=@attendance.xlsx"
```

## 🎯 Flujo de Integración

```
Mock API (7001)
    ↓ HTTP GET
Integration API (5001)
    ↓ Procesa
SQLite Database
    ↓ Consulta
Sistema de Liquidaciones
```

## 🛠️ Tecnologías

- .NET 8
- HttpClient para integración
- Entity Framework Core 8
- SQLite / SQL Server
- Serilog
- Swagger/OpenAPI
- EPPlus (Excel)
- JWT Authentication
