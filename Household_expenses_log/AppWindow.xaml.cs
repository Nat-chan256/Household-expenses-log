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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Заполняем WrapPanel иконками с категориям в зависимости от значения в комбобоксе
            if (((ComboBoxItem)cb_sent_got.SelectedItem).Content.ToString() == "Потрачено")
            {
                //Добавляем картинки в список иконок
                Uri uri = new Uri(@"C:\Учёбка и не учёбка\Проги\Git Repositories\Household-expenses-log\Household_expenses_log\Images\icon_auto.png");
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.UriSource = uri;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                Image img1 = new Image();
                img1.Height = 48;
                img1.Width = 48;
                img1.Margin = new Thickness(5, 5, 0, 0);
                img1.Source = bitmapimage;
                img1.Stretch = Stretch.Uniform;

                wp_categories.Children.Add(img1);
            }
        }
    }
}
