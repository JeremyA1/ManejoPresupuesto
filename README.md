# ManejoPresupuesto

Aplicación web para registrar ingresos y gastos y consultar el movimiento de cuentas personales.

## Funciones

- Registro, inicio de sesión y recuperación de contraseña por correo.
- Administración de tipos de cuentas, cuentas y categorías.
- Registro y edición de transacciones.
- Consultas semanales, mensuales y en calendario.
- Reportes en Excel.

## Tecnologías

ASP.NET Core MVC sobre .NET 10, Razor, ASP.NET Core Identity, SQL Server, Dapper, Bootstrap y ClosedXML.

## Ejecutar en desarrollo

Se necesitan el SDK de .NET 10 y una base de datos SQL Server con las tablas y los procedimientos utilizados por los repositorios. Este proyecto no incluye un script para crear la base de datos.

Desde la carpeta que contiene `ManejoPresupuesto.slnx`:

```powershell
dotnet restore ManejoPresupuesto.slnx
cd ManejoPresupuesto
Copy-Item appsettings.Development.example.json appsettings.Development.json
```

Copia el archivo de ejemplo solo en una instalación nueva. Si ya tienes `appsettings.Development.json`, conserva tu configuración existente.

Configura `ConnectionStrings:DefaultConnection` para tu instancia y base de datos. Si la conexión incluye credenciales, guárdala mediante secretos de usuario:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "TU_CADENA_DE_CONEXION"
dotnet user-secrets set "CONFIGURACIONES_EMAIL:EMAIL" "TU_CORREO_EMISOR"
dotnet user-secrets set "CONFIGURACIONES_EMAIL:PASSWORD" "TU_CREDENCIAL_SMTP"
```

El archivo de ejemplo utiliza SMTP de Gmail en el puerto 587; ajusta el servidor y el puerto si utilizas otro proveedor. La credencial SMTP debe ser válida para el método de autenticación de ese proveedor.

```powershell
dotnet build
dotnet run --launch-profile http
```

Abre `http://localhost:5132`. El registro y las operaciones de la aplicación requieren una conexión a SQL Server correctamente configurada. La recuperación de contraseña también requiere acceso al servidor SMTP.

## Archivos locales

El `.gitignore` excluye configuraciones locales, credenciales, bases de datos, compilaciones y archivos temporales de Visual Studio. El archivo `appsettings.Development.example.json` documenta la estructura necesaria sin incluir credenciales.
