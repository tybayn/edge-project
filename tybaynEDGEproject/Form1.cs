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

using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tybaynEDGEproject
{
    public partial class mainWinForm : Form
    {
        //Variables needed to do face detection and run states
        private FaceHaarCascade cascade;
        private HaarObjectDetector detector;
        private FileHandler data;
        private Thread detectFace;
        private static bool run = true;
        private int state = 0;
        private bool isMatch = false;
        private String name = "";
        private String userMessage = "";

        //+mainWinForm(): Constructor, gets parts initialized
        public mainWinForm()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            data = new FileHandler();
            InitializeComponent();
            this.bntStart.Enabled = false;
            this.bntStop.Enabled = false;
            this.recordBtn.Enabled = false;
            this.capture.Enabled = false;
            this.send.Enabled = false;
            this.bntVideoSource.Enabled = false;
            this.userInput.Enabled = false;
            this.mic.SelectedIndexChanged += mic_SelectedIndexChanged;

            Log("[Software Started]");
            Log("[Components initialized]");

            this.cascade = new FaceHaarCascade();
            this.detector = new HaarObjectDetector(cascade, minSize: 50, searchMode: ObjectDetectorSearchMode.NoOverlap);

            Log(">Please choose an audio source.");
        }

        //-mainWinForm_Load(): Once the form is loaded, initialize the webcam
        private void mainWinForm_Load(object sender, EventArgs e)
        {
            webcam.deviceInit();
        }

        //-mainWinForm_Closing(): When form is closed, shut down form and clear threads
        private void mainWinForm_Closing(object sender, EventArgs e)
        {
            Log("Closing software...");
            run = false;
            Thread.Sleep(5000);
            detectFace.Abort();
            Environment.Exit(Environment.ExitCode);
        }

        //-detectThread(): This thread runs in the background to detect faces and controls flow of states
        private void detectThread()
        {
            Bitmap curImage;
            Bitmap clone;
            Rectangle[] rectangles;

            //Keep the thread running until program ends or state is changed
            while (state == 0)
            {
                try
                {
                    //Get current frame, convert to proper pixel format
                    curImage = (Bitmap)webcam.getFrame();
                    clone = new Bitmap(curImage.Width, curImage.Height, PixelFormat.Format24bppRgb);

                    using (Graphics gr = Graphics.FromImage(clone))
                    {
                        gr.DrawImage(curImage, new Rectangle(0, 0, clone.Width, clone.Height));
                    }

                    //Get list of faces in image
                    rectangles = detector.ProcessFrame(clone);

                    //If a face is detected, tell the user
                    if (rectangles.Length > 0)
                    {

                        //debug line// this.Invoke(new Action(() => Log("[Face detected at (" + rectangles[0].X + "," + rectangles[0].Y + ")]")));
                        //If face is detected within the acceptable area
                        if (rectangles[0].X >= 95 && rectangles[0].X <= 140 &&
                            rectangles[0].Y >= 70 && rectangles[0].Y <= 130)
                        {
                            //Get the face from the webcam
                            this.Invoke(new Action(() =>
                            {
                                Log("\r\n[Face detected]");
                                startCapture(rectangles[0]);
                            }));

                            //Wait until capture is complete
                            while (state == 1) { }

                            //Prompt user to record voice
                            if (state == 2)
                            {
                                state = 3;
                                this.Invoke(new Action(() =>
                                {
                                    Log("[Audio recorded]");
                                    startCompare();
                                }));
                            }

                            //Wait for voice to be recorded
                            while (state == 3) { }

                            //Get the difference between the faces and display them
                            if (state == 4)
                            {
                                this.Invoke(new Action(() => Log("\r\n[" + data.getMinPerDif() + " difference]")));

                                //State if a match is found
                                if (isMatch)
                                    this.Invoke(new Action(() => Log(">Found a match!")));
                                else
                                    this.Invoke(new Action(() => Log(">No match found...")));

                                state = 5;
                            }

                            //Wait in case of threading issue
                            while (state == 4) { }

                            //If there was a match found
                            if (state == 5 && isMatch)
                            {
                                //Confirm the user is who the AI thinks it is
                                this.Invoke(new Action(() => startConfirmUser()));

                                //Wait for user to confirm
                                while (state == 5) { }

                                //If the user is who the AI thinks it is
                                if (userMessage.Equals("YES"))
                                {
                                    this.Invoke(new Action(() => Log(">Success!")));
                                    state = 0;
                                }

                                //If the AI is wrong
                                else
                                {
                                    state = 5;
                                    isMatch = false;
                                }
                                
                            }

                            //If there is no acceptable match found
                            if (state == 5 && !isMatch)
                            {
                                //Ask if user wants to be added to the system
                                this.Invoke(new Action(() => startNoUser()));

                                //Wait for user response
                                while (state == 5) { }

                                //If user does want to be added
                                if (userMessage.Equals("YES"))
                                {
                                    //Get photos of the user and wait for response
                                    this.Invoke(new Action(() => startNewRecord()));
                                    while (state == 6) { }

                                    //Get voice recording from user and wait for response
                                    this.Invoke(new Action(() => startNewRecord2()));
                                    while (state == 7) { }

                                    //Get name of user and wait for response
                                    this.Invoke(new Action(() => startNewRecord3()));
                                    while (state == 8) { }

                                    //Add new user and reset detection thread
                                    this.Invoke(new Action(() => addNewRecord()));
                                    state = 0;
                                }

                                //If user does not want to be added
                                else
                                {
                                    this.Invoke(new Action(() => Log("[Starting detection]")));
                                    state = 0;
                                }
                            }
                        }
                    }

                    //Pause between detection captures
                    if(run)
                        Thread.Sleep(1500);
                }
                catch(Exception ex)
                {
                    //om nom nom
                }
            }
        }

        //-bntStart_Click(): What to do when start button is clicked
        private void bntStart_Click(object sender, EventArgs e)
        {
            //Enable/Disable some buttons
            this.bntStart.Enabled = false;
            this.bntStop.Enabled = true;
            this.recordBtn.Enabled = false;
            this.capture.Enabled = false;
            this.send.Enabled = false;
            this.bntVideoSource.Enabled = true;
            this.userInput.Enabled = false;
            this.mic.lockMic();

            //Start inputs
            mic.start();
            Log("[Audio started on device " + mic.getDevice() + "]");
            webcam.start();
            Log("[Video started]");
            webcam.overlayOn();
            detectFace = new Thread(detectThread);
            Task.Delay(2000).ContinueWith(t => detectFace.Start());
            Log("[Detector thread started]");
        }

        //-bntStop_Click(): Runs when Stop button is clicked
        private void bntStop_Click(object sender, EventArgs e)
        {

            //Enable/Disable some buttons
            this.bntStart.Enabled = true;
            this.bntStop.Enabled = false;
            this.recordBtn.Enabled = false;
            this.capture.Enabled = false;
            this.send.Enabled = false;
            this.bntVideoSource.Enabled = false;
            this.userInput.Enabled = false;
            this.mic.unlockMic();

            //Stop inputs
            state = 0;
            mic.stop();
            Log("[Audio stopped]");
            webcam.stop();
            Log("[Video stopped]");
            detectFace.Abort();
            Log("[Detection stopped]");

        }

        //-bntVideoFormat_Click(): Runs when resolution button is clicked
        private void bntVideoFormat_Click(object sender, EventArgs e)
        {
            webcam.resolutionSettings();
        }

        //-bntVideoSource_Click(): Runs when the video source button is clicked
        private void bntVideoSource_Click(object sender, EventArgs e)
        {
            webcam.sourceSettings();
        }

        //-startCapture(): Starts capture of face from the webcam image
        private void startCapture(Rectangle faceSquare)
        {
            //Set state
            state = 1;

            //Capture face based on found face coordinates
            webcam.capture(faceSquare.X,faceSquare.Y,faceSquare.Width,faceSquare.Height);
            Log("[Image captured]");

            //Prompt user to record voice and enable buttons to do so
            Log(">Please press 'Record' and state your name");
            recordBtn.Enabled = true;
        }

        //-startCompare(): Starts the comparison between the current user and all saved users
        private void startCompare()
        {
            Log("[Searching database...]");
            isMatch = data.hasMatch();

            //Stop the thread to give other thread time to complete running
            new Thread(() =>
            {
                Thread.Sleep(1000);
                state = 4;
            }).Start();
        }

        //-startNewRecord(): Starts first prompt to get a new user, photo
        private void startNewRecord()
        {
            //Toggle buttons
            this.recordBtn.Enabled = false;
            this.capture.Enabled = true;
            this.bntStop.Enabled = false;
            Log("\r\n>Lets get you added");
            Log(">Line up your face on screen, look straight ahead, and press 'Capture'");
        }

        //-startNewRecord2(): Starts second prompt to get a new user, voice
        private void startNewRecord2()
        {
            //Toggle buttons
            this.recordBtn.Enabled = true;
            this.capture.Enabled = false;
            Log("\r\n>Next step!");
            Log(">Press 'Record' and state your name.");
        }

        //-startNewRecord3(): Starts third prompt to get a new user, name
        private void startNewRecord3()
        {
            //Toggle buttons
            this.recordBtn.Enabled = false;
            this.capture.Enabled = false;
            this.userInput.Enabled = true;
            this.send.Enabled = true;
            Log("\r\n>Last step!");
            Log(">Type in your first and last name and press 'Submit'");
        }

        //-addNewRecord(): Add the new user to the system
        private void addNewRecord()
        {
            this.bntStop.Enabled = true;
            Log(">Success! You have been added.");
            this.data.addNew(name);
        }

        //-startConfirmUser(): Starts prompt to confirm if the user is who the AI suspects
        private void startConfirmUser()
        {
            this.userInput.Enabled = true;
            this.send.Enabled = true;
            Log("\r\n>Are you " + data.getName() + "?");
            Log(">Type 'yes' or 'no' then press 'Submit'");
        }

        //-startNoUser(): Starts prompt to see if user wants to be added to the system
        private void startNoUser()
        {
            this.userInput.Enabled = true;
            this.send.Enabled = true;
            Log("\r\n>You were not found in our system!");
            Log(">Would you like to be added?");
            Log(">Type 'yes' or 'no' then press 'Submit'");
        }

        //-Log(): Logs data or output to the Log textbox
        private void Log(String data)
        {
            textBox1.AppendText(data + "\r\n");
        }

        //-send_Click(): Runs when the send click button is pressed
        private void send_Click(object sender, EventArgs e)
        {
            //Get data from the textbox and set variables
            Log("< " + this.userInput.Text);
            userMessage = this.userInput.Text.ToUpper();
            name = this.userInput.Text;
            this.userInput.Text = "";
            this.userInput.Enabled = false;
            this.send.Enabled = false;
            state++;
        }

        //-recordBtn_Click(): Runs when record button is clicked, starts audio recording
        private void recordBtn_Click(object sender, EventArgs e)
        {
            //Start recording audio
            recordBtn.Enabled = false;
            Log("[Recording audio...]");
            int src = mic.getDevice();
            AudioHandler.recordAudio(5, src);

            //Record for 5 seconds
            new Thread(() =>
            {
                Thread.Sleep(5200);
                state++;
            }).Start();
        }

        //-UNUSED
        private void recordBtn_MouseDown(object sender, EventArgs e)
        { }

        //-UNUSED
        private void recordBtn_MouseUp(object sender, EventArgs e)
        { }

        //-mic_SelectedIndexChanged(): Runs when option in combobox is changed
        private void mic_SelectedIndexChanged(object sender, EventArgs e)
        {
            Log("[Mic Input Changed]");
            this.bntStart.Enabled = true;
        }

        //-capture_Click(): Runs when capture button is pressed
        private void capture_Click(object sender, EventArgs e)
        {
            //Grab new images of user for system storage
            Log("[Images Captured]");
            this.webcam.newCapture();
            state++;
        }
    }
}
