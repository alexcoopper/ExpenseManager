using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseManager.Constants;

public class ExpenseFilterService
{
    public List<IList<object>> FilterNewExpenses(List<Expense> newExpenses, IList<IList<object>> existingData)
    {
        var filteredExpenses = new List<IList<object>>();

        foreach (var expense in newExpenses)
        {
            bool exists = existingData.Any(row => row.Count > 4 &&
                DateTime.Parse(row[0].ToString()) == expense.Date &&
                row[4].ToString() == expense.ExpenseOwner.ToString());

            if (!exists)
            {
                filteredExpenses.Add(new List<object>
                {
                    expense.Date.ToString(GoogleSheetsConstants.DateTimeFormat),
                    expense.Description,
                    expense.Category,
                    expense.Summ,
                    expense.ExpenseOwner.ToString()
                });
            }
        }

        return filteredExpenses;
    }
}
