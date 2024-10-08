using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

public class GoogleSheetsService
{
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static readonly string ApplicationName = "ExpenseManager";
    static readonly string SpreadsheetId = "1ru9WzJnh2sTHAbeGyt6-E-tZGOQdlFCdzJT3K0YCTuQ"; // Замініть на ваш ID таблиці
    static SheetsService service;

    public GoogleSheetsService()
    {
        UserCredential credential;

        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            string credPath = "token.json"; // This is where the token will be saved after the first login
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
        }

        // Create the Google Sheets API service.
        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    public void WriteDataToSheet(List<Expense> expenses)
    {
        var range = "Sheet1!A2";
        var valueRange = new ValueRange();

        var values = new List<IList<object>>();
        foreach (var expense in expenses)
        {
            values.Add(new List<object> { expense.Date, expense.Description, expense.Category, expense.Summ });
        }
        valueRange.Values = values;

        var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        updateRequest.Execute();
    }
}
