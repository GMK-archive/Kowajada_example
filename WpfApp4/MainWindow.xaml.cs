using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        GameOfLife game;
        WriteableBitmap writeableBitmap;
        byte[] pixelBuffer; // gray8: 1 byte per pixel
        CancellationTokenSource runCts;
        int fpsTarget = 120;

        public MainWindow()
        {
            InitializeComponent();
            NewGame();
            DrawBoardImmediately(); // initial render
        }

        void NewGame()
        {
            int x = 1024;
            int y = 1024;
            int weight = 2;
            int maxSeed = (x * y) / weight;

            game = new GameOfLife(x, y);
            game.RandomSeed(maxSeed);

            // create WriteableBitmap once
            writeableBitmap = new WriteableBitmap(x, y, 96, 96, PixelFormats.Gray8, null);
            pixelBuffer = new byte[x * y]; // reuse for each frame

            GameOfLifeImage.Source = writeableBitmap;
        }

        // Szybkie, jednorazowe zaktualizowanie obrazu (UI thread)
        void DrawBoardImmediately()
        {
            var field = game.GetGameField().ToArray();
            // mapujemy 1 -> 0 (czarny), 0 -> 255 (biały) - możesz odwrócić
            for (int i = 0; i < field.Length; i++)
            {
                pixelBuffer[i] = (byte)(field[i] == 1 ? 0 : 255);
            }

            int stride = writeableBitmap.PixelWidth; // Gray8 : 1 byte per pixel
            writeableBitmap.WritePixels(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight),
                pixelBuffer, stride, 0);
        }

        // Uruchom symulację w tle; aktualizacja obrazu na UI thread
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (runCts != null) return; // już działa
            runCts = new CancellationTokenSource();
            var ct = runCts.Token;
            int frameMs = Math.Max(1, 1000 / fpsTarget);

            Task.Run(async () =>
            {
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        var sw = Stopwatch.StartNew();

                        // 1) oblicz następny cykl (w tle)
                        game.NextCycle();

                        // 2) przygotuj bufor pikseli (w tle)
                        var field = game.GetGameField().ToArray();
                        for (int i = 0; i < field.Length; i++)
                        {
                            pixelBuffer[i] = (byte)(field[i] == 1 ? 0 : 255);
                        }

                        // 3) zaktualizuj WriteableBitmap na wątku UI (krótka operacja)
                        await Dispatcher.InvokeAsync(() =>
                        {
                            int stride = writeableBitmap.PixelWidth;
                            writeableBitmap.WritePixels(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight),
                                pixelBuffer, stride, 0);
                        });

                        sw.Stop();
                        int wait = frameMs - (int)sw.ElapsedMilliseconds;
                        if (wait > 0) await Task.Delay(wait, ct);
                    }
                }
                catch (OperationCanceledException) { }
            }, ct);
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (runCts == null) return;
            runCts.Cancel();
            // pozwól zadaniu się zakończyć
            await Task.Delay(1);
            runCts = null;
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            // pojedynczy krok na UI thread
            game.NextCycle();
            DrawBoardImmediately();
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (runCts != null)
            {
                runCts.Cancel();
                await Task.Delay(1);
                runCts = null;
            }

            NewGame();
            DrawBoardImmediately();
        }
    }
}