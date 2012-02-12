using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Research.Kinect.Audio;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace KinectWpfSynth
{
    public class SpeechRecogniser
    {
        public enum Instruments
        {
            Piano,
            Organ
        } ;

        public enum Orders
        {
            Instrument,
            Keyboard
        } ;

        public enum Keyboards
        {
            Circle,
            Flat
        } ;

        struct WhatSaid
        {
            public Instruments instruments;
            public Orders orders;
            public Keyboards keyboards;
        }

        private Dictionary<string, WhatSaid> InstrumentPhrases = new Dictionary<string, WhatSaid>()
                                                                     {
                                                                         {
                                                                             "Piano",
                                                                             new WhatSaid()
                                                                                 {instruments = Instruments.Piano}
                                                                             },
                                                                         {
                                                                             "Organ",
                                                                             new WhatSaid()
                                                                                 {instruments = Instruments.Organ}
                                                                             }
                                                                     };

        private Dictionary<string, WhatSaid> OrderPhrases = new Dictionary<string, WhatSaid>()
                                                                {
                                                                    {
                                                                        "Instrument",
                                                                        new WhatSaid() {orders = Orders.Instrument}
                                                                        },
                                                                    {
                                                                        "Keyboard",
                                                                        new WhatSaid() {orders = Orders.Keyboard}
                                                                        }
                                                                };

        private Dictionary<string, WhatSaid> KeyboardPhrases = new Dictionary<string, WhatSaid>()
                                                                {
                                                                    {
                                                                        "Circle",
                                                                        new WhatSaid() {keyboards = Keyboards.Circle}
                                                                        },
                                                                    {
                                                                        "Flat",
                                                                        new WhatSaid() {keyboards = Keyboards.Flat}
                                                                        }
                                                                };

        public class SaidSomethingArgs : EventArgs
        {
            public Instruments Instruments { get; set; }
            public Keyboards Keyboards { get; set; }
            public Orders Orders { get; set; }
            public string Matched { get; set; }
        
        }
        public event EventHandler<SaidSomethingArgs> SaidSomething;

        private KinectAudioSource kinectSource;
        private SpeechRecognitionEngine sre;
        private const string RecognizerId = "SR_MS_en-US_Kinect_10.0";
        private bool paused = false;
        private bool valid = false;

        public SpeechRecogniser()
        {
            RecognizerInfo ri = SpeechRecognitionEngine.InstalledRecognizers().Where(r => r.Id == RecognizerId).FirstOrDefault();
            if (ri == null)
                return;
                
            sre = new SpeechRecognitionEngine(ri.Id);

            // Build a simple grammar of shapes, colors, and some simple program control
            var instruments = new Choices();
            foreach (var phrase in InstrumentPhrases)
                instruments.Add(phrase.Key);
            
            var objectChoices = new Choices();
            objectChoices.Add(instruments);
            

            var actionGrammar = new GrammarBuilder();
            //actionGrammar.AppendWildcard();
            actionGrammar.Append(objectChoices);

            var gb = new GrammarBuilder();
            gb.Append(actionGrammar);

            var g = new Grammar(gb);
            sre.LoadGrammar(g);
            sre.SpeechRecognized += sre_SpeechRecognized;
            sre.SpeechHypothesized += sre_SpeechHypothesized;
            sre.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sre_SpeechRecognitionRejected);

            var t = new Thread(StartDMO);
            t.Start();

            valid = true;
	    }

        public bool IsValid()
        {
            return valid;
        }

        private void StartDMO()
        {
            kinectSource = new KinectAudioSource();
            kinectSource.SystemMode = SystemMode.OptibeamArrayOnly;
            kinectSource.FeatureMode = true;
            kinectSource.AutomaticGainControl = false;
            kinectSource.MicArrayMode = MicArrayMode.MicArrayAdaptiveBeam;
            var kinectStream = kinectSource.Start();
            sre.SetInputToAudioStream(kinectStream, new SpeechAudioFormatInfo(
                                                  EncodingFormat.Pcm, 16000, 16, 1,
                                                  32000, 2, null));
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void Stop()
        {
            if (sre != null)
            {
                sre.RecognizeAsyncCancel();
                sre.RecognizeAsyncStop();
                kinectSource.Dispose();
            }
        }

        void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            var said = new SaidSomethingArgs();
            said.Instruments = Instruments.Piano;
            said.Matched = "?";
            SaidSomething(new object(), said);
            Console.WriteLine("\nSpeech Rejected - Default Piano Selected");
        }

        void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.Write("\rSpeech Recognized: \t{0}", e.Result.Text);
            
            if (SaidSomething == null)
                return;
            
            var said = new SaidSomethingArgs();
            said.Instruments = 0;

            // First check for color, in case both color _and_ shape were both spoken

            foreach (var phrase in InstrumentPhrases)
                if (e.Result.Text.Contains(phrase.Key))
                {
                    said.Instruments = phrase.Value.instruments;
                    said.Matched = phrase.Key;
                    break;
                }

            SaidSomething(new object(), said);
        }
    }
}