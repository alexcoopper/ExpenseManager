using System.Collections.Generic;
using ExpenseManager.Constants;

public class GoogleSheetsService
{
    private readonly GoogleSheetsApiService _apiService;
    private readonly ExpenseFilterService _filterService;

    public GoogleSheetsService()
    {
        _apiService = new GoogleSheetsApiService();
        _filterService = new ExpenseFilterService();
    }

    public void WriteDataToSheet(List<Expense> expenses)
    {
        var columnRange = _apiService.GetColumnRange();

        var existingData = _apiService.GetSheetData($"{GoogleSheetsConstants.SheetName}!A2:{columnRange}");
        var newRecords = _filterService.FilterNewExpenses(expenses, existingData);

        if (newRecords.Count > 0)
        {
            _apiService.AppendDataToSheet(newRecords, $"{GoogleSheetsConstants.SheetName}!A:{columnRange}");
            _apiService.SortSheetByDate();
        }
        else
        {
            Console.WriteLine("No new records to append.");
        }
    }
}
