using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectWpfSynth
{
    public class WedgeKey : IKey
    {
        public double InnerWidth { get; set; }
        public double StartAngle { get; set; }
        public double StopAngle { get; set; }
        public Path Shape { get; set; }

        public int Note { get; set; }

        public bool IsPressed
        {
            get;
            set;
        }
        public bool IsPlaying { get; set; }

        public Brush Brush { get; set; }

        public Color Color { get; set; }

        public List<PianoRoll> Rolls
        {
            get { throw new NotImplementedException(); }
        }

        //public bool IsKeyAlreadyPressed { get; set; }
    }
}