using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;

namespace KinectWpfSynth
{
    public class RectangleKeyboard : IKeyboard
    {
        public DateTime KeyLastPressed
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        private List<IKey> keys = new List<IKey>();
        public List<IKey> Keys
        {
            get { return keys; }

        }

        public void InitKeyboard(List<Color> rainbow)
        {
            keys.Clear();
            int i = 0;
            foreach (var color in rainbow)
            {
                var key = new Key {Note = i, Brush = new SolidColorBrush(color), IsPressed = false, IsPlaying = false};
                keys.Add(key);
                i++;
            }
        }

        public void UpdateKeys(Canvas playfield)
        {
            double keyWidth = playfield.ActualWidth/keys.Count;
            double keyHeight = playfield.ActualHeight/2;

            int count = 0;
            foreach (var key in keys)
            {
                //TODO Heights, widths, etc, need sorting out
                //TODO Some magic numbers in here need turning into named variables.
                var height = playfield.ActualHeight - ((Key)key).Y;
                CreateRectangle((Key)key, count, keyWidth, keyHeight, height, playfield);
                foreach (var roll in key.Rolls)
                {
                    roll.Y = roll.Y - 2;
                    CreateRectangle(roll, count, keyWidth, roll.Y, 20, playfield);
                }

                count++;
            }

        }

        private void CreateRectangle(IDisplayRectangle shape, int count, double keyWidth, double keyHeight, double height, Canvas playfield)
        {
            shape.X = count * keyWidth;
            shape.Y = keyHeight;
            shape.Width = keyWidth;

            var r = new Rectangle();
            r.Width = shape.Width;
            r.Fill = shape.Brush;
            r.Height = height;
            r.Opacity = 1;

            Canvas.SetLeft(r, shape.X);
            Canvas.SetTop(r, shape.Y);
            playfield.Children.Add(r);
        }

        public void CheckKeyPress(Dictionary<PlayerUtils.Bone, PlayerUtils.BoneData> segments, IKinectSynth kinectSynth)
        {
            foreach (var key in keys)
            {
                //TODO Add CheckPressed to iKey
                foreach (var boneData in segments)
                {
                    
                    if (
                        boneData.Value.seg.y1 > ((Key)key).Y &&
                        boneData.Value.seg.x1 > ((Key)key).X &&
                        boneData.Value.seg.x1 < ((Key)key).X + ((Key)key).Width
                        )
                    {
                        key.IsPressed = true;
                        break;
                    }
                    key.IsPressed = false;
                    
                }

                if (key.IsPressed)
                {
                    kinectSynth.PlayNote(key);

                }
                else
                {
                    kinectSynth.StopNote(key);
                }
            }
        }


    }
}