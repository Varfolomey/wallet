# REST API для мобильного приложения

## Обзор

Добавлен REST API для работы с Android-приложением. API предоставляет endpoints для управления расходами (Spendings), доходами (Incomes) и отчётами.

## Структура

```
WalletTelegramBot/
├── API/
│   ├── Controllers/
│   │   ├── SpendingsController.cs    # CRUD для трат
│   │   ├── IncomesController.cs      # CRUD для доходов
│   │   └── ReportsController.cs      # Отчёты за день/месяц
│   └── DTOs/
│       └── ApiDTOs.cs                # Модели данных для API
└── Business/Features/
    ├── GetSpendingsQuery.cs          # Запросы на получение списков
    ├── GetReportsQuery.cs            # Запросы на отчёты
    └── CrudOperations.cs             # Команды создания/удаления
```

## Endpoints

### Spendings (Траты)

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/spendings` | Получить список трат |
| POST | `/api/spendings` | Создать новую трату |
| DELETE | `/api/spendings/{id}` | Удалить трату |

**Параметры GET /api/spendings:**
- `userName` (optional) - фильтр по пользователю
- `fromDate` (optional) - дата начала периода
- `toDate` (optional) - дата окончания периода

**POST /api/spendings body:**
```json
{
  "amount": 1500,
  "comment": "Продукты",
  "userName": "alex"
}
```

### Incomes (Доходы)

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/incomes` | Получить список доходов |
| POST | `/api/incomes` | Создать новый доход |
| DELETE | `/api/incomes/{id}` | Удалить доход |

**Параметры GET /api/incomes:**
- `fromDate` (optional) - дата начала периода
- `toDate` (optional) - дата окончания периода

**POST /api/incomes body:**
```json
{
  "amount": 50000,
  "comment": "Зарплата"
}
```

### Reports (Отчёты)

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/reports/day/{date}` | Отчёт за день |
| GET | `/api/reports/month/{year}/{month}` | Отчёт за месяц |

**Пример GET /api/reports/day/2025-01-15:**
```json
{
  "date": "2025-01-15T00:00:00",
  "totalIncome": 50000,
  "totalSpent": 3500,
  "balance": 46500,
  "spendings": [
    { "comment": "Продукты", "amount": 1500 },
    { "comment": "Такси", "amount": 2000 }
  ],
  "incomes": [
    { "comment": "Зарплата", "amount": 50000 }
  ]
}
```

**Пример GET /api/reports/month/2025/1:**
```json
[
  {
    "month": "01.2025",
    "spendings": [
      { "comment": "Продукты", "amount": 25000 },
      { "comment": "ЖКХ", "amount": 8000 }
    ]
  }
]
```

## DTO модели

### SpendingDto
- `id` (int)
- `amount` (decimal) - отрицательное число для трат
- `dateTime` (DateTime)
- `comment` (string)
- `userName` (string)

### IncomeDto
- `id` (int)
- `amount` (decimal) - положительное число для доходов
- `date` (DateTime)
- `comment` (string)

## Бизнес-логика

- **Траты** всегда сохраняются с отрицательным Amount
- **Доходы** всегда сохраняются с положительным Amount
- При создании через API знак определяется автоматически

## Использование в Android-приложении

### Пример запроса на Retrofit:

```kotlin
interface WalletApiService {
    @GET("api/spendings")
    suspend fun getSpendings(
        @Query("userName") userName: String? = null,
        @Query("fromDate") fromDate: String? = null,
        @Query("toDate") toDate: String? = null
    ): List<SpendingDto>
    
    @POST("api/spendings")
    suspend fun createSpending(@Body spending: CreateSpendingDto): Int
    
    @GET("api/reports/day/{date}")
    suspend fun getDayReport(@Path("date") date: String): DayReportDto
}
```

## Примечания

- Category пока не учитывается (можно добавить позже)
- Аутентификация не реализована (требуется добавить JWT)
- TelegramMessageId не используется в API (только для бота)
