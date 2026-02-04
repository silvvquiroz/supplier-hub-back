# SupplierHubAPI - Backend

Descripción
-----------
Proyecto backend para la plataforma SupplierHub. Este repositorio contiene la API que expone los endpoints relacionados con la gestión de proveedores y utilidades (helpers). Aquí se definen los controladores, modelos y la configuración necesaria para ejecutar la API y conectar con servicios externos utilizados por la plataforma.

Principales responsabilidades
---------------------------
- Exponer endpoints REST para crear, leer, actualizar y eliminar proveedores.
- Proveer endpoints helper para obtener la lista de países y consultas a servicios externos (ScrapX: OFAC y Offshore).
- Persistencia de datos con Entity Framework Core y migraciones.

Despliegue en Azure
-------------------
Este backend ha sido desplegado en producción mediante los siguientes servicios de Microsoft Azure:

- **Azure App Services**: aloja la aplicación SupplierHubAPI. El servicio de aplicación gestiona la ejecución, escalado automático y monitoreo de la API.
- **Azure SQL Database**: base de datos relacional SQL Server administrada que almacena los datos de proveedores y configuración.

URL base de producción: https://supplier-hub-api-fha8daf4g7gzdhag.spaincentral-01.azurewebsites.net

Consideraciones de despliegue:
- Las migraciones de Entity Framework Core se aplican durante el proceso de despliegue o antes del arranque de la aplicación.
- Las cadenas de conexión y credenciales sensibles se configuran mediante variables de entorno en Azure App Services (Configuration settings / Connection strings).
- Se mantienen backups automáticos de Azure SQL Database según la política de retención configurada.


Estructura relevante del proyecto
---------------------------------
- Controladores: [SupplierHubAPI/Controllers/ProveedorController.cs](SupplierHubAPI/Controllers/ProveedorController.cs) y [SupplierHubAPI/Controllers/HelperController.cs](SupplierHubAPI/Controllers/HelperController.cs)
- Modelos: [SupplierHubAPI/Models/Proveedor.cs](SupplierHubAPI/Models/Proveedor.cs), [SupplierHubAPI/Models/ApplicationDbContext.cs](SupplierHubAPI/Models/ApplicationDbContext.cs) y modelos auxiliares en [SupplierHubAPI/Models](SupplierHubAPI/Models)
- Configuración y arranque: [SupplierHubAPI/Program.cs](SupplierHubAPI/Program.cs)

API - Endpoints principales
---------------------------
Todos los endpoints están prefijados por `api/Proveedor` o `api/Helper` según corresponda.

- Obtener todos los proveedores activos (sin paginar)
	- Método: GET
	- Ruta: `api/Proveedor/all`
	- Respuesta: colección de `Proveedor` filtrada por `Activo == true`.

- Obtener proveedores paginados
	- Método: GET
	- Ruta: `api/Proveedor` (parámetros query: `page`, `pageSize`)
	- Respuesta: objeto con `TotalProveedores`, `TotalPages`, `CurrentPage` y `Proveedores`.

- Obtener un proveedor por id
	- Método: GET
	- Ruta: `api/Proveedor/{id}`
	- Comportamiento: devuelve 404 si no existe o si `Activo == false`.

- Crear proveedor
	- Método: POST
	- Ruta: `api/Proveedor`
	- Cuerpo: objeto `Proveedor` (validaciones definidas en el modelo)
	- Respuesta: 201 Created con la entidad creada.

- Actualizar proveedor
	- Método: PUT
	- Ruta: `api/Proveedor/{id}`
	- Cuerpo: objeto `Proveedor` con `Id` coincidente
	- Respuesta: 204 No Content o errores apropiados (400/404).

- Eliminar proveedor (eliminación lógica)
	- Método: DELETE
	- Ruta: `api/Proveedor/{id}`
	- Comportamiento: marca `Activo = false` y actualiza `FechaUltimaEdicion`.

- Consultas a APIs externas (ScrapX)
	- OFAC: GET `api/Proveedor/external/ofac/{entityName}` → devuelve `ScrapXResponse<OfacResult>`
	- Offshore: GET `api/Proveedor/external/offshore/{entityName}` → devuelve `ScrapXResponse<OffShoreResult>`
	- Estas llamadas usan un `HttpClient` configurado (nombre `ExternalApi`) en el arranque del servicio.

- Obtener lista de países (helper)
	- Método: GET
	- Rutas disponibles: `api/Helper/paises` y `api/Proveedor/paises`
	- Comportamiento: consulta `https://restcountries.com` y devuelve una lista de nombres de países.

Modelo `Proveedor` 
---------------------------
Campos principales definidos en [SupplierHubAPI/Models/Proveedor.cs](SupplierHubAPI/Models/Proveedor.cs):

- `Id` (int)
- `RazonSocial` (string) — requerido
- `NombreComercial` (string) — requerido
- `IdentificacionTributaria` (string) — requerido, regex para 11 dígitos
- `NumeroTelefonico` (string?) — formato de teléfono
- `CorreoElectronico` (string?) — formato email
- `SitioWeb` (string?) — formato URL
- `DireccionFisica` (string?)
- `Pais` (string) — requerido
- `FacturacionAnual` (decimal) — decimal(18,2), >= 0
- `FechaUltimaEdicion` (DateTime) — actualizado en crear/actualizar/eliminar lógico
- `Activo` (bool) — marca si el proveedor está activo (eliminación lógica)

Persistencia y migraciones
--------------------------
- Contexto EF Core: [SupplierHubAPI/Models/ApplicationDbContext.cs](SupplierHubAPI/Models/ApplicationDbContext.cs). Contiene el DbSet `Proveedores` y la configuración de tipo para `FacturacionAnual`.
- Migraciones: la carpeta `SupplierHubAPI/Migrations` contiene las migraciones generadas. Para aplicar migraciones en una base de datos local:

```bash
cd SupplierHubAPI
dotnet ef database update
```

Configuración y variables
-------------------------
- Revisar `appsettings.json` y `appsettings.Development.json` para cadenas de conexión y configuración.
- En `Program.cs` se configura el `HttpClient` con nombre `ExternalApi` usado por `ProveedorController` para las llamadas a ScrapX.

Ejecución local
---------------
Comandos básicos para compilar y ejecutar la API en entorno de desarrollo:

```bash
cd SupplierHubAPI
dotnet build
dotnet run
```

Tras ello la API estará disponible en el endpoint que indique la salida de `dotnet run` (por defecto en `https://localhost:5001` o según `launchSettings.json`).

Buenas prácticas
---------------
- Asegurar cadenas de conexión y secretos mediante variables de entorno en despliegue.
- Configurar `HttpClient` y tiempos de espera para llamadas externas.
- Ejecutar migraciones en entornos controlados antes de abrir la API en producción.

Colección Postman
-----------------
Se incluye una colección de Postman para facilitar pruebas y demostraciones de uso de la API. Para mayor referencia y para probar los endpoints use la colección `SupplierHub.postman_collection.json` incluida en el repositorio.

Archivo de la colección: [SupplierHub.postman_collection.json](SupplierHub.postman_collection.json)

