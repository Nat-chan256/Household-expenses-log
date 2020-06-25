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
using TextBox = System.Windows.Controls.TextBox;

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

            CreateUsersTable();
        }

        //Создание таблицы с пользователями, если она не ещё не создана
        private void CreateUsersTable()
        {
            string query = "CREATE TABLE IF NOT EXISTS `users` (name VARCHAR(30) NOT NULL, surname VARCHAR(30) NOT NULL," +
                "login VARCHAR(30) NOT NULL, email VARCHAR(30) NOT NULL, password VARCHAR(20) NOT NULL, cur_budget INT DEFAULT 0);";

            MySqlConnection databaseConnection = new MySqlConnection(_connection_string);
            MySqlCommand command = new MySqlCommand(query, databaseConnection);
            command.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                //Открытие базы данных
                databaseConnection.Open();

                //Исполнение запроса
                reader = command.ExecuteReader();
                reader.Close();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            if (this.Visibility == Visibility.Hidden || this.Visibility == Visibility.Collapsed) return; //Выходим без вызова MessageBox

            MessageBoxResult dialog_result = System.Windows.MessageBox.Show("Закрыть приложение?", "Завершение работы", MessageBoxButton.YesNo,
                MessageBoxImage.Question);

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

        private void tb_budget_TextChanged(object sender, TextChangedEventArgs e)
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

        //Проверка на корректность введенных полей
        private bool fieldsCheckSuccesful()
        {
            bool ret_value = true;//Флаг, определяющий, можно ли регистрировать пользователя

            //Проверка полей
            if (lb_warning1.Visibility == Visibility.Visible)
            {
                tb_users_name.BorderBrush = Brushes.Red;
                tb_users_name.BorderThickness = new Thickness(3);
                ret_value = false;
            }
            else if (tb_users_name.Text.Length == 0)
            {
                tb_users_name.BorderBrush = Brushes.Red;
                tb_users_name.BorderThickness = new Thickness(3);
                lb_enter_name.Visibility = Visibility.Visible;
                ret_value = false;
            }

            if (lb_warning2.Visibility == Visibility.Visible)
            {
                tb_email.BorderBrush = Brushes.Red;
                tb_email.BorderThickness = new Thickness(3);
                ret_value = false;
            }
            else if (tb_email.Text.Length == 0)
            {
                tb_email.BorderBrush = Brushes.Red;
                tb_email.BorderThickness = new Thickness(3);
                lb_enter_email.Visibility = Visibility.Visible;
                ret_value = false;
            }

            if (lb_warning3.Visibility == Visibility.Visible)
            {
                pb_pass.BorderBrush = Brushes.Red;
                pb_pass.BorderThickness = new Thickness(3);
                ret_value = false;
            }
            else if (pb_pass.Password.Length == 0)
            {
                pb_pass.BorderBrush = Brushes.Red;
                pb_pass.BorderThickness = new Thickness(3);
                lb_enter_pass.Visibility = Visibility.Visible;
                ret_value = false;
            }

            if (lb_warning4.Visibility == Visibility.Visible)
            {
                pb_pass_repeat.BorderBrush = Brushes.Red;
                pb_pass_repeat.BorderThickness = new Thickness(3);
                ret_value = false;
            }

            if (lb_warning5.Visibility == Visibility.Visible)
            {
                tb_surname.BorderBrush = Brushes.Red;
                tb_surname.BorderThickness = new Thickness(3);
                ret_value = false;
            }
            else if (tb_surname.Text.Length == 0)
            {
                tb_surname.BorderBrush = Brushes.Red;
                tb_surname.BorderThickness = new Thickness(3);
                lb_enter_surname.Visibility = Visibility.Visible;
                ret_value = false;
            }

            if (tb_login.Text.Length == 0)
            {
                tb_login.BorderBrush = Brushes.Red;
                tb_login.BorderThickness = new Thickness(3);
                lb_enter_login.Visibility = Visibility.Visible;
                ret_value = false;
            }

            if (pb_pass_repeat.Password.Length == 0 && pb_pass.Password.Length > 0)
            {
                pb_pass_repeat.BorderBrush = Brushes.Red;
                pb_pass_repeat.BorderThickness = new Thickness(3);
                lb_warning4.Visibility = Visibility.Visible;
                ret_value = false;
            }

            return ret_value;
        }

        //Обработчики событий для кнопок
        private void b_back_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            _previous_window.ClearAllFields();
            _previous_window.Show();
        }

        private void b_sign_up_Click(object sender, RoutedEventArgs e)
        {
            //Проверяем, все ли поля заполнены
            if (!fieldsCheckSuccesful()) return;
        
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
            if (chb_set_budget.IsChecked == true && tb_start_budget.Text.Length > 0)
                registerUser(tb_users_name.Text, tb_surname.Text, tb_login.Text, tb_email.Text, pb_pass.Password, Int32.Parse(tb_start_budget.Text));
            else
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
        private void registerUser(string name, string surname, string login, string email, string password, int budget = 0)
        {   
            string connectionString = "server=localhost;port=3306;user=root;password=;database=household_expenses_log;";
            string query = $"INSERT INTO users(`name`, `surname`, `login`, `email`, `password`, `cur_budget`) VALUES ('{name}', '{surname}', " +
                $"'{login.ToLower().Trim(' ')}', '{email.ToLower()}', '{password}', '{budget}');";

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


        //Метод, убирающий красные рамки с текстбоксов
        private void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox)
            {
                PasswordBox text_box = (PasswordBox)sender;
                text_box.ClearValue(BorderBrushProperty);
                text_box.BorderThickness = new Thickness(1);

                foreach (object child in grid_layout.Children)
                {
                    if (!(child is Label)) continue;

                    if (((Label)child).Tag != null && text_box.Tag != null && ((Label)child).Tag.ToString() == text_box.Tag.ToString())
                        ((Label)child).Visibility = Visibility.Hidden;
                }
            }
            else
            {
                TextBox text_box = (TextBox)sender;
                text_box.ClearValue(BorderBrushProperty);
                text_box.BorderThickness = new Thickness(1);

                foreach (object child in grid_layout.Children)
                {
                    if (!(child is Label)) continue;

                    if (((Label)child).Tag != null && text_box.Tag != null && ((Label)child).Tag.ToString() == text_box.Tag.ToString())
                        ((Label)child).Visibility = Visibility.Hidden;
                }
            }
        }

        //Методы, работающие с checkBox-ом
        private void chb_Checked(object sender, RoutedEventArgs e)
        {
            lb_start_budget.Visibility = Visibility.Visible;
            tb_start_budget.Visibility = Visibility.Visible;
        }

        private void chb_Unchecked(object sender, RoutedEventArgs e)
        {
            lb_start_budget.Visibility = Visibility.Hidden;
            tb_start_budget.Visibility = Visibility.Hidden;
        }
    }
}
