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
using MySql.Data.MySqlClient;
using System.Data;

namespace Household_expenses_log
{
    /// <summary>
    /// Логика взаимодействия для AppWindow.xaml
    /// </summary>
    public partial class AppWindow : Window
    {
        private string _cur_user_login;
        private string _connection_string = "server=localhost;port=3306;user=root;password=;database=household_expenses_log;";
        private List<Image> _spent_icons;

        public AppWindow(string cur_user_login)
        {
            InitializeComponent();
            _cur_user_login = cur_user_login;
            lb_user_login.Content = cur_user_login;
            _spent_icons = new List<Image>();

            //Ищем баланс пользователя в базе данных
            string query = $"SELECT `cur_budget` FROM `users` WHERE `login` = '{_cur_user_login}';";

            MySqlConnection databaseConnection = new MySqlConnection(_connection_string);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                //Открытие базы данных
                databaseConnection.Open();

                //Исполнение запроса
                reader = commandDatabase.ExecuteReader();
                reader.Read();
                lb_balance.Content = "Баланс: " + reader.GetValue(0).ToString();

                databaseConnection.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AppWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Если окно было скрыто
            if (this.Visibility == Visibility.Hidden || this.Visibility == Visibility.Collapsed) return; //Выходим без вызова MessageBox

            MessageBoxResult dialog_result = MessageBox.Show("Закрыть приложение?", "Завершение работы", MessageBoxButton.YesNo);

            if (dialog_result == MessageBoxResult.Yes)
                Application.Current.Shutdown(); //Завершаем работу приложения
            else
            {
                e.Cancel = true;
            }
        }

        //Метод, ограничивающий ввод не-цифр
        private void tb_sum_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text_box = (TextBox)sender;
            int carret_index = text_box.CaretIndex;

            //Проверяем, не начинается ли строка с 0
            if (text_box.Text.Length > 0 && text_box.Text[0] == '0')
            {
                text_box.Text = text_box.Text.Remove(0, 1);
                if (carret_index > 0) carret_index--;
            }

            //Проверяем, не введены ли буквы или другие посторонние символы
            for (int i = 0; i < text_box.Text.Length; ++i)
                if (!Char.IsDigit(text_box.Text[i]))
                {
                    text_box.Text = text_box.Text.Remove(i, 1);
                    if (i < carret_index) carret_index--;
                }

            text_box.CaretIndex = carret_index;
        }

        private void img_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            Border border = (Border)img.Parent;
            border.BorderBrush.Opacity = 1.0;
        }

        private void img_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            Border border = (Border)img.Parent;
            border.BorderBrush.Opacity = 0.0;
        }

        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            Border border = (Border)img.Parent;
            WrapPanel wrap_panel = (WrapPanel)border.Parent;

            //Проверяем, выделена ли в текущий момент выбранная иконка
            
        }
    }
}
