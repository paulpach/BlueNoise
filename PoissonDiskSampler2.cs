
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
public readonly struct PoissonDiskSampler2
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
    public PoissonDiskSampler2(int bits, uint seed)
    {
        this.Noise = new SquirrelNoise(seed);
        this.Bits = bits;
    }

    /// Sequence of numbers such that s(i+1) - s(i) > cellSize / 2
    /// such that as long as samples are placed in the range
    /// they are guaranteed to be at least n blocks appart
    /// or -1 if not possible
    private int GenerateRnd(int i, int seq, int axis)
    {
        int cellSize = 1 << Bits;
        int mask = cellSize - 1;
        int rnd = (int)Noise[i, seq, axis] & mask;

        if (((i + seq) & 0x1) == 0)
        {
            return rnd;
        }
        else {
            int prev = (int)Noise[i-1, seq, axis] & mask;
            int next = (int)Noise[i+1, seq, axis] & mask;;

            if (prev > next)
                return -1;

            return Lerp(prev, next, rnd);
        }
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

    public Sample GenerateSample(int row, int col)
    {
        int sx = GenerateRnd(col, row, 0);
        int sy = GenerateRnd(row, col, 1);

        if (sx == -1 || sy == -1) {
            return new Sample { X = sx, Y = sy, Value = 0 };
        }

        return new Sample{ X = sx, Y = sy, Value = 1 };       
    }

    public Sample this [int x, int y]
    {
        get {            
            int col = x >> Bits;
            int row = y >> Bits;

            int x0 = col << Bits;
            int y0 = row << Bits;

            Sample sample = GenerateSample(row,col);

           // now make sure we don't clash with the diagonals
            Sample ur = GenerateSample(row+1, col+1);
            Sample lr = GenerateSample(row-1, col+1); 

            if (sample.X > ur.X && sample.Y > ur.Y && ur.Valid) {
                return new Sample { X = sample.X, Y = sample.Y, Value = 0 };
            }
            if (sample.X > lr.X && sample.Y < lr.Y && lr.Valid) {
                return new Sample { X = sample.X, Y = sample.Y, Value = 0 };
            }

            sample.X += x0;
            sample.Y += y0;


            return sample;
        }
    }

}