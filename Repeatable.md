# Repeatable

My game is multiplayer.  Given a seed I need to replicate the exact same samples in the client and in the server.  This requires 2 particular features from the sample generators:

1) Seeded.  All object placements must be derived from a single 32 bit seed,  so as long as the server and the client have the same seed, they get the same samples. I use a Pseudo random noise function called [Squirrel3](https://www.youtube.com/watch?v=LWFzPP8ZbdU), as a basis for my seeded random number generator, but other noise functions would work fine.
2) No floating point math. This is a big problem because different platforms and even different compilers might produce slightly different results when doing floating point math.  All math must be done exclusively with integer math that is consistent accross platforms and compilers.
