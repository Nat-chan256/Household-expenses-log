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
using MySql.Data.MySqlClient;
using System.Data;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;

namespace Household_expenses_log
{
    /// <summary>
    /// Логика взаимодействия для SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private MainWindow _previous_window;
        //Связь с базой данных
        private string _connection_string = "server=localhost;port=3306;user=root;password=;database=household_expenses_log;";

        public SignUpWindow(MainWindow previous_window)
        {
            InitializeComponent();
            _previous_window = previous_window;
        }

        //Метод, очищающий все поля в окне
        public void ClearAllFields()
        {
            tb_users_name.Clear();
            tb_surname.Clear();
            tb_login.Clear();
            tb_email.Clear();
            pb_pass.Clear();
            pb_pass_repeat.Clear();
        }

        private void SignUpWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Если окно было скрыто
            if (this.Visibility == Visibility.Hidden) return; //Выходим без вызова MessageBox

            MessageBoxResult dialog_result = System.Windows.MessageBox.Show("Закрыть приложение?", "Завершение работы", MessageBoxButton.YesNo);

            if (dialog_result == MessageBoxResult.Yes)
                System.Windows.Application.Current.Shutdown(); //Завершаем работу приложения
            else
            {
                e.Cancel = true;
            }
        }

        //Методы, проверяющие корректность введенных данных
        private void tb_users_name_TextChanged(object sender, TextChangedEventArgs e)
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

        private void tb_email_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Проверка корректности email
            string email_pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

            if (tb_email.Text.Length == 0 || Regex.IsMatch(tb_email.Text, email_pattern, RegexOptions.IgnoreCase))
            {
                lb_warning2.Visibility = Visibility.Hidden;
            }             
        }

        private void pb_pass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pb_pass.Password.Length >= 6 || pb_pass.Password.Length == 0)
            {
                lb_warning3.Visibility = Visibility.Hidden;
            }

            //Сравнение введенных паролей
            if (pb_pass.Password == pb_pass_repeat.Password)
            {
                lb_warning4.Visibility = Visibility.Hidden;
            }
        }

        private void tb_email_LostFocus(object sender, RoutedEventArgs e)
        {
            //Проверка корректности email
            string email_pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

            if (tb_email.Text.Length > 0 && !Regex.IsMatch(tb_email.Text, email_pattern, RegexOptions.IgnoreCase))
            {
                lb_warning2.Visibility = Visibility.Visible;
            }
            else
            {
                lb_warning2.Visibility = Visibility.Hidden;
            }
        }

        private void pb_pass_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pb_pass.Password.Length < 6 && pb_pass.Password.Length > 0)
            {
                lb_warning3.Visibility = Visibility.Visible;
            }
            else
            {
                lb_warning3.Visibility = Visibility.Hidden;
            }

            //Сравнение введенных паролей
            if (pb_pass.Password != pb_pass_repeat.Password && pb_pass_repeat.Password.Length > 0)
            {
                lb_warning4.Visibility = Visibility.Visible;
            }
        }

        private void pb_pass_repeat_LostFocus(object sender, RoutedEventArgs e)
        {
            //Сравнение паролей
            if (pb_pass_repeat.Password != pb_pass.Password)
            {
                lb_warning4.Visibility = Visibility.Visible;
            }
        }

        private void pb_pass_repeat_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pb_pass_repeat.Password == pb_pass.Password)
            {
                lb_warning4.Visibility = Visibility.Hidden;
            }
        }

        private void tb_surname_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (char l in tb_surname.Text)
            {
                if (!char.IsLetter(l))
                {
                    //Делаем видимой предупреждающую надпись, если встретили не букву
                    lb_warning5.Visibility = Visibility.Visible;
                    return;
                }
            }
            //Если были введены только буквы, скрываем предупреждение
            lb_warning5.Visibility = Visibility.Hidden;
        }


        //Обработчики событий для кнопок
        private void b_back_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            _previous_window.Show();
        }

        private void b_sign_up_Click(object sender, RoutedEventArgs e)
        {
            //Проверка полей
            if (lb_warning1.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Введенное имя некорректно.");
                return;
            }
            else if (lb_warning2.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Введенный email некорректен.");
                return;
            }
            else if (lb_warning3.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Введенный пароль слишком короткий.");
                return;
            }
            else if (lb_warning4.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Введенные пароли не совпадают.");
                return;
            }
            else if (lb_warning5.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Введенная фамилия некорректна.");
                return;
            }
            else if (tb_login.Text.Length == 0)
            {
                MessageBox.Show("Введите логин.");
                return;
            }
            else if (tb_users_name.Text.Length == 0)
            {
                MessageBox.Show("Введите имя.");
                return;
            }
            else if (tb_surname.Text.Length == 0)
            {
                MessageBox.Show("Введите фамилию.");
                return;
            }
            else if (tb_email.Text.Length == 0)
            {
                MessageBox.Show("Введите email.");
                return;
            }
            else if (pb_pass.Password.Length == 0)
            {
                MessageBox.Show("Введите пароль.");
                return;
            }
            else if (pb_pass_repeat.Password.Length == 0)
            {
                MessageBox.Show("Введите пароль повторно.");
                return;
            }

            //Проверяем, существует ли пользователь с введенным email
            if (userWithEmailExists(tb_email.Text))
            {
                MessageBoxResult mb_result = MessageBox.Show("Пользователь с таким email уже существует. Хотите осуществить вход в систему?",
                    "Message", MessageBoxButton.YesNo);
                if (mb_result == MessageBoxResult.Yes) //Если пользователь нажал "Да"
                {
                    _previous_window.Show(); //Открываем форму для входа
                    this.Hide();
                }
                return;
            }

            //Проверяем, существует ли пользователь с таким логином
            if (userWithLoginExists(tb_login.Text))
            {
                MessageBoxResult mb_result = MessageBox.Show("Пользователь с таким логином уже существует. Хотите осуществить вход в систему?",
                    "Message", MessageBoxButton.YesNo);
                if (mb_result == MessageBoxResult.Yes) //Если пользователь нажал "Да"
                {
                    _previous_window.Show(); //Открываем форму для входа
                    this.Hide();
                }
                return;
            }

            //Добавляем пользователя в базу данных 
            registerUser(tb_users_name.Text, tb_surname.Text, tb_login.Text, tb_email.Text, pb_pass.Password);

            //Открываем окно с приложением
            AppWindow app_window = new AppWindow(tb_login.Text.ToLower().Trim(' '));
            app_window.Show();
            this.Hide();
        }

        //Проверка, существует ли email в базе данных
        private bool userWithEmailExists(string user_email)
        {
            //Запрос
            string query = $"SELECT * FROM `users` WHERE `email` = '{user_email.ToLower()}';";

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

        //Проверка, существует ли логин в базе данных
        private bool userWithLoginExists(string login)
        {
            //Запрос
            string query = $"SELECT * FROM `users` WHERE `login` = '{login.ToLower()}';";

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

        //Регистрируем пользователя, т.е. добавляем в БД
        private void registerUser(string name, string surname, string login, string email, string password)
        {   
            string connectionString = "server=localhost;port=3306;user=root;password=;database=household_expenses_log;";
            string query = $"INSERT INTO users(`name`, `surname`, `login`, `email`, `password`) VALUES ('{name}', '{surname}', " +
                $"'{login.ToLower().Trim(' ')}', '{email.ToLower()}', '{password}');";

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                MySqlDataReader myReader = commandDatabase.ExecuteReader(); //Добавляем пользователя в БД
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
