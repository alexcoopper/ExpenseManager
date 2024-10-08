using ExpenseManager.Models;
using NPOI.SS.UserModel;

public class PrivatDmytroParser : IExcelParser
{
    private readonly IWorkbook _workbook;

    public PrivatDmytroParser(IWorkbook workbook)
    {
        _workbook = workbook;
    }

    public List<Expense> ParseExcel()
    {
        var expenses = new List<Expense>();
        ISheet worksheet = _workbook.GetSheetAt(0); // First sheet

        for (int row = 2; row <= worksheet.LastRowNum; row++) // Skip header row
        {
            IRow currentRow = worksheet.GetRow(row);
            if (currentRow == null) continue;

            var date = currentRow.GetCell(0)?.ToString();
            var time = currentRow.GetCell(1)?.ToString();
            if (date == null || time == null) continue;

            var expense = new Expense
            {
                Date = DateTime.ParseExact($"{date} {time}", "dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                Description = currentRow.GetCell(4)?.ToString(),
                Sum = currentRow.GetCell(5)?.NumericCellValue ?? 0,
                ExpenseOwner = ExpenseOwner.Dmytro
            };
            expenses.Add(expense);
        }

        return expenses;
    }
}
