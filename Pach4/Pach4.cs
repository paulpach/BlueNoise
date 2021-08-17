
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
                int mangled = (int)lcg.shuffle(col << Bits | row);

                PlaceSample(mangled >> Bits, mangled & mask);
            }
        }
    }

    private struct Rect {
        public int xmin;
        public int xmax;
        public int ymin;
        public int ymax;

        public bool Valid => xmax > xmin && ymax > ymin;

        public int Area => Valid ? (xmax - xmin + 1) * (ymax - ymin + 1) : -1;

        internal Rect cutLeft(int x)
        {
            return new Rect {
                xmin = xmin,
                xmax = Math.Min(x, xmax),
                ymin = ymin,
                ymax = ymax
            };
        }
        internal Rect cutRight(int x)
        {
            return new Rect {
                xmin = x,
                xmax = xmax,
                ymin = ymin,
                ymax = ymax
            };
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

        Rect baseRect = new Rect {
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

        Rect rect = OcludeLL(ll, lr, ul, ur, baseRect);
        
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

    private Rect OcludeLL(Sample ll, Sample lr, Sample ul, Sample ur, Rect r)
    {
        if (!ll.Valid)
            return OcludeLR(lr, ul, ur, r);
        if (ll.X <= r.xmin || ll.Y <= r.ymin)
            return OcludeLR(lr, ul, ur, r);
        if (ll.Y > r.ymax)
        {
            r.xmin = ll.X;
            return OcludeLR(lr, ul, ur, r);
        }
        if (ll.X > r.xmax)
        {
            r.ymin = ll.Y;
            return OcludeLR(lr, ul, ur, r);
        }

        // left side
        Rect left = r.cutLeft(ll.X);
        left.ymin = ll.Y;

        Rect result = OcludeLR(lr, ul, ur, left);
        Rect right = r.cutRight(ll.X);
        Rect rightResult = OcludeLR(lr, ul, ur, right);
        if (rightResult.Area > result.Area)
            result = rightResult;

        return result;
    }

    private Rect OcludeLR(Sample lr, Sample ul, Sample ur, Rect r)
    {
        if (!r.Valid)
            return r;
        if (!lr.Valid)
            return OcludeUL(ul, ur, r);
        if (lr.X >= r.xmax || lr.Y <= r.ymin)
            return OcludeUL(ul, ur, r);
        if (lr.Y > r.ymax)
        {
            r.xmax = lr.X;
            return OcludeUL(ul, ur, r);
        }
        if (lr.X < r.xmin)
        {
            r.ymin = lr.Y;
            return OcludeUL(ul, ur, r);
        }

        // right side
        Rect right = r.cutRight(lr.X);
        right.ymin = lr.Y;
        Rect result = OcludeUL(ul, ur, right);
        Rect left = r.cutLeft(lr.X);
        Rect leftResult = OcludeUL(ul, ur, left);
        if (leftResult.Area > result.Area)
            result = leftResult;
        return result;
    }

    private Rect OcludeUL(Sample ul, Sample ur, Rect r)
    {
        if (!r.Valid)
            return r;
        if (!ul.Valid)
            return OcludeUR(ur, r);
        if (ul.X <= r.xmin || ul.Y >= r.ymax)
            return OcludeUR(ur, r);
        if (ul.Y < r.ymin)
        {
            r.xmin = ul.X;
            return OcludeUR(ur, r);
        }
        if (ul.X > r.xmax)
        {
            r.ymax = ul.Y;
            return OcludeUR(ur, r);
        }

            // left side
        Rect left = r.cutLeft(ul.X);
        left.ymax = ul.Y;

        Rect result = OcludeUR(ur, left);
        Rect right = r.cutRight(ul.X);
        Rect rightResult = OcludeUR(ur, right);
        if (rightResult.Area > result.Area)
            result = rightResult;
        return result;

    }

    private Rect OcludeUR(Sample ur, Rect r)
    {
        if (!r.Valid)
            return r;
        if (!ur.Valid)
            return r;
        if (ur.X >= r.xmax || ur.Y >= r.ymax)
            return r;
        if (ur.Y < r.ymin)
        {
            r.xmax = ur.X;
            return r;
        }
        if (ur.X < r.xmin)
        {
            r.ymax = ur.Y;
            return r;
        }

        // right side
        Rect result = r.cutRight(ur.X);
        result.ymax = ur.Y;
        Rect leftResult = r.cutLeft(ur.X);
        if (leftResult.Area > result.Area)
            result = leftResult;

        return result;
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