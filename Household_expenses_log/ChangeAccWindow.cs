using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Label = System.Windows.Controls.Label;

namespace Household_expenses_log
{
    public class ChangeAccWindow: MainWindow
    {
        private AppWindow _previous_window;
        private TextBlock _tbk_back;

        public ChangeAccWindow(AppWindow prev_win)
        {
            _previous_window = prev_win;

            //Настраиваем лэйбл
            _tbk_back = new TextBlock();
            _tbk_back.Text = "Вернуться к прошлому профилю";
            _tbk_back.FontSize = 14;
            _tbk_back.Foreground = Brushes.DeepSkyBlue;
            _tbk_back.FontFamily = new FontFamily("Times New Roman");
            _tbk_back.Margin = new Thickness(b_enter.Margin.Left - 90, b_enter.Margin.Top + b_enter.Height + 7, 0, 0);
            _tbk_back.MouseEnter += _tbk_back_MouseEnter;
            _tbk_back.MouseLeave += _tbk_back_MouseLeave;
            _tbk_back.MouseLeftButtonDown += _tbk_back_MouseLeftButtonDown;

            Grid grid_layout = (Grid)b_enter.Parent;
            grid_layout.Children.Add(_tbk_back);
        }

        private void _tbk_back_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _tbk_back.Foreground = Brushes.YellowGreen;
            _tbk_back.TextDecorations = TextDecorations.Underline;
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void _tbk_back_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _tbk_back.Foreground = Brushes.DeepSkyBlue;
            _tbk_back.ClearValue(TextBlock.TextDecorationsProperty);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void _tbk_back_MouseLeftButtonDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Hide();
            _previous_window.Show();
        }


    }
}
