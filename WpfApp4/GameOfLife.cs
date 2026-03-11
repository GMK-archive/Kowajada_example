using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    public class GameOfLife
    {
        // 0 - dead cell, 1 - alive cell
        public byte[,] GameField { get; private set; }
        //constructor (default size 64x64)
        public GameOfLife(int width = 64, int height = 64)
        {
            GameField = new byte[width, height];
        }

        public void NextCycle()
        {
            byte[,] newField = new byte[GameField.GetLength(0), GameField.GetLength(1)];

            for(int x = 0; x<GameField.GetLength (0); x++)
            {
                for (int y = 0; y < GameField.GetLength(1); y++)
                {
                    int liveNeibors = CountNeibors(x, y);

                    if (GameField[x, y] == 1) // alive cell
                    {
                        if (liveNeibors < 2 || liveNeibors > 3)
                            newField[x, y] = 0; // cell dies
                        else
                            newField[x, y] = 1; // cell stays alive
                    }
                    else // dead cell
                    {
                        if (liveNeibors == 3)
                            newField[x, y] = 1; // cell becomes alive , since it has 3 neighbour that are alive
                        else
                            newField[x, y] = 0; // cell stays dead since it does not aply to the rule of becoming alive
                    }
                }
            }

            GameField = newField;
        }
        public int CountNeibors(int x, int y)
        {
            int count = 0;

            for(int i = x-1; i<=x+1; i++)
            {
                for(int j = y-1; j<=y+1; j++)
                {
                    if (i == x && j == y) continue; // skip the cell itself
                    if (i >= 0 && i < GameField.GetLength(0) && j >= 0 && j < GameField.GetLength(1))
                    {
                        if(GameField[i, j] == 1) count++;
                    }
                }
            }

            return count;
        }

        public byte[,] GetGameField()
        {
                       return GameField;
        }

        public void RandomSeed(int count = 128)
        {
            Random rand = new Random();
            while(count > 0)
            {
                int x = rand.Next(GameField.GetLength(0));
                int y = rand.Next(GameField.GetLength(1));
                if (GameField[x, y] == 0)
                {
                    GameField[x, y] = 1;
                    count--;
                }
            }
        }
        
    }
}
