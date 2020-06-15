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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Household_expenses_log
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SignUpWindow _sign_up_window;
        //Связь с базой данных
        private string _connection_string = "server=localhost;port=3306;user=root;password=;database=household_expenses_log;";

        public MainWindow()
        {
            InitializeComponent();
            _sign_up_window = new SignUpWindow(this);
        }

        private void b_sign_up_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //Очищаем поля в окне регистрации, если они были заполнены до этого
            _sign_up_window.ClearAllFields();
            _sign_up_window.Show();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Если окно было скрыто
            if (this.Visibility == Visibility.Hidden) return; //Выходим без вызова MessageBox

            MessageBoxResult dialog_result = MessageBox.Show("Закрыть приложение?", "Завершение работы", MessageBoxButton.YesNo);

            if (dialog_result == MessageBoxResult.Yes)
                Application.Current.Shutdown(); //Завершаем работу приложения
            else
            {
                e.Cancel = true;
            }
        }

        private void b_enter_Click(object sender, RoutedEventArgs e)
        {
            //Ищем пользователя в бд
            string query = $"SELECT * FROM `users` WHERE `login` = '{tb_login.Text.ToLower().Trim(' ')}' AND `password` = '{pb_pass.Password}';";

            //Подготовка соединения
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

                if (reader.HasRows) //Если уже есть пользователь с таким email
                {
                    //Закрываем соединение
                    databaseConnection.Close();
                    return true;
                }
                else
                {
                    //Закрываем соединение
                    databaseConnection.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }
    }
}
