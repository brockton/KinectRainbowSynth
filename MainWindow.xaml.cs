using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Research.Kinect.Nui;

// Some of this code is taken from the MS Research SDK Example project
// ShapeGame. This is included in the Kinect SDK download.
// It also uses NAudio - an open source C# Audio Player which can be found at http://naudio.codeplex.com
// The piano samples are taken from http://audio.ibeat.org/?ccm=/media/files/DaDeMo/469


// Since the timer resolution defaults to about 10ms precisely, we need to
// increase the resolution to get framerates above between 50fps with any
// consistency.
using System.Runtime.InteropServices;
using Vector = Microsoft.Research.Kinect.Nui.Vector;

public class Win32
{
    [DllImport("Winmm.dll")]
    public static extern int timeBeginPeriod(UInt32 uPeriod);
}

namespace KinectWpfSynth
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables
        const int TimerResolution = 2;  // ms
        const double MinFramerate = 15;

        SpeechRecogniser recognizer = null;

        private IKinectSynth kinectSynth = new PianoSynth();
        //private IKinectSynth kinectSynth = new SanfordSynth();

        private IKeyboard keyboard = new CircleKeyboard();
        //private IKeyboard keyboard = new RectangleKeyboard();
        private List<Color> rainbow = new List<Color> { Colors.DarkRed, Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.BlueViolet, Colors.Indigo };

        private DateTime keyLastPressed = DateTime.Now;

        public Dictionary<int, Player> players = new Dictionary<int, Player>();

        // Player(s) placement in scene (z collapsed):
        Rect playerBounds;
        Rect screenRect;

        double targetFramerate = 50;
        int frameCount = 0;
        bool runningGameThread = false;
        bool nuiInitialized = false;
        int playersAlive = 0;

        Runtime nui = new Runtime();

        DateTime lastFrameDrawn = DateTime.MinValue;
        DateTime predNextFrame = DateTime.MinValue;
        double actualFrameTime = 0;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            playfield.ClipToBounds = true;

            UpdatePlayfieldSize();


            if ((nui != null) && InitializeNui())
            {
                nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_ColorFrameReady);
                nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
                InitialiseSpeechRecognition();
            }

            StartThread();
        }

        private void StartThread()
        {
            Win32.timeBeginPeriod(TimerResolution);
            var gameThread = new Thread(GameThread);
            gameThread.SetApartmentState(ApartmentState.STA);
            gameThread.Start();
        }

        private void InitialiseSpeechRecognition()
        {
            try
            {
                recognizer = new SpeechRecogniser();
            }
            catch
            {
                recognizer = null;
            }
            if ((recognizer == null) || !recognizer.IsValid())
            {
                BannerText.NewBanner("No speech recognition", screenRect, false, Color.FromArgb(90, 255, 255, 255));
                recognizer = null;
            }
            else
            {
                recognizer.SaidSomething += recognizer_SaidSomething;
                banner.Content = "Piano";
            }
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //TODO Pick this apart a bit
            SkeletonFrame skeletonFrame = e.SkeletonFrame;

            int iSkeletonSlot = 0;

            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {
                    Player player;
                    if (players.ContainsKey(iSkeletonSlot))
                    {
                        player = players[iSkeletonSlot];
                    }
                    else
                    {
                        player = new Player(iSkeletonSlot);
                        player.setBounds(playerBounds);
                        players.Add(iSkeletonSlot, player);
                    }

                    player.lastUpdated = DateTime.Now;
                    
                    // Update player's bone and joint positions
                    if (data.Joints.Count > 0)
                    {
                        player.isAlive = true;

                        // Head, hands, feet (hit testing happens in order here)
                        // We are only interested in Hands, but left everything else in for now in case
                        //player.UpdateJointPosition(data.Joints, JointID.Head);
                        player.UpdateJointPosition(data.Joints, JointID.HandLeft);
                        player.UpdateJointPosition(data.Joints, JointID.HandRight);
                        //player.UpdateJointPosition(data.Joints, JointID.FootLeft);
                        //player.UpdateJointPosition(data.Joints, JointID.FootRight);

                        // Hands and arms
                        //player.UpdateBonePosition(data.Joints, JointID.HandRight, JointID.WristRight);
                        //player.UpdateBonePosition(data.Joints, JointID.WristRight, JointID.ElbowRight);
                        //player.UpdateBonePosition(data.Joints, JointID.ElbowRight, JointID.ShoulderRight);

                        //player.UpdateBonePosition(data.Joints, JointID.HandLeft, JointID.WristLeft);
                        //player.UpdateBonePosition(data.Joints, JointID.WristLeft, JointID.ElbowLeft);
                        //player.UpdateBonePosition(data.Joints, JointID.ElbowLeft, JointID.ShoulderLeft);

                        //// Head and Shoulders
                        //player.UpdateBonePosition(data.Joints, JointID.ShoulderCenter, JointID.Head);
                        //player.UpdateBonePosition(data.Joints, JointID.ShoulderLeft, JointID.ShoulderCenter);
                        //player.UpdateBonePosition(data.Joints, JointID.ShoulderCenter, JointID.ShoulderRight);

                        //// Legs
                        //player.UpdateBonePosition(data.Joints, JointID.HipLeft, JointID.KneeLeft);
                        //player.UpdateBonePosition(data.Joints, JointID.KneeLeft, JointID.AnkleLeft);
                        //player.UpdateBonePosition(data.Joints, JointID.AnkleLeft, JointID.FootLeft);

                        //player.UpdateBonePosition(data.Joints, JointID.HipRight, JointID.KneeRight);
                        //player.UpdateBonePosition(data.Joints, JointID.KneeRight, JointID.AnkleRight);
                        //player.UpdateBonePosition(data.Joints, JointID.AnkleRight, JointID.FootRight);

                        //player.UpdateBonePosition(data.Joints, JointID.HipLeft, JointID.HipCenter);
                        //player.UpdateBonePosition(data.Joints, JointID.HipCenter, JointID.HipRight);

                        //// Spine
                        //player.UpdateBonePosition(data.Joints, JointID.HipCenter, JointID.ShoulderCenter);
                        
                        PlayMusic();
                    }
                }
                iSkeletonSlot++;
            }
        }

        void CheckPlayers()
        {
            foreach (var player in players.Where(player => !player.Value.isAlive))
            {
                // Player left scene since we aren't tracking it anymore, so remove from dictionary
                players.Remove(player.Value.getId());
                break;
            }

            // Count alive players
            int alive = 0;
            foreach (var player in players)
            {
                if (player.Value.isAlive)
                    alive++;
            }
            playersAlive = alive;

        }

        void nui_ColorFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // 32-bit per pixel, RGBA image
            PlanarImage Image = e.ImageFrame.Image;
            video.Source = BitmapSource.Create(
                Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, Image.Bits, Image.Width * Image.BytesPerPixel);
        }

        private bool InitializeNui()
        {
            UninitializeNui();
            if (nui == null)
                return false;
            try
            {
                nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            }
            catch (Exception _Exception)
            {
                Console.WriteLine(_Exception.ToString());
                return false;
            }

            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            nui.SkeletonEngine.TransformSmooth = true;
            nuiInitialized = true;
            return true;
        }

        private void UninitializeNui()
        {
            if ((nui != null) && (nuiInitialized))
                nui.Uninitialize();
            nuiInitialized = false;
        }

        private void Playfield_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePlayfieldSize();
        }

        private void UpdatePlayfieldSize()
        {
            // Size of player wrt size of playfield, putting ourselves low on the screen.
            screenRect.X = 0;
            screenRect.Y = 0;
            screenRect.Width = playfield.ActualWidth;
            screenRect.Height = playfield.ActualHeight;

            BannerText.UpdateBounds(screenRect);

            playerBounds.X = 0;
            playerBounds.Width = playfield.ActualWidth;
            playerBounds.Y = playfield.ActualHeight * 0.2;
            playerBounds.Height = playfield.ActualHeight;

            foreach (var player in players)
                player.Value.setBounds(playerBounds);

        }

        void recognizer_SaidSomething(object sender, SpeechRecogniser.SaidSomethingArgs e)
        {
            //CHeck to see if 2 seconds have passed since last note played
            //If so, ok to make change. THis is to try and mitigate issue
            //where playing notes cause recogniser to switch instrument
            //but is not a satisfactory solution
            //Looking to improve the grammar for selection

            if(keyLastPressed.AddMilliseconds(2000) < DateTime.Now)
            {
                switch (e.Instruments)
                {
                    case SpeechRecogniser.Instruments.Piano:
                        //kinectSynth = new PianoSynth();
                        break;
                    case SpeechRecogniser.Instruments.Organ:
                        //kinectSynth = new SanfordSynth();
                        break;

                }
                resetKeys();
            }
            banner.Content = e.Matched;
        }

        private void resetKeys()
        {
            foreach (var key in keyboard.Keys)
            {
                key.IsPlaying = false;
                key.IsPressed = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            runningGameThread = false;
            Properties.Settings.Default.Save();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (recognizer != null)
                recognizer.Stop();
            UninitializeNui();
            Environment.Exit(0);
        }

        private void GameThread()
        {
            runningGameThread = true;
            predNextFrame = DateTime.Now;
            actualFrameTime = 1000.0 / targetFramerate;

            // Try to dispatch at as constant of a framerate as possible by sleeping just enough since
            // the last time we dispatched.
            while (runningGameThread)
            {
                // Calculate average framerate.  
                DateTime now = DateTime.Now;
                if (lastFrameDrawn == DateTime.MinValue)
                    lastFrameDrawn = now;
                double ms = now.Subtract(lastFrameDrawn).TotalMilliseconds;
                actualFrameTime = actualFrameTime * 0.95 + 0.05 * ms;
                lastFrameDrawn = now;

                // Adjust target framerate down if we're not achieving that rate
                frameCount++;
                if (((frameCount % 100) == 0) && (1000.0 / actualFrameTime < targetFramerate * 0.92))
                    targetFramerate = Math.Max(MinFramerate, (targetFramerate + 1000.0 / actualFrameTime) / 2);

                if (now > predNextFrame)
                    predNextFrame = now;
                else
                {
                    double msSleep = predNextFrame.Subtract(now).TotalMilliseconds;
                    if (msSleep >= TimerResolution)
                        Thread.Sleep((int)(msSleep + 0.5));
                }
                predNextFrame += TimeSpan.FromMilliseconds(1000.0 / targetFramerate);

                Dispatcher.Invoke(DispatcherPriority.Send,
                    new Action<int>(HandleGameTimer), 0);
            }
        }

        private void HandleGameTimer(int param)
        {

            // Draw new Wpf scene by adding all objects to canvas
            playfield.Children.Clear();
            
            if(keyboard.Keys.Count == 0) keyboard.InitKeyboard(rainbow);
            keyboard.UpdateKeys(playfield);

            foreach (var player in players)
                player.Value.Draw(playfield.Children);

            CheckPlayers();
        }

        private void PlayMusic()
        {
            foreach (var player in players)
            {
                keyboard.CheckKeyPress(player.Value.segments, kinectSynth);
            }

        }


        
    }
}
