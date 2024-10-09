using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseManager.Constants;

public class ExpenseFilterService
{
    public List<IList<object>> FilterNewExpenses(List<Expense> newExpenses, IList<IList<object>> existingData)
    {
        var dateColumnIndex = 0;
        var expenseOwnerColumnIndex = 3;

        var filteredExpenses = new List<IList<object>>();

        foreach (var expense in newExpenses)
        {
            bool exists = existingData.Any(row => row.Count > 3 &&
                DateTime.Parse(row[dateColumnIndex].ToString()) == expense.Date &&
                row[expenseOwnerColumnIndex].ToString() == expense.ExpenseOwner.ToString());

            if (!exists)
            {
                filteredExpenses.Add(new List<object>
                {
                    expense.Date.ToString(GoogleSheetsConstants.DateTimeFormat),
                    expense.Description,
                    expense.Sum,
                    expense.ExpenseOwner.ToString()
                });
            }
        }

        return filteredExpenses;
    }
}
