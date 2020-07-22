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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace tybaynEDGEproject
{
    public class WaveFormVisualizer : Panel
    {
        //Variables
        private Color WaveColor2;
        private SmoothingMode smoothMode = SmoothingMode.Default;
        private CompositingMode compostingMode = CompositingMode.SourceOver;
        private CompositingQuality composingQuality = CompositingQuality.Default;
        private InterpolationMode intpolMode = InterpolationMode.Default;
        private PixelOffsetMode pixelMode = PixelOffsetMode.Default;
        private int maxSamples;
        private int insertPos;
        private List<float> leftSamples = new List<float>(); // Stereo Side Left
        private List<float> rightSamples = new List<float>(); // Stereo Side Right
        private LinearGradientBrush br; // Used for gradiant
        private int x;

        //+Constructor: Creates an extended Windows Form Panel
        public WaveFormVisualizer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            OnForeColorChanged(EventArgs.Empty);
            OnResize(EventArgs.Empty);
        }

        //~OnForeColorChanged: From panel class, set colors
        protected override sealed void OnForeColorChanged(EventArgs e)
        {
            WaveColor2 = ForeColor;
            base.OnForeColorChanged(e);
        }

        //+WaveColor
        public Color WaveColor { get; set; }

        //+OnResized: Add samples from stereo audio
        public void addAudio(float leftSample, float rightSample)
        {
            if (maxSamples == 0)
                return;

            // Input the samples for the left track of audio
            if (leftSamples.Count <= maxSamples)
                leftSamples.Add(leftSample);
            else if (insertPos < maxSamples)
                leftSamples[insertPos] = leftSample;

            // Input the samples for the right track of audio
            if (rightSamples.Count <= maxSamples)
                rightSamples.Add(rightSample);
            else if (insertPos < maxSamples)
                rightSamples[insertPos] = rightSample;

            insertPos += 1;
            insertPos = insertPos % maxSamples;

            Invalidate();
        }


        //-blend: blend the gradient colors on the waveform
        private static void blend(ref LinearGradientBrush brush)
        {
            float[] relativeIntensities = { 0f, 1f, 0f };
            float[] relativePositions = { 0f, 0.5f, 1f };

            Blend blend = new Blend { Factors = relativeIntensities, Positions = relativePositions };
            brush.Blend = blend;
        }

        //-getLeft: get left sample from audio
        private float getLeft(int index)
        {
            if (index < 0)
                index += maxSamples;

            if (index >= 0 & index < leftSamples.Count)
                return leftSamples[index];

            return 0;
        }

        //-getRight: get left sample from audio
        private float getRight(int index)
        {
            if (index < 0)
                index += maxSamples;

            if (index >= 0 & index < rightSamples.Count)
                return rightSamples[index];

            return 0;
        }

        //~OnResized: From panel class, event, set samples for stereo audio
        protected override sealed void OnResize(EventArgs e)
        {
            maxSamples = Width;
            leftSamples = new List<float>(maxSamples);
            rightSamples = new List<float>(maxSamples);
            insertPos = 0;
            base.OnResize(e);
        }

        //~OnPaint: From panel class, event, how to paint the waveforms
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            var waveDraw = pe.Graphics;
            waveDraw.SmoothingMode = smoothMode;
            waveDraw.CompositingMode = compostingMode;
            waveDraw.CompositingQuality = composingQuality;
            waveDraw.InterpolationMode = intpolMode;
            waveDraw.PixelOffsetMode = pixelMode;

            float middle = (float)(Math.Floor((double) (Height / 2)) - 1);

            // Paint the waveform
            using (GraphicsPath path = new GraphicsPath())
            {
                for (x = 0; x <= Width - 1; x++)
                {
                    float mixed = (getLeft(x - Width + insertPos) + getRight(x - Width + insertPos)) / 2;

                    path.AddLines(new[] {
                        new Point(x, (int) middle),
                        new Point(x, Convert.ToInt32(middle - middle * mixed))
                    });
                }

                //Apply gradient color
                using (br = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), WaveColor, WaveColor2, LinearGradientMode.Vertical))
                {
                    blend(ref br);
                    pe.Graphics.DrawPath(new Pen(br), path);
                }
            }
        }
    }
}