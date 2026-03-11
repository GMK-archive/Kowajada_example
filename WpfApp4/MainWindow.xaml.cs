using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap bmp;
        GameOfLife game;
        DispatcherTimer timer;
        public MainWindow()
        {

            InitializeComponent();
            NewGame();
            DrawBoard();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // Set the interval for the timer (e.g., 16 ms)
            timer.Stop(); // Start with the timer stopped
            timer.Tick += Timer_Tick; // Subscribe to the Tick event
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            game.NextCycle(); // Advance the game by one cycle
            DrawBoard(); // Redraw the board to reflect the new state
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Start(); // Start the timer when the Start button is clicked
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop(); // Stop the timer when the Stop button is clicked
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            game.NextCycle(); // Advance the game by one cycle
            DrawBoard(); // Redraw the board to reflect the new state
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop(); // Stop the timer when resetting the game
            NewGame(); // Reinitialize the game
            DrawBoard(); // Redraw the board to reflect the new state

        }

        void NewGame()
        {
            // Initialize the bitmap with the same size as the game field
            int x = 256;
            int y = 256;
            int weight = 8;
            int maxSeed = (int)(x * y) / weight; //max seed is made from x multiplied by y which creates one half of generation of max seed which is size of image and then divide the size by weight of randomisation which is an amount of concentration of the dots
            bmp = new Bitmap(x, y);
            game = new GameOfLife(x, y);
            game.RandomSeed(maxSeed); // Initialize the game with a random seed

        }

        void DrawBoard()
        {
            for (int x = 0; x < game.GetGameField().GetLength(0); x++)
            {
                for (int y = 0; y < game.GetGameField().GetLength(1); y++)
                {
                    if (game.GetGameField()[x, y] == 1)
                        bmp.SetPixel(x, y, System.Drawing.Color.Black); // Set alive cells to black
                    else
                        bmp.SetPixel(x, y, System.Drawing.Color.White); // Set dead cells to white
                }
            }
            // Convert Bitmap to BitmapSource for WPF Image control
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            GameOfLifeImage.Source = bitmapSource; // Set the Image control's source to the BitmapSource
        }
    }
}