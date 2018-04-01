using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using SpaceTime.Frames;

namespace SpaceTime
{
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
	  private const int BytesPerPixel = 4;
	  private const int WindowSize = 30;

	  private KinectSensor _kinectSensor;
    private BodyIndexFrameReader _bodyIndexFrameReader;
	  private readonly ColorFrameReader _colorFrameReader;
		private readonly WriteableBitmap _bodyIndexBitmap;

	  private readonly FrameFactory _frameFactory = new FrameFactory();
	  private readonly IDisposable _closeStreamDisposable;

	  public MainWindow()
    {
	    _kinectSensor = KinectSensor.GetDefault();
      _bodyIndexFrameReader = _kinectSensor.BodyIndexFrameSource.OpenReader();
      var bodyIndexFrameDescription = _kinectSensor.BodyIndexFrameSource.FrameDescription;
      _bodyIndexBitmap = new WriteableBitmap(bodyIndexFrameDescription.Width, bodyIndexFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
	    _colorFrameReader = _kinectSensor.ColorFrameSource.OpenReader();

	    _kinectSensor.Open();
			DataContext = this;
      InitializeComponent();

	    var colourFrames = Observable.FromEventPattern<ColorFrameArrivedEventArgs>(
		    h => _colorFrameReader.FrameArrived += h,
		    h => { })
		    .Select(colourFrame => ReadColourFrame(colourFrame, bodyIndexFrameDescription))
		    .Where(IsValidFrame);

	    var bodyIndexFrames = Observable.FromEventPattern<BodyIndexFrameArrivedEventArgs>(
			    h => _bodyIndexFrameReader.FrameArrived += h, 
			    h => { })
		    .Select(bodyIndexFrame => ReadBodyIndexFrame(bodyIndexFrame, bodyIndexFrameDescription))
		    .Where(IsValidFrame);

	    _closeStreamDisposable = bodyIndexFrames.Zip(colourFrames, (bodyIndexFrame, colourFrame) => new { BodyFrame = bodyIndexFrame, ColourFrame = colourFrame })
					.Select(combinedFrameData => _frameFactory.Build(combinedFrameData.BodyFrame, combinedFrameData.ColourFrame))
					.Window(WindowSize,1)
					.Subscribe(DisplayFrameWindow);

    }

	  private void DisplayFrameWindow(IObservable<Frame> frames)
	  {
		  var frameBuffer = new FrameBuffer();
		  frames.Subscribe(
			  frame => frameBuffer.Add(frame),
			  err => { },
			  () =>
			  {
				  frameBuffer.Render(imagePixels =>
				  {
					  _bodyIndexBitmap.WritePixels(
						  new Int32Rect(0, 0, _bodyIndexBitmap.PixelWidth, _bodyIndexBitmap.PixelHeight),
						  imagePixels,
						  _bodyIndexBitmap.PixelWidth * BytesPerPixel,
						  0);
				  });
			  });
	  }

	  private static bool IsValidFrame(byte[] x)
	  {
		  return x.Length > 0;
	  }

	  private static byte[] ReadBodyIndexFrame(EventPattern<BodyIndexFrameArrivedEventArgs> bodyIndexFrameEvent, FrameDescription bodyIndexFrameDescription)
	  {
		  using (var bodyIndexFrame = bodyIndexFrameEvent.EventArgs.FrameReference.AcquireFrame())
		  {
			  if (bodyIndexFrame == null) return new byte[0];
			  var frameArraySize = bodyIndexFrameDescription.Width * bodyIndexFrameDescription.Height;
			  var bodyIndexData = new byte[frameArraySize];
			  bodyIndexFrame.CopyFrameDataToArray(bodyIndexData);
			  return bodyIndexData;
		  }
	  }

	  private static byte[] ReadColourFrame(EventPattern<ColorFrameArrivedEventArgs> colourFrameEvent, FrameDescription colourFrameDescription)
	  {
			using (var colourFrame = colourFrameEvent.EventArgs.FrameReference.AcquireFrame())
			{
				if (colourFrame == null) return new byte[0];
				var frameArraySize = colourFrameDescription.Width * colourFrameDescription.Height;
				var frameData = new byte[frameArraySize];
				colourFrame.CopyConvertedFrameDataToArray(frameData, ColorImageFormat.Rgba);
				return frameData;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

    public ImageSource ImageSource => _bodyIndexBitmap;

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
      if (_bodyIndexFrameReader != null)
      {
				_closeStreamDisposable.Dispose();
        _bodyIndexFrameReader.Dispose();
        _bodyIndexFrameReader = null;
      }

      if (_kinectSensor != null)
      {
        _kinectSensor.Close();
        _kinectSensor = null;
      }
    }
  }
}