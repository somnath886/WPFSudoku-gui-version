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
            tCanvas.Children.Clear();
            canvasrender = new CanvasRender(mCanvas, tCanvas);
        }

        private async void Solve(object sender, RoutedEventArgs e)
        {
            await canvasrender.SolveSudoku();
        }

    }
}
