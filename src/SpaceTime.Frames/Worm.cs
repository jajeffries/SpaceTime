using System.Collections.Generic;
using System.Linq;

namespace SpaceTime.Frames
{
	public class Worm
	{
		private readonly IEnumerable<Frame> _frames;

		public static Worm FromFrames(IEnumerable<Frame> frames)
		{
			return new Worm(frames);
		}

		private Worm(IEnumerable<Frame> frames)
		{
			_frames = frames;
		}

		public uint[] Pixels
		{
			get
			{
				var outputFrame = _frames.First().BodyIndexPixels;
				var count = 0;
				foreach (var frame in _frames.Skip(1).Where((f, i) => i % 2 == 0))
				{
					CombineFrames(outputFrame, frame, count);
					count++;
				}

				return outputFrame;
			}
		}

		private void CombineFrames(uint[] outputFrame, Frame frame, int frameCount)
		{
			var frameBodyIndexPixels = frame.BodyIndexPixels;
			for (var i = 0; i < outputFrame.Length; i++)
			{
				var bodyIndexPixel = frameBodyIndexPixels[i];
				if (bodyIndexPixel != 0x00000000)
				{
					outputFrame[i] = bodyIndexPixel;
				}
			}
		}
	}
}