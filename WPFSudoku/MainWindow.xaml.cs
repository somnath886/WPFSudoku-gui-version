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

namespace WPFSudoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CanvasRender canvasrender;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Render(object sender, RoutedEventArgs e)
        {
            canvasrender = new CanvasRender(mCanvas);
        }

        private void Solve(object sender, RoutedEventArgs e)
        {
            canvasrender.SolveSudoku();
        }

    }
}
