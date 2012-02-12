using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace KinectWpfSynth
{
    public class Key : IDisplayRectangle, IKey
    {
        public int Note { get; set; }
        public bool IsPressed { get; set; }
        public bool IsPlaying { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public Brush Brush { get; set; }
        public Color Color { get; set; }
        private List<PianoRoll> rolls = new List<PianoRoll>();
        public List<PianoRoll> Rolls { get { return rolls; } }

        //public bool IsKeyAlreadyPressed { get; set; }

        //public bool CheckKeyPressed(Dictionary<PlayerUtils.Bone, PlayerUtils.BoneData> segments)
        //{
        //    bool checkIsPressed = false;
        //    foreach (var segment in segments)
        //    {
        //        if ((segment.Value.seg.x1 > X && segment.Value.seg.x1 < X + Width) && (segment.Value.seg.y1 > Y))
        //        {
        //            checkIsPressed = true;                    
        //        }

        //    }
            
        //    if(checkIsPressed)
        //    {
        //        if (IsPressed) return false;
        //        IsPressed = true;
        //        return IsPressed;
        //    }
        //    IsPressed = false;
        //    return false;
        //}


        
    }
    
}