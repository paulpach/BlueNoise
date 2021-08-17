/// <summary>
/// Simple linear congruential generator mangling
/// </summary>
public readonly struct LinearCongruentialGenerator
{
    private readonly int a;
    private readonly int c;
    private readonly int mmask;

    /// <summary>
    /// Creates a LCG using the parameters
    /// </summary>
    /// <param name="a"></param>
    /// <param name="c"></param>
    /// <param name="mlog2">logarithm base 2 of the bounds</param>
    public LinearCongruentialGenerator(int a, int c, int mlog2)
    {
        mmask = (1 << mlog2) - 1;
        this.a = a;
        this.c = c;

    }

    /// <summary>
    /// mangle a value 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public int shuffle(int x)
    {
        return (x * a + c) & mmask;
    }
}