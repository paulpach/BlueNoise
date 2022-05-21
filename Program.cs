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
        /// <param name="coverage">Runs the algorithm and determines how close to maximal it is</param>
        static void Main(int algorithm = 2, int bits = 8, uint seed = 0, bool benchmark=false, bool coverage=false)
        {
            ISampler sampler = algorithm switch {
                1 => new Pach1(bits, seed),
                2 => new Pach2(bits, seed),
                3 => new Pach3(bits, seed),
                4 => new Pach4(bits, seed),
                5 => new Pach5(bits, seed),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm), "algorithm must be between 1 and 4")
            };

            if (benchmark)
                BenchmarkRunner.Run(typeof(Program).Assembly);
            else if (coverage)
                Stats(sampler, bits);
            else
                RunSampler(sampler, bits);
        }

        private static void Stats(ISampler sampler, int bits)
        {
            int cellSize = 1 << bits;

            int gridSize = 30;
            int missed = 0;
            int placed = 0;

            for (int x = 0; x < gridSize * cellSize; x += cellSize)
            {
                for (int y = 0; y < gridSize * cellSize; y += cellSize)
                {
                    var sample = sampler[x, y];

                    if (sample.Valid )
                        placed++;
                    else
                    {
                        Rect rect = GetUncoveredRect(sampler, bits, x , y );

                        if (rect.Valid)
                        {
                            missed++;
                        }
                    }
                }
            }

            Console.WriteLine($"Placed {placed}, missed {missed}, percentage {placed * 100 / (missed + placed)}%");
        }

        private static Rect GetUncoveredRect(ISampler sampler, int bits, int x0, int y0)
        {
            int cellSize = 1 << bits;

            Sample ll = sampler[x0 - cellSize, y0 - cellSize];
            Sample lm = sampler[x0, y0 - cellSize];
            Sample lr = sampler[x0 + cellSize, y0 - cellSize];
            Sample ml = sampler[x0 - cellSize, y0];
            Sample mm = sampler[x0, y0];
            Sample mr = sampler[x0 + cellSize, y0];
            Sample ul = sampler[x0 - cellSize, y0 + cellSize];
            Sample um = sampler[x0, y0 + cellSize];
            Sample ur = sampler[x0 + cellSize, y0 + cellSize];
            ll.X -= x0 - cellSize;
            ll.Y -= y0 - cellSize;
            lm.X -= x0;
            lm.Y -= y0 - cellSize;
            lr.X -= x0 + cellSize;
            lr.Y -= y0 - cellSize;
            ml.X -= x0 - cellSize;
            ml.Y -= y0;
            mr.X -= x0 + cellSize;
            mr.Y -= y0;
            ul.X -= x0 - cellSize;
            ul.Y -= y0 + cellSize;
            um.X -= x0;
            um.Y -= y0 + cellSize;
            ur.X -= x0 + cellSize;
            ur.Y -= y0 + cellSize;            

            var rect = new Rect
            {
                xmin = ml.Valid ? ml.X : 0,
                xmax = mr.Valid ? mr.X : cellSize - 1,
                ymin = lm.Valid ? lm.Y : 0,
                ymax = um.Valid ? um.Y : cellSize - 1
            };

            return OverlapCalculator.CutOut(ll, lr, ul, ur, rect);

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
