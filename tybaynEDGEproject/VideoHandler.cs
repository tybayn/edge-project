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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using WinFormCharpWebCam;

namespace tybaynEDGEproject
{
    class VideoHandler
    {
        //Variables
        private WebCam webcam;
        public PictureBox feed;
        public PictureBox faceTemplate;

        //+Constructor: initialiazes the PictureBox element
        public VideoHandler(){
            feed = new PictureBox();
            faceTemplate = new PictureBox();
            feed.Controls.Add(faceTemplate);
        }

        //+initialize(): intitializes forms...
        public void initialize()
        {
            this.feed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.feed.Location = new System.Drawing.Point(12, 39);
            this.feed.Name = "pictureBox1";
            this.feed.Size = new System.Drawing.Size(512, 384);
            this.feed.SizeMode = PictureBoxSizeMode.StretchImage;

            this.faceTemplate.Location = new System.Drawing.Point(0, 0);
            this.faceTemplate.Name = "pictureBox2";
            this.faceTemplate.Size = new System.Drawing.Size(512, 384);
            this.faceTemplate.Image = Image.FromFile(@"../data/resources/faceOverlay.png");
            this.faceTemplate.BringToFront();
            this.faceTemplate.BackColor = Color.Transparent;
        }

        //+deviceInit(): initializes the webcam
        public void deviceInit()
        {
            webcam = new WebCam();
            webcam.InitializeWebCam(ref feed);
        }

        //+start(): starts the feed
        public void start()
        {
            webcam.Start();
        }

        //+stop(): stops the feed
        public void stop()
        {
            webcam.Stop();
        }

        //+resolutionSettings(): opens up a menu for resolution settings
        public void resolutionSettings()
        {
            webcam.ResolutionSetting();
        }

        //+sourceSettings(): opens up device settings for webcam
        public void sourceSettings()
        {
            webcam.AdvanceSetting();
        }

        //+capture(): saves the current frame of the feed
        public void capture()
        {
            this.feed.Image.Save("tempImg.png", ImageFormat.Png);
        }

        //+newCapture(): saves new images for storing to file
        public void newCapture()
        {
            Bitmap snap = (Bitmap)this.feed.Image;
            resizeImage(snap.Clone(new Rectangle(131, 104, 60, 60), snap.PixelFormat), 500, 500).Save("newFace1.png", ImageFormat.Png);
            resizeImage(snap.Clone(new Rectangle(126, 99, 70, 70), snap.PixelFormat),500,500).Save("newFace2.png", ImageFormat.Png);
            resizeImage(snap.Clone(new Rectangle(119, 92, 85, 85), snap.PixelFormat),500,500).Save("newFace3.png", ImageFormat.Png);
        }

        //+capture(): saves a sub image of the original
        public void capture(int x, int y, int w, int h, String file = "tempFace.png")
        {
            this.feed.Image.Save("tempImg.png", ImageFormat.Png);
            Bitmap source = new Bitmap(@"tempImg.png");
            Bitmap face = source.Clone(new Rectangle(x, y, w, h), source.PixelFormat);
            face = resizeImage(face,500,500);
            face.Save(file, ImageFormat.Png);
        }

        //-resizeImage(): resizes the image
        private Bitmap resizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        //+getFrame(): returns the raw frame image from the feed
        public Image getFrame()
        {
            return this.feed.Image;
        }

        //+overlayOn(): turns on the overlay
        public void overlayOn()
        {
            this.faceTemplate.Visible = true;
        }

        //+overlayOff(): turns off the overlay
        public void overlayOff()
        {
            this.faceTemplate.Visible = false;
        }
    }
}
