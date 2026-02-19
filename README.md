# 📚 Sistema de Gestión de Libros - Clean Architecture

Este proyecto es una solución profesional de gestión bibliotecaria desarrollada con **.NET 8** y **Blazor**, diseñada bajo estándares de arquitectura empresarial para garantizar escalabilidad y mantenibilidad.

## 🛠️ Tecnologías Utilizadas

Este proyecto es una solución profesional de gestión bibliotecaria desarrollada con **.NET 8** y **Blazor WebAssembly**, diseñada bajo estándares de arquitectura empresarial para garantizar escalabilidad, mantenibilidad y buenas prácticas de desarrollo.

### 🔹 Backend
- **.NET 8**
- **ASP.NET Core Web API (.NET 8)**
- **Entity Framework Core**

### 🔹 Frontend
- **Blazor WebAssembly**
- **HttpClient / IHttpClientFactory**
- **Bootstrap (UI)**

### 🔹 Base de Datos
- **SQL Server**

✅ La arquitectura está diseñada en capas (API, Application, Infrastructure y Frontend), facilitando pruebas, mantenimiento y escalabilidad futura.


---

## 🏗️ Arquitectura del Sistema

El sistema implementa **Clean Architecture** (Arquitectura Limpia), lo que permite un desacoplamiento total entre la lógica de negocio y los detalles de infraestructura.

* **Domain (Núcleo):** El corazón del sistema. Contiene las entidades (`Autor`, `Libro`), el objeto `Result` y los tipos de error. Es la capa que no tiene dependencias externas (Cero acoplamiento).
* **Application:** Define las interfaces de servicios, los DTOs y la lógica de los casos de uso. Aquí reside la "inteligencia" del negocio.
* **Infrastructure:** Implementa el acceso a datos. Contiene el `DbContext`, las migraciones de **Entity Framework Core**, y la implementación del **Unit of Work** y los **Repositorios**.
* **Web API:** Capa de presentación que expone los endpoints. Utiliza controladores delgados que delegan la lógica a los servicios de aplicación.
* **Blazor UI:** Interfaz de usuario interactiva y moderna que consume la API utilizando un cliente HTTP nombrado y tipado.

## 📂 Estructura del Proyecto (Encarpetado)

A continuación se detalla la organización de la solución siguiendo el estándar de carpetas `src/tests`, agrupando los mecanismos de entrega en una capa de presentación:

```text
📂 Libreria
│
├── 📂 src (Código Fuente)
│   ├── 📂 Presentation (Capa de Presentación)
│   │   ├── 📂 API                  --> Web API (.NET 8)
│   │   │   ├── 📂 Controllers      --> Endpoints de Autores y Libros
│   │   │   ├── 📂 Middleware       --> Manejo global de excepciones (GlobalExceptionHandler)
│   │   │   └── 📄 appsettings.json --> Configuración de URL y Conexión
│   │   └── 📂 UI                   --> Cliente Blazor WASM
│   │       ├── 📂 Pages            --> Componentes Razor (CRUD)
│   │       └── 📄 Program.cs       --> Configuración HttpClient y DI
│   │
│   ├── 📂 Application (Lógica de Negocio)
│   │   ├── 📂 DTOs                 --> Objetos de transferencia (AutorDto, LibroDto)
│   │   ├── 📂 Interfaces           --> IAutorService, IUnitOfWork, IGenericRepository
│   │   ├── 📂 Services             --> Implementación de la lógica de negocio
│   │   └── 📂 Common               --> Mapeos, comportamientos y lógica compartida de aplicación
│   │
│   ├── 📂 Domain (Núcleo / Reglas de Empresa)
│   │   ├── 📂 Entities             --> Autor y Libro (Modelos de Base de Datos)
│   │   ├── 📂 Enums                --> Tipos de Error (NotFound, Conflict, etc.)
│   │   └── 📂 Common               --> Clase Result<T> y BaseError (Core del Result Pattern)
│   │
│   └── 📂 Infrastructure (Persistencia y Datos)
│       ├── 📂 Data                 --> ApplicationDbContext
│       ├── 📂 Repositories         --> Implementación de Repositorios y Unit of Work
│       └── 📂 Configuration        --> Fluent API (Configuración de relaciones SQL)
│
├── 📂 tests (Pruebas - Fase Planificada)
│   ├── 📂 UnitTests                --> Pruebas unitarias (xUnit + Moq)
│   └── 📂 IntegrationTests         --> Pruebas de integración (Base de datos real/InMemory)
│
├── 📄 .gitignore
├── 📄 SolucionBiblioteca.sln
└── 📄 README.md
```
---

## 🛠️ Patrones de Diseño e Implementaciones Clave

### 1. Repository & Unit of Work Pattern
Hemos centralizado el acceso a datos para evitar la duplicación de lógica de persistencia. El **Unit of Work** garantiza la **Atomicidad** (ACID) de las operaciones complejas.
* *Ejemplo:* Si la eliminación de un autor falla después de haber marcado sus libros para borrar, la transacción hace un **Rollback** automático, protegiendo la integridad de la base de datos.



