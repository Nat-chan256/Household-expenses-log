using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Label = System.Windows.Controls.Label;

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

        //Метод, очищающий все поля в окне
        public void ClearAllFields()
        {
            tb_users_name.Clear();
            tb_email.Clear();
            pb_pass.Clear();
            pb_pass_repeat.Clear();
        }

        private void SignUpWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult dialog_result = System.Windows.MessageBox.Show("Закрыть приложение?", "Завершение работы", MessageBoxButton.YesNo);

            if (dialog_result == MessageBoxResult.Yes)
                System.Windows.Application.Current.Shutdown(); //Завершаем работу приложения
            else
            {
                e.Cancel = true;
            }
        }

        //Метод, проверяющий, не содержит ли поле с именем пользователя посторонних символов
        private void tb_users_name_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (char l in tb_users_name.Text)
            {
                if (!char.IsLetter(l))
                {
                    //Делаем видимой предупреждающую надпись, если встретили не букву
                    lb_warning1.Visibility = Visibility.Visible;
                    return;
                }
            }
            //Если были введены только буквы, скрываем предупреждение
            lb_warning1.Visibility = Visibility.Hidden;
        }

        private void tb_email_LostFocus(object sender, RoutedEventArgs e)
        {
            //Проверка корректности email
            string email_pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

            if (!Regex.IsMatch(tb_email.Text, email_pattern, RegexOptions.IgnoreCase))
            {
                
                return;
            }
        }
    }
}
