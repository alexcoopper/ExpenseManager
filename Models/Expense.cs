using System;

namespace ExpenseManager.Models
{
    public class Expense
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}