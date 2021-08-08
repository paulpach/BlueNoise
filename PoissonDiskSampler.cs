
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
public readonly struct PoissonDiskSampler
{
    /// Repeatable random number generator
    private readonly SquirrelNoise Noise;

    /// the cell size will be 2 ^ bits
    private readonly int Bits;

    /// A sample is a (x,y) coordinate plus a random value
    public struct Sample {
        public int X;
        public int Y;

        // value is just a random number associated with this sample
        // but there is a special case,  if Value == 0, then 
        // this is not a valid sample, and should be ignored
        public uint Value;  

        public bool Valid => Value != 0;
    }

    /// <summary>
    ///   Generates samples with minimum distance
    /// </summary>
    /// <param name="seed">
    ///   Seed for random number generator
    ///   Different seeds produce different samples
    /// </param>
    /// <param name="width">
    public PoissonDiskSampler(int bits, uint seed)
    {
        this.Noise = new SquirrelNoise(seed);
        this.Bits = bits;
    }

    // generate a possible sample at a corner
    // the result is a random x,y number between 0 and 2^bits
    private Sample GenerateCandidate(int row, int col)
    {
        uint cellSize = 1u << Bits;
        uint mask = cellSize - 1;

        uint rnd = Noise[row,col];
        int xr = (int)(rnd & mask);
        rnd >>= Bits;
        int yr = (int)(rnd & mask);
        rnd >>= Bits;

        // plus one so that this is not considered an invalid sample
        return new Sample { X = xr, Y = yr, Value = rnd + 1 };
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

    public Sample this [int x, int y]
    {
        get {            
            int col = x >> Bits;
            int row = y >> Bits;

            // this is the bottom left origin of the cell where (x,y) is
            int x0 = col << Bits;
            int y0 = row << Bits;

            Sample sample;

            // there are 4 cases:
            if ((row & 1) == 0 && (col & 1) == 0) {
                // even row, even column
                // these cells are fully randomized, just get a sample and return it
                sample = GenerateCandidate(col, row);
            }
            else if ((row & 1) == 0 && (col & 1) == 1) {
                // even row, odd column

                sample = GetEvenRowSample(col, row);
            }
            else if ((row & 1) == 1 && (col & 1) == 0) {
                sample = GetEvenColSample(col, row);
            }
            else
            {
                sample = GetEvenRowEvenColSample(col, row);
            }

            sample.X += x0;
            sample.Y += y0;
            return sample;

            // row is even, col is odd
            // row is odd, col is even
            // row is even, col is even
        }
    }

    private Sample GetEvenRowSample(int col, int row)
    {
        Sample s0 = GenerateCandidate(col - 1, row);
        Sample s1 = GenerateCandidate(col + 1, row);
        return GetRowAdjustedSample(col, row, s0, s1);
    }

    private Sample GetRowAdjustedSample(int col, int row, Sample s0, Sample s1)
    {
        int cellSize = 1 << Bits;
        int cellEnd = cellSize - 1;

        // generate a new sample for this cell and adjust it
        // for the neighbor cells
        // both neightbors are on the same row
        Sample result = GenerateCandidate(col, row);

        // invalidate sample if it does not fit
        if (s0.X >= s1.X)
            result.Value = 0;
        else
            result.X = Lerp(s0.X, s1.X, result.X);


        int yprev = (int)(s0.Value & cellEnd);
        int ynext = cellEnd  - (int)(s1.Value & cellEnd);

        result.Y = Lerp(yprev, ynext, result.X);

        return result;
    }

    private Sample GetEvenColSample(int col, int row)
    {
        Sample s0 = GenerateCandidate(col, row - 1);
        Sample s1 = GenerateCandidate(col, row + 1);
        return GetColAdjustedSample(col, row, s0, s1);
    }

    // s0 is the sample at the previous column
    // s1 is the sample at the next column
    // both of them would be at the same row
    private Sample GetColAdjustedSample(int col, int row, Sample s0, Sample s1)
    {
        int cellSize = 1 << Bits;
        int halfCell = cellSize >> 1;
        int cellEnd = cellSize - 1;

        // generate a new sample for this cell and adjust it
        // for the neighbor cells
        // both neightbors are on the same column
        Sample result = GenerateCandidate(col, row);
        
        // invalidate sample if it does not fit
        if (s0.Y >= s1.Y)
            result.Value = 0;
        else
            result.Y = Lerp(s0.Y, s1.Y, result.Y);

        int xprev = cellEnd  - (int)(s0.Value & cellEnd);
        int xnext = (int)(s1.Value & cellEnd);

        result.X = Lerp(xprev, xnext, result.Y);

        return result;
    }

    private Sample GetEvenRowEvenColSample(int col, int row)
    {
        int cellSize = 1 << Bits;
        int halfCell = cellSize >> 1;

        // samples at 4 corners
        Sample ll = GenerateCandidate(col - 1, row - 1);
        Sample ul = GenerateCandidate(col - 1, row + 1);
        Sample lr = GenerateCandidate(col + 1, row - 1);
        Sample ur = GenerateCandidate(col + 1, row + 1);

        // samples at 4 neighbor middles
        Sample lowerMiddle = GetRowAdjustedSample(col, row - 1, ll, lr);
        Sample upperMiddle = GetRowAdjustedSample(col, row + 1, ul, ur);
        Sample middleLeft = GetColAdjustedSample(col - 1, row, ll, ul);
        Sample middleRight = GetColAdjustedSample(col + 1, row, lr, ur);


        int minx = middleLeft.Valid ? middleLeft.X : 0;
        int miny = lowerMiddle.Valid ? lowerMiddle.Y : 0;
        int maxx = middleRight.Valid ? middleRight.X : cellSize - 1;
        int maxy = upperMiddle.Valid ? upperMiddle.Y : cellSize - 1;

        // also take into account corners, just in case they are close to the center
        if (ll.Y > halfCell)
            minx = Math.Max(minx, ll.X);
        if (ul.Y <= halfCell)
            minx = Math.Max(minx, ul.X);
        if (lr.Y > halfCell)
            maxx = Math.Min(maxx, lr.X);
        if (ur.Y <= halfCell)
            maxx = Math.Min(maxx, ur.X);
        
        if (ll.X > halfCell)
            miny = Math.Max(miny, ll.Y);
        if (ul.X <= halfCell)
            miny = Math.Max(miny, ul.Y);
        if (lr.X > halfCell)
            maxy = Math.Min(maxy, lr.Y);
        if (ur.X <= halfCell)
            maxy = Math.Min(maxy, ur.Y);

        // generate a new sample for this cell and adjust it
        // for the neighbor cells
        Sample result = GenerateCandidate(col, row);    

        // invalidate sample if it does not fit
        if (miny >= maxy || minx >= maxx)
            result.Value = 0;
        else
        {
            result.X = Lerp(minx, maxx, result.X);
            result.Y = Lerp(miny, maxy, result.X);            
        }

        return result;
    }

}