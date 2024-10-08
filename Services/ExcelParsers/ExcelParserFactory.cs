using NPOI.HSSF.UserModel;  // For .xls
using NPOI.XSSF.UserModel;  // For .xlsx
using NPOI.SS.UserModel;    // Common interface for both formats
using System.IO;
using NPOI.POIFS.FileSystem;

public class ExcelParserFactory
{
    public IExcelParser GetParser(string filePath)
    {
        IWorkbook workbook = CreateWorkbook(filePath); // First create the correct workbook

        // Now that we have the workbook, check the content of the first cell to determine the parser
        ISheet worksheet = workbook.GetSheetAt(0);
        IRow firstRow = worksheet.GetRow(0);
        ICell firstCell = firstRow.GetCell(0); // A1

        string identifier = firstCell?.ToString(); // Ensure there's a value in A1

        if (string.IsNullOrEmpty(identifier)) 
        {
            throw new InvalidOperationException("Unknown Excel format or no matching parser found.");
        }

        if (identifier.Contains("Клієнт: Баланенко Олексій Євгенович"))
        {
            return new MonoOleksiiParser(workbook);
        }

        if (identifier.Contains("Виписка з Ваших карток за період"))
        {
            return new PrivatDmytroParser(workbook);
        }

        throw new InvalidOperationException("Unknown Excel format or no matching parser found.");
    }

    private IWorkbook CreateWorkbook(string filePath)
    {
        // Read the file into memory first to avoid issues with closing the stream
        byte[] fileBytes = File.ReadAllBytes(filePath);

        try
        {
            // Try to open as .xls format
            using (var memoryStream = new MemoryStream(fileBytes))
            {
                return new HSSFWorkbook(memoryStream);  // Binary Excel file (97-2003)
            }
        }
        catch (OfficeXmlFileException)
        {
            // If it's not an .xls, try to open as .xlsx
            using (var memoryStream = new MemoryStream(fileBytes))
            {
                return new XSSFWorkbook(memoryStream);  // Office Open XML Excel file (.xlsx)
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unsupported Excel format or error creating the workbook: " + ex.Message);
        }
    }

}
