using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceTime.Frames
{
  public class FrameBuffer
  {
    private const int NoOfFramesToStore = 100;

    private readonly Queue<Frame> _frames = new Queue<Frame>(NoOfFramesToStore);

    public void Add(Frame frame)
    {
      _frames.Enqueue(frame);
      if (_frames.Count() > NoOfFramesToStore)
      {
        _frames.Dequeue();
      }
    }

    public void Render(Action<uint[]> displayImage)
    {
      var pixels = Worm.FromFrames(_frames).Pixels;
      displayImage(pixels);
    }
    
  }

  internal class Worm
  {
    private readonly IEnumerable<Frame> _frames;

    internal static Worm FromFrames(Queue<Frame> frames)
    {
      return new Worm(frames);
    }

    private Worm(IEnumerable<Frame> frames)
    {
      _frames = frames;
    }

    public uint[] Pixels { get
      {
        var outputFrame = _frames.First().BodyIndexPixels;
        foreach(var frame in _frames.Skip(1))
        {
          CombineFrames(outputFrame, frame);
        }
        return outputFrame;
      }
    }

    private void CombineFrames(uint[] outputFrame, Frame frame)
    {
      for (int i = 0; i < outputFrame.Length; i++)
      {
        var bodyIndexPixel = frame.BodyIndexPixels[i];
        if (IsBodyPixel(bodyIndexPixel))
        {
          outputFrame[i] = bodyIndexPixel;
        }
      }
    }

    private bool IsBodyPixel(uint frameBodyIndexPixel)
    {
      return true;
    }
  }
}
