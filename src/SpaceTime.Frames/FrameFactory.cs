using System;

namespace SpaceTime.Frames
{
  public class FrameFactory
  {
    private const int BackgroundColour = 0x00000000;

    private static readonly uint[] BodyColor =
    {
      0x0000FF00,
      0x00FF0000,
      0xFFFF4000,
      0x40FFFF00,
      0xFF40FF00,
      0xFF808000
    };

    public Frame Build(byte[] bodyIndexFrameData)
    {
      var bodyIndexPixels = CopyPixels(bodyIndexFrameData);
      return new Frame(bodyIndexPixels);
    }

    private unsafe uint[] CopyPixels(byte[] frameData)
    {
	    var bodyIndexFrameDataSize = frameData.Length;
	    var bodyIndexPixels = new uint[bodyIndexFrameDataSize];
	    for (var i = 0; i < bodyIndexFrameDataSize; ++i)
	    {
		    SetColourForSourcePixel(frameData[i], bodyIndexPixels, i);

	    }

	    return bodyIndexPixels;
    }

	  private void SetColourForSourcePixel(byte currentFrameData, uint[] bodyIndexPixels, int offsetInFrame)
	  {
		  if (currentFrameData < BodyColor.Length)
		  {
			  bodyIndexPixels[offsetInFrame] = BodyColor[currentFrameData];
		  }
		  else
		  {
			  bodyIndexPixels[offsetInFrame] = BackgroundColour;
		  }
	  }
  }
}