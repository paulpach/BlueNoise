# Pach1

![Samples](example.svg)

## The Good

* Minimum euclidean distance: `n/sqrt(2)`
* Maximum distance to closest neighbor: `3n/sqrt(2)`
* Minimum square distance: `n/2`
* Maximum Square distance to closest neighbor: `3n/2` 
* No tiles
* Infinite
* Suitable for blue noise sampling?
* Every cell has a sample
* O(1) very fast initialization, around 5 ns
* O(1) very fast sampling, around 30ms for 1 million samples

## The Bad

* Pattern is too regular for my taste it does not look organic
* the minimum block distance is relatively small compared to the maximum distance. That means objects will be placed far appart in the game on average.

# How does it work?

First you should understand how the [Dr. Robert's algorithm](http://extremelearning.com.au/isotropic-blue-noise-point-sets/) works. 

Long story short: Dr. Roberts calculates what he calls "balanced permutations", and uses them to shuffle rows and columns of a cannonical grid layout.

## Balanced sequences

Instead of balanced permutations I build "balanced sequences". 
A balanced sequence is an "infinite" sequence of numbers `s(i)` such that `0 <= s(i) < n` and `|s(i+1) - s(i)| <= n / 2`
A balanced sequence is pseudorandom, with no discernible pattern.
A balanced sequence is repeatable,  meaning that if I evaluate `s(i)` more than once, I get the same result.
A balanced sequence can be evaluated at any random point `s(i)` without having to evaluate all previous values, this is in contrast to `rand()` and most other pseudo random number generators.

By "infinite" I mean that the sequence is limited only by the limits of the data types. 
If using 32 bit integers, that means the sequence has 2^32 numbers.  
This can be easily extended to 64 bits values or more, provided a suitable noise function.

For example,  if n = 8, a valid portion of a balanced sequence would be:
```..., 0, 4, 3, 7, 6, 3, 2, 5, 6, ...```

But the following would not be part of a balanced sequence:
```..., 0, 8, ....```

because the difference between 0 and 8 is greater than n/2

I use 2 separate balanced sequences,  one for rows and one for columns.

To calculate s(i):
1. if i is odd,  I simply get a pseudo random number between `[0,n)` using [Squirrel3](https://www.youtube.com/watch?v=LWFzPP8ZbdU).
2. if i is even:
    1. I get the value of s(i-1) and s(i+1). 
    2. calculate the range of numbers that are in range of both values
    3. pick one randomly using [Squirrel3](https://www.youtube.com/watch?v=LWFzPP8ZbdU)

## Picking rows and columns.

When calculating the value for a cell at (x,y):
1. I calculate the row and column by dividing by the cell size (I use bit shift since my cell sizes are powers of 2)
2. I sample s1(row) and s2(col),  
3. I get the canonical grid layout cell using s1(row) and s2(col)






