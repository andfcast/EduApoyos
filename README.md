# SOLUCIÓN EDUAPOYOS
EduApoyos, una aplicación web que permita a los asesores registrar y gestionar solicitudes de apoyo económico, y a los estudiantes consultar el estado de su solicitud desde un portal de autogestión.

Actualmente la solución se divide en dos componentes:

# Backend
Contiene las APIs y la conexión a la base de datos de estudiantes, asesores y solicitudes. 
Se utiliza .NET 8 en su último release(8.0.29).

Adicionalmente, para propósitos de la prueba, se usó LocalDB como una versión ligera y simplificada de SQL Server Express pensada exclusivamente para desarrolladores.
Con LocalDB permite que ejecute las migraciones de Entity Framework Core, haga las pruebas y facilite el despliegue local sin depender de una conexión a Internet.
Permite fidelidad de motor con Azure SQL Database(si se desea depslegar en ambiente Cloud) al usar el mismo motor T-SQL, tipos de datos y lengueje que Azure SQL Database y
no hay que preocuparse con la infraestructura local.

Para la aplicación de las Unit Test, se utilizó xUnit con Mock, y WebApplicationFactory, utlizando BD de test o En Memoria para poder construir la data necesaria para poder
propiciar las condiciones establecidas para los distintos escenarios de prueba.

## Arquitectura

El proyecto está construido bajo una Arquitectura en Capas / Clean Architecture desacoplada, sólida y completamente orientada a las buenas prácticas de desarrollo empresarial.
Con este esquema, permite que la lógica del negocio permanezca independiente de los marcos de trabajo, las bases de datos y la interfaz de usuario.
Adicionalmente se aplicó la Inyección de Dependencias para desacoplar las clases y facilitar el mocking durante las pruebas unitarias, mediante 
el registro de repositorios, `IUnitOfWork` y servicios del dominio en el contenedor nativo de .NET ('Program.cs') mediante Scoped Services.


### Descripción de las capas

### Capa de Dominio y Datos (Data & Domain Layer)

Se utlizó EntityFramework Core 8 mediante mediante el enfoque Code-First con migraciones controladas.
Se utilizó el patron Generic Repository que abstrae las operaciones CRUD fundamentales de la base de datos, evitando código duplicado en el acceso a datos.
Se utilizó el patrón UnitOfWork para que coordine las operaciones de múltiples repositorios en una sola transacción explícita de base de datos, garantizando consistencia de datos 
y atomicidad (cumplimiento ACID) durante operaciones complejas como la radicación de solicitudes e historial de estados.

### Capa de Aplicación y Servicios (Application / Business Layer)
Uso de BCrypt.Net para el hashing y salting seguro de contraseñas, y emisión de tokens JWT (JSON Web Tokens) para autenticación stateless (sin estado).
Se manejaron DTOs para que exista desacomplamiento total entre las entidades del dominio y los objetos expuestos en las operaciones declaradas en los controladores.
La paginación se hace en base de datos, ealizando la paginación a nivel de consulta SQL (Skip/Take) para minimizar la transferencia de datos y optimizar el consumo de memoria.

### Capa de Presentación (API Layer)
Se crearon endpoints RESTFul, garantizando un manejo correcto de códigos de estado HTTP, sin estado y en formato estandarizado(JSON)
Se construyó un middleware de Excepciones y Autorización por Roles.

### Capa de infraestructura
Se realizó el Control de Acceso basado en Roles y Claims (RBAC / Claims-Based Authorization) para centralizar la seguridad y los permisos de usuario
Extracción de claims como 'Role' y 'UserId' desde los tokens JWT para aplicar reglas de negocio diferenciadas (por ejemplo, filtrar solicitudes si el rol es Estudiante o dar acceso total a Asesor)


# Frontend
Contiene la interfaz gráfica de aplicación con la que los usuarios finales harán la respectiva interacción.
Se utilizó la última versión disponible de Angular(v. 22), garantizando una interfaz reactiva, modular y escalable para dos tipos de usuarios principales: estudiantes y asesores.
Se eligió Angular, en primer lugar, por mayor afinidad con el framework, y segundo, proporciona un sistema de ruteo robusto, gestión de formularios tipados y un cliente HTTP integrado 
ideal para consumir APIs RESTful en .NET.

## Decisiones tomadas
A nivel visual, se eligió usar Angular Material onjunto de componentes accesibles y responsivos (tablas paginadas, modales/diálogos, formularios, menus y navegación) bajo los estándares 
de Material Design, reduciendo el tiempo de desarrollo de UI y asegurando consistencia visual.

Con la versión actual, se hizo refinamiento de las Signals, y usando también RxJsPermite un manejo fluido e inmutable del flujo de datos, la gestión reactiva de llamadas HTTP asíncronas y el control de eventos en tiempo real,
lo cual se había convertido en uno de los incovenientes más comunes en desarrollo de formularios reactivos y el manejo de eventos a partir de los cambios aplicados en tiempo real. 

Se dispuso de la implementación de un AuthGuard para interceptar la navegación y proteger rutas privadas y se agregó la validación del token JWT emitido por la API, verificando la validez 
del token y los roles de usuario (Estudiante vs. Asesor) antes de permitir el acceso a las vistas correspondientes.

Se usó el HttpInterceptor para proporcionar inyección automática de Tokens: Adjunta el encabezado Authorization: Bearer <token> a todas las peticiones salientes hacia la API, y la gestión centralizada de errores, ya que captura 
respuestas de error de la API (como 401 Unauthorized o 403 Forbidden) para redirigir al login o mostrar notificaciones amigables.

Para el manejo de los formularios, se construyeron formularios con validación en tiempo real (formatos de correo, contraseñas seguras, campos requeridos) para los módulos de Login, Registro y Creación de Solicitudes de Apoyo.

## Ejecución y despliegue

Aunque existan los archivos dotnet.yml, angular-ci.yml y docker-compose.yml declarados, y potencialmente funcionales(en especial si se revisa la comprobación realizada en Github al procesar los PR y hacer la operación de CI,
para la ejecución de la aplicación se optó por el método clásico de levantar las dos instancias por aparte usando los GUI y bash respectivos:

### Requisitos previos

.NET 8 SDK instalado.
Node.js (v20 o v22 LTS) y npm instalados.
Instancia local de SQL Server o LocalDB disponible.

### Pasos a seguir

1. Descargar el proyecto. 
2. En línea de comandos(cmd), ubicarse en la carpeta EduApoyosBackend
3. Ejecutamos comando para restaurar paquetes Nuget:

	dotnet restore
	
4. Si previamente no se ha hecho, aplicar las migraciones a la BD:

	dotnet ef database update
	
5. Iniciar la ejecución teniendo en cuenta que está habilitado el https:

	dotnet run --launch-profile https
	
El backend se cargará en la dirección https://localhost:7232/
Para comprobarlo y observar la pantalla generada por Swagger, puede entrar a https://localhost:7232/swagger/index.html

6. Abrir otra terminal de cmd, y ubicarse en la carpeta EduApoyosWebApp
7. Ejecutar comando para descargar e instalar los paquetes que usa la aplicacíón

	npm install
	
8. Ejecutar el comando para lanzar el frontend

	ng serve -o

El frontend se desplegará y se podrá ver en la dirección http://localhost:4200/

## Datos iniciales
Las tablas básicas tienen ya información anidada y va incluída en la declaración de la base de datos.
Se tiene un usuario asesor con la siguiente informacion:
correo: admin@eduapoyos.com
pass: Prueba123*

Sin embargo, si desea, puede registrar nuevos asesores y estudiantes en la página de registro.
O desde el perfil de asesor, puede crear estudiantes.


## Scripts solicitados en el enunciado

Los scripts se encuentran en la carpeta Scripts de la raíz del proyecto.

## Infraestructura y Servicios Cloud (Azure)

Para el despliegue y operación en producción de la plataforma **EduApoyos**, se selecciona una arquitectura PaaS (Platform as a Service) en Microsoft Azure, optimizada para alta disponibilidad, seguridad y costos escalables.

### 1. Azure App Service
* **Servicio:** Hosting de la Web API (.NET 8) y la aplicación Web Frontend (Angular 22).
* **Justificación de Tier y Configuración:**
  * **Tier:** **Basic (B1)** para entornos de pruebas/demo o **Standard (S1)** para entorno productivo inicial.
  * **Justificación:** 
    * El nivel **Basic (B1)** ofrece recursos dedicados (1 vCPU, 1.75 GB RAM) suficientes para soportar las solicitudes de estudiantes y asesores con un costo controlado.
    * Para producción se recomienda **Standard (S1)**, ya que habilita soporte para *Auto-scaling* (escalado horizontal automático en épocas de alta demanda de convocatorias), enlaces de dominios personalizados con certificados SSL/TLS automáticos y soporta *Deployment Slots* (Staging/Production) para despliegues sin tiempo de inactividad (*Zero-downtime deployment*).

### 2. Azure SQL Database
* **Servicio:** Base de datos relacional para el almacenamiento de Usuarios, Estudiantes, Solicitudes de Apoyo, Tipos de Apoyo e Historiales.
* **Justificación de Tier:**
  * **Tier:** **Standard (DTU-based) - S0 a S1** (10-20 DTUs) o **General Purpose (Serverless vCore)**.
  * **Justificación:** 
    * El nivel **Basic (5 DTUs)** resultaría insuficiente por los límites de transacciones concurrentes cuando múltiples estudiantes radican solicitudes simultáneamente.
    * El nivel **Standard (S1)** garantiza un rendimiento constante, lecturas/escrituras fluidas en transacciones de la base de datos `DbSistemaApoyos`, soporte para copias de seguridad automáticas (Point-in-Time Restore) y cifrado transparente de datos (TDE - Transparent Data Encryption) por defecto.

### 3. Azure Blob Storage
* **Servicio:** Almacenamiento no estructurado de archivos y documentos adjuntos (Requisito RF-04: Carga de soportes académicos, socioeconómicos y certificados).
* **Justificación:**
  * **Tier/Acceso:** **Standard Hot / Cool Access Tier** en una cuenta de almacenamiento tipo *StorageV2 (general purpose v2)*.
  * **Justificación:** 
    * Almacenar documentos PDF o imágenes directamente en la base de datos SQL degrada drásticamente el rendimiento de las consultas y eleva los costos de almacenamiento relacional.
    * Azure Blob Storage proporciona una solución altamente escalable, de bajo costo y segura. Permite generar URLs firmadas temporales (*Shared Access Signatures - SAS tokens*) para que solo los asesores autorizados y el propio estudiante puedan visualizar/descargar los soportes adjuntos a las solicitudes.

### 4. Azure Key Vault
* **Servicio:** Gestión centralizada de secretos, claves de cifrado y cadenas de conexión en producción.
* **Justificación:**
  * **Tier:** **Standard**.
  * **Justificación:** 
    * Cumple con las buenas prácticas de seguridad (*Zero Trust* y OWASP) al evitar el almacenamiento de credenciales en código fuente o archivos `appsettings.json`.
    * Centraliza la custodia de la cadena de conexión a **Azure SQL Database**, los secretos del firmante de tokens **JWT** (`Jwt:Key`), la credencial de acceso a **Azure Blob Storage** y las claves del hash de passwords. Se integra de manera nativa y transparente con **Azure App Service** mediante *Managed Identities* (Identidades Administradas).
