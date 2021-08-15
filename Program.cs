using System;

namespace BlueNoise
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length ==0 || args[0] == "--pach1")
            {
                // the size of the cells is 2 ^ bits
                RunSampler(new Pach1(8, 0));
            }
            else if (args[0] == "--pach2")
            {
                RunSampler(new Pach2(8, 0));
            }
            else if (args[0] == "--pach3")
            {
                RunSampler(new Pach3(8, 0));
            }
            else if (args[0] == "--pach4")
            {
                RunSampler(new Pach4(8, 0));
            }
        }
        
        private static void RunSampler<T>(T sampler) where T: ISampler
        {
            int bits = 8;
            int cellSize = 1 << bits;

            int gridSize = 30;

            for (int x = 0; x < gridSize * cellSize; x += cellSize)
            {
                for (int y = 0; y < gridSize * cellSize; y += cellSize)
                {

                    var sample = sampler[x, y];

                    if (sample.Valid )
                        Console.WriteLine($"{sample.X}, {sample.Y}");
                }
            }
        }
    }
}
