
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
public readonly struct PoissonDiskSampler3
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
    public PoissonDiskSampler3(int bits, uint seed)
    {
        this.Noise = new SquirrelNoise(seed);
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

    public Sample GenerateSample(int row, int col)
    {
        uint cellSize = 1u << Bits;
        uint mask = cellSize - 1u;
        uint rnd = Noise[row, col];

        int x = (int)(rnd & mask);
        int y = (int) ((rnd >> Bits) & mask);
        uint value = rnd >> (Bits + Bits);

        return new Sample{ X = x, Y = y, Value = value + 1u };       
    }

    public Sample this [int x, int y]
    {
        get {            
            int cellSize = 1 << Bits;

            int x0 = col << Bits;
            int y0 = row << Bits;

            for (int i =x0 - cellSize; i<= x0 + cellSize; i+= cellSize)
            {
                for (int j =y0 - cellSize; j<= 1; j++)
                {
                    if (j == 0 && i == 0)
                        continue;

                    Sample s = GenerateSample(row + i, col + j)


                }
            }

            Sample sll = GenerateSample(row-1, col-1);
            Sample slm = GenerateSample(row-1, col);
            Sample slr = GenerateSample(row-1, col+1);
            Sample sml = GenerateSample(row, col-1);
            Sample smm = GenerateSample(row, col);
            Sample smr = GenerateSample(row, col+1);
            Sample sul = GenerateSample(row+1, col-1);
            Sample sum = GenerateSample(row+1, col);
            Sample sur = GenerateSample(row+1, col+1);

            // now invalidate some
            RowFight(ref sll, ref slm);
            RowFight(ref slm, ref slr);
            RowFight(ref sul, ref sum);
            RowFight(ref sum, ref sur);

            ColFight(ref sll, ref slm);

            return s00;
        }

        private void RowFight(ref Sample left, Sample ref right)
        {
            if (left.X > right.X && left.Valid && right.Valid)
            {
                // they are in conflict, only one wins
                if (left.Value < right.Value)
                    left.Value = 0;
                else
                    right.Value = 0;
            }
        }
    }

}