using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Research.Kinect.Nui;

namespace KinectWpfSynth
{
    public interface IKeyboard
    {
        DateTime KeyLastPressed { get; set; }
        List<IKey> Keys { get; }

        void InitKeyboard(List<Color> rainbow);
        void UpdateKeys(Canvas playfield);
        void CheckKeyPress(Dictionary<PlayerUtils.Bone, PlayerUtils.BoneData> segments, IKinectSynth kinectSynth);
    }
}