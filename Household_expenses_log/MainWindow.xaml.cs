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
using System.Windows.Controls.Primitives;

namespace Household_expenses_log
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private SignUpWindow _sign_up_window;
        private Popup _login_popup, _pass_popup;
        //Связь с базой данных
        private string _connection_string = "server=localhost;port=3306;user=root;password=;database=household_expenses_log;";

        public MainWindow()
        {
            InitializeComponent();
            _sign_up_window = new SignUpWindow(this);

            createDataBase();
            createUsersTable();

            setPopups();
        }

        private void setPopups()
        {
            _login_popup = new Popup();
            setPopup(_login_popup, "Введите логин.", tb_login);

            _pass_popup = new Popup();
            setPopup(_pass_popup, "Введите пароль.", pb_pass);
        }

        private void setPopup(Popup popup, string text, UIElement target)
        {
            Label popupContent = new Label();
            popupContent.Content = text;
            popupContent.Background = Brushes.PapayaWhip;
            popupContent.Foreground = Brushes.Tomato;
            popup.Child = popupContent;
            popup.PlacementTarget = target;
            popup.Placement = PlacementMode.Bottom;
            popup.IsOpen = false;
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
            //Проверка полей на пустоту
            if (fieldsAreEmpty()) return;

            //Ищем пользователя в бд
            string login = tb_login.Text.ToLower().Trim(' ');
            string query = $"SELECT `password` FROM `users` WHERE `login` = '{login}';";

            //Подготовка соединения
            MySqlConnection databaseConnection = new MySqlConnection(_connection_string);
            MySqlCommand select_command = new MySqlCommand(query, databaseConnection);
            select_command.CommandTimeout = 60;
            MySqlDataReader select_reader;

            try
            {
                //Открытие базы данных
                databaseConnection.Open();

                //Исполнение запросa
                select_reader = select_command.ExecuteReader();

                if (select_reader.HasRows) //Если нашелся пользователь с таким логином
                {
                    select_reader.Read();

                    checkPassword(login, select_reader.GetValue(0).ToString(), pb_pass.Password);
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

        private void tb_login_GotFocus(object sender, RoutedEventArgs e)
        {
            _login_popup.IsOpen = false;
        }

        private void pb_pass_GotFocus(object sender, RoutedEventArgs e)
        {
            _pass_popup.IsOpen = false;
        }


        //-----------------------------------------Работа с БД-------------------------------------------------------------------
        //Метод, создающий БД, если она не существует
        private void createDataBase()
        {
            string query = "CREATE DATABASE IF NOT EXISTS `household_expenses_log`;";

            MySqlConnection databaseConnection = new MySqlConnection("server=localhost;port=3306;user=root;password=;");
            MySqlCommand command = new MySqlCommand(query, databaseConnection);
            command.CommandTimeout = 60;

            try
            {
                //Открытие базы данных
                databaseConnection.Open();

                //Исполнение запроса
                command.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Метод, создающий таблицу "user", если она ещё не создана
        private void createUsersTable()
        {
            string query = "CREATE TABLE IF NOT EXISTS `users` (name VARCHAR(30) NOT NULL, surname VARCHAR(30) NOT NULL," +
                "login VARCHAR(30) NOT NULL, email VARCHAR(30) NOT NULL, password VARCHAR(20) NOT NULL, cur_budget INT DEFAULT 0);";

            MySqlConnection databaseConnection = new MySqlConnection(_connection_string);
            MySqlCommand command = new MySqlCommand(query, databaseConnection);
            command.CommandTimeout = 60;

            try
            {
                //Открытие базы данных
                databaseConnection.Open();

                //Исполнение запроса
                command.ExecuteNonQuery();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


//----------------------------------------Методы-проверки-----------------------------------------------------------------
        private void checkPassword(string login, string pass_from_db, string input_password)
        {
            if (pass_from_db == input_password) //Если пароль введен верно
            {
                this.Hide();
                AppWindow app_window = new AppWindow(login);
                app_window.Show(); //Открываем окно с приложением
            }
            else
            {
                MessageBox.Show("Пароль неверен.");
            }
        }

        private bool fieldsAreEmpty()
        {
            bool ret_value = false;
            if (tb_login.Text.Length == 0)
            {
                _login_popup.IsOpen = true;
                ret_value = true;
            }
            if (pb_pass.Password.Length == 0)
            {
                _pass_popup.IsOpen = true;
                ret_value = true;
            }
            return ret_value;
        }
    }
}
