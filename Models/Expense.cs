using ExpenseManager.Models;

public class Expense
{
    public DateTime Date { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Summ { get; set; }
    public ExpenseOwner ExpenseOwner { get; set; }
}