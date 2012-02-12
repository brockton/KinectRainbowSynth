using System.Collections.Generic;
using System.Windows.Media;

namespace KinectWpfSynth
{
    public interface IKey
    {
        int Note { get; set; }
        bool IsPressed { get; set; }

        bool IsPlaying { get; set; }

        Brush Brush { get; set; }
        Color Color { get; set; }
        List<PianoRoll> Rolls { get; }

        

    }
}