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
        

        return expenses;
    }
}
