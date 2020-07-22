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

using System.Windows.Forms;

namespace tybaynEDGEproject
{
    partial class mainWinForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private VideoHandler webcam;
        private AudioHandler mic;
        private System.Windows.Forms.Button bntStart;
        private System.Windows.Forms.Button bntStop;
        private System.Windows.Forms.Button bntVideoSource;
        private System.Windows.Forms.Label label1;
        private TextBox textBox1;
        private Label label2;

        private void InitializeAudioVideo()
        {
            webcam = new VideoHandler();
            mic = new AudioHandler();
            webcam.initialize();
            mic.initialize();

            this.Controls.Add(this.webcam.feed);
            this.Controls.Add(this.mic.wave);
            this.Controls.Add(this.mic.device);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bntStart = new System.Windows.Forms.Button();
            this.bntStop = new System.Windows.Forms.Button();
            this.bntVideoSource = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.userInput = new System.Windows.Forms.TextBox();
            this.send = new System.Windows.Forms.Button();
            this.recordBtn = new System.Windows.Forms.Button();
            this.capture = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bntStart
            // 
            this.bntStart.Location = new System.Drawing.Point(736, 479);
            this.bntStart.Name = "bntStart";
            this.bntStart.Size = new System.Drawing.Size(41, 23);
            this.bntStart.TabIndex = 2;
            this.bntStart.Text = "Start";
            this.bntStart.UseVisualStyleBackColor = true;
            this.bntStart.Click += new System.EventHandler(this.bntStart_Click);
            // 
            // bntStop
            // 
            this.bntStop.Location = new System.Drawing.Point(681, 479);
            this.bntStop.Name = "bntStop";
            this.bntStop.Size = new System.Drawing.Size(49, 23);
            this.bntStop.TabIndex = 3;
            this.bntStop.Text = "Stop";
            this.bntStop.UseVisualStyleBackColor = true;
            this.bntStop.Click += new System.EventHandler(this.bntStop_Click);
            // 
            // bntVideoSource
            // 
            this.bntVideoSource.Location = new System.Drawing.Point(12, 479);
            this.bntVideoSource.Name = "bntVideoSource";
            this.bntVideoSource.Size = new System.Drawing.Size(95, 23);
            this.bntVideoSource.TabIndex = 8;
            this.bntVideoSource.Text = "Video Source ...";
            this.bntVideoSource.UseVisualStyleBackColor = true;
            this.bntVideoSource.Click += new System.EventHandler(this.bntVideoSource_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Live Video/Audio Feed";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(526, 39);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(251, 384);
            this.textBox1.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(527, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Log";
            // 
            // userInput
            // 
            this.userInput.Location = new System.Drawing.Point(526, 426);
            this.userInput.Name = "userInput";
            this.userInput.Size = new System.Drawing.Size(174, 20);
            this.userInput.TabIndex = 13;
            // 
            // send
            // 
            this.send.Location = new System.Drawing.Point(706, 425);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(71, 22);
            this.send.TabIndex = 14;
            this.send.Text = "Submit";
            this.send.UseVisualStyleBackColor = true;
            this.send.Click += new System.EventHandler(this.send_Click);
            // 
            // recordBtn
            // 
            this.recordBtn.Location = new System.Drawing.Point(655, 451);
            this.recordBtn.Name = "recordBtn";
            this.recordBtn.Size = new System.Drawing.Size(122, 23);
            this.recordBtn.TabIndex = 15;
            this.recordBtn.Text = "Record";
            this.recordBtn.UseVisualStyleBackColor = true;
            this.recordBtn.Click += new System.EventHandler(this.recordBtn_Click);
            this.recordBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.recordBtn_MouseDown);
            this.recordBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.recordBtn_MouseUp);
            // 
            // capture
            // 
            this.capture.Location = new System.Drawing.Point(526, 451);
            this.capture.Name = "capture";
            this.capture.Size = new System.Drawing.Size(123, 23);
            this.capture.TabIndex = 16;
            this.capture.Text = "Capture";
            this.capture.UseVisualStyleBackColor = true;
            this.capture.Click += new System.EventHandler(this.capture_Click);
            // 
            // mainWinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 508);
            this.Controls.Add(this.capture);
            this.Controls.Add(this.recordBtn);
            this.Controls.Add(this.send);
            this.Controls.Add(this.userInput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bntVideoSource);
            this.Controls.Add(this.bntStop);
            this.Controls.Add(this.bntStart);
            InitializeAudioVideo();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "mainWinForm";
            this.Text = "Ty Bayn EDGE Project";
            this.Load += new System.EventHandler(this.mainWinForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox userInput;
        private Button send;
        private Button recordBtn;
        private Button capture;
    }
}

