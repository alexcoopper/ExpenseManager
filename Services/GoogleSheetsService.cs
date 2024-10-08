using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System.IO;
using System.Linq;
using System.Threading;

public class GoogleSheetsService
{
    // Constants for Spreadsheet configuration
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private const string ApplicationName = "ExpenseManager";
    private const string SpreadsheetId = "1ru9WzJnh2sTHAbeGyt6-E-tZGOQdlFCdzJT3K0YCTuQ";
    private const string CredentialFilePath = "credentials.json";
    private const string TokenFilePath = "token.json";
    private const string SheetName = "Full"; // Constant for sheet name

    private static SheetsService _service;

    public GoogleSheetsService()
    {
        UserCredential credential;
        using (var stream = new FileStream(CredentialFilePath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(TokenFilePath, true)).Result;
        }

        _service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    public void WriteDataToSheet(List<Expense> expenses)
    {
        var existingData = GetExistingDataFromSheet();
        var newRecords = FilterNewExpenses(expenses, existingData);
        AppendDataToSheet(newRecords);
    }

    private IList<IList<object>> GetExistingDataFromSheet()
    {
        // Dynamically calculate the range for existing data based on the number of rows and columns
        var columnRange = GetColumnRange();
        int lastRow = GetLastRow(); // Fetch the last row with data
        var range = $"{SheetName}!A2:{columnRange}{lastRow}"; // Dynamically set the range up to the last row

        var request = _service.Spreadsheets.Values.Get(SpreadsheetId, range);
        var response = request.Execute();
        return response.Values ?? new List<IList<object>>();
    }

    private int GetLastRow()
    {
        var spreadsheet = _service.Spreadsheets.Get(SpreadsheetId).Execute();
        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == SheetName);

        // Use the GridProperties.RowCount to determine the last row if available, otherwise, fall back to 2 (first data row)
        return sheet?.Properties.GridProperties.RowCount ?? 2;
    }

    private List<IList<object>> FilterNewExpenses(List<Expense> newExpenses, IList<IList<object>> existingData)
    {
        var filteredExpenses = new List<IList<object>>();

        foreach (var expense in newExpenses)
        {
            bool exists = existingData.Any(row => row.Count > 4 &&
                DateTime.Parse(row[0].ToString()) == expense.Date &&
                row[4].ToString() == expense.ExpenseOwner.ToString());

            if (!exists)
            {
                filteredExpenses.Add(new List<object>
                {
                    expense.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                    expense.Description,
                    expense.Category,
                    expense.Summ,
                    expense.ExpenseOwner.ToString()
                });
            }
        }

        return filteredExpenses;
    }

    private void AppendDataToSheet(List<IList<object>> newRecords)
    {
        if (newRecords.Count == 0)
        {
            Console.WriteLine("No new records to append.");
            return;
        }

        // Dynamically calculate the range for appending data
        var columnRange = GetColumnRange();
        var range = $"{SheetName}!A:{columnRange}";

        var valueRange = new ValueRange
        {
            Values = newRecords
        };

        var appendRequest = _service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
        appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
        appendRequest.Execute();

        Console.WriteLine("New records appended.");
        SortDataByDate();
    }

    private void SortDataByDate()
    {
        var sortRequest = new SortRangeRequest
        {
            Range = new GridRange
            {
                SheetId = GetSheetId(),
                StartRowIndex = 1, // Start sorting from row 2, assuming row 1 is header
                StartColumnIndex = 0, // Column A
                EndColumnIndex = GetColumnCount() // Dynamically determine number of columns
            },
            SortSpecs = new List<SortSpec>
            {
                new SortSpec
                {
                    DimensionIndex = 0, // Sort by column A (Date)
                    SortOrder = "ASCENDING"
                }
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = new List<Request> { new Request { SortRange = sortRequest } }
        };

        var batchRequest = _service.Spreadsheets.BatchUpdate(batchUpdateRequest, SpreadsheetId);
        batchRequest.Execute();

        Console.WriteLine("Data sorted by Date.");
    }

    private int GetSheetId()
    {
        var spreadsheet = _service.Spreadsheets.Get(SpreadsheetId).Execute();
        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == SheetName);
        return sheet?.Properties.SheetId ?? 0;
    }

    private int GetColumnCount()
    {
        var spreadsheet = _service.Spreadsheets.Get(SpreadsheetId).Execute();
        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == SheetName);
        return sheet?.Properties.GridProperties.ColumnCount ?? 5; // Fallback to 5 if not found
    }

    private string GetColumnRange()
    {
        int columnCount = GetColumnCount();
        return ConvertColumnIndexToLetter(columnCount); // Convert number to column letter (e.g., 5 -> E)
    }

    // Converts a 1-based column index to an Excel-style column letter (A, B, ..., Z, AA, AB, etc.)
    private string ConvertColumnIndexToLetter(int index)
    {
        string letter = string.Empty;
        while (index > 0)
        {
            index--;
            letter = (char)('A' + (index % 26)) + letter;
            index /= 26;
        }
        return letter;
    }
}
