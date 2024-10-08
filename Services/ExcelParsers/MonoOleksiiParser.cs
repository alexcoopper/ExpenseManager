using ExpenseManager.Models;
using NPOI.SS.UserModel;

public class MonoOleksiiParser : IExcelParser
{
    private readonly IWorkbook _workbook;

    public MonoOleksiiParser(IWorkbook workbook)
    {
        _workbook = workbook;
    }

    public List<Expense> ParseExcel()
    {
        var expenses = new List<Expense>();
        ISheet worksheet = _workbook.GetSheetAt(0); // First sheet

        for (int row = 22; row <= worksheet.LastRowNum; row++) // Skip header row
        {
            IRow currentRow = worksheet.GetRow(row);
            if (currentRow == null) continue;

            var date = currentRow.GetCell(0)?.ToString();
            if (date == null) continue;

            var expense = new Expense
            {
                Date = DateTime.ParseExact(date, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                Description = currentRow.GetCell(1)?.ToString(),
                Sum = currentRow.GetCell(4)?.NumericCellValue ?? 0,
                ExpenseOwner = ExpenseOwner.Oleksii
            };
            expenses.Add(expense);
        }

        return expenses;
    }
}
