namespace SpaceTime.Frames
{
  public class Frame
  {
    public uint[] BodyIndexPixels { get; }

    public Frame(uint[] bodyIndexPixels)
    {
      BodyIndexPixels = bodyIndexPixels;
    }
  }
}