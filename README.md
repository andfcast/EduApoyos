# SOLUCIÓN EDUAPOYOS
EduApoyos, una aplicación web que permita a los asesores registrar y gestionar solicitudes de apoyo económico, y a los estudiantes consultar el estado de su solicitud desde un portal de autogestión.

Actualmente la solución se divide en dos componentes:

# Backend
Contiene las APIs y la conexión a la base de datos de estudiantes, asesores y solicitudes
# Frontend
Contiene la interfaz gráfica de aplicación con la que los usuarios finales harán la respectiva interacción.


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
