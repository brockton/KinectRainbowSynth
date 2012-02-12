using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;


namespace KinectWpfSynth
{
    public class CircleKeyboard : IKeyboard
    {
 
        private GeometryUtils geometryUtils = new GeometryUtils();
        private double StartAngle = 0;

        private double centreX { get; set; }
        private double centreY { get; set; }
        private double diameter { get; set; }
        private double keyHeight { get; set; }
        
        public DateTime KeyLastPressed
        {
            get; set;
        }

        private List<IKey> keys = new List<IKey>();
        public List<IKey> Keys
        {
            get { return keys; }
        }

        public void InitKeyboard( List<Color> rainbow)
        {
            keys.Clear();
            
            int i = 0;
            foreach (var color in rainbow)
            {
                keys.Add(new WedgeKey {Brush = new SolidColorBrush(color), Note = i});
                i++;
            }

        }

        
        
        private void DrawKey(WedgeKey key, double diameter, double keyHeight, double angle, Canvas playfield)
        {
            
            var geometry = new StreamGeometry();

            geometryUtils.DrawArrowGeometry(geometry, playfield.ActualWidth / 2, playfield.ActualHeight / 2, diameter, diameter, keyHeight, StartAngle,
                                            StartAngle + angle);

            var arc = new Path { Stroke = Brushes.Transparent, StrokeThickness = 0, Fill = key.Brush, Data = geometry };

            

            key.InnerWidth = 30;

            key.StartAngle = StartAngle;
            key.StopAngle = StartAngle + angle;
            key.Shape = arc;

            playfield.Children.Add(arc);

            StartAngle = StartAngle + angle;
            
            if (StartAngle > 360) StartAngle = 0;
        }

        public void UpdateKeys(Canvas playfield)
        {

            diameter = playfield.ActualWidth < playfield.ActualHeight ? playfield.ActualWidth : playfield.ActualHeight;
            //make circle smaller test
            diameter = diameter - 100;

            centreX = playfield.ActualWidth/2;
            centreY = playfield.ActualHeight/2;

            keyHeight = diameter / 5; 
            double angle = 360 / keys.Count;
            StartAngle = 0;
            foreach (var key in keys)
            {

                DrawKey((WedgeKey)key, diameter, keyHeight, angle, playfield);

            }
        }

        public void CheckKeyPress(Dictionary<PlayerUtils.Bone, PlayerUtils.BoneData> segments, IKinectSynth kinectSynth)
        {
            var innerR = Math.Pow((diameter / 2) - keyHeight, 2);
            var outerR = Math.Pow(diameter / 2, 2);

            foreach (var key in Keys)
            {
                foreach (var segment in segments)
                {
                    var relativeX = centreX - segment.Value.seg.x1;
                    var relativeY = centreY - segment.Value.seg.y1;
                    var checkR = Math.Pow(relativeX, 2) +
                                 Math.Pow(relativeY, 2);
                    
                    var theta = Math.Atan2(relativeY, relativeX);
                    if (theta < 0) theta += 2*Math.PI;
                    var angle = theta*(180/Math.PI);
                    
                    if (checkR >= innerR 
                        && checkR <= outerR
                        && angle > ((WedgeKey) key).StartAngle 
                        && angle < ((WedgeKey) key).StopAngle)
                    {
                        key.IsPressed = true;
                        break;
                    }
                    key.IsPressed = false;
                }

                if(key.IsPressed)
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