using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
namespace BlueNoise
{
    /// <summary>
    /// Benchmark all sampling algorithms
    /// </summary>
    public class MillionSamples {

        /// <summary>
        /// Samplers to benchmark
        /// </summary>
        /// <returns></returns> 
        public IEnumerable<object[]> Samplers()
        {
            yield return new object[] { new Pach1(8,0), 8 };
            yield return new object[] { new Pach2(8,0), 8 };
            yield return new object[] { new Pach3(8,0), 8 };
            yield return new object[] { new Pach4(8,0), 8 };
        }

        /// <summary>
        /// Runs each sampler 1 million times
        /// </summary>
        /// <param name="sampler"></param>
        /// <param name="bits"></param>
        [Benchmark]
        [ArgumentsSource(nameof(Samplers))]
        public void benchmark(ISampler sampler, int bits)
        {
            int cellSize = 1 << bits;

            int gridSize = 1000;

            for (int x = 0; x < gridSize * cellSize; x += cellSize)
            {
                for (int y = 0; y < gridSize * cellSize; y += cellSize)
                {
                    var sample = sampler[x, y];
                }
            }
        }
    }
}