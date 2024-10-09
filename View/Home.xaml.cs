using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ExpenseManager.View
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var colorAnimation = new ColorAnimation
                {
                    To = Colors.Red,
                    Duration = TimeSpan.FromSeconds(0.3)
                };

                ((SolidColorBrush)UploadRectangle.Stroke).BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
            }
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    ProcessFile(file);
                }
            }
            resetRectangleColor();
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            resetRectangleColor();
        }

        private void BrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;
                ProcessFile(selectedFile);
            }
        }

        private void resetRectangleColor()
        {
            var colorAnimation = new ColorAnimation
            {
                To = Colors.Gray,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            ((SolidColorBrush)UploadRectangle.Stroke).BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

        private void ProcessFile(string fileName)
        {
            try
            {
                var parser = new ExpenseParser();
                List<Expense> expenses = parser.ProcessFile(fileName);
                StatusLabel.Text = $"Імпортовано {expenses.Count} записів.";
                var googleSheetsService = new GoogleSheetsService();
                googleSheetsService.WriteDataToSheet(expenses);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час імпорту: {ex.Message}");
            }
        }
    }
}
