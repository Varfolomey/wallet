# WalletApp - MAUI приложение для учёта семейных расходов

## Описание
Кроссплатформенное мобильное приложение (Android, iOS, macOS, Windows) для управления бюджетом семьи. Работает с REST API сервера WalletTelegramBot.

## Структура проекта

```
app/WalletApp/
├── MauiProgram.cs          # Точка входа и настройка DI
├── App.cs                  # Главный класс приложения
├── AppShell.cs             # Навигационная оболочка
├── Services/
│   └── ApiService.cs       # HTTP клиент для работы с API + DTO
└── Pages/
    ├── DashboardPage.cs    # Главная страница с балансом за день
    ├── SpendingsPage.cs    # Управление тратами (CRUD)
    ├── IncomesPage.cs      # Управление доходами (CRUD)
    └── ReportsPage.cs      # Отчёты за месяц
```

## Функционал

### Дашборд
- Отображение баланса за текущий день
- Показ доходов и трат за день
- Кнопка обновления данных

### Траты
- Просмотр списка всех трат
- Добавление новой траты (сумма, комментарий, имя пользователя)
- Удаление траты
- Автоматическое сохранение трат как отрицательных значений

### Доходы
- Просмотр списка всех доходов
- Добавление нового дохода (сумма, комментарий)
- Удаление дохода
- Сохранение доходов как положительных значений

### Отчёты
- Выбор месяца и года
- Просмотр дневной детализации:
  - Доходы за день
  - Траты за день
  - Баланс за день
- Итоговый баланс за месяц

## Настройка подключения к API

В файле `MauiProgram.cs` измените базовый адрес API:

```csharp
client.BaseAddress = new Uri("http://ваш-сервер:5000/");
```

Для продакшена используйте HTTPS и реальный домен сервера.

## Сборка и запуск

### Требования
- .NET 8 SDK
- Visual Studio 2022 с workload MAUI (для Windows/macOS)
- Или VS Code с расширением C# Dev Kit

### Сборка для Android
```bash
cd app/WalletApp
dotnet build -t:Run -f net8.0-android
```

### Сборка для других платформ
```bash
# Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0

# macOS (MacCatalyst)
dotnet build -t:Run -f net8.0-maccatalyst

# iOS (требуется Mac)
dotnet build -t:Run -f net8.0-ios
```

## Зависимости
- Microsoft.Maui.Controls (8.0.80)
- Microsoft.Extensions.Http (8.0.0)
- System.Text.Json (8.0.5)

## API Endpoints

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/spendings` | Список трат |
| POST | `/api/spendings` | Создать трату |
| DELETE | `/api/spendings/{id}` | Удалить трату |
| GET | `/api/incomes` | Список доходов |
| POST | `/api/incomes` | Создать доход |
| DELETE | `/api/incomes/{id}` | Удалить доход |
| GET | `/api/reports/day/{date}` | Отчёт за день |
| GET | `/api/reports/month/{year}/{month}` | Отчёт за месяц |

См. полную документацию в `/API/README.md`.
