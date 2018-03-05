using NUnit.Framework;
using SpaceTime.Frames;

namespace SpaceTime.Tests
{
	[TestFixture]
	public class WormTests
	{
		[Test]
		public void TestBackgroundPixelsArentOverwrittenByBackgroundPixels()
		{
			var worm = Worm.FromFrames(new[]
			{
				new Frame(new uint[] {0x00000000}),
				new Frame(new uint[] {0x00000000})
			});
			Assert.That(worm.Pixels, Is.EqualTo(new uint[] { 0x00000000 }));
		}


		[Test]
		public void TestBackgroundPixelsAreOverwrittenByBodyPixels()
		{
			var worm = Worm.FromFrames(new[]
			{
				new Frame(new uint[] {0x00000001}),
				new Frame(new uint[] {0x00000000})
			});
			Assert.That(worm.Pixels, Is.EqualTo(new uint[] { 0x00000001 }));
		}


		[Test]
		public void TestNonBackgroundPixelsAreOverwrittenByBodyPixels()
		{
			var worm = Worm.FromFrames(new[]
			{
				new Frame(new uint[] {0x00000001}),
				new Frame(new uint[] {0x00000010})
			});
			Assert.That(worm.Pixels, Is.EqualTo(new uint[] { 0x00000010 }));
		}
	}
}