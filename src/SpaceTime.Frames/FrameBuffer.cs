using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceTime.Frames
{
	public class FrameBuffer
	{
		private const int NoOfFramesToStore = 50;

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
//			displayImage(_frames.Last().BodyIndexPixels);
		}

	}
}
