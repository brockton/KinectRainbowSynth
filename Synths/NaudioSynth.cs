using System;
using Sanford.Multimedia.Synth;
using NAudio.Midi;

namespace KinectWpfSynth
{
    public class NaudioSynth : IKinectSynth
    {
        public NaudioSynth()
        {
            
        }

        public void PlayNote(IKey note)
        {
            throw new NotImplementedException();
        }

        public void StopNote(IKey note)
        {
            throw new NotImplementedException();
        }

        public void StopPlaying()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void ChangePitch(int value)
        {
            throw new NotImplementedException();
        }

        public void ChangeWave(int value)
        {
            throw new NotImplementedException();
        }
    }
}