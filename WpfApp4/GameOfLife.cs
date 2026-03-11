using System;
using System.Threading.Tasks;

namespace WpfApp4
{
    public class GameOfLife
    {
        public int Width { get; }
        public int Height { get; }

        // jednowymiarowy bufor: index = y * Width + x
        public byte[] Field { get; private set; }
        private byte[] _buffer;

        public GameOfLife(int width = 64, int height = 64)
        {
            Width = width;
            Height = height;
            Field = new byte[Width * Height];
            _buffer = new byte[Width * Height];
        }

        public ReadOnlySpan<byte> GetGameField() => Field;

        // Optymalizowany NextCycle, równoległy po wierszach
        public void NextCycle()
        {
            int w = Width;
            int h = Height;
            byte[] src = Field;
            byte[] dst = _buffer;

            Parallel.For(0, h, y =>
            {
                int row = y * w;
                for (int x = 0; x < w; x++)
                {
                    int count = 0;

                    int y0 = Math.Max(0, y - 1);
                    int y1 = Math.Min(h - 1, y + 1);
                    int x0 = Math.Max(0, x - 1);
                    int x1 = Math.Min(w - 1, x + 1);

                    for (int ny = y0; ny <= y1; ny++)
                    {
                        int noff = ny * w;
                        for (int nx = x0; nx <= x1; nx++)
                        {
                            if (nx == x && ny == y) continue;
                            count += src[noff + nx];
                        }
                    }

                    int idx = row + x;
                    if (src[idx] == 1)
                    {
                        dst[idx] = (count < 2 || count > 3) ? (byte)0 : (byte)1;
                    }
                    else
                    {
                        dst[idx] = (count == 3) ? (byte)1 : (byte)0;
                    }
                }
            });

            // swap
            Field = dst;
            _buffer = src;
        }

        public void RandomSeed(int count = 128)
        {
            var rand = new Random();
            int remaining = Math.Min(count, Width * Height);
            Array.Clear(Field, 0, Field.Length);
            while (remaining > 0)
            {
                int idx = rand.Next(Field.Length);
                if (Field[idx] == 0)
                {
                    Field[idx] = 1;
                    remaining--;
                }
            }
        }
    }
}
