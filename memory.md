# Memory use

Pach1,Pach2 and Pach3 require no storage other than the seed and the bit count.  It does not matter how big n is.

Pach4 requires precalculating a tile of samples.  In this code, if the cell size is n,  I precalculate tiles of n*n, although any arbitrary tile size will do.  Thus,  Pach4 requires storage proportional to n*n,  that is O(n*n).  

I did not choose a particularly compact data type,  if n <= 128, then 2 bytes per cell will suffice.
