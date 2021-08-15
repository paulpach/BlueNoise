
/// A sample is a (x,y) coordinate plus a random value
public struct Sample
{
    public int X;
    public int Y;

    // value is just a random number associated with this sample
    // but there is a special case,  if Value == 0, then 
    // this is not a valid sample, and should be ignored
    public uint Value;

    public bool Valid => Value != 0;
}
