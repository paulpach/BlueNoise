# Initialization benchmark

Measures the time it takes to initialize a sample generator for cells of size n.

Pach1, Pach2, Pach3 have O(1) initialization time,  it does not matter how big n is.
Pach4 has initialization time proportional to `n*n`,  in other words O(n*n)
