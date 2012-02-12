using System.Windows.Media;

namespace KinectWpfSynth
{
    public interface IDisplayRectangle
    {
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        Brush Brush { get; set; }
    }
}