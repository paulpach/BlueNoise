using BenchmarkDotNet.Attributes;
namespace BlueNoise
{
    /// <summary>
    /// Benchmark all sampling algorithms
    /// </summary>
    public class InitializeBenchmark {

        /// <summary>
        /// benchmark pach1
        /// </summary>
        [Benchmark]
        public ISampler pach1() => new Pach1(8, 0);

        /// <summary>
        /// benchmark pach2
        /// </summary>
        [Benchmark]
        public ISampler pach2() => new Pach2(8, 0);

        /// <summary>
        /// benchmark pach3
        /// </summary>
        [Benchmark]
        public ISampler pach3() => new Pach3(8, 0);

        /// <summary>
        /// benchmark pach4
        /// </summary>
        [Benchmark]
        public ISampler pach4() => new Pach4(8, 0);

        /// <summary>
        /// benchmark pach5
        /// </summary>
        [Benchmark]
        public ISampler pach5() => new Pach5(8, 0);
    }
}