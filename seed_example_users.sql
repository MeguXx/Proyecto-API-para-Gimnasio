-- Script SQL para insertar usuarios de ejemplo en GYMAPI (SQL Server)
-- Ejecutar después de crear la base de datos con las tablas
-- Password para todos: "password123"

-- Primero, asegurar que los roles existen
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'ADMIN') INSERT INTO Roles (Name) VALUES ('ADMIN');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'ENTRENADOR') INSERT INTO Roles (Name) VALUES ('ENTRENADOR');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'SOCIO') INSERT INTO Roles (Name) VALUES ('SOCIO');

-- Usuarios de ejemplo
-- ADMIN: Franklin Garcia
INSERT INTO Users (Username, PasswordHash, Email, IsActive) VALUES ('fgarcia', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'franklin.garcia@example.com', 1);
DECLARE @AdminUserId INT = SCOPE_IDENTITY();
INSERT INTO UserRoles (UserId, RoleId) SELECT @AdminUserId, RoleId FROM Roles WHERE Name = 'ADMIN';

-- ENTRENADOR: Ricky Estrella
INSERT INTO Users (Username, PasswordHash, Email, IsActive) VALUES ('restrella', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'ricky.estrella@example.com', 1);
DECLARE @EntrenadorUserId INT = SCOPE_IDENTITY();
INSERT INTO UserRoles (UserId, RoleId) SELECT @EntrenadorUserId, RoleId FROM Roles WHERE Name = 'ENTRENADOR';
INSERT INTO Entrenadores (UserId, Especialidad, Certificaciones, FechaIngreso, IsActive) VALUES (@EntrenadorUserId, 'Fuerza y acondicionamiento', 'Certificado en Entrenamiento Personal', '2023-01-15', 1);

-- SOCIO 1: Maria Lopez
INSERT INTO Users (Username, PasswordHash, Email, IsActive) VALUES ('mlopez', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'maria.lopez@example.com', 1);
DECLARE @Socio1UserId INT = SCOPE_IDENTITY();
INSERT INTO UserRoles (UserId, RoleId) SELECT @Socio1UserId, RoleId FROM Roles WHERE Name = 'SOCIO';
INSERT INTO Socios (UserId, FechaNacimiento, Telefono, Direccion, FechaIngreso, IsActive) VALUES (@Socio1UserId, '1990-05-20', '555-1234', 'Calle Ficticia 123', '2024-01-01', 1);

-- SOCIO 2: Carlos Ramirez
INSERT INTO Users (Username, PasswordHash, Email, IsActive) VALUES ('cramirez', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'carlos.ramirez@example.com', 1);
DECLARE @Socio2UserId INT = SCOPE_IDENTITY();
INSERT INTO UserRoles (UserId, RoleId) SELECT @Socio2UserId, RoleId FROM Roles WHERE Name = 'SOCIO';
INSERT INTO Socios (UserId, FechaNacimiento, Telefono, Direccion, FechaIngreso, IsActive) VALUES (@Socio2UserId, '1985-08-10', '555-5678', 'Avenida Imaginaria 456', '2024-02-15', 1);

-- SOCIO 3: Ana Torres
INSERT INTO Users (Username, PasswordHash, Email, IsActive) VALUES ('atorres', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'ana.torres@example.com', 1);
DECLARE @Socio3UserId INT = SCOPE_IDENTITY();
INSERT INTO UserRoles (UserId, RoleId) SELECT @Socio3UserId, RoleId FROM Roles WHERE Name = 'SOCIO';
INSERT INTO Socios (UserId, FechaNacimiento, Telefono, Direccion, FechaIngreso, IsActive) VALUES (@Socio3UserId, '1995-12-03', '555-9012', 'Plaza Inventada 789', '2024-03-20', 1);

-- Membresías de ejemplo
IF NOT EXISTS (SELECT 1 FROM Membresias WHERE Nombre = 'Básica') INSERT INTO Membresias (Nombre, Descripcion, DuracionDias, Precio, EsRenovable, IsActive, CreatedAt) VALUES ('Básica', 'Acceso básico al gimnasio', 30, 50.00, 1, 1, GETDATE());
IF NOT EXISTS (SELECT 1 FROM Membresias WHERE Nombre = 'Premium') INSERT INTO Membresias (Nombre, Descripcion, DuracionDias, Precio, EsRenovable, IsActive, CreatedAt) VALUES ('Premium', 'Acceso completo con entrenador', 30, 100.00, 1, 1, GETDATE());

-- Asignar membresías a socios
INSERT INTO SocioMembresias (SocioId, MembresiaId, FechaInicio, FechaFin, IsActive)
SELECT s.SocioId, m.MembresiaId, '2024-01-01', '2024-12-31', 1
FROM Socios s CROSS JOIN Membresias m
WHERE s.UserId = @Socio1UserId AND m.Nombre = 'Básica' AND NOT EXISTS (SELECT 1 FROM SocioMembresias WHERE SocioId = s.SocioId AND MembresiaId = m.MembresiaId);

