

/// <summary>
/// Calculates a rectangle that does not overlap with any other rectangles.
/// </summary>
public static partial class OverlapCalculator
{
    /// <summary>
    /// Given a rectangle and 4 samples at the diagonals
    /// finds the biggest rectangle that does not overlap the diagonal samples
    /// </summary>
    /// <param name="ll"></param>
    /// <param name="lr"></param>
    /// <param name="ul"></param>
    /// <param name="ur"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public static Rect CutOut(Sample ll, Sample lr, Sample ul, Sample ur, Rect r)
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

    private static Rect OcludeLR(Sample lr, Sample ul, Sample ur, Rect r)
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

    private static Rect OcludeUL(Sample ul, Sample ur, Rect r)
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

    private static Rect OcludeUR(Sample ur, Rect r)
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


}