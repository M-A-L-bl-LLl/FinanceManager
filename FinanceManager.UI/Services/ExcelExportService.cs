using ClosedXML.Excel;
using FinanceManager.Core.Models;
using FinanceManager.UI.ViewModels;

namespace FinanceManager.UI.Services;

public static class ExcelExportService
{
    public static void Export(string filePath, IEnumerable<TransactionDto> transactions)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Транзакции");

        // Header
        string[] headers = { "Дата", "Категория", "Тип", "Сумма (₽)", "Комментарий" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#7C4DFF");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // Data rows
        int row = 2;
        foreach (var t in transactions)
        {
            ws.Cell(row, 1).Value = t.Date.ToString("dd.MM.yyyy");
            ws.Cell(row, 2).Value = t.CategoryName;
            ws.Cell(row, 3).Value = t.TypeLabel;

            var amountCell = ws.Cell(row, 4);
            amountCell.Value = (double)t.Amount;
            amountCell.Style.NumberFormat.Format = "#,##0.00";
            amountCell.Style.Font.FontColor = t.Type == TransactionType.Income
                ? XLColor.FromHtml("#4CAF50")
                : XLColor.FromHtml("#F44336");

            ws.Cell(row, 5).Value = t.Comment;
            row++;
        }

        // Totals row
        if (row > 2)
        {
            ws.Cell(row, 3).Value = "Итого:";
            ws.Cell(row, 3).Style.Font.Bold = true;
            ws.Cell(row, 4).FormulaA1 = $"SUMIF(C2:C{row - 1},\"Доход\",D2:D{row - 1})-SUMIF(C2:C{row - 1},\"Расход\",D2:D{row - 1})";
            ws.Cell(row, 4).Style.Font.Bold = true;
            ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
        }

        ws.Columns().AdjustToContents();
        ws.SheetView.FreezeRows(1);

        wb.SaveAs(filePath);
    }
}
