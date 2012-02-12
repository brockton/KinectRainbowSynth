using System;
using System.Diagnostics;
using Sanford.Multimedia.Synth;

namespace KinectWpfSynth
{
    public class OscFMProgrammer : IProgrammable
    {
        public enum ParameterId
        {
            FMSource,
        }

        public enum FMSource
        {
            None,
            Lfo1,
            Lfo2,
            Envelope1,
            Envelope2
        }

        private const int FMSourceCount = (int)FMSource.Envelope2 + 1;

        private string name = string.Empty;

        private Oscillator osc;

        private EmptyMonoComponent emptyFMSource;

        private AdsrEnvelope env1;

        private AdsrEnvelope env2;

        private Lfo lfo1;

        private Lfo lfo2;

        private FMSource fmSource = FMSource.None;

        public OscFMProgrammer(string name, Oscillator osc, EmptyMonoComponent emptyFMSource, AdsrEnvelope env1, AdsrEnvelope env2, Lfo lfo1, Lfo lfo2)
        {
            #region Require

            if (osc == null)
            {
                throw new ArgumentNullException("osc");
            }
            else if (emptyFMSource == null)
            {
                throw new ArgumentNullException("emptyFMSource");
            }
            else if (env1 == null)
            {
                throw new ArgumentNullException("env1");
            }
            else if (env2 == null)
            {
                throw new ArgumentNullException("env2");
            }
            else if (lfo1 == null)
            {
                throw new ArgumentNullException("lfo1");
            }
            else if (lfo2 == null)
            {
                throw new ArgumentNullException("lfo2");
            }

            #endregion

            this.name = name;
            this.osc = osc;
            this.emptyFMSource = emptyFMSource;
            this.env1 = env1;
            this.env2 = env2;
            this.lfo1 = lfo1;
            this.lfo2 = lfo2;
        }

        #region IProgrammable Members

        public string GetParameterName(int index)
        {
            #region Require

            if (index < 0 || index >= ParameterCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            #endregion

            string result = string.Empty;
            string s = name;

            if (!string.IsNullOrEmpty(s))
            {
                s = s + " ";
            }

            switch ((ParameterId)index)
            {
                case ParameterId.FMSource:
                    result = s + "FM Source";
                    break;

                default:
                    Debug.Fail("Unhandled parameter");
                    break;
            }

            return result;
        }

        public string GetParameterLabel(int index)
        {
            #region Require

            if (index < 0 || index >= ParameterCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            #endregion

            string result = string.Empty;

            switch ((ParameterId)index)
            {
                case ParameterId.FMSource:
                    result = "Source";
                    break;

                default:
                    Debug.Fail("Unhandled parameter");
                    break;
            }

            return result;
        }

        public string GetParameterDisplay(int index)
        {
            #region Require

            if (index < 0 || index >= ParameterCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            #endregion

            string result = string.Empty;

            switch ((ParameterId)index)
            {
                case ParameterId.FMSource:
                    result = fmSource.ToString();
                    break;

                default:
                    Debug.Fail("Unhandled parameter");
                    break;
            }

            return result;
        }

        public float GetParameterValue(int index)
        {
            #region Require

            if (index < 0 || index >= ParameterCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            #endregion

            float result = 0;

            switch ((ParameterId)index)
            {
                case ParameterId.FMSource:
                    result = (float)(int)(fmSource) / (FMSourceCount - 1);
                    break;

                default:
                    Debug.Fail("Unhandled parameter");
                    break;
            }

            return result;
        }

        public void SetParameterValue(int index, float value)
        {
            #region Require

            if (index < 0 || index >= ParameterCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            else if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            #endregion

            switch ((ParameterId)index)
            {
                case ParameterId.FMSource:
                    fmSource = (FMSource)(int)Math.Round(value * (FMSourceCount - 1));

                    switch (fmSource)
                    {
                        case FMSource.None:
                            osc.FMModulator = emptyFMSource;
                            break;

                        case FMSource.Envelope1:
                            osc.FMModulator = env1;
                            break;

                        case FMSource.Envelope2:
                            osc.FMModulator = env2;
                            break;

                        case FMSource.Lfo1:
                            osc.FMModulator = lfo1;
                            break;

                        case FMSource.Lfo2:
                            osc.FMModulator = lfo2;
                            break;

                        default:
                            Debug.Fail("Unhandled FM Source");
                            break;
                    }
                    break;

                default:
                    Debug.Fail("Unhandled parameter");
                    break;
            }
        }

        public int ParameterCount
        {
            get
            {
                return (int)ParameterId.FMSource + 1;
            }
        }

        #endregion
    }
}
