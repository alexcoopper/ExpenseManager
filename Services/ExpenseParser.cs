public class ExpenseParser
{
    private readonly ExcelParserFactory _factory = new ExcelParserFactory();

    public List<Expense> ProcessFile(string filePath)
    {
        try
        {
            IExcelParser parser = _factory.GetParser(filePath);
            return parser.ParseExcel();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }

        return new List<Expense>();
    }
}