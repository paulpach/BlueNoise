using System;

namespace BlueNoise
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length ==0 || args[0] == "--blue")
            {
                // the size of the cells is 2 ^ bits
                RunBlueNoiseSampler();
            }
            else if (args[0] == "--poisson")
            {
                RunPoissonDisk();
            }
            else if (args[0] == "--poisson2")
            {
                RunPoissonDisk2();
            }
        }

        private static void RunBlueNoiseSampler()
        {
            int bits = 8;
            int cellSize = 1 << bits;

            BlueNoiseSampler sampler = new BlueNoiseSampler(bits, (uint)new System.DateTime().Millisecond);

            int gridSize = 30;

            double radius = gridSize / (2 * Math.Sqrt(2));
            for (int x = 0; x < gridSize * cellSize; x += cellSize)
            {
                for (int y = 0; y < gridSize * cellSize; y += cellSize)
                {

                    var (xs, ys) = sampler[x, y];
                    Console.WriteLine($"{xs}, {ys}");
                }
            }
        }

        private static void RunPoissonDisk()
        {
            int bits = 8;
            int cellSize = 1 << bits;

            PoissonDiskSampler sampler = new PoissonDiskSampler(bits, 0);

            int gridSize = 30;

            for (int x = 0; x < gridSize * cellSize; x += cellSize)
            {
                for (int y = 0; y < gridSize * cellSize; y += cellSize)
                {

                    var sample = sampler[x, y];

                    int row = x >> bits;
                    int col = y >> bits;

                    if (sample.Valid )
                        Console.WriteLine($"{sample.X}, {sample.Y}");
                }
            }
        }
        private static void RunPoissonDisk2()
        {
            int bits = 8;
            int cellSize = 1 << bits;

            PoissonDiskSampler2 sampler = new PoissonDiskSampler2(bits, 0);

            int gridSize = 30;

            for (int x = 0; x < gridSize * cellSize; x += cellSize)
            {
                for (int y = 0; y < gridSize * cellSize; y += cellSize)
                {

                    var sample = sampler[x, y];

                    int row = x >> bits;
                    int col = y >> bits;

                    if (sample.Valid )
                        Console.WriteLine($"{sample.X}, {sample.Y}");
                }
            }
        }
       
    }
}
