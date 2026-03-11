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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap bmp;
        GameOfLife game ;
        public MainWindow()
        {
            
            InitializeComponent();
            // Initialize the bitmap with the same size as the game field
            int x = 128;
            int y = 128;
            int weight = 128;
            int maxSeed = (int)(x*y)/weight; //max seed is made from x multiplied by y which creates one half of generation of max seed which is size of image and then divide the size by weight of randomisation which is an amount of concentration of the dots
            bmp = new Bitmap(x, y);
            game = new GameOfLife(x, y);
            game.RandomSeed(maxSeed); // Initialize the game with a random seed
            for(int xd = 0; xd < bmp.Width; xd++)
            {
                for(int yd = 0; yd < bmp.Height; yd++)
                {
                    if (game.GameField[xd, yd] == 1)
                        bmp.SetPixel(xd, yd, System.Drawing.Color.Black);
                    else
                        bmp.SetPixel(xd, yd, System.Drawing.Color.White);
                }
            }
            ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            GameOfLifeImage.Source = imgSrc;
        }
    }
}