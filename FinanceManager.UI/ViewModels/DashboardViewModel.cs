using CommunityToolkit.Mvvm.ComponentModel;
using FinanceManager.Core.Interfaces;
using FinanceManager.Core.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Windows.Media;

namespace FinanceManager.UI.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly ITransactionRepository _transactionRepo;

    private static readonly string[] MonthShortNames =
    {
        "Янв", "Фев", "Мар", "Апр", "Май", "Июн",
        "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек"
    };

    [ObservableProperty] private string balance = "₽ 0,00";
    [ObservableProperty] private string income = "₽ 0,00";
    [ObservableProperty] private string expenses = "₽ 0,00";
    [ObservableProperty] private bool hasPieData;

    [ObservableProperty] private ISeries[] lineSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] xAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] yAxes = Array.Empty<Axis>();
    [ObservableProperty] private IEnumerable<ISeries> pieSeries = Array.Empty<ISeries>();

    public DashboardViewModel(ITransactionRepository transactionRepo)
    {
        _transactionRepo = transactionRepo;
    }

    public override async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var now = DateTime.Today;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

            var monthTx = (await _transactionRepo.GetByPeriodAsync(monthStart, monthEnd)).ToList();

            var incomeAmt = monthTx.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var expAmt = monthTx.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            Balance = $"₽ {incomeAmt - expAmt:N2}";
            Income = $"₽ {incomeAmt:N2}";
            Expenses = $"₽ {expAmt:N2}";

            await LoadLineChartAsync(now);
            LoadPieChart(monthTx);
        }
        finally { IsLoading = false; }
    }

    private async Task LoadLineChartAsync(DateTime now)
    {
        var incomeValues = new List<double>();
        var expenseValues = new List<double>();
        var labels = new List<string>();

        for (int i = 5; i >= 0; i--)
        {
            var m = now.AddMonths(-i);
            var from = new DateTime(m.Year, m.Month, 1);
            var to = from.AddMonths(1).AddTicks(-1);
            var txs = (await _transactionRepo.GetByPeriodAsync(from, to)).ToList();

            incomeValues.Add((double)txs.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount));
            expenseValues.Add((double)txs.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount));
            labels.Add($"{MonthShortNames[m.Month - 1]} {m.Year}");
        }

        LineSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Name = "Доходы",
                Values = incomeValues,
                Fill = null,
                Stroke = new SolidColorPaint(new SKColor(76, 175, 80)) { StrokeThickness = 2 },
                GeometryFill = new SolidColorPaint(new SKColor(76, 175, 80)),
                GeometryStroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 },
                GeometrySize = 6
            },
            new LineSeries<double>
            {
                Name = "Расходы",
                Values = expenseValues,
                Fill = null,
                Stroke = new SolidColorPaint(new SKColor(244, 67, 54)) { StrokeThickness = 2 },
                GeometryFill = new SolidColorPaint(new SKColor(244, 67, 54)),
                GeometryStroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 },
                GeometrySize = 6
            }
        };

        XAxes = new[]
        {
            new Axis
            {
                Labels = labels,
                TextSize = 11,
                LabelsPaint = new SolidColorPaint(new SKColor(120, 144, 156))
            }
        };

        YAxes = new[]
        {
            new Axis
            {
                TextSize = 11,
                LabelsPaint = new SolidColorPaint(new SKColor(120, 144, 156)),
                Labeler = v => $"₽ {v:N0}"
            }
        };
    }

    private void LoadPieChart(List<Transaction> monthTx)
    {
        var groups = monthTx
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => new { t.CategoryId, t.Category?.Name, t.Category?.Color })
            .Select(g => new
            {
                g.Key.Name,
                g.Key.Color,
                Total = (double)g.Sum(t => t.Amount)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        HasPieData = groups.Count > 0;

        if (!HasPieData)
        {
            PieSeries = Array.Empty<ISeries>();
            return;
        }

        PieSeries = groups.Select(item =>
        {
            var skColor = ParseHex(item.Color);
            return (ISeries)new PieSeries<double>
            {
                Name = item.Name ?? "Другое",
                Values = new[] { item.Total },
                Fill = new SolidColorPaint(skColor),
                Stroke = null,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 11,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
            };
        }).ToArray();
    }

    private static SKColor ParseHex(string? hex)
    {
        if (hex is null) return SKColors.Gray;
        try
        {
            var c = (Color)ColorConverter.ConvertFromString(hex);
            return new SKColor(c.R, c.G, c.B);
        }
        catch { return SKColors.Gray; }
    }
}
