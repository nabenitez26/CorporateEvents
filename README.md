# Corporate Events Framework

Framework reutilizable desarrollado en .NET 8 para la publicación y procesamiento de eventos corporativos mediante una arquitectura orientada a eventos (Event Driven Architecture).

---

## Resumen ejecutivo

Este framework permite desacoplar los procesos de negocio mediante eventos, facilitando la escalabilidad, mantenibilidad y trazabilidad en sistemas empresariales. Incluye mecanismos robustos de reintentos, manejo de errores y persistencia de eventos fallidos (Dead Letter Queue), asegurando confiabilidad y auditabilidad.

---

## Características principales

- **Publicación de eventos** desacoplada.
- **Suscripción de handlers** extensible.
- **Procesamiento paralelo** y asíncrono.
- **Retry configurable**.
- **Dead Letter Queue (DLQ)** persistente.
- **Idempotencia**.
- **Logging estructurado**.
- **Métricas** para monitoreo.

---

## Arquitectura


**Flujo de eventos:**
1. El controlador invoca `EventBus.Publish()`.
2. El `SubscriptionManager` resuelve los handlers suscritos.
3. Cada handler procesa el evento de forma asíncrona.
4. Si ocurre un error, se aplica la política de reintentos.
5. Si el evento falla tras los reintentos, se almacena en la Dead Letter Queue.
6. El procesamiento puede ser monitoreado y auditado.

---

## Estructura de proyectos

- **CorporateEvents.Abstractions**  
  Interfaces, contratos, eventos base, DTOs y configuraciones compartidas.
- **CorporateEvents.Core**  
  Implementación del EventBus, SubscriptionManager y RetryManager.
- **CorporateEvents.Persistence.SqlServer**  
  Persistencia de eventos y DLQ en SQL Server.
- **CorporateEvents.SampleApi**  
  Proyecto de ejemplo que demuestra el uso del framework.

---

## Instalación y configuración

1. **Ejecuta el script de base de datos:**
   - `Scripts/01_CreateDatabaseAndSchema.sql`
2. **Configura la cadena de conexión y parámetros en `appsettings.json`:**
   
3. **Ejecuta el proyecto de ejemplo:**
   - `CorporateEvents.SampleApi`

---

## Ejemplo de uso

await _eventBus.Publish(new PolicyIssuedEvent
{
    PolicyNumber = "POL-10001",
    CustomerName = "Nilson Benitez",
    Email = "cliente@correo.com",
    Phone = "123456789"
});

---

## Tecnologías utilizadas

- .NET 8
- C#
- SQL Server
- Dependency Injection
- ILogger
- System.Text.Json

---

## Decisiones de diseño

- Uso de interfaces y abstracciones para facilitar la extensibilidad y reutilización.
- Implementación de un EventBus para la publicación y suscripción de eventos.
- Manejo de errores y reintentos configurables para garantizar la confiabilidad.
- Dead Letter Queue para eventos no procesados tras varios intentos.
- Logging estructurado y métricas para monitoreo y depuración.
- Idempotencia mediante persistencia en SQL Server.
- Patrones de suscripción y reintento para escalabilidad y mantenibilidad.
- Proyecto de ejemplo para facilitar la adopción.

---

## Limitaciones

- Solo soporta SQL Server como almacenamiento.
- No incluye integración con brokers de mensajería (RabbitMQ, Kafka, Azure Service Bus, etc.).
- Las métricas se obtienen mediante consultas a la base de datos.
- La detección de handlers es manual mediante `Subscribe()`.

---

## Contacto

Autor: [Nilson Andres Benitez Rodriguez]  
Email: nabenitez26@hotmail.com 
LinkedIn: https://www.linkedin.com/in/nabenitezr
