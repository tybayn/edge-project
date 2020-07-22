// MIT License
//
// tybaynEDGEproject
//
// Copyright © 2020 Ty Bayn
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
// LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN 
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace tybaynEDGEproject
{
    class AudioHandler
    {
        //Variables
        private WaveInEvent source;
        private BufferedWaveProvider provider;
        private WaveOut player;
        private static WaveFileWriter waveFile = null;
        private int audioSrc = 0;
        private int numDevices = 0;

        private int count;
        private const int Speed = 150;
        private float leftMax, rightMax;
        private object sampleObject;
        private NotifyingSampleProvider notify;

        //Must be placed into a form (public)
        public WaveFormVisualizer wave;
        public ComboBox device;

        //Variables for passing events
        public event EventHandler SelectedIndexChanged;

        //Handler Functions
        protected virtual void indexChanged(EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }
        private void device_SelectedIndexChanged(object sender, EventArgs e)
        {
            indexChanged(e);
        }

        //+Constructor: Initializes the form components and gets a list of devices
        public AudioHandler()
        {
            wave = new WaveFormVisualizer();
            device = new ComboBox();

            //Get a list of system audio input devices
            MMDeviceEnumerator names = new MMDeviceEnumerator();
            var devices = names.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            numDevices = devices.Count();
            device.Items.AddRange(devices.ToArray());

            //Add event listener
            device.SelectedIndexChanged += device_SelectedIndexChanged;
        }

        //+initialize: build form pieces... cause visual studio will erase them...
        public void initialize()
        {
            this.wave.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.wave.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.wave.WaveColor = System.Drawing.Color.Lime;
            this.wave.Location = new System.Drawing.Point(12, 424);
            this.wave.Name = "wave";
            this.wave.Size = new System.Drawing.Size(512, 51);
            this.wave.TabIndex = 13;

            this.device.FormattingEnabled = true;
            this.device.Location = new System.Drawing.Point(114, 480);
            this.device.Name = "comboBox1";
            this.device.Size = new System.Drawing.Size(280, 21);
            this.device.Text = "Audio Source...";
            this.device.TabIndex = 15;
        }

        //-DrawAudio: takes the waveform and draws it
        private void DrawAudioWave(object sender, SampleEventArgs e)
        {
            lock (sampleObject)
            {
                if (count >= Speed)
                {
                    wave.addAudio(leftMax, rightMax);

                    leftMax = 0;
                    rightMax = 0;
                    count = 0;
                }
                else
                {
                    if (Math.Abs(e.Left) + Math.Abs(e.Right) > Math.Abs(leftMax) + Math.Abs(rightMax))
                    {
                        leftMax = e.Left;
                        rightMax = e.Right;
                    }

                    count++;
                }
            }
        }

        //-BufferPlay(): playback of audio
        private void BufferPlay(byte[] recv)
        {
            if (recv.Length > 0)
            {
                try
                {
                    provider.AddSamples(recv, 0, recv.Length);
                }
                catch(Exception ex)
                {
                    provider.ClearBuffer();
                }
            }
        }

        //+start(): initialize webcam and start feed
        public void start(float playbackVolume = 0)
        {
            //Get desired audio device
            audioSrc = numDevices - device.SelectedIndex - 1;

            //Initialize device
            source = new WaveInEvent { WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(audioSrc).Channels) };
            source.DataAvailable += sourceDataAvailable;
            provider = new BufferedWaveProvider(new WaveFormat());
            player = new WaveOut();
            sampleObject = new object();

            //Initialize waveForm painter
            notify = new NotifyingSampleProvider(provider.ToSampleProvider());
            notify.Sample += DrawAudioWave;

            //Start feed
            source.StartRecording();
            source.BufferMilliseconds = 10;
            player.Init(notify);
            player.Play();
            player.Volume = playbackVolume;
        }

        //+getDevice(): Returns the current device id
        public int getDevice()
        {
            return audioSrc;
        }

        //-sourceDataAvailable(): runs if feed has new data
        private void sourceDataAvailable(object sender, WaveInEventArgs e)
        {
            BufferPlay(e.Buffer);
        }

        //+recordAudio(): creates a second feed to record file
        public static void recordAudio(int sec, int src, String file = "tempWav.wav")
        {
            //Create new feed
            WaveInEvent snippit = new WaveInEvent();
            snippit.WaveFormat = new WaveFormat(44100, 16, 1);

            //Setup recording protocol
            snippit.DataAvailable += new EventHandler<WaveInEventArgs>(dataAvailable);
            snippit.RecordingStopped += new EventHandler<StoppedEventArgs>(recordingStopped);

            //Set file to record to
            waveFile = new WaveFileWriter(file, snippit.WaveFormat);

            //Start recording
            snippit.StartRecording();
            Task.Delay(sec * 1000).ContinueWith(t => snippit.StopRecording());
        }

        //-dataAvailable(): runs if recording feed has new data
        private static void dataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();
            }
        }

        //-recordingStopped(): ends recording, closes file
        private static void recordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
        }

        //+stop(): stops all feeds
        public void stop()
        {
            if (player == null) return;

            player.Dispose();
            player.Stop();
            provider.ClearBuffer();
        }

        //+lockMic(): disables controls
        public void lockMic()
        {
            this.device.Enabled = false;
        }

        //-unlockMic(): enables controls
        public void unlockMic()
        {
            this.device.Enabled = true;
        }
    }
}
