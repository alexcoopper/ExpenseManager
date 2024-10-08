using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;

public class ExcelParser
{
    public List<Expense> ParseExcel(string filePath)
    {
        var expenses = new List<Expense>();
        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheet(1);
            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                var expense = new Expense
                {
                    Date = row.Cell(1).GetString(),
                    Category = row.Cell(2).GetString(),
                    Description = row.Cell(3).GetString(),
                    Summ = row.Cell(5).GetString()
                };
                expenses.Add(expense);
            }
        }
        return expenses;
    }
}

public class Expense
{
    public string Date { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Summ { get; set; }
}