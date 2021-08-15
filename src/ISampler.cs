
/// <summary>
///   Interfaces for sampling algorithms.
/// </summary>
public interface ISampler
{
    /// <summary>
    ///   Generates a sample at the cell containing (x,y)
    /// </summary>
    Sample this[int x, int y] { get; }
}
