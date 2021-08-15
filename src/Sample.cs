
/// A sample is a (x,y) coordinate plus a random value
public struct Sample
{
    /// <summary>
    /// The x coordinate of the sample
    /// </summary>
    public int X;

    /// <summary>
    /// The y coordinate of the sample
    /// </summary>
    public int Y;

    /// <summary>
    /// The random value of the sample
    /// 0 if the sample is not valid
    /// </summary>
    public uint Value;

    /// <summary>
    /// True if the sample is valid
    /// </summary>
    public bool Valid => Value != 0;
}
