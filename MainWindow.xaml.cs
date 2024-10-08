using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ExpenseManager
{
    public partial class MainWindow : Window
    {
        private List<Expense> expenses;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var parser = new ExpenseParser();
                    expenses = parser.ProcessFile(openFileDialog.FileName);
                    StatusLabel.Content = $"Імпортовано {expenses.Count} записів.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка під час імпорту: {ex.Message}");
                }
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (expenses == null || expenses.Count == 0)
            {
                MessageBox.Show("Спершу імпортуйте дані з Excel.");
                return;
            }

            try
            {
                var googleSheetsService = new GoogleSheetsService();
                googleSheetsService.WriteDataToSheet(expenses);
                StatusLabel.Content = "Дані успішно завантажено в Google Sheets!";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час завантаження: {ex.Message}");
            }
        }
    }
}