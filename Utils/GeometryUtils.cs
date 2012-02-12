using System;
using System.Windows;
using System.Windows.Media;

namespace KinectWpfSynth
{
    public class GeometryUtils
    {
        public double GetRadian(double theta)
        {
            return theta * Math.PI / 180;

        }

        //public double GetTheta(double x, double y)
        //{
        //    return 
        //}

        public StreamGeometry DrawArrowGeometry(StreamGeometry geometry, double centreX, double centreY, double circleWidth, double circleHeight, double innerWidth,
            double startAngle, double stopAngle)
        {

            //This code is based on http://marktinderholt.wordpress.com/2009/01/30/donut-shape-in-wpf/

            var context = geometry.Open();

            // Setup the Center Point & Radius
            Point c = new Point(centreX, centreY);
            double rOutterX = circleWidth/ 2;
            double rOutterY = circleHeight / 2;


            double rInnerX = rOutterX - innerWidth;
            double rInnerY = rOutterY - innerWidth;

            double theta = 0;
            bool hasBegun = false;
            double x;
            double y;
            Point currentPoint;

            // Draw the Outside Edge
            for (theta = startAngle; theta <= stopAngle; theta++)
            {
                x = c.X + rOutterX * Math.Cos(GetRadian(theta));
                y = c.Y + rOutterY * Math.Sin(GetRadian(theta));
                currentPoint = new Point(x, y);
                if (!hasBegun)
                {
                    context.BeginFigure(currentPoint, true, true);
                    hasBegun = true;
                }
                context.LineTo(currentPoint, true, true);
            }

            // Connect the Outside Edge to the Inner Edge
            x = c.X + rInnerX * Math.Cos(GetRadian(stopAngle));
            y = c.Y + rInnerY * Math.Sin(GetRadian(stopAngle));
            currentPoint = new Point(x, y);
            context.LineTo(currentPoint, true, true);

            // Draw the Inner Edge
            for (theta = stopAngle; theta >= startAngle; theta--)
            {
                x = c.X + rInnerX * Math.Cos(GetRadian(theta));
                y = c.Y + rInnerY * Math.Sin(GetRadian(theta));
                currentPoint = new Point(x, y);
                context.LineTo(currentPoint, true, true);
            }

            // Connect the Inner Edge to the Outside Edge
            x = c.X + rOutterX * Math.Cos(GetRadian(startAngle));
            y = c.Y + rOutterY * Math.Sin(GetRadian(startAngle));
            currentPoint = new Point(x, y);
            context.LineTo(currentPoint, true, true);

            context.Close();
            return geometry;
        }

    }
}