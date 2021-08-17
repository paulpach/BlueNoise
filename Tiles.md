# Tiles

The ideal algorithm will be able to sample any cell in the entire plane,  and there would be no discernible repeating pattern.

Some algorithms such as (Bridson's)[https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf]
require pre calculating a list of samples and storing them in memory. 

To calculate the sample at any arbitrary cell in the plane,  one could simply pre calculate a tile that can be repeated an infinite amount of times, and just get the sample in the right place in the tile.  (Some techniques)[https://kops.uni-konstanz.de/bitstream/123456789/5956/1/Recursive_Wang_Tiles_for_Real_Time_Blue_Noise_2006.pdf] generate many tiles that can be combined together to avoid repetitive patterns.

The disadvantage for tiles is that they require additional memory, and they generate repetitive patterns. There is an inherent tradeoff between how often patterns repeat and how much storage is required.

Pach1,Pach2 and Pach3 require no tiles at all. They can generate in real time a sample in any cell without precalculating a bunch of samples.

Pach4 is tile based, which requires extra storage and it takes much longer to initialize the sampler.