INSERT INTO SocioMembresias (SocioId, MembresiaId, FechaInicio, FechaFin, IsActive)
SELECT s.SocioId, m.MembresiaId, '2024-02-15', '2024-12-31', 1
FROM Socios s CROSS JOIN Membresias m
WHERE s.UserId = @Socio2UserId AND m.Nombre = 'Premium' AND NOT EXISTS (SELECT 1 FROM SocioMembresias WHERE SocioId = s.SocioId AND MembresiaId = m.MembresiaId);

INSERT INTO SocioMembresias (SocioId, MembresiaId, FechaInicio, FechaFin, IsActive)
SELECT s.SocioId, m.MembresiaId, '2024-03-20', '2024-12-31', 1
FROM Socios s CROSS JOIN Membresias m
WHERE s.UserId = @Socio3UserId AND m.Nombre = 'Básica' AND NOT EXISTS (SELECT 1 FROM SocioMembresias WHERE SocioId = s.SocioId AND MembresiaId = m.MembresiaId);

-- Asignar entrenador a algunos socios
INSERT INTO SocioEntrenadors (SocioId, EntrenadorId, FechaAsignacion, IsActive)
SELECT s.SocioId, e.EntrenadorId, '2024-01-01', 1
FROM Socios s CROSS JOIN Entrenadores e
WHERE s.UserId IN (@Socio1UserId, @Socio2UserId) AND e.UserId = @EntrenadorUserId AND NOT EXISTS (SELECT 1 FROM SocioEntrenadors WHERE SocioId = s.SocioId AND EntrenadorId = e.EntrenadorId);

-- Ejercicios de ejemplo
IF NOT EXISTS (SELECT 1 FROM Ejercicios WHERE Nombre = 'Press de banca') INSERT INTO Ejercicios (Nombre, Descripcion, GrupoMuscular, IsActive) VALUES ('Press de banca', 'Ejercicio para pecho', 'Pecho', 1);
IF NOT EXISTS (SELECT 1 FROM Ejercicios WHERE Nombre = 'Sentadillas') INSERT INTO Ejercicios (Nombre, Descripcion, GrupoMuscular, IsActive) VALUES ('Sentadillas', 'Ejercicio para piernas', 'Piernas', 1);

-- Rutina de ejemplo
INSERT INTO Rutinas (Nombre, Descripcion, EntrenadorId, IsActive)
SELECT 'Rutina Básica', 'Rutina para principiantes', EntrenadorId, 1
FROM Entrenadores WHERE UserId = @EntrenadorUserId AND NOT EXISTS (SELECT 1 FROM Rutinas WHERE Nombre = 'Rutina Básica' AND EntrenadorId = Entrenadores.EntrenadorId);

DECLARE @RutinaId INT = (SELECT TOP 1 RutinaId FROM Rutinas WHERE Nombre = 'Rutina Básica' ORDER BY RutinaId DESC);

-- RutinaEjercicios
INSERT INTO RutinaEjercicios (RutinaId, EjercicioId, Series, Repeticiones, Peso, Notas)
SELECT @RutinaId, e.EjercicioId, 3, 10, 50, 'Ejecutar con control'
FROM Ejercicios e WHERE e.Nombre = 'Press de banca' AND NOT EXISTS (SELECT 1 FROM RutinaEjercicios WHERE RutinaId = @RutinaId AND EjercicioId = e.EjercicioId);

INSERT INTO RutinaEjercicios (RutinaId, EjercicioId, Series, Repeticiones, Peso, Notas)
SELECT @RutinaId, e.EjercicioId, 3, 12, 0, 'Mantener espalda recta'
FROM Ejercicios e WHERE e.Nombre = 'Sentadillas' AND NOT EXISTS (SELECT 1 FROM RutinaEjercicios WHERE RutinaId = @RutinaId AND EjercicioId = e.EjercicioId);

-- Asistencias de ejemplo
INSERT INTO Asistencias (SocioId, FechaHoraEntrada, Observaciones)
SELECT s.SocioId, '2024-04-01 08:00:00', 'Buen entrenamiento'
FROM Socios s WHERE s.UserId = @Socio1UserId AND NOT EXISTS (SELECT 1 FROM Asistencias WHERE SocioId = s.SocioId AND FechaHoraEntrada = '2024-04-01 08:00:00');

INSERT INTO Asistencias (SocioId, FechaHoraEntrada, Observaciones)
SELECT s.SocioId, '2024-04-02 07:45:00', NULL
FROM Socios s WHERE s.UserId = @Socio2UserId AND NOT EXISTS (SELECT 1 FROM Asistencias WHERE SocioId = s.SocioId AND FechaHoraEntrada = '2024-04-02 07:45:00');