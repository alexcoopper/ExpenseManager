using ExpenseManager.Constants;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System.IO;
using System.Linq;

public class GoogleSheetsApiService
{
    private static SheetsService _service;

    public GoogleSheetsApiService()
    {
        InitializeService();
    }

    private void InitializeService()
    {
        UserCredential credential;
        using (var stream = new FileStream(GoogleSheetsConstants.CredentialFilePath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                GoogleSheetsConstants.Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(GoogleSheetsConstants.TokenFilePath, true)).Result;
        }

        _service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = GoogleSheetsConstants.ApplicationName,
        });
    }

    public IList<IList<object>> GetSheetData(string range)
    {
        var request = _service.Spreadsheets.Values.Get(GoogleSheetsConstants.SpreadsheetId, range);
        var response = request.Execute();
        return response.Values ?? new List<IList<object>>();
    }

    public void AppendDataToSheet(List<IList<object>> newRecords, string range)
    {
        var valueRange = new ValueRange { Values = newRecords };

        var appendRequest = _service.Spreadsheets.Values.Append(valueRange, GoogleSheetsConstants.SpreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
        appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
        appendRequest.Execute();
    }

    public void SortSheetByDate()
    {
        var sortRequest = new SortRangeRequest
        {
            Range = new GridRange
            {
                SheetId = GetSheetId(),
                StartRowIndex = 1,
                StartColumnIndex = 0,
                EndColumnIndex = GetColumnCount()
            },
            SortSpecs = new List<SortSpec>
            {
                new SortSpec
                {
                    DimensionIndex = 0,
                    SortOrder = "ASCENDING"
                }
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = new List<Request> { new Request { SortRange = sortRequest } }
        };

        var batchRequest = _service.Spreadsheets.BatchUpdate(batchUpdateRequest, GoogleSheetsConstants.SpreadsheetId);
        batchRequest.Execute();
    }

    private int GetSheetId()
    {
        var spreadsheet = _service.Spreadsheets.Get(GoogleSheetsConstants.SpreadsheetId).Execute();
        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == GoogleSheetsConstants.SheetName);
        return sheet?.Properties.SheetId ?? 0;
    }

    private int GetColumnCount()
    {
        var spreadsheet = _service.Spreadsheets.Get(GoogleSheetsConstants.SpreadsheetId).Execute();
        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == GoogleSheetsConstants.SheetName);
        return sheet?.Properties.GridProperties.ColumnCount ?? GoogleSheetsConstants.DefaultColumnCount;
    }

    public string GetColumnRange()
    {
        int columnCount = GetColumnCount();
        return ConvertColumnIndexToLetter(columnCount);
    }

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
