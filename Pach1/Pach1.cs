
using System;
/// <summary>
///   Generates samples with minimum distance
/// </summary>
/// <remarks>
///   This algorithm is a modification of http://extremelearning.com.au/isotropic-blue-noise-point-sets/
///   which can generate infinite amount of samples without tiling
///   and can calculate points at individual cells
///   without calculating neighbors.
///   Requires no storage or initial computations.
/// </remarks>
public readonly struct Pach1 : ISampler
{
    /// Repeatable random number generator
    private readonly Squirrel3 Noise;

    /// the cell size will be 2 ^ bits
    private readonly int Bits;


    /// <summary>
    ///   Generates samples with minimum distance
    /// </summary>
    /// <param name="seed">
    ///   Seed for random number generator
    ///   Different seeds produce different samples
    /// </param>
    /// <param name="width">
    public Pach1(int bits, uint seed)
    {
        this.Noise = new Squirrel3(seed);
        this.Bits = bits;
    }

    /// <summary>
    /// Linear interpolation between a0 and a1
    /// by w/cellSize
    /// </summary>
    public int Lerp(int a0, int a1, int w)
    {
        int cellSize = 1 << Bits;
        // ((a1 - a0) * w + a0;
        return ((a1 - a0) * w + a0 * cellSize + (cellSize >> 1) ) >> Bits;
    }

    /// <summary>   
    /// gets a random value between [0,cellSize) for a given number
    /// but make it so that 2 consecutive values 
    /// are never more than cellSize/2 appart
    /// </summary>
    /// <remark>
    /// in the original article, Dr Roberts used a
    /// balanced permutation.
    /// Instead, I use an infinite randomized
    /// sequence of rows and columns that are "balanced",
    /// meaning 2 consecutive numbers are never more than cellSize/2 appart
    /// Note they may repeat, but that is 
    /// not a problem
    /// </remark>
    private int BalancedSequence(int i, int axis) {
        int cellSize = 1 << Bits;
        int mask = cellSize - 1;
        int rnd = (int)(Noise[i,axis] & mask);

        // to generate the sequence, I take
        // into account two cases
        if ((i & 1) != 0) {
            // odd columns just get a random number
            return rnd;
        }
        else
        {
            // even columns must be chosen
            // so that they are within cellSize/2
            // of both the previous and next column
            int range = cellSize >> 1;

            int prev = BalancedSequence(i - 1, axis);
            int next = BalancedSequence(i + 1, axis);

            int pmin = prev - range;
            int pmax = prev + range;

            int nmin = next - range;
            int nmax = next + range;

            int min = Math.Max(pmin, nmin);
            int max = Math.Min(pmax, nmax);
            min = Math.Max(min, 0);
            max = Math.Min(max, cellSize - 1);
            
            return Lerp(min, max, rnd);
        }

    }

    public Sample this [int x, int y]
    {
        get {            
            int cellSize = 1 << Bits;
            int mask = ~(cellSize - 1);

            // this is the bottom left origin of the cell where (x,y) is
            int x0 = x & mask;
            int y0 = y & mask;

            int row = x >> Bits;
            int col = y >> Bits;

            // pick a random column and row for the cell
            int rndColumn = BalancedSequence(row, 0);
            int rndRow = BalancedSequence(col, 1);

            // pick a row and column from the cannonical grid layout
            // as described in the original article
            return new Sample {
                X = x0 + rndRow,
                Y = y0 + cellSize - rndColumn - 1,
                Value = 1
            };
        }
    }

}