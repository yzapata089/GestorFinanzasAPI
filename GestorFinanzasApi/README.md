# Gestor de Finanzas Personales

API REST para la gestión de finanzas personales, desarrollada como proyecto académico con ASP.NET Core Web API, .NET 9 y PostgreSQL.

## Descripción

El sistema permite registrar ingresos y gastos, organizarlos por categorías, administrar presupuestos y sus abonos, guardar configuraciones por dispositivo y consultar un resumen financiero por período.

## Tecnologías utilizadas

- C#
- ASP.NET Core Web API
- .NET 9
- Entity Framework Core
- PostgreSQL
- Npgsql
- Swagger / OpenAPI
- Visual Studio 2022

## Funcionalidades

- Gestión de dispositivos.
- Gestión de categorías de ingresos y gastos.
- Registro, consulta, actualización y eliminación de movimientos.
- Gestión de presupuestos.
- Registro de abonos asociados a presupuestos.
- Configuración de moneda, tema e idioma por dispositivo.
- Consulta de ingresos, gastos, saldo y cantidad de movimientos por período.
- Filtros de movimientos por dispositivo, tipo y fechas.
- Validaciones para mantener la integridad de los datos y sus relaciones.

## Modelo de datos

La base de datos está compuesta por las siguientes tablas:

| Tabla | Descripción |
|---|---|
| `dispositivo` | Identifica los dispositivos registrados. |
| `categoria` | Almacena las categorías de ingresos y gastos. |
| `movimiento` | Registra ingresos y gastos. |
| `presupuesto` | Almacena presupuestos asociados a categorías. |
| `abono` | Registra abonos asociados a presupuestos. |
| `configuracion` | Guarda las preferencias de cada dispositivo. |

Las tablas están relacionadas mediante claves primarias y foráneas para mantener la integridad referencial.

## Requisitos previos

- Windows y Visual Studio 2022 con soporte para .NET 9.
- PostgreSQL instalado y en ejecución.
- SDK de .NET 9.
- Acceso a Internet para restaurar los paquetes NuGet, si es necesario.

## Instalación y ejecución

### 1. Obtener el proyecto

Clona el repositorio o descarga el código fuente desde GitHub.

### 2. Crear la base de datos

En PostgreSQL, crea una base de datos llamada `gestor_finanzas`.

### 3. Crear las tablas

Ejecuta el script SQL incluido en la carpeta `BD` del proyecto. Este script contiene las instrucciones DDL necesarias para crear las tablas, relaciones y restricciones.

### 4. Configurar la conexión

Para el entorno de desarrollo, las credenciales se configuran mediante **User Secrets**, evitando publicar la contraseña en el repositorio.

En Visual Studio, abre una terminal o la Consola del Administrador de paquetes NuGet, ubícate en la carpeta del proyecto y ejecuta:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=gestor_finanzas;Username=postgres;Password=TU_CONTRASEÑA"
```

Reemplaza `TU_CONTRASEÑA` por la contraseña de tu instalación local de PostgreSQL.

Si el proyecto ya tiene configurado `UserSecretsId`, no es necesario ejecutar `dotnet user-secrets init` nuevamente.

### 5. Abrir y ejecutar el proyecto

1. Abre la solución en Visual Studio 2022.
2. Restaura los paquetes NuGet.
3. Compila el proyecto.
4. Ejecuta la API con F5.
5. Abre la dirección local de Swagger que muestre Visual Studio.

## Endpoints principales

La API expone endpoints REST documentados mediante Swagger/OpenAPI.

- `/api/Dispositivo` — gestión de dispositivos.
- `/api/Categoria` — gestión de categorías.
- `/api/Movimiento` — gestión y consulta de movimientos.
- `/api/Presupuesto` — gestión de presupuestos.
- `/api/Abono` — gestión de abonos.
- `/api/Configuracion` — gestión de configuraciones.
- `/api/Resumen/{idDispositivo}` — resumen financiero de un dispositivo.

Los métodos HTTP disponibles y los parámetros de cada endpoint se pueden consultar en Swagger.

## Seguridad

- No se deben publicar contraseñas ni cadenas de conexión con credenciales.
- En desarrollo se utilizan User Secrets para almacenar la cadena de conexión.
- Para otros entornos se recomienda utilizar variables de entorno o un sistema seguro de gestión de secretos.
- El archivo `secrets.json` debe permanecer fuera del repositorio.

## Estado del proyecto

API implementada y probada manualmente mediante Swagger. Las operaciones principales y los endpoints adicionales se verificaron durante el desarrollo.
