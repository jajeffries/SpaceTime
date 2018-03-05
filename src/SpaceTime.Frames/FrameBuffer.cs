using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceTime.Frames
{
	public class FrameBuffer
	{
		private readonly IList<Frame> _frames = new List<Frame>();

		public void Add(Frame frame)
		{
			_frames.Add(frame);
		}

		public void Render(Action<uint[]> displayImage)
		{
      var pixels = Worm.FromFrames(_frames).Pixels;
      displayImage(pixels);
//			displayImage(_frames.Last().BodyIndexPixels);
		}

	}
}
