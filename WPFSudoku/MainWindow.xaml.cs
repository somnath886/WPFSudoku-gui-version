using System.Collections.Generic;
using System.Windows;

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
            ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateLayout();
        }

    }
}
