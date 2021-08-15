
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
public readonly struct Pach4 : ISampler
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
    /// <param name="bits">
    ///   Number of bits for the cell size
    /// </param>
    public Pach4(int bits, uint seed)
    {
        this.Noise = new Squirrel3(seed);
        this.Bits = bits;
    }

    private Sample GenerateSample(int row, int col)
    {
        int cellSize = 1 << Bits;
        int mask = cellSize - 1;
        uint rnd = Noise[row, col];

        int x = (int)(rnd & mask);
        int y = (int)((rnd >> Bits) & mask);
        
        return new Sample{ X = x, Y = y, Value = rnd };
                
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
    ///   Generates a sample at the cell containing (x,y)
    /// </summary>
    public Sample this [int x, int y]
    {
        get {            
            int col = x >> Bits;
            int row = y >> Bits;

            int x0 = col << Bits;
            int y0 = row << Bits;
            
            int halfCell = (1 << Bits) >> 1;

            Sample s00 = GenerateSample(row,col);
            Sample s01 = GenerateSample(row,col+1);
            Sample s10 = GenerateSample(row+1,col);
            Sample s11 = GenerateSample(row+1,col+1);

            int x1 = 0;
            int x2 = 256;
            int x3 = (s00.X + s01.X) >> 1;
            int x4 = (s10.X + s11.X) >> 1;

            int y1 = (s00.Y + s10.Y) >> 1;
            int y2 = (s10.Y + s11.Y) >> 1;
            int y3 = 0;
            int y4 = 256;

            int d = (x1-x2) * (y3-y4) - (y1-y2) * (x3-x4);

            int px = (x1*y2 - y1*x2) * (x3-x4) - (x1-x2)*(x3*y4 - y3*x4);
            int py = (x1*y2 - y1*x2) * (y3-y4) - (y1-y2)*(x3*y4 - y3*x4);

            if (d == 0) {
                // no solution
                return new Sample { X = 0, Y = 0, Value = 0 };
            }


            return new Sample { X = px/d + x0, Y = py/d + y0, Value = s00.Value };
        }
    }

}