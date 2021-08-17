using System;
using BenchmarkDotNet.Running;

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
        /// <param name="benchmark">runs a benchmark on the algorithm</param>
        static void Main(int algorithm = 2, int bits = 8, uint seed = 0, bool benchmark=false)
        {
            ISampler sampler = algorithm switch {
                1 => new Pach1(bits, seed),
                2 => new Pach2(bits, seed),
                3 => new Pach3(bits, seed),
                4 => new Pach4(bits, seed),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm), "algorithm must be between 1 and 4")
            };

            if (benchmark)
                BenchmarkRunner.Run(typeof(Program).Assembly);
            else
                RunSampler(sampler, bits);
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
