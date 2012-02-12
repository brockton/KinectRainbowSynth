using System.Windows.Media;

namespace KinectWpfSynth
{
    public class PianoRoll : IDisplayRectangle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public Brush Brush { get; set; }
        public Color Color { get; set; }
    }
}