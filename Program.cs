using System;

namespace BlueNoise
{
    class Program
    {
        /// <summary>
        /// Calculates random samples within an area that maintain minimum distance
        /// </summary>
        /// <param name="algorithm">The algorithm to use,  there are 4 algorithms</param>
        /// <param name="bits">number of bits to use for cell size: 1-8</param>
        /// <param name="seed">seed to derive the samples from</param>
        static void Main(int algorithm = 2, int bits = 8, uint seed = 0)
        {
            switch (algorithm) {
                case 1:
                    RunSampler(new Pach1(bits, seed), bits);
                    break;
                case 2:
                    RunSampler(new Pach2(bits, seed), bits);
                    break;
                case 3:
                    RunSampler(new Pach3(bits, seed), bits);
                    break;
                case 4:
                    RunSampler(new Pach4(bits, seed), bits);
                    break;

            }
        }
        
        private static void RunSampler<T>(T sampler, int bits) where T: ISampler
        {
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
