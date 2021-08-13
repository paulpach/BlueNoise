
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
public readonly struct Pach2
{
    /// Repeatable random number generator
    private readonly Squirrel3 Noise;

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
    public Pach2(int bits, uint seed)
    {
        this.Noise = new Squirrel3(seed);
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
                // odd row, even column
                sample = GetEvenColSample(col, row);
            }
            else
            {
                // odd row, odd column
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
        int halfCell = cellSize >> 1;
        int mask = cellSize - 1;

        // every even cell will be colored with white or black and will alternate
        // That means that only one of s0 and s1 will be black
        // this variable determines which one it is
        bool prevIsWhite = ((row ^ col) & 2) == 0; 

        uint whiteValue = prevIsWhite ? s0.Value : s1.Value;
        uint blackValue = prevIsWhite ? s1.Value : s0.Value;

        // get the y value from the black one one:
        int y = prevIsWhite ? (int)blackValue & mask : ((int)blackValue >> Bits) & mask;


        // get the y values from the black one:
        int x1 = (int)(whiteValue & mask);
        int x2 = (int)((whiteValue >> Bits) & mask);

        int x = prevIsWhite ? Math.Max(x1, x2) : Math.Min(x1,x2);

        bool valid = x >= s0.X && x <= s1.X;

        return new Sample {
            X = x,
            Y = y,
            Value = valid ? 1u : 0u
        };
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
        int mask = cellSize - 1;

        // every even cell will be colored with white or black and will alternate
        // That means that only one of s0 and s1 will be black
        // this variable determines which one it is
        bool prevIsWhite = ((row ^ col) & 2) == 0; 
        
        // get the x value from the white one:
        int x = prevIsWhite ? (int)s0.Value & mask : ((int)s1.Value >> Bits) & mask;

        Sample blackSample = prevIsWhite ? s1 : s0;

        // get the y values from the black one:
        int y1 = (int)(blackSample.Value & mask);
        int y2 = (int)((blackSample.Value >> Bits) & mask);

        int y = prevIsWhite ? Math.Min(y1, y2) : Math.Max(y1,y2);

        bool valid = y >= s0.Y && y <= s1.Y;

        return new Sample {
            X = x,
            Y = y,
            Value = valid ? 1u : 0u
        };
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

        // no where to put the sample
        if (minx > maxx || miny > maxy) {
            return new Sample{ X = 0, Y = 0, Value = 0 };
        }


        Sample sample = GenerateCandidate(col, row);

        // (x,y) do not interfere with the sides
        int x = Lerp(minx, maxx, sample.X);
        int y = Lerp(miny, maxy, sample.Y);
        
        int conflictMinx = minx;
        int conflictMaxx = maxx;
        int conflictMiny = miny;
        int conflictMaxy = maxy;

        if (x < ll.X)
            conflictMiny = Math.Max(conflictMiny, ll.Y);
        if (x < ul.X)
            conflictMaxy = Math.Min(conflictMaxy, ul.Y);
        if (x > lr.X)
            conflictMiny = Math.Max(conflictMiny, lr.Y);
        if (x > ur.X)
            conflictMaxy = Math.Min(conflictMaxy, ur.Y);

        if (y < ll.Y)
            conflictMinx = Math.Max(conflictMinx, ll.X);
        if (y < lr.Y)
            conflictMaxx = Math.Min(conflictMaxx, lr.X);
        if (y > ul.Y)
            conflictMinx = Math.Max(conflictMinx, ul.X);
        if (y > ur.Y)
            conflictMaxx = Math.Min(conflictMaxx, ur.X);

        // find out conflicts
        bool llConflict = ll.X > x && ll.Y > y;
        bool ulConflict = ul.X > x && ul.Y < y;
        bool lrConflict = lr.X < x && lr.Y > y;
        bool urConflict = ur.X < x && ur.Y < y;

        if (llConflict || ulConflict || lrConflict || urConflict) {
            // can we solve the conflict by moving in X or Y direction?
            if (conflictMinx > conflictMaxx && conflictMiny > conflictMaxy) {
                return new Sample { X = 0, Y = 0, Value = 0 };
            }

            // shift only x or y to solve the conflict,  never both
            if (conflictMaxx - conflictMinx > conflictMaxy - conflictMiny) {
                x = Lerp(conflictMinx, conflictMaxx, sample.X);
            }
            else {
                y = Lerp(conflictMiny, conflictMaxy, sample.Y);
            }

        }

        return new Sample{ X = x, Y = y, Value = 1 };
   }

}