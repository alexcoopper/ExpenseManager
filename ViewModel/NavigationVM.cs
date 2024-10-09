using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.Utilities;
using System.Windows.Input;

namespace ExpenseManager.ViewModel
{
    class NavigationVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand HomeCommand { get; set; }
        public ICommand TransactionsCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void Transaction(object obj) => CurrentView = new TransactionVM();

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            TransactionsCommand = new RelayCommand(Transaction);

            // Startup Page
            CurrentView = new HomeVM();
        }
    }
}
