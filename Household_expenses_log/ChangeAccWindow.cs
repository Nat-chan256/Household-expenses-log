using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            _tbk_back.FontSize = 12;
            _tbk_back.Foreground = Brushes.DeepSkyBlue;
            _tbk_back.TextDecorations = TextDecorations.Underline;
            _tbk_back.FontFamily = new FontFamily("Times New Roman");
            _tbk_back.Margin = new Thickness(b_enter.Margin.Left - 65, b_enter.Margin.Top + b_enter.Height + 5, 0, 0);

            Grid grid_layout = (Grid)b_enter.Parent;
            grid_layout.Children.Add(_tbk_back);
        }
    }
}
