using System;

namespace BlueNoise
{
    class Program
    {
        static void Main(string[] args)
        {
            // the size of the cells is 2 ^ bits
            int bits = 8;
            int cellSize = 1 << bits;

            BlueNoiseSampler sampler = new BlueNoiseSampler(bits, (uint)new System.DateTime().Millisecond);

            int gridSize = 30;

            double radius = gridSize/ (2 * Math.Sqrt(2));
            for (int x = 0; x < gridSize * cellSize; x+= cellSize)
            {
                for (int y = 0; y < gridSize * cellSize; y+= cellSize)
                {

                    var (xs,ys) = sampler[x,y];
                    Console.WriteLine($"{xs}, {ys}");
                }
            }
        }


    }
}
