<<<<<<< HEAD
# GYMAPI

API RESTful para gestión de gimnasio desarrollada en .NET Core 8 con SQL Server.

## Requisitos

- .NET 8 SDK
- SQL Server 2022
- Postman para pruebas

## Instalación

1. Clonar el repositorio.
2. Ejecutar el script SQL `script-bdgym.sql` en SQL Server para crear la base de datos.
3. Configurar la cadena de conexión en `appsettings.json`.
4. Ejecutar `dotnet run`.

## Autenticación

Usa JWT. Obtén un token con POST /api/auth/login.

Incluye el token en el header: `Authorization: Bearer {token}`

## Seguridad y Roles

- **ADMIN**: Gestiona socios, entrenadores y membresías. Puede registrar entrenadores y membresías.
- **ENTRENADOR**: Consulta socios asignados y registra asistencias.
- **SOCIO**: Consulta su historial de asistencia y plan de entrenamiento.

## Usuarios de Ejemplo

Ejecuta el script `seed_example_users.sql` después de crear la BD para insertar usuarios de prueba.

**Password para todos: "password123"**

- **ADMIN**: Franklin Garcia (username: fgarcia)
- **ENTRENADOR**: Ricky Estrella (username: restrella)
- **SOCIOS**:
  - Maria Lopez (username: mlopez)
  - Carlos Ramirez (username: cramirez)
  - Ana Torres (username: atorres)

## Endpoints

### Autenticación
- POST /api/auth/login (Público)
- POST /api/auth/register (ADMIN)
- GET /api/auth/perfil (Autenticado)

### Socios
- GET /api/socios (ADMIN)
- GET /api/socios/{id} (ADMIN, SOCIO)
- POST /api/socios (ADMIN)
- PUT /api/socios/{id} (ADMIN)
- DELETE /api/socios/{id} (ADMIN)
- POST /api/socios/{socioId}/membresia (ADMIN)
- GET /api/socios/{socioId}/rutinas (SOCIO)

### Entrenadores
- GET /api/entrenadores (ADMIN)
- GET /api/entrenadores/{id} (ADMIN)
- POST /api/entrenadores (ADMIN)
- PUT /api/entrenadores/{id} (ADMIN)
- DELETE /api/entrenadores/{id} (ADMIN)
- GET /api/entrenadores/{id}/socios (ADMIN, ENTRENADOR)

### Membresías
- GET /api/membresias (ADMIN)
- POST /api/membresias (ADMIN)
- GET /api/membresias/{id} (ADMIN, SOCIO)
- PUT /api/membresias/{id} (ADMIN)

### Asistencias
- POST /api/asistencias (ADMIN, ENTRENADOR)
- GET /api/asistencias/socio/{id} (ADMIN, ENTRENADOR)
- GET /api/asistencias/mi-historial (SOCIO)
- PUT /api/asistencias/{id} (ADMIN, ENTRENADOR)
- DELETE /api/asistencias/{id} (ADMIN, ENTRENADOR)
- GET /api/asistencias/reporte (ADMIN)

### Planes de Entrenamiento
- GET /api/planes (ADMIN, ENTRENADOR)
- POST /api/planes (ADMIN, ENTRENADOR)
- GET /api/planes/mi-plan (SOCIO)
- GET /api/planes/{id} (ADMIN, ENTRENADOR)
- PUT /api/planes/{id} (ADMIN, ENTRENADOR)
- DELETE /api/planes/{id} (ADMIN)

## Pruebas con Postman

Importa el archivo `postman_collection.json` en Postman para tener todos los endpoints listos para probar.

Configura las variables:
- `base_url`: URL de tu API (ej: https://localhost:5001)
- `token`: Se actualiza automáticamente al hacer login

Ejecuta los tests en orden: primero login, luego las operaciones protegidas.

## Casos de Prueba

| Caso de Prueba | Código HTTP | Resultado | Observación |
|----------------|-------------|-----------|-------------|
| POST /auth/login (ADMIN) | 200 OK | PASS | Token JWT generado correctamente |
| POST /auth/login (SOCIO) | 200 OK | PASS | Token con rol SOCIO en claims |
| POST /auth/login (cred. erróneas) | 401 Unauthorized | PASS | Mensaje de error correcto |
| POST /socios (como ADMIN) | 201 Created | PASS | Socio registrado exitosamente |
| POST /socios (como SOCIO) | 403 Forbidden | PASS | Acceso denegado correctamente |
| GET /socios (sin token) | 401 Unauthorized | PASS | Requiere autenticación |
| POST /entrenadores (ADMIN) | 201 Created | PASS | Entrenador creado correctamente |
| POST /membresias (ADMIN) | 201 Created | PASS | Membresía asignada al socio |
| POST /asistencias (ENTRENADOR) | 201 Created | PASS | Asistencia registrada con timestamp |
| GET /asistencias/mi-historial (SOCIO) | 200 OK | PASS | Solo muestra datos del socio auth. |
| GET /planes/mi-plan (SOCIO) | 200 OK | PASS | Plan de entrenamiento del socio |
| POST /socios (campos vacíos) | 400 Bad Request | PASS | Validación de DTO activa |
| GET /socios/9999 (inexistente) | 404 Not Found | PASS | Middleware de errores funcionando |

## Validación y Errores

- Validación automática con FluentValidation.
- Manejo centralizado de errores.

Importa la colección incluida. Prueba login y operaciones según roles.

## Diagrama ER

Ver el esquema en el script SQL.

