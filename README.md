# Identity Server API

Проект реализует сервер аутентификации и авторизации с использованием ASP.NET Core, gRPC и современных технологий безопасности.

## Основные возможности
- 🔐 Регистрация и аутентификация пользователей
- 🔑 Генерация JWT-токенов с ролевой моделью (admin/standart)
- 📡 gRPC сервис для получения публичных профилей пользователей
- 🗄️ Управление учетными записями (CRUD операции)
- 🔒 Ролевая модель доступа (администратор/стандартный пользователь)

## Технологический стек
- **Язык**: C# (.NET 7)
- **База данных**: PostgreSQL (с Entity Framework Core)
- **Кэширование**: Redis (с поддержкой distributed locks)
- **Логирование**: Elasticsearch + Serilog
- **Аутентификация**: JWT Bearer Tokens
- **Контейнеризация**: Docker
- **Тестирование**: xUnit + Moq + FluentAssertions

## Архитектура
HCL.IdentityServer.API/  
├── API # Контроллеры и middleware  
├── BLL # Бизнес-логика и сервисы  
├── DAL # Репозитории и работа с БД  
├── Domain # DTO, Entities, Enums  
├── Test # Тесты (юнит и интеграционные)  
├── HostedServices # Фоновые сервисы  
└── Midleware # Промежуточное ПО  

## Запуск проекта

### Требования
- Docker
- .NET 7 SDK
- PostgreSQL + Redis (или Docker для запуска контейнеров)

### 1. Запуск через Docker Compose
```bash
docker-compose -f "docker-compose.yml" up -d --build
```

### 2. Настройка окружения
Создайте файл **`appsettings.Development.json`** с настройками:

```json
{
  "ConnectionStrings": {
    "NpgConnectionString": "User Id=postgres;Password=pg;Server=localhost;Port=5433;Database=HCL_IdentityServer;"
  },
  "JWTSettings": {
    "SecretKey": "your_secure_key_here",
    "Issuer": "HCL.IdentityServer",
    "Audience": "HCL.Clients"
  },
  "RedisOptions": {
    "Host": "localhost:6379,password=redis"
  }
}
```

### 3. Применение миграций
```bash
dotnet ef database update --project HCL.IdentityServer.API
```
## Ключевые конечные точки API
### Регистрация пользователя  
`POST /api/IdentityServer/v1/registration`

```json
{
  "Login": "user@example.com",
  "Password": "P@ssw0rd123"
}
```

### Аутентификация
`POST /api/IdentityServer/v1/authenticate`

```json
{
  "Login": "user@example.com",
  "Password": "P@ssw0rd123"
}
```

### Удаление аккаунта (требует роли admin)
`DELETE /api/IdentityServer/v1/account?id={guid}`

## gRPC сервис
- Сервис: **`AthorPublicProfile`**
- Метод: **`GetProfile`**
- Порт: 5001

Пример запроса:
```protobuf
message AthorIdRequest {
  string AccountId = 1;
}
```

## Тестирование
Запуск всех тестов:

```bash
dotnet test
```
**Типы тестов**
- Интеграционные:
  - Тестирование API контроллеров
  - Работа с БД и внешними сервисами
- Юнит-тесты:
  - Сервисы (Account, Registration, Token)
  - gRPC сервисы
  - Валидация бизнес-логики

## Настройки безопасности
- Пароли хранятся в виде хешей (HMACSHA512)
- JWT токены с ограниченным временем жизни
- Автоматическая валидация токенов
- Ролевая модель доступа (на основе claims)

## Логирование
Система использует Elasticsearch для централизованного логирования. Пример конфигурации:

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
    .CreateLogger();
```
## CI/CD
Проект включает:
- GitHub Actions для автоматического тестирования
- Конфигурацию для деплоя в Google Cloud Run (**`cloudbuild.yaml`**)
- Dockerfile для контейнеризации приложения

## Администрирование
После инициализации БД автоматически создается администратор:  
**Логин**: admin  
**Пароль**: admin  
**Роль**: Admin  

## Важные компоненты
- **JwtHelper**: Кастомная валидация времени жизни токенов
- **RedisLockService**: Блокировки для конкурентного доступа
- **ExceptionHandlingMiddleware**: Централизованная обработка ошибок
- **CheckDBMiddleware**: Автоматическое создание БД при запуске