### 2. Patrón Result (Functional Error Handling)
En lugar de manejar el flujo con excepciones (lo cual es costoso en CPU y ensucia el código), utilizamos el patrón `Result<T>`.
* Permite devolver estados de éxito o falla de forma explícita.
* Mejora la legibilidad y obliga a manejar los casos de error desde el controlador de forma tipada.

### 3. BaseApiController & Global Error Handling
* **BaseApiController:** Centraliza el método `HandleResult`, traduciendo los estados de dominio (`NotFound`, `Validation`, `ServerFault`) a códigos HTTP reales (404, 400, 500).
* **Middleware:** Contamos con una malla de seguridad final (Global Exception Middleware) para capturar errores inesperados y devolver un `ErrorResponse` estandarizado.

### 4. Gestión de Persistencia (Borrado Físico)
Actualmente, el sistema aplica un **borrado físico** (`Hard Delete`) de los registros. 
* **Oportunidad de Mejora:** Como evolución técnica, se plantea implementar **Borrado Lógico** (`Soft Delete`) mediante una propiedad `IsDeleted` o `Activo`, para mantener la trazabilidad histórica y evitar la pérdida permanente de datos.

---

## ⚙️ Configuración del Proyecto

### 1. Cadena de Conexión (Backend)

El proyecto utiliza **LocalDB de SQL Server** por defecto para facilitar el primer despliegue.  
Para cambiar a una instancia de **SQL Server Standard, Azure o Docker**:

1. Localiza el archivo `appsettings.json` en el proyecto **API**.
2. Modifica la cadena `DefaultConnection` según tu entorno:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=BibliotecaDB;User Id=TU_USER;Password=TU_PASS;Trusted_Connection=False;TrustServerCertificate=True;"
  }
}
```

---

### 2. URL de la API (Frontend - Blazor)
La URL de la API se gestiona de forma dinámica desde el archivo de configuración para facilitar cambios entre entornos (Desarrollo/Producción).

**Archivo `wwwroot/appsettings.json`:**
```json
{
  "ApiUrls": {
    "LibreriaApi": "https://localhost:7261/"
  }
}
```

---

### 3. Ejecución de Migraciones (Entity Framework Core)

Para generar la estructura de la base de datos, elige el comando según tu editor:

---

#### **A. Desde Visual Studio (2022+)**

1. Abre la **Consola de Administración de Paquetes**.
2. Selecciona el proyecto **Infrastructure** como proyecto predeterminado.
3. Ejecuta el siguiente comando:

```powershell
Update-Database
```

---

#### **B. Desde Visual Studio Code (CLI)**

1. Abre la terminal en la raíz del proyecto.
2. Ejecuta el siguiente comando:

```bash
dotnet ef database update --project Infrastructure --startup-project API
```

---

> [!IMPORTANT]  
> **Nota sobre LocalDB:**  
> La cadena de conexión por defecto usa `(localdb)\mssqllocaldb`.  
> Si utilizas **Visual Studio Code**, esta configuración no funcionará.  
> Debes cambiar la cadena en el archivo `appsettings.json` del proyecto **API** por una instancia real de SQL Server antes de ejecutar las migraciones.



---

## ✅ Principios y Buenas Prácticas Aplicadas

En este desarrollo no solo tiramos código, aplicamos ingeniería de software real:

* **SOLID:** Cumplimiento del principio de Inversión de Dependencias (DI) mediante el uso de interfaces en todas las capas del sistema.
* **DRY (Don't Repeat Yourself):** Centralización de la lógica de respuestas y manejo de errores en el `BaseApiController`.
* **KISS (Keep It Simple):** Servicios de aplicación enfocados exclusivamente en la orquestación de negocio.
* **Encapsulamiento:** Uso estricto de **DTOs** para evitar la exposición innecesaria de las entidades de dominio hacia la interfaz de usuario.
* **Seguridad:** Ocultamiento de detalles técnicos de excepciones (`ex.Message`) al cliente final, devolviendo mensajes de negocio controlados.

---

## 🧪 Pruebas Unitarias

> [!IMPORTANT]
> El sistema ha sido diseñado desde cero para ser 100% testeable. En la siguiente iteración se implementará:
> * **xUnit:** Como framework principal para la ejecución de pruebas unitarias.
> * **Moq:** Para la creación de objetos simulados (mocks) del `IUnitOfWork` y repositorios.
> * **FluentAssertions:** Para garantizar aserciones de código altamente legibles y expresivas.


---

## 🚀 Cómo Ejecutar el Proyecto

### 1️⃣ Restaurar paquetes

Desde la raíz de la solución, ejecuta:

```bash
dotnet restore
```

---

### 2️⃣ Lanzamiento de la Aplicación

Elige el método según tu entorno de desarrollo:

#### 🅰️ Visual Studio 2022+

1. Configura la solución para **Inicio Múltiple**.
2. Establece los proyectos **API** y **Blazor** como proyectos de inicio.
3. Ejecuta la solución (F5 o botón ▶).

---

#### 🅱️ Visual Studio Code

1. Abre una terminal y ejecuta la API:

```bash
dotnet run --project API
```

2. Abre una segunda terminal y ejecuta el proyecto Blazor:

```bash
dotnet run --project Blazor
```

---

✅ ¡Listo! Ahora puedes gestionar tu catálogo de biblioteca correctamente.

