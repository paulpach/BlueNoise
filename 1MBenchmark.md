# 1 Million Samples Benchmark

In this benchmark,  I compare how long it takes to
generate samples for 1 million cells.

Note I only measure how long it takes to get the samples after the sampler has been initialized.  

Pach4 is extremelly fast after it has been initialized,  all it does is lookup a sample in the pre generated tile.  Pach2 is about 10x slower,  but it initializes 1 million times faster.