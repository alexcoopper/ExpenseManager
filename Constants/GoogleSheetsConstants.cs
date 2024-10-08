using Google.Apis.Sheets.v4;

namespace ExpenseManager.Constants
{
    public static class GoogleSheetsConstants
    {
        public static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        public const string ApplicationName = "ExpenseManager";
        public const string SpreadsheetId = "1ru9WzJnh2sTHAbeGyt6-E-tZGOQdlFCdzJT3K0YCTuQ";
        public const string CredentialFilePath = "credentials.json";
        public const string TokenFilePath = "token.json";
        public const string SheetName = "Full";
        public const int DefaultStartRow = 2; // First data row (assuming headers in row 1)
        public const int DefaultColumnCount = 5; // Default fallback column count
        public const string DateTimeFormat = "MM/dd/yyyy HH:mm:ss"; // Format for date string
    }

}
