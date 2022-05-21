

using System;

/// <summary>
/// A rectangular space of valid samples
/// </summary>
public struct Rect
{
    /// <summary>
    /// Minimum x
    /// </summary>
    public int xmin;

    /// <summary>
    /// Maximum x
    /// </summary>
    public int xmax;

    /// <summary>
    /// Minimum y
    /// </summary>
    public int ymin;

    /// <summary>
    /// Maximum y
    /// </summary>
    public int ymax;

    /// <summary>
    /// is this a non empty rectangle?
    /// </summary>
    public bool Valid => xmax >= xmin && ymax >= ymin;

    /// <summary>
    /// How many points are covered by this rectangle?
    /// -1 if it is not valid
    /// </summary>
    public int Area => Valid ? (xmax - xmin + 1) * (ymax - ymin + 1) : -1;

    internal Rect cutLeft(int x)
    {
        return new Rect
        {
            xmin = xmin,
            xmax = Math.Min(x, xmax),
            ymin = ymin,
            ymax = ymax
        };
    }
    internal Rect cutRight(int x)
    {
        return new Rect
        {
            xmin = x,
            xmax = xmax,
            ymin = ymin,
            ymax = ymax
        };
    }
}

