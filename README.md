# Finance Manager

<div align="center">

![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-0078D4?style=for-the-badge&logo=windows&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

Десктопное приложение для управления личными финансами / Desktop app for personal finance management

</div>

---

## 🇷🇺 Русский

### О проекте

**Finance Manager** — десктопное приложение на WPF (.NET 8) для управления личными финансами. Поддерживает учёт доходов и расходов, бюджетирование по категориям, наглядную аналитику и экспорт данных.

### Возможности

- **Дашборд** — карточки баланса, доходов и расходов за текущий месяц; линейный график динамики за 6 месяцев; круговая диаграмма расходов по категориям
- **Транзакции** — добавление, редактирование, удаление; фильтрация по дате, категории и комментарию; экспорт в Excel
- **Категории** — создание категорий с выбором иконки и цвета; отдельно для доходов и расходов
- **Бюджеты** — лимиты расходов по категориям на каждый месяц; прогресс-бары с индикацией предупреждения (80%) и превышения (100%); навигация по месяцам со встроенным выбором из календаря
- **Настройки** — переключение светлой/тёмной темы с сохранением; экспорт и импорт базы данных

### Стек технологий

| Слой | Технологии |
|------|-----------|
| UI | WPF .NET 8, MaterialDesignInXaml 5.3 |
| MVVM | CommunityToolkit.Mvvm 8.4 |
| БД | SQLite, Entity Framework Core 8 |
| Графики | LiveChartsCore (SkiaSharp) 2.0 |
| Экспорт | ClosedXML 0.105 |
| Тесты | xUnit, Moq |

### Архитектура

Проект разделён на 4 слоя по принципам Clean Architecture:

```
FinanceManager.Core     — модели, интерфейсы репозиториев, бизнес-логика
FinanceManager.Data     — EF Core контекст, репозитории, миграции
FinanceManager.UI       — WPF приложение (MVVM, навигация, DI)
FinanceManager.Tests    — юнит-тесты (xUnit + Moq)
```

### Запуск

```bash
# Клонировать репозиторий
git clone https://github.com/M-A-L-bl-LLl/FinanceManager.git
cd FinanceManager

# Сборка
dotnet build FinanceManager.slnx

# Запуск
dotnet run --project FinanceManager.UI/

# Тесты
dotnet test FinanceManager.Tests/
```

> База данных `finance.db` создаётся автоматически при первом запуске рядом с `.exe`.

---

## 🇬🇧 English

### About

**Finance Manager** is a desktop WPF (.NET 8) application for personal finance management. It supports income and expense tracking, category-based budgeting, visual analytics, and data export.

### Features

- **Dashboard** — balance, income and expense cards for the current month; 6-month trend line chart; expense breakdown pie chart by category
- **Transactions** — add, edit, delete; filter by date range, category and comment; export to Excel
- **Categories** — create categories with custom icon and color picker; separate for income and expenses
- **Budgets** — spending limits by category for each month; progress bars with warning (80%) and over-limit (100%) indicators; month navigation with a built-in calendar picker
- **Settings** — light/dark theme toggle with persistence; database export and import

### Tech Stack

| Layer | Technologies |
|-------|-------------|
| UI | WPF .NET 8, MaterialDesignInXaml 5.3 |
| MVVM | CommunityToolkit.Mvvm 8.4 |
| Database | SQLite, Entity Framework Core 8 |
| Charts | LiveChartsCore (SkiaSharp) 2.0 |
| Export | ClosedXML 0.105 |
| Tests | xUnit, Moq |

### Architecture

The project follows Clean Architecture principles and is split into 4 layers:

```
FinanceManager.Core     — models, repository interfaces, business logic
FinanceManager.Data     — EF Core DbContext, repositories, migrations
FinanceManager.UI       — WPF app (MVVM, navigation, DI)
FinanceManager.Tests    — unit tests (xUnit + Moq)
```

### Getting Started

```bash
# Clone the repository
git clone https://github.com/M-A-L-bl-LLl/FinanceManager.git
cd FinanceManager

# Build
dotnet build FinanceManager.slnx

# Run
dotnet run --project FinanceManager.UI/

# Tests
dotnet test FinanceManager.Tests/
```

> The `finance.db` database is created automatically on first launch next to the `.exe`.
