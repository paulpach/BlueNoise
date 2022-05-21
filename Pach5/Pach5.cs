
using System;

/// <summary>
///   Generates samples with minimum manhatan distance
/// </summary>
public readonly struct Pach5 : ISampler
{
    /// Repeatable random number generator
    private readonly Squirrel3 Noise;

    /// the cell size will be 2 ^ bits
    private readonly int Bits;

    // The grid cells are labeled as follows
    // 
    // 2 3 2 3 2 3 2 3 2 ..
    // 0 1 0 1 0 1 0 1 0 ..
    // 2 3 2 3 2 3 2 3 2 ..
    // 0 1 0 1 0 1 0 1 0 ..
    // 2 3 2 3 2 3 2 3 2 ..
    // 0 1 0 1 0 1 0 1 0 ..
    //
    // 0 == even row, even column
    // 1 == even row, odd column
    // 2 == odd row, even column
    // 3 == odd row, odd column
    //
    // cells type 0 always get a sample
    // cells type 1 get a sample as long as it does not conflict with the adjacent 0 cells
    // cells type 2 get a sample as long as it does not conflict with the adjacent 0 and 1 cells
    // cells type 3 get a sample as long as it does not conflict with the adjacent 0, 1 and 2 cells


    public Pach5(int bits, uint seed)
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

        uint rnd = Noise[row, col];
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
        return ((a1 - a0) * w + a0 * cellSize + (cellSize >> 1)) >> Bits;
    }
    
    /// <summary>
    ///   Generates a sample at the cell containing (x,y)
    /// </summary>
    public Sample this[int x, int y] 
    {
        get
        {
            int cellx = x >> Bits;
            int celly = y >> Bits;

            // this is the bottom left origin of the cell where (x,y) is
            int x0 = cellx << Bits;
            int y0 = celly << Bits;

            Sample sample;

            // there are 4 cases:
            if ((celly & 1) == 0 && (cellx & 1) == 0)
            {
                // even row, even column
                // these cells are fully randomized, just get a sample and return it
                sample = GenerateCandidate(cellx, celly);
            }
            else if ((celly & 1) == 0 && (cellx & 1) == 1)
            {
                // even row, odd column
                sample = GetCell1(cellx, celly);
                // sample.Value = 0;
            }
            else if ((celly & 1) == 1 && (cellx & 1) == 0)
            {
                // odd row, even column
                sample = GetCell2(cellx, celly);
                // sample.Value = 0;
            }
            else
            {
                // odd row, odd column
                sample = GetCell3(cellx, celly);
                // sample.Value = 0;
            }
            sample.X += x0;
            sample.Y += y0;
            return sample;

            // row is even, col is odd
            // row is odd, col is even
            // row is even, col is even
        }
    }

    // cell type 1 get a sample as long
    // as it does not conflict with the adjacent 0 cells
    private Sample GetCell1(int cellx, int celly)
    {
        Sample s0 = GenerateCandidate(cellx - 1, celly);
        Sample s1 = GenerateCandidate(cellx + 1, celly);
        return GetCell1(cellx, celly, s0, s1);
    }

    private Sample GetCell1(int cellx, int celly, Sample sleft, Sample sright)
    {
        if (sleft.X > sright.X)
            return default;

        Sample result = GenerateCandidate(cellx, celly);
        result.X = Lerp(sleft.X, sright.X, result.X);
        return result;
    }

    // cell type 2 get a sample as long
    // as it does not conflict with the adjacent 0 and 1 cells
    private Sample GetCell2(int cellx, int celly)
    {
        Sample lm = GenerateCandidate(cellx, celly - 1);
        Sample um = GenerateCandidate(cellx, celly + 1);
        if (lm.Y > um.Y)
            return default;

        Sample ll2 = GenerateCandidate(cellx - 2, celly - 1);
        Sample ll = GetCell1(cellx - 1, celly - 1, ll2, lm);

        Sample lr2 = GenerateCandidate(cellx + 2, celly - 1);
        Sample lr = GetCell1(cellx + 1, celly - 1, lm, lr2);

        Sample ul2 = GenerateCandidate(cellx - 2, celly + 1);
        Sample ul = GetCell1(cellx - 1, celly + 1, ul2, um);

        Sample ur2 = GenerateCandidate(cellx + 2, celly + 1);
        Sample ur = GetCell1(cellx + 1, celly + 1, um, ur2);

        return GetCell2(cellx, celly, ll, lm, lr, ul, um, ur);
    }

    private Sample GetCell2(int cellx, int celly, Sample ll, Sample lm, Sample lr, Sample ul, Sample um, Sample ur)
    {
        int cellSize = 1 << Bits;

        Rect baseRect = new Rect
        {
            xmin = 0,
            ymin = lm.Y,
            xmax = cellSize - 1,
            ymax = um.Y
        };

        if (!baseRect.Valid)
            return new Sample();

        Rect rect = OverlapCalculator.CutOut(ll, lr, ul, ur, baseRect);

        if (rect.Valid)
        {
            Sample rnd = GenerateCandidate(cellx, celly);

            return new Sample {
                X = Lerp(rect.xmin, rect.xmax, rnd.X),
                Y = Lerp(rect.ymin, rect.ymax, rnd.Y),
                Value = 1
            };
        }

        return new Sample();
    }

    // cell type 3 get a sample as long
    // as it does not conflict with the adjacent 0, 1 and 2 cells
    private Sample GetCell3(int cellx, int celly)
    {
        int cellSize = 1 << Bits;
        int halfCell = cellSize >> 1;
        // calculate the 4 corner cells which are type 0
        Sample ll = GenerateCandidate(cellx - 1, celly - 1);
        Sample lr = GenerateCandidate(cellx + 1, celly - 1);
        Sample ul = GenerateCandidate(cellx - 1, celly + 1);
        Sample ur = GenerateCandidate(cellx + 1, celly + 1);

        // now the type 1 on top and bottom
        Sample lm = GetCell1(cellx, celly - 1, ll, lr);
        Sample um = GetCell1(cellx, celly + 1, ul, ur);

        // short cirtcuit if the type 1 prevent placing
        if (lm.Y > um.Y && lm.Valid && um.Valid)
            return default;

        Sample ml = default;

        if (ll.Y <= ul.Y || !ll.Valid || !ul.Valid)
        {
            // calculate all 7 samples below
            Sample ll3 = GenerateCandidate(cellx - 3, celly - 1);
            Sample ll2 = GetCell1(cellx - 2, celly - 1, ll3, ll);
            Sample ul3 = GenerateCandidate(cellx - 3, celly + 1);        
            Sample ul2 = GetCell1(cellx - 2, celly + 1, ul3, ul);
            ml = GetCell2(cellx - 1, celly, ll2, ll, lm, ul2, ul, um);
        }

        Sample mr = default;

        if (lr.Y <= ur.Y || !lr.Valid || !ur.Valid) {
            Sample lr3 = GenerateCandidate(cellx + 3, celly - 1);
            Sample lr2 = GetCell1(cellx + 2, celly - 1, lr, lr3);
            // and all 7 samples above
            Sample ur3 = GenerateCandidate(cellx + 3, celly + 1);
            Sample ur2 = GetCell1(cellx + 2, celly + 1, ur, ur3);        

            // calculate samples at left and right
            mr = GetCell2(cellx + 1, celly, lm, lr, lr2, um, ur, ur2);
        }

        // figure out boundaries from left right, top and bottom samples
        // this ensures we don't conflict with type 1 and 2 cells
        Rect baseRect = new Rect
        {
            xmin = ml.Valid ? ml.X : 0,
            ymin = lm.Valid ? lm.Y : 0,
            xmax = mr.Valid ? mr.X : cellSize - 1,
            ymax = um.Valid ? um.Y : cellSize - 1,
        };

        // cut out samples from the corners
        // this ensures we don't conflict with type 0 cells
        Rect rect = OverlapCalculator.CutOut(ll, lr, ul, ur, baseRect);

        if (rect.Valid)
        {
            // there is space for a new sample, so 
            // generate one randomly
            Sample rnd = GenerateCandidate(cellx, celly);

            return new Sample {
                X = Lerp(rect.xmin, rect.xmax, rnd.X),
                Y = Lerp(rect.ymin, rect.ymax, rnd.Y),
                Value = 1
            };
        }

        return default;
    }
}