using ExpenseManager.Models;

public class Expense
{
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public double Sum { get; set; }
    public ExpenseOwner ExpenseOwner { get; set; }
}