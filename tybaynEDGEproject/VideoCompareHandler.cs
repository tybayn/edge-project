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
using XnaFan.ImageComparison;

namespace tybaynEDGEproject
{
    class VideoCompareHandler
    {
        //+VideoCompareHandler(): Constructor
        public VideoCompareHandler() { }

        //+compare(): Compares two images and returns the difference between the two
        public double compare(String image1Path, String image2Path, byte threshold = 20)
        {
            //Load the two files
            Bitmap firstBmp = (Bitmap)Image.FromFile(image1Path);
            Bitmap secondBmp = (Bitmap)Image.FromFile(image2Path);

            //Save a difference image for debug
            firstBmp.GetDifferenceImage(secondBmp, true).Save("difImg.png");

            //Return the difference between the images
            return firstBmp.PercentageDifference(secondBmp, threshold) * 100;
        } 
    }
}
