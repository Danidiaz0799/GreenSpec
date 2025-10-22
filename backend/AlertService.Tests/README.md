# AlertService.Tests

Proyecto de tests unitarios para el sistema de alertas industriales GreenSpec.

## 📊 Cobertura de Tests

**Total: 33 tests** organizados por categoría:

### Controllers (17 tests)

#### AuthController (5 tests)
- ✅ Login con credenciales válidas retorna JWT
- ✅ Login con credenciales inválidas retorna Unauthorized
- ✅ Login con username vacío retorna BadRequest
- ✅ Login con password vacío retorna BadRequest
- ✅ Login con credenciales nulas retorna BadRequest

#### ConfigController (5 tests)
- ✅ GetConfig retorna configuración existente
- ✅ GetConfig retorna NotFound si no existe
- ✅ UpdateConfig valida TempMax > 0
- ✅ UpdateConfig valida HumidityMax entre 0-100
- ✅ UpdateConfig valida HumidityMax no puede ser 0

#### AlertsController (7 tests)
- ✅ GetAllAlerts retorna lista de alertas
- ✅ GetAlertById retorna alerta específica
- ✅ GetAlertById retorna NotFound para ID inválido
- ✅ AcknowledgeAlert retorna NotFound para ID inválido
- ✅ AcknowledgeAlert retorna BadRequest si ya está reconocida

### Repositories (8 tests)

#### ConfigRepository (3 tests)
- ✅ GetConfigAsync retorna configuración
- ✅ UpdateConfigAsync actualiza valores
- ✅ UpdateConfigAsync actualiza timestamp

#### AlertRepository (5 tests)
- ✅ AddAlertAsync agrega alerta a la BD
- ✅ GetAllAlertsAsync retorna ordenado por fecha descendente
- ✅ GetAlertByIdAsync retorna alerta correcta
- ✅ GetAlertByIdAsync retorna null para ID inválido
- ✅ UpdateAlertAsync actualiza estado

### Entities (8 tests)

#### AlertEntityTests (4 tests)
- ✅ Alert tiene estado por defecto "open"
- ✅ Alert permite setear todas las propiedades
- ✅ AlertType tiene constantes válidas (Temperature, Humidity)
- ✅ AlertStatus tiene constantes válidas (open, ack)

#### ConfigEntityTests (4 tests)
- ✅ Config permite setear todas las propiedades
- ✅ Config acepta varios valores de umbrales válidos

## 🔧 Tecnologías Utilizadas

- **xUnit** - Framework de testing
- **Moq** - Librería de mocking
- **EntityFrameworkCore.InMemory** - BD en memoria para tests
- **.NET 9.0**

## 🚀 Ejecutar Tests

```bash
# Desde la carpeta backend
dotnet test

# Con más verbosidad
dotnet test --verbosity normal

# Con resumen detallado
dotnet test --verbosity detailed
```

## 📈 Resultado Esperado

```
Resumen de pruebas: total: 33; con errores: 0; correcto: 33; omitido: 0
```

## 🏗️ Estructura de Archivos

```
AlertService.Tests/
├── Controllers/
│   ├── AuthControllerTests.cs
│   ├── ConfigControllerTests.cs
│   └── AlertsControllerTests.cs
├── Repositories/
│   ├── ConfigRepositoryTests.cs
│   └── AlertRepositoryTests.cs
├── Entities/
│   ├── AlertEntityTests.cs
│   └── ConfigEntityTests.cs
└── README.md
```

## 💡 Notas Importantes

- Los tests de repositorio usan **InMemory Database** para no depender de PostgreSQL
- Los tests de controllers usan **Mocks** para aislar las dependencias
- Cada test es **independiente** y **determinístico**
- Los tests de AuthController usan configuración real (`ConfigurationBuilder`) en lugar de mocks para evitar problemas con métodos de extensión
