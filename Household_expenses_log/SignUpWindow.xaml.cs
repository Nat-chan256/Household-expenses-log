using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Household_expenses_log
{
    /// <summary>
    /// Логика взаимодействия для SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private MainWindow _previous_window;
        public SignUpWindow(MainWindow previous_window)
        {
            InitializeComponent();
            _previous_window = previous_window;
        }

        public void ClearAllFields()
        { 
            
        }

        private void SignUpWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult dialog_result = MessageBox.Show("Закрыть приложение?", "Завершение работы", MessageBoxButton.YesNo);

            if (dialog_result == MessageBoxResult.Yes)
                Application.Current.Shutdown(); //Завершаем работу приложения
            else
            {
                e.Cancel = true;
            }
        }
    }
}
