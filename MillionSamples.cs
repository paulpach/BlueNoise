using BenchmarkDotNet.Attributes;
namespace BlueNoise
{
    /// <summary>
    /// Benchmark all sampling algorithms
    /// </summary>
    public class MillionSamples {

        /// <summary>
        /// benchmark pach1
        /// </summary>
        [Benchmark]
        public void pach1() => RunSamples(new Pach1(8, 0), 8);

        /// <summary>
        /// benchmark pach2
        /// </summary>
        [Benchmark]
        public void pach2() => RunSamples(new Pach2(8, 0), 8);

        /// <summary>
        /// benchmark pach3
        /// </summary>
        [Benchmark]
        public void pach3() => RunSamples(new Pach3(8, 0), 8);

        /// <summary>
        /// benchmark pach4
        /// </summary>
        [Benchmark]
        public void pach4() => RunSamples(new Pach4(8, 0), 8);

        private static void RunSamples<T>(T sampler, int bits) where T: ISampler
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