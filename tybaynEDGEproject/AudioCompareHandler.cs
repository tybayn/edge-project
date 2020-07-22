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
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using Mike.Rules; //Plugin for Pitch shifting algorithm, License requires name

namespace tybaynEDGEproject
{
    class AudioCompareHandler
    {
        //Variables needed
        bool completed;
        private String name1 = "", name2 = "";
        int file = 1;

        //+AudioCompareHandler(): Constructor
        public AudioCompareHandler() { }

        //+compare(): Compares to audio files based on the converted strings
        public double compare(String wav1, String wav2, String[] inNames)
        {
            //Create and load a grammar. 
            //Grammar is based on viable names
            Choices names = new Choices(inNames);
            GrammarBuilder nameBuilder = new GrammarBuilder(names);
            GrammarBuilder fullnameBuilder = new GrammarBuilder(nameBuilder, 0, 2);
            Grammar myGrammar = new Grammar(fullnameBuilder);
            myGrammar.Name = "Name Grammar";

            //Start getting string of Audio File 1
            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine())
            {
                file = 1;
                name1 = "";
                recognizer.LoadGrammar(myGrammar);

                // onfigure the input to the recognizer. 
                recognizer.SetInputToWaveFile(wav1);

                //Attach event handlers for the results of recognition 
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
                recognizer.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(recognizer_RecognizeCompleted);

                //Perform recognition on the entire file
                completed = false;
                recognizer.Recognize();
            }

            //Start getting string of Audio File 2
            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine())
            {
                file = 2;
                name2 = "";
                recognizer.LoadGrammar(myGrammar);

                //Configure the input to the recognizer 
                recognizer.SetInputToWaveFile(wav2);

                //Attach event handlers for the results of recognition
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
                recognizer.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(recognizer_RecognizeCompleted);

                //Perform recognition on the entire file 

                completed = false;
                recognizer.Recognize();
            }

            //Return the difference between the two strings
            return  getDif(name1, name2);
        }

        //-getDif(): returns how many of the words/names are different as a percentage
        private double getDif(String a, String b)
        {
            //If both names are empty
            if ((a.Length == 0) && (b.Length == 0))
                return 100 - 0;

            //If both names are the same
            if (a == b)
                return 100 - 100;

            //If one or the other is empty
            if ((a.Length == 0) || (b.Length == 0))
            {
                return 100 - 0;
            }

            //Variables needed for comparisons
            int sameCharAtIndex = 0;
            String[] sentence1 = a.Split(' ');
            String[] sentence2 = b.Split(' ');
            double maxLen = sentence1.Length > sentence2.Length ? sentence1.Length : sentence2.Length;
            int minLen = sentence1.Length < sentence2.Length ? sentence1.Length : sentence2.Length;

            //Loop through each string by word
            foreach (String n1 in sentence1)
            {
                foreach (String n2 in sentence2)
                {
                    if (n1.Equals(n2))
                        sameCharAtIndex++;
                }
            }

            //Get the percentage, and return it
            double val = 100 - (sameCharAtIndex / maxLen * 100);
            return Convert.ToDouble(string.Format("{0:0.00}", val));
        }

        //-recognizer_SpeechRecognized(): Handle the SpeechRecognized event.  
        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null && e.Result.Text != null)
            {
                if (this.file == 1)
                {
                    name1 += e.Result.Text;
                }
                else
                    name2 += e.Result.Text;
            }
        }

        //-recognizer_RecognizeCompleted(): Handle the RecognizeCompleted event.  
        private void recognizer_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            completed = true;
        }

        //+makeLower(): returns a string for a lower pitch version of the file
        public String makeLower(String file, String newFile, float pitchShift)
        {
            // Read header, data and channels as separated data

            // Normalized data stores. Store values in the format -1.0 to 1.0
            byte[] waveheader = null;
            byte[] wavedata = null;

            int sampleRate = 0;

            float[] in_data_l = null;
            float[] in_data_r = null;

            GetWaveData(file, out waveheader, out wavedata, out sampleRate, out in_data_l, out in_data_r);

            //
            // Apply Pitch Shifting
            //

            if (in_data_l != null)
                PitchShifter.PitchShift(pitchShift, in_data_l.Length, (long)1024, (long)10, sampleRate, in_data_l);

            if (in_data_r != null)
                PitchShifter.PitchShift(pitchShift, in_data_r.Length, (long)1024, (long)10, sampleRate, in_data_r);

            //
            // Time to save the processed data
            //

            // Backup wave data
            byte[] copydata = new byte[wavedata.Length];
            Array.Copy(wavedata, copydata, wavedata.Length);

            GetWaveData(in_data_l, in_data_r, ref wavedata);

            // Save modified wavedata

            string targetFilePath = newFile;
            if (File.Exists(targetFilePath))
                File.Delete(targetFilePath);

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(targetFilePath)))
            {
                writer.Write(waveheader);
                writer.Write(wavedata);
            }

            return newFile;
        }

        // Returns left and right float arrays. 'right' will be null if sound is mono.
        private void GetWaveData(string filename, out byte[] header, out byte[] data, out int sampleRate, out float[] left, out float[] right)
        {
            byte[] wav = File.ReadAllBytes(filename);

            // Determine if mono or stereo
            int channels = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

            // Get sample rate
            sampleRate = BitConverter.ToInt32(wav, 24);

            int pos = 12;

            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
            while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
            {
                pos += 4;
                int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }

            pos += 4;

            int subchunk2Size = BitConverter.ToInt32(wav, pos);
            pos += 4;

            // Pos is now positioned to start of actual sound data.
            int samples = subchunk2Size / 2;     // 2 bytes per sample (16 bit sound mono)
            if (channels == 2)
                samples /= 2;        // 4 bytes per sample (16 bit stereo)

            // Allocate memory (right will be null if only mono sound)
            left = new float[samples];

            if (channels == 2)
                right = new float[samples];
            else
                right = null;

            header = new byte[pos];
            Array.Copy(wav, header, pos);

            data = new byte[subchunk2Size];
            Array.Copy(wav, pos, data, 0, subchunk2Size);

            // Write to float array/s:
            int i = 0;
            while (pos < subchunk2Size)
            {

                left[i] = BytesToNormalized_16(wav[pos], wav[pos + 1]);
                pos += 2;
                if (channels == 2)
                {
                    right[i] = BytesToNormalized_16(wav[pos], wav[pos + 1]);
                    pos += 2;
                }
                i++;
            }
        }

        // Return byte data from left and right float data. Ignore right when sound is mono
        private void GetWaveData(float[] left, float[] right, ref byte[] data)
        {
            // Calculate k
            // This value will be used to convert float to Int16
            // We are not using Int16.Max to avoid peaks due to overflow conversions            
            float k = (float)Int16.MaxValue / left.Select(x => Math.Abs(x)).Max();

            // Revert data to byte format
            Array.Clear(data, 0, data.Length);
            int dataLenght = left.Length;
            int byteId = -1;
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream(data)))
            {
                for (int i = 0; i < dataLenght; i++)
                {
                    byte byte1 = 0;
                    byte byte2 = 0;

                    byteId++;
                    NormalizedToBytes_16(left[i], k, out byte1, out byte2);
                    writer.Write(byte1);
                    writer.Write(byte2);

                    if (right != null)
                    {
                        byteId++;
                        NormalizedToBytes_16(right[i], k, out byte1, out byte2);
                        writer.Write(byte1);
                        writer.Write(byte2);
                    }
                }
            }
        }

        // Convert two bytes to one double in the range -1 to 1
        private float BytesToNormalized_16(byte firstByte, byte secondByte)
        {
            // convert two bytes to one short (little endian)
            short s = (short)((secondByte << 8) | firstByte);
            // convert to range from -1 to (just below) 1
            return s / 32678f;
        }

        // Convert a float value into two bytes (use k as conversion value and not Int16.MaxValue to avoid peaks)
        private void NormalizedToBytes_16(float value, float k, out byte firstByte, out byte secondByte)
        {
            short s = (short)(value * k);
            firstByte = (byte)(s & 0x00FF);
            secondByte = (byte)(s >> 8);
        }
    }
}
