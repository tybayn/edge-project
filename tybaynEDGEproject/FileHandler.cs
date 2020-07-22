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
using System.Collections;
using System.IO;

namespace tybaynEDGEproject
{
    class FileHandler
    {
        //Variables that contain comparison data and file paths
        private VideoCompareHandler vidCompare;
        private AudioCompareHandler audCompare;
        private String dataCore = @"../data/core.txt";
        private String audioFile = @"../data/audio/";
        private String imageFile = @"../data/images/";
        private const String tempFace = @"tempFace.png";
        private const String tempAudio = @"tempWav.wav";
        private const double matchVal = 50.0;
        private double minPerDif = 100;
        private String minFileName = "";

        //Structure to hold a file name and a difference value
        private struct person : IComparable
        {
            public String file;
            public double dif;

            //Struct constructor
            public person(String f, double d)
            {
                file = f;
                dif = d;
            }

            //Comparison Overload
            public int CompareTo(Object item)
            {
                person that = (person)item;

                if (this.dif > that.dif)
                    return -1;
                if (this.dif < that.dif)
                    return 1;
                
                return 0;
            }
        }

        //+FileHandler(): Constructor, will create files if needed
        public FileHandler() {

            if(!Directory.Exists(@"../data"))
            {
                Directory.CreateDirectory(@"../data");
            }

            if (!File.Exists(dataCore))
            {
                File.Create(dataCore);
            }

            if (!Directory.Exists(audioFile))
            {
                Directory.CreateDirectory(audioFile);
            }

            if (!Directory.Exists(imageFile))
            {
                Directory.CreateDirectory(imageFile);
            }

            //Set up compare classes
            audCompare = new AudioCompareHandler();
            vidCompare = new VideoCompareHandler();
        }

        //+hasMatch(): Determines if the current user image has a match in the system
        public bool hasMatch()
        {
            DirectoryInfo images= new DirectoryInfo(imageFile);
            DirectoryInfo audios = new DirectoryInfo(audioFile);
            FileInfo[] imgFiles = images.GetFiles();
            FileInfo[] audFiles = audios.GetFiles();
            double perDif;
            ArrayList personList = new ArrayList();
            ArrayList avgDif = new ArrayList();
            String[] names = getNameList();
            int mindex = -1;
            minPerDif = 100;
            String tempAudioLow = audCompare.makeLower(tempAudio, @"tempWavLow.wav", 0.75f);

            //Compare faces
            foreach(FileInfo file in imgFiles)
            {
                //Get the difference percentage
                perDif = vidCompare.compare(tempFace, imageFile + "/" + file.Name);

                //Add file and val to array
                personList.Add(new person(file.Name, perDif));
            }

            //Sort the personList
            personList.Sort();

            //Start comparing audio files, slow, possible duplicates
            double audDif;
            foreach (person p in personList)
            {
                audDif = audCompare.compare(tempAudio, audioFile + getAudioFile(p.file), names);

                //If needed, test lower pitches
                if(audDif == 100)
                    audDif = audCompare.compare(tempAudioLow, audioFile + getAudioFileLow(p.file), names);

                avgDif.Add((p.dif * 0.6) + (audDif * 0.4));
            }

            //Run through and get the value with the smallest dif
            for(int i = 0; i < avgDif.Count; i++)
            {
                if((double)avgDif[i] < minPerDif)
                {
                    minPerDif = (double)avgDif[i];
                    mindex = i;
                }
            }

            //Set the minimum file
            if (mindex > -1)
                minFileName = ((person)personList[mindex]).file;

            return minPerDif < matchVal;
        }

        //-getNameList(): Gets a string array of all stored strings
        private String[] getNameList()
        {
            ArrayList names = new ArrayList();
            String[] namesStr;
            String curLine;

            //Open file
            using (StreamReader reader = new StreamReader(dataCore))
            {
                while ((curLine = reader.ReadLine()) != null)
                {
                    if (curLine.Split(',')[0].Contains(" "))
                    {
                        names.Add(curLine.Split(',')[0].Split(' ')[0].ToUpper());
                        names.Add(curLine.Split(',')[0].Split(' ')[1].ToUpper());
                    }
                    else
                    {
                        names.Add(curLine.Split(',')[0]);
                    }
                }
                reader.Close();
            }

            namesStr = new String[names.Count];
            for (int i = 0; i < names.Count; i++)
                namesStr[i] = (String)names[i];

            return namesStr;
        }

        //+getName(): Gets the name of the closet face
        public String getName()
        {
            String curLine;
            String imgFile = minFileName;

            //Open file
            using (StreamReader reader = new StreamReader(dataCore))
            {
                while((curLine = reader.ReadLine()) != null)
                {
                    //Get the name of the matching record
                    if (curLine.Split(',')[3].Equals(imgFile) || curLine.Split(',')[4].Equals(imgFile) || curLine.Split(',')[5].Equals(imgFile))
                    {
                        String name = curLine.Split(',')[0];
                        reader.Close();
                        return name;
                    }
                }
                reader.Close();
            }
            return "null";
        }

        //+getAudioFile(): Gets the audiofile of a record
        public String getAudioFile(String imgFile)
        {
            String curLine;

            //Open file
            using (StreamReader reader = new StreamReader(dataCore))
            {
                while ((curLine = reader.ReadLine()) != null)
                {
                    //Get the name of the matching record
                    if (curLine.Split(',')[3].Equals(imgFile) || curLine.Split(',')[4].Equals(imgFile) || curLine.Split(',')[5].Equals(imgFile))
                    {
                        String file = curLine.Split(',')[1];
                        reader.Close();
                        return file;
                    }
                }
                reader.Close();
            }
            return "null";
        }
        //+getAudioFileLow(): Gets the lower pitch audiofile of a record
        public String getAudioFileLow(String imgFile)
        {
            String curLine;

            //Open file
            using (StreamReader reader = new StreamReader(dataCore))
            {
                while ((curLine = reader.ReadLine()) != null)
                {
                    //Get the name of the matching record
                    if (curLine.Split(',')[3].Equals(imgFile) || curLine.Split(',')[4].Equals(imgFile) || curLine.Split(',')[5].Equals(imgFile))
                    {
                        String file = curLine.Split(',')[2];
                        reader.Close();
                        return file;
                    }
                }
                reader.Close();
            }
            return "null";
        }

        //+getNameShort(): gets a name based on a file, returns name with no white space
        public String getNameShort(String imgFile)
        {
            String curLine;

            //Open file for reading
            using (StreamReader reader = new StreamReader(dataCore))
            {
                while ((curLine = reader.ReadLine()) != null)
                {
                    //If the record contains the filename
                    if (curLine.Split(',')[3].Equals(imgFile) || curLine.Split(',')[4].Equals(imgFile) || curLine.Split(',')[5].Equals(imgFile))
                    {
                        //Get the name
                        String name = curLine.Split(',')[0];
                        reader.Close();
                        return name.Replace(" ","");
                    }
                }
                reader.Close();
            }
            return "null";
        }

        //+getMinPerDif(): Returns the minimum difference
        public double getMinPerDif()
        {
            return minPerDif;
        }

        //+addNew(): Saves a new user to the system
        public void addNew(String name)
        {
            //If the name is not blank
            if (!name.Equals(""))
            {
                //Set file paths
                String nameShort = name.Replace(" ", "");
                String newVidFile1 = @nameShort + "1.png";
                String newVidFile2 = @nameShort + "2.png";
                String newVidFile3 = @nameShort + "3.png";
                String newAudFile1 = @nameShort + "1.wav";
                String newAudFile2 = @nameShort + "2.wav";

                //Copy temp files to system
                File.Copy(@"newFace1.png", imageFile + newVidFile1, true);
                File.Copy(@"newFace2.png", imageFile + newVidFile2, true);
                File.Copy(@"newFace3.png", imageFile + newVidFile3, true);

                //Create lower pitch copy
                String tempAudioL = audCompare.makeLower(tempAudio, @"tempWavLow.wav", 0.75f);
                File.Copy(tempAudio, audioFile + newAudFile1, true);
                File.Copy(tempAudioL, audioFile + newAudFile2, true);

                //If the user doesn't already exists
                if (getNameShort(newVidFile1).Equals("null"))
                {
                    //Write new record
                    using (StreamWriter writer = File.AppendText(dataCore))
                    {
                        writer.WriteLine(name + "," + newAudFile1 + "," + newAudFile2 + "," + newVidFile1 + "," + newVidFile2 + "," + newVidFile3);
                        writer.Close();
                    }
                }
            }
        }
    }
}
