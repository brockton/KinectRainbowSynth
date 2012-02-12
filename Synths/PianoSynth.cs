using System.IO;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Media;
using System.Timers;

namespace KinectWpfSynth
{
    public class PianoSynth : IKinectSynth
    {

        private List<Stream> noteFiles = new List<Stream>
                                              {

                                                      Properties.Resources.Grand_Piano___Fazioli___major_C_middle,

                                                      Properties.Resources.Grand_Piano___Fazioli___major_D_middle,

                                                      Properties.Resources.Grand_Piano___Fazioli___major_E_middle,

                                                      Properties.Resources.Grand_Piano___Fazioli___major_F_middle,

                                                      Properties.Resources.Grand_Piano___Fazioli___major_G_middle, 

                                                      Properties.Resources.Grand_Piano___Fazioli___major_A_middle,
                                                  
                                                      Properties.Resources.Grand_Piano___Fazioli___major_B_middle,

                                                      Properties.Resources.Grand_Piano___Fazioli___major_C_middle

                                              };

        public IWavePlayer waveOutDevice;
        private WaveMixerStream32 mixer;
        WaveFileReader[] reader = new WaveFileReader[8];
        WaveOffsetStream[] offsetStream = new WaveOffsetStream[8];
        WaveChannel32[] channelSteam = new WaveChannel32[8];

        public PianoSynth()
        {
            mixer = new WaveMixerStream32();
            mixer.AutoStop = false;

            int i = 0;
            foreach (var note in noteFiles)
            {
                reader[i] = new WaveFileReader(note);

                offsetStream[i] = new WaveOffsetStream(reader[i]);
                channelSteam[i] = new WaveChannel32(offsetStream[i]);
                channelSteam[i].Position = channelSteam[i].Length;
                mixer.AddInputStream(channelSteam[i]);
                
                i++;
            }

            if (waveOutDevice == null)
            {
                waveOutDevice = new WaveOut {DeviceNumber = 0};
                waveOutDevice.Init(mixer);
                waveOutDevice.Volume = 0;
                waveOutDevice.Play();
                waveOutDevice.Volume = 100;
            }

        }

        public void PlayNote(IKey note)
        {
            if(!note.IsPlaying)
                channelSteam[note.Note].Position = 0;
            note.IsPlaying = true;
        }

        public void StopNote(IKey note)
        {
            //As this is playing a wave file it stops anyway.
            //No need to do anything other than let it play again.
            note.IsPlaying = false;
        }


        public void ChangePitch(int value)
        {
            throw new System.NotImplementedException();
        }

        public void ChangeWave(int value)
        {
           throw new System.NotImplementedException();

           
        }


    }
}