
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

    private readonly Sample[,] samples;

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
        this.samples = new Sample[1 << bits, 1 << bits];
        fillSamples();        
    }

    private void fillSamples()
    {
        var lcg = new LinearCongruentialGenerator(4 * 4321 + 1, 8671, 16);
        int cellSize = 1 << Bits;
        int mask = cellSize - 1;

        for (int col = 0; col < this.samples.GetLength(0); col++)
        {
            for (int row = 0; row < this.samples.GetLength(1); row++)
            {
                int mangled = (int)lcg.shuffle((col << Bits) | row);

                PlaceSample((mangled >> Bits) & mask, mangled & mask);
            }
        }
    }

    private void PlaceSample(int col, int row)
    {
        int cellSize = 1 << Bits;
        int mask = cellSize - 1;


        int lrow = (row - 1) & mask;
        int lcol = (col - 1) & mask;
        int rcol = (col + 1) & mask;
        int rrow = (row + 1) & mask;

        var lm = samples[col, lrow];
        var ml = samples[lcol, row];
        var mr = samples[rcol, row];
        var um = samples[col, rrow];

        var baseRect = new Rect {
            xmin = ml.Valid ? ml.X : 0,
            xmax = mr.Valid ? mr.X : cellSize-1,
            ymin = lm.Valid ? lm.Y : 0,
            ymax = um.Valid ? um.Y : cellSize-1
        };

        if (!baseRect.Valid)
            return;

        var ll = samples[lcol, lrow];
        var lr = samples[rcol, lrow];
        var ul = samples[lcol, rrow];
        var ur = samples[rcol, rrow];    

        Rect rect = OverlapCalculator.CutOut(ll, lr, ul, ur, baseRect);
        
        if (rect.Valid)
        {
            Sample rnd = GenerateSample(col, row);

            Sample sample = new Sample {
                X = Lerp(rect.xmin, rect.xmax, rnd.X),
                Y = Lerp(rect.ymin, rect.ymax, rnd.Y),
                Value = 1
            };
            samples[col, row] = sample;
        }
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
        get
        {
            int col = x >> Bits;
            int row = y >> Bits;
            int cellSize = 1 << Bits;
            int mask = cellSize - 1;

            Sample sample = samples[col & mask, row & mask];

            int x0 = col << Bits;
            int y0 = row << Bits;

            sample.X += x0;
            sample.Y += y0;
            return sample;
        }
    }

}