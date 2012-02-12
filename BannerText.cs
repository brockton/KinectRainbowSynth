
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KinectWpfSynth
{
    public class BannerText
    {

            private static BannerText bannerText = null;
            private Brush brush;
            private Color color;
            private Label label;
            private Rect boundsRect;
            private Rect renderedRect;
            private bool doScroll;
            private double offset = 0;
            private string text;

            public BannerText(string s, Rect rect, bool scroll, Color col)
            {
                text = s;
                boundsRect = rect;
                doScroll = scroll;
                brush = null;
                label = null;
                color = col;
                offset = (doScroll) ? 1.0 : 0.0;
            }

            public static void NewBanner(string s, Rect rect, bool scroll, Color col)
            {
                bannerText = new BannerText(s, rect, scroll, col);
            }

            public static Label MakeSimpleLabel(string text, Rect bounds, Brush brush)
            {
                Label label = new Label();
                label.Content = text;
                if (bounds.Width != 0)
                {
                    label.SetValue(Canvas.LeftProperty, bounds.Left);
                    label.SetValue(Canvas.TopProperty, bounds.Top);
                    label.Width = bounds.Width;
                    label.Height = bounds.Height;
                }
                label.Foreground = brush;
                label.FontFamily = new FontFamily("Arial");
                label.FontWeight = FontWeight.FromOpenTypeWeight(600);
                label.FontStyle = FontStyles.Normal;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                return label;
            }

            private Label GetLabel()
            {
                if (brush == null)
                    brush = new SolidColorBrush(color);

                if (label == null)
                {
                    label = MakeSimpleLabel(text, boundsRect, brush);
                    if (doScroll)
                    {
                        label.FontSize = Math.Max(20, boundsRect.Height / 30);
                        label.Width = 10000;
                    }
                    else
                        label.FontSize = Math.Min(Math.Max(10, boundsRect.Width * 2 / text.Length),
                                                  Math.Max(10, boundsRect.Height / 20));
                    label.VerticalContentAlignment = VerticalAlignment.Bottom;
                    label.HorizontalContentAlignment = (doScroll) ? HorizontalAlignment.Left : HorizontalAlignment.Center;
                    label.SetValue(Canvas.LeftProperty, offset * boundsRect.Width);
                }

                renderedRect = new Rect(label.RenderSize);

                if (doScroll)
                {
                    offset -= 0.0015;
                    if (offset * boundsRect.Width < boundsRect.Left - 10000)
                        return null;
                    label.SetValue(Canvas.LeftProperty, offset * boundsRect.Width + boundsRect.Left);
                }
                return label;
            }

            public static void UpdateBounds(Rect rect)
            {
                if (bannerText == null)
                    return;
                bannerText.boundsRect = rect;
                bannerText.label = null;
            }

            public static void Draw(UIElementCollection children)
            {
                if (bannerText == null)
                    return;

                Label text = bannerText.GetLabel();
                if (text == null)
                {
                    bannerText = null;
                    return;
                }
                children.Add(text);
            }
        

    }
}