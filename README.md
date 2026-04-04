# Tandil Bank - App de Home Banking 🏦

Tandil Bank es un proyecto de Home Banking desarrollado con **.NET 10 MAUI**. El objetivo de esta app es tener un cliente bancario funcional que corra de forma nativa tanto en teléfonos Android, IOS, como en Windows o macOS, compartiendo la misma base de código.

## ¿Qué hace la app?
* **Login y Seguridad:** Inicio de sesión clásico y soporte para entrada con huella dactilar/biometría.
* **Panel de Cuentas:** Vista del saldo principal y un sistema de bienvenida para reclamar dinero en la cuenta.
* **Transferencias:** Flujo para enviar dinero a otros usuarios.
* **UI Adaptativa:** Soporte completo para Modo Oscuro/Claro del sistema.
* **Pagos de Servicios:** Sistema de pagos a servicios típicos con interfaz simple y limpia.

## Stack Tecnológico
* **Framework:** .NET 10 MAUI.
* **Arquitectura:** MVVM (Model-View-ViewModel).
* **Datos:** Entity Framework Core (`Microsoft.Data.SqlClient`).
* **Paquetes externos:**
  * `Plugin.Fingerprint` (para la biometría).
  * `Plugin.LocalNotification` (para alertas locales).

## Estructura del Código
El proyecto está dividido en distintas capas:

* `UI`: El proyecto principal de MAUI. Contiene el `AppShell`, las vistas en XAML, los estilos y los recursos gráficos.
* `ViewModels`: La capa intermedia que maneja el estado de la UI y los comandos.
* `Services`: Toda la lógica de negocio y llamadas a repositorios mediante interfaces.
* `DataAccess`: El `DbContext` de Entity Framework y las consultas a la base de datos.
* `Models`: Las entidades de la base de datos y los DTOs.

## Cómo levantar el proyecto
1. Necesitas **Visual Studio 2022** con la carga de trabajo de .NET MAUI y el **SDK de .NET 10**.
2. Clona el repo y abre la solución `HomeBanking.sln`.
3. Marca el proyecto **`UI`** como el proyecto de inicio.
4. Elige `Windows Machine` o tu emulador de Android en la barra de arriba y dale a F5.

### Para generar el instalador de Android (.APK)
Si queres probar la app en un teléfono físico, abrí la terminal dentro de `UI` y corre este comando para compilar la versión de producción:

```bash
dotnet publish -f net10.0-android -c Release
```

Vas a encontrar el .apk listo para instalar en:
`UI\bin\Release\net10.0-android\com.tandilbank.app-Signed.apk`
