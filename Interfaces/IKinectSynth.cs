using Sanford.Multimedia.Synth;

namespace KinectWpfSynth
{
    public interface IKinectSynth
    {
        void PlayNote(IKey note);
        void StopNote(IKey note);
        void ChangePitch(int value);
        void ChangeWave(int value);

        //TODO Add dispose when have ability to switch between synths
    }
}