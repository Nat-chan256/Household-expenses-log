﻿using System;
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
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;

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
        private Image _selected_icon;

        public AppWindow(string cur_user_login)
        {
            InitializeComponent();
            _cur_user_login = cur_user_login;
            lb_user_login.Content = cur_user_login;
            _spent_icons = new List<Image>();

            //Ищем баланс пользователя в базе данных
            string balance_query = $"SELECT `cur_budget` FROM `users` WHERE `login` = '{_cur_user_login}';";

            MySqlConnection databaseConnection = new MySqlConnection(_connection_string);
            MySqlCommand commandDatabase = new MySqlCommand(balance_query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader b_reader;

            //Вывод истории
            string history_query = $"SELECT * FROM `operations` WHERE `login` = '{_cur_user_login}';";
            MySqlCommand history_command = new MySqlCommand(history_query, databaseConnection);
            history_command.CommandTimeout = 60;
            MySqlDataReader history_reader;

            try
            {
                //Открытие базы данных
                databaseConnection.Open();

                //Вывод баланса
                b_reader = commandDatabase.ExecuteReader();
                b_reader.Read();
                lb_balance.Content = "Баланс: " + b_reader.GetValue(0).ToString();
                b_reader.Close();

                //Вывод истории
                history_reader = history_command.ExecuteReader();
                history_reader.Read();
                //object[] data = new object[history_reader.FieldCount];
                
              
                //for (int i = 0; i < history; ++i)
                //{ 
                    
                //}
                

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

        //Методы для иконок
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

            if (img != _selected_icon)
                border.BorderBrush.Opacity = 0.0;
        }

        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Убираем красные рамку и надпись
            tc_categories.BorderBrush = Brushes.Black;
            tc_categories.BorderThickness = new Thickness(1);
            lb_choose_category.Foreground = Brushes.Black;
            lb_choose_category.FontWeight = FontWeights.Normal;
            lb_choose_category2.Foreground = Brushes.Black;
            lb_choose_category2.FontWeight = FontWeights.Normal;

            Image img = (Image)sender;

            //Проверяем, выделена ли в текущий момент выбранная иконка
            if (img == _selected_icon) //Если выделена
            {
                _selected_icon = null; //Снимаем с неё выделение
                return;
            }

            //Убираем выделение с уже выбранной иконки, если таковая имеется
            if (_selected_icon != null)
            {
                foreach (Border border in wp_categories.Children)
                {
                    if ((Image)border.Child == _selected_icon)
                    {
                        border.BorderBrush.Opacity = 0;
                        break;
                    }
                }

                foreach (Border border in wp_categories2.Children)
                {
                    if ((Image)border.Child == _selected_icon)
                    {
                        border.BorderBrush.Opacity = 0;
                        break;
                    }
                }
            }
            _selected_icon = img;
        }


        private void b_add_Click(object sender, RoutedEventArgs e)
        {
            //Проверка текстбокса на пустоту
            if (tb_sum.Text.Length == 0)
            {
                tb_sum.BorderBrush = Brushes.Red;
                tb_sum.BorderThickness = new Thickness(3);
                lb_warning.Opacity = 1;
                return;
            }

            //Проверка, выбрана ли категория
            if (_selected_icon == null)
            {
                tc_categories.BorderBrush = Brushes.Red;
                tc_categories.BorderThickness = new Thickness(3);
                lb_choose_category.Foreground = Brushes.Red;
                lb_choose_category.FontWeight = FontWeights.Bold;
                lb_choose_category2.Foreground = Brushes.Red;
                lb_choose_category2.FontWeight = FontWeights.Bold;
                return;
            }

            //Делаем запись в базу данных
            int money_amount = Int32.Parse(tb_sum.Text);
            string spent_got = ((TabItem)tc_categories.SelectedItem).Header.ToString().ToLower();
            DateTime now = DateTime.Now;

            //Создаем таблицу
            string create_table_query = $"CREATE TABLE IF NOT EXISTS `operations` (login VARCHAR(30) NOT NULL, spent_got VARCHAR(15) NOT NULL," +
                $"amount INT NOT NULL, category VARCHAR(40) NOT NULL, date DATETIME NOT NULL);";

            MySqlConnection databaseConnection = new MySqlConnection(_connection_string);
            MySqlCommand create_table_command = new MySqlCommand(create_table_query, databaseConnection);
            create_table_command.CommandTimeout = 60;

            //Создаем запрос на вставку операции
            string insert_op_query = $"INSERT INTO `operations` (`login`, `spent_got`, `amount`, `category`, `date`) VALUES ('{_cur_user_login}'," +
                $" '{spent_got}', {money_amount}, '{_selected_icon.Tag}', '{now.ToString("yyyy-MM-dd H:mm:ss")}');";

            MySqlCommand insert_op_command = new MySqlCommand(insert_op_query, databaseConnection);
            insert_op_command.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                MySqlDataReader reader1 = create_table_command.ExecuteReader();
                reader1.Close();
                MySqlDataReader reader2 = insert_op_command.ExecuteReader(); 
                databaseConnection.Close();

                lb_succes.Opacity = 1;
                //Анимируем затухание надписи о добавлении операции
                DoubleAnimation myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 1.0;
                myDoubleAnimation.To = 0.0;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));

                Storyboard myStoryboard = new Storyboard();
                myStoryboard.Children.Add(myDoubleAnimation);
                Storyboard.SetTarget(myDoubleAnimation, lb_succes);
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Label.OpacityProperty));

                myStoryboard.Begin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void tb_sum_GotFocus(object sender, RoutedEventArgs e)
        {
            //Убираем красные рамку и надпись
            tb_sum.BorderBrush = Brushes.Black;
            tb_sum.BorderThickness = new Thickness(1);
            lb_warning.Opacity = 0;
        }


        private void tab_control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            { 
                
            }
        }
    }
}
