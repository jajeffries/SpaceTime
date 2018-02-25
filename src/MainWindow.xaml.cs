using System.ComponentModel;
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

    private KinectSensor _kinectSensor;
    private BodyIndexFrameReader _bodyIndexFrameReader;
    private readonly FrameDescription _bodyIndexFrameDescription;
    private readonly WriteableBitmap _bodyIndexBitmap;
    private string _statusText;

    private readonly FrameBuffer _frameBuffer = new FrameBuffer();
    private readonly FrameFactory _frameFactory = new FrameFactory();

    public MainWindow()
    {
      _kinectSensor = KinectSensor.GetDefault();
      _bodyIndexFrameReader = _kinectSensor.BodyIndexFrameSource.OpenReader();
      _bodyIndexFrameReader.FrameArrived += Reader_FrameArrived;
      _bodyIndexFrameDescription = _kinectSensor.BodyIndexFrameSource.FrameDescription;
      _bodyIndexBitmap = new WriteableBitmap(_bodyIndexFrameDescription.Width, _bodyIndexFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
      _kinectSensor.IsAvailableChanged += Sensor_IsAvailableChanged;
      _kinectSensor.Open();

      StatusText = _kinectSensor.IsAvailable
        ? Properties.Resources.RunningStatusText
        : Properties.Resources.NoSensorStatusText;

      DataContext = this;
      InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ImageSource ImageSource => _bodyIndexBitmap;

    public string StatusText
    {
      get { return _statusText; }

      set
      {
        if (_statusText != value)
        {
          _statusText = value;

          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatusText"));
        }
      }
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
      if (_bodyIndexFrameReader != null)
      {
        _bodyIndexFrameReader.FrameArrived -= Reader_FrameArrived;
        _bodyIndexFrameReader.Dispose();
        _bodyIndexFrameReader = null;
      }

      if (_kinectSensor != null)
      {
        _kinectSensor.Close();
        _kinectSensor = null;
      }
    }

    private void Reader_FrameArrived(object sender, BodyIndexFrameArrivedEventArgs e)
    {
      using (var bodyIndexFrame = e.FrameReference.AcquireFrame())
      {
        if (bodyIndexFrame != null)
        {
          using (var bodyIndexBuffer = bodyIndexFrame.LockImageBuffer())
          {
            if (FrameIsCorrectSize(bodyIndexBuffer))
            {
              var frame = _frameFactory.Build(bodyIndexBuffer.UnderlyingBuffer, (int)bodyIndexBuffer.Size);
              _frameBuffer.Add(frame);
              _frameBuffer.Render(imagePixels =>
              {
                _bodyIndexBitmap.WritePixels(
                  new Int32Rect(0, 0, _bodyIndexBitmap.PixelWidth, _bodyIndexBitmap.PixelHeight),
                  imagePixels,
                  _bodyIndexBitmap.PixelWidth * BytesPerPixel,
                  0);
              });
            }
          }
        }
      }
    }

    private bool FrameIsCorrectSize(KinectBuffer bodyIndexBuffer)
    {
      return _bodyIndexFrameDescription.Width * _bodyIndexFrameDescription.Height == bodyIndexBuffer.Size;
    }

    private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
    {
      StatusText = _kinectSensor.IsAvailable
        ? Properties.Resources.RunningStatusText
        : Properties.Resources.SensorNotAvailableStatusText;
    }
  }
}