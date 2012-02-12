using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
//using System.Windows.Forms;
//using LiteWaveDemo;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Synth;
//using SimpleSynth;

namespace KinectWpfSynth
{
    public class SanfordSynth : IKinectSynth
    {
        private int midinote = 45;

        private int[] majorScale = new int[8]{45, 47, 49, 50, 52, 54, 56, 57};
        

        public SanfordSynth()
        {
            CreateSynthesizer(0, 2048, 44100);
        }

        private Synthesizer synth;        

        public Synthesizer CreateSynthesizer(int deviceId, int bufferSize, int sampleRate)
        {
            synth = new Synthesizer(
                "Lite Wave",
                deviceId,
                bufferSize,
                sampleRate,
                new VoiceFactory(delegate(SampleRate sr, StereoBuffer buffer, string name)
                {
                    return new SynthVoice(sr, buffer, name);
                }),
                8,
                delegate(SampleRate sr, StereoBuffer inputBuffer)
                {
                    List<EffectComponent> myEffects = new List<EffectComponent>();

                    myEffects.Add(new Sanford.Multimedia.Synth.Chorus(sr, inputBuffer));
                    myEffects.Add(new Sanford.Multimedia.Synth.Echo(sr, inputBuffer));

                    return myEffects;
                });

            if (File.Exists("LiteWaveDefaultBank.bnk"))
            {
                try
                {
                    // Load default bank into Lite Wave synthesizer.
                    synth.LoadBank("LiteWaveDefaultBank.bnk");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            else
            {
                MessageBox.Show("No Soundbank Found", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            synth.SetParameterValue(11, (float)1.0);

            //Set to organ
            synth.SetSelectedProgramIndex(2);
            
            return synth;
        }

        public void PlayNote(IKey note)
        {
            if (!note.IsPlaying)
            {
                synth.Play();

                synth.ProcessChannelMessage(new ChannelMessage(ChannelCommand.NoteOn, 0, majorScale[note.Note],
                                                               ChannelMessage.DataMaxValue));
                
            }
            note.IsPlaying = true;
        }

        public void StopNote(IKey note)
        {
            note.IsPlaying = false;
            synth.ProcessChannelMessage(new ChannelMessage(ChannelCommand.NoteOff, 0, majorScale[note.Note]));
            
        }

        
        public void ChangePitch(int value)
        {
            //synth.SetParameterValue(1, value);
            //synth.SetParameterValue(12, value);
        }

        public void ChangeWave(int value)
        {
            //synth.SetSelectedProgramIndex(value);
            //float wave;
            //if (value > 2)
            //{
            //    wave = (float) 1.0;
                
            //}
            ////else if (value > 3)
            ////{
            ////    wave = (float)0.2857143;
            ////}
            //else
            //{
            //    wave = (float)0.2857143;
                
            //}
            //synth.SetParameterValue(11, wave);
            
            
            
        }


    }
}