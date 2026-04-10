# Tandil Bank - Banca Personal 🏦

Tandil Bank es un proyecto de Home Banking desarrollado con **.NET 10 MAUI**. El objetivo de esta app es ofrecer un cliente bancario moderno, robusto y 100% funcional que corra de forma nativa tanto en teléfonos Android e iOS, como en escritorio (Windows/macOS), compartiendo la misma base de código.

---

## 📦 Descargas (Releases)
¿No querés compilar el código y solo querés probar la app? 
Puedes descargar la última versión lista para instalar (APK para Android o MSIX para Windows) directamente desde la **[Pestaña de Releases](https://github.com/tu-usuario/tu-repo/releases)** del repositorio.

---

## ✨ Características Principales
* **Asistente Inteligente (NUEVO):** Chatbot integrado de baja latencia potenciado por la API de **Groq** para asistencia al usuario en tiempo real.
* **Seguridad y Accesos:** Inicio de sesión clásico y soporte para autenticación biométrica (huella dactilar).
* **Gestión de Cuentas:** Panel principal con visualización de saldos, últimos movimientos y sistema de bienvenida.
* **Operaciones Bancarias:** Flujo completo para transferencias de dinero y pago de servicios.
* **UI/UX Premium:** Interfaz adaptativa con soporte nativo e inteligente para **Modo Claro y Oscuro**.
* **Observabilidad:** Sistema de trazabilidad y monitoreo de eventos impulsado por **Serilog**.

## 🛠️ Stack Tecnológico
* **Framework:** .NET 10 MAUI.
* **Arquitectura:** MVVM (Model-View-ViewModel).
* **Base de Datos:** Entity Framework Core con **SQLite** (Ideal para desarrollo local y emuladores offline).
* **Paquetes Destacados:**
  * `Plugin.Fingerprint` (Biometría).
  * `Plugin.LocalNotification` (Alertas locales).
  * `Serilog` (Logs estructurados).
  * `Integración con IA` mediante HTTP Clients (Groq).

## 📂 Estructura del Proyecto
La solución está construida bajo una arquitectura limpia y modularizada:
* **`UI`**: El proyecto principal de MAUI. Contiene la navegación (`AppShell`), las vistas XAML, componentes personalizados y recursos gráficos.
* **`ViewModels`**: La capa de presentación que maneja el estado de la aplicación y la lógica de la interfaz.
* **`Services`**: Lógica de negocio, integración con APIs externas (Groq) y llamadas a repositorios.
* **`DataAccess`**: Configuración del `AppDbContext` de Entity Framework, migraciones y conexión a SQLite.
* **`Models`**: Entidades centrales del dominio y Data Transfer Objects (DTOs).

## 🚀 Cómo levantar el proyecto para Desarrollo
1. Asegurate de tener **Visual Studio 2026** con la carga de trabajo de **.NET MAUI** y el **SDK de .NET 10** instalados.
2. Clona este repositorio y abre la solución `HomeBanking.sln`.
3. Establece el proyecto **`UI`** como el proyecto de inicio.
4. Selecciona tu entorno destino (`Windows Machine` o un `Emulador de Android`) en la barra superior.
5. Presiona `F5` o el botón de Play.

## 📱 Generar el instalador de Android (.APK)
Si necesitas compilar la aplicación para probarla en un dispositivo físico sin pasar por Visual Studio, abre una terminal dentro de la carpeta `UI` y ejecuta:

```bash
dotnet publish -f net10.0-android -c Release
```

Una vez finalizado, encontrarás tu instalador firmado y listo para usar en:
`UI\bin\Release\net10.0-android\com.tandilbank.app-Signed.apk`

***Desarrollado por Jano en Tandil, Buenos Aires.***
