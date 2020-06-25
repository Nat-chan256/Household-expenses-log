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
using MySql.Data.MySqlClient;
using System.Data;

namespace Household_expenses_log
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private SignUpWindow _sign_up_window;
        //Связь с базой данных
        private string _connection_string = "server=localhost;port=3306;user=root;password=;database=household_expenses_log;";

        public MainWindow()
        {
            InitializeComponent();
            _sign_up_window = new SignUpWindow(this);
        }

        public void ClearAllFields()
        {
            tb_login.Clear();
            pb_pass.Clear();
        }

        private void b_sign_up_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //Очищаем поля в окне регистрации, если они были заполнены до этого
            _sign_up_window.ClearAllFields();
            _sign_up_window.Show();
        }

        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Если окно было скрыто
            if (this.Visibility == Visibility.Hidden || this.Visibility == Visibility.Collapsed) return; //Выходим без вызова MessageBox

            MessageBoxResult dialog_result = MessageBox.Show("Закрыть приложение?", "Завершение работы", MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (dialog_result == MessageBoxResult.Yes)
                Application.Current.Shutdown(); //Завершаем работу приложения
            else
            {
                e.Cancel = true;
            }
        }

        private void b_enter_Click(object sender, RoutedEventArgs e)
        {
            //Создаем бд с таблицей, если они не существуют
            string create_db_query = "CREATE DATABASE IF NOT EXISTS `household_expenses_log`;";
            string create_table_query = "CREATE TABLE IF NOT EXISTS `users` (name VARCHAR(30) NOT NULL, surname VARCHAR(30) NOT NULL," +
                "login VARCHAR(30) NOT NULL, email VARCHAR(30) NOT NULL, password VARCHAR(20) NOT NULL, cur_budget INT DEFAULT 0);";

            //Ищем пользователя в бд
            string query = $"SELECT `password` FROM `users` WHERE `login` = '{tb_login.Text.ToLower().Trim(' ')}';";

            //Подготовка соединения
            MySqlConnection databaseConnection = new MySqlConnection(_connection_string);
            MySqlCommand create_db_command = new MySqlCommand(create_db_query, databaseConnection);
            MySqlCommand create_table_command = new MySqlCommand(create_table_query, databaseConnection);
            MySqlCommand select_command = new MySqlCommand(query, databaseConnection);
            select_command.CommandTimeout = 60;
            create_table_command.CommandTimeout = 60;
            create_db_command.CommandTimeout = 60;
            MySqlDataReader create_db_reader;
            MySqlDataReader create_table_reader;
            MySqlDataReader select_reader;

            try
            {
                //Открытие базы данных
                databaseConnection.Open();

                //Исполнение запроса
                create_db_reader = select_command.ExecuteReader();
                create_db_reader.Close();
                create_table_reader = select_command.ExecuteReader();
                create_table_reader.Close();
                select_reader = select_command.ExecuteReader();

                if (select_reader.HasRows) //Если нашелся пользователь с таким логином
                {
                    select_reader.Read();
                    if (select_reader.GetValue(0).ToString() == pb_pass.Password) //Если пароль введен верно
                    {
                        this.Hide();
                        AppWindow app_window = new AppWindow(tb_login.Text);
                        app_window.Show(); //Открываем окно с приложением
                    }
                    else 
                    {
                        MessageBox.Show("Пароль неверен.");
                        //Закрываем соединение
                        select_reader.Close();
                        databaseConnection.Close();
                        return;
                    }
                    //Закрываем соединение
                    select_reader.Close();
                    databaseConnection.Close();
                }
                else
                {
                    MessageBoxResult mb_result = MessageBox.Show("Пользователь с таким логином не найден. Хотите зарегистрироваться?",
                        "Ошибка входа", MessageBoxButton.YesNo);

                    if (mb_result == MessageBoxResult.Yes)
                    {
                        this.Hide();
                        _sign_up_window.ClearAllFields();
                        _sign_up_window.Show();
                    }

                    //Закрываем соединение
                    select_reader.Close();
                    databaseConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
