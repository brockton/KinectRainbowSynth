using Sanford.Multimedia.Synth;
namespace KinectWpfSynth
{
    public class SynthVoice : Voice
    {
        #region Fields

        // For use as a portamento source.
        private SlewLimiter slewLimiter;

        // Two oscillators; typical of most subtractive synths.
        private Oscillator osc1;
        private Oscillator osc2;

        // A couple of LFO's to use as modulation sources.
        private Lfo lfo1;
        private Lfo lfo2;

        // A couple of ADSR envelopes to use as modulation source. The
        // first envelope is hardwired to modulate the overall amplitude.
        private AdsrEnvelope envelope1;
        private AdsrEnvelope envelope2;

        // For filtering the sound.
        private StateVariableFilter filter;

        // Convert the mono output of the filter into stereo.
        private MonoToStereoConverter converter;

        // For use when no modulation source is set.
        private EmptyMonoComponent emptyFMModulator;

        // For choosing which waveform the oscillators use.
        private OscWaveProgrammer oscWaveProgrammer1;
        private OscWaveProgrammer oscWaveProgrammer2;

        // For choosing the oscillators' FM source.
        private OscFMProgrammer oscFMProgrammer1;
        private OscFMProgrammer oscFMProgrammer2;

        #endregion

        #region Constructors

        public SynthVoice(SampleRate sampleRate, StereoBuffer buffer)
            : base(sampleRate, buffer)
        {
            Initialize(sampleRate, buffer);
        }

        public SynthVoice(SampleRate sampleRate, StereoBuffer buffer, string name)
            : base(sampleRate, buffer, name)
        {
            Initialize(sampleRate, buffer);
        }

        #endregion

        private void Initialize(SampleRate sampleRate, StereoBuffer buffer)
        {
            emptyFMModulator = new EmptyMonoComponent(sampleRate, new MonoBuffer(0));

            slewLimiter = new SlewLimiter(sampleRate, new MonoBuffer(0), "Portamento");
            slewLimiter.SynthesizeReplaceEnabled = true;

            envelope1 = new AdsrEnvelope(sampleRate, new MonoBuffer(0), "Envelope 1");
            envelope1.SynthesizeReplaceEnabled = true;

            envelope2 = new AdsrEnvelope(sampleRate, new MonoBuffer(0), "Envelope 2");
            envelope2.SynthesizeReplaceEnabled = true;

            lfo1 = new Lfo(sampleRate, new MonoBuffer(0), "LFO 1");
            lfo1.SynthesizeReplaceEnabled = true;

            lfo2 = new Lfo(sampleRate, new MonoBuffer(0), "LFO 2");
            lfo2.SynthesizeReplaceEnabled = true;

            Wavetable wave = Wavetable.Load("Sawtooth");

            osc1 = new Oscillator(sampleRate, new MonoBuffer(0), "Oscillator 1", wave, emptyFMModulator, slewLimiter);
            osc1.SynthesizeReplaceEnabled = true;

            osc2 = new Oscillator(sampleRate, new MonoBuffer(0), "Oscillator 2", wave, emptyFMModulator, slewLimiter);
            osc2.SynthesizeReplaceEnabled = true;

            filter = new StateVariableFilter(sampleRate, new MonoBuffer(0), "State Variable Filter", osc1, osc2, envelope2, lfo2, envelope1);
            filter.SynthesizeReplaceEnabled = true;

            converter = new MonoToStereoConverter(sampleRate, buffer, filter);
            converter.SynthesizeReplaceEnabled = false;

            oscWaveProgrammer1 = new OscWaveProgrammer("Oscillator 1", osc1);
            oscWaveProgrammer2 = new OscWaveProgrammer("Oscillator 2", osc2);

            oscFMProgrammer1 = new OscFMProgrammer("Oscillator 1", osc1, emptyFMModulator, envelope1, envelope2, lfo1, lfo2);
            oscFMProgrammer2 = new OscFMProgrammer("Oscillator 2", osc2, emptyFMModulator, envelope1, envelope2, lfo1, lfo2);

            AddComponent(emptyFMModulator);
            AddComponent(slewLimiter);
            AddComponent(osc1);
            AddComponent(osc2);
            AddComponent(envelope1);
            AddComponent(envelope2);
            AddComponent(lfo1);
            AddComponent(lfo2);
            AddComponent(filter);
            AddComponent(converter);

            AddParameters(slewLimiter);
            AddParameters(osc1);
            AddParameters(oscWaveProgrammer1);
            AddParameters(oscFMProgrammer1);
            AddParameters(osc2);
            AddParameters(oscWaveProgrammer2);
            AddParameters(oscFMProgrammer2);
            AddParameters(envelope1);
            AddParameters(envelope2);
            AddParameters(lfo1);
            AddParameters(lfo2);
            AddParameters(filter);

            AddBendable(osc1);
            AddBendable(osc2);

            AddControllable(lfo1);
            AddControllable(lfo2);
        }

        protected override bool IsPlaying
        {
            get
            {
                return !envelope1.HasCompleted;
            }
        }

        public override bool SynthesizeReplaceEnabled
        {
            get
            {
                return converter.SynthesizeReplaceEnabled;
            }
            set
            {
                converter.SynthesizeReplaceEnabled = value;
            }
        }
        
    }
}