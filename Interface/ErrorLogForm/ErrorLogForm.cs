using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ChatbotApp.Interface.ErrorLog
{
    public partial class ErrorLogForm : Form
    {    
        private TextBox errorTextBox;
        private Button saveButton;
        private Label SlimeCountLabel;
        private Label SlimeCountLabel2;
        private List<ErrorLogEntry> errorLogEntries;

        public ErrorLogForm()
        {
            InitializeComponent();
            errorLogEntries = new List<ErrorLogEntry>();
        }

        private void InitializeComponent()
        {
            // Set up TextBox
            errorTextBox = new TextBox();
            errorTextBox.Multiline = true;
            errorTextBox.ScrollBars = ScrollBars.Vertical;
            this.errorTextBox.Location = new System.Drawing.Point(10,20);
            this.errorTextBox.Size = new Size(365, 610);
            errorTextBox.ReadOnly = true;
            this.errorTextBox.BackColor = Color.FromArgb(50,50,50);
            this.errorTextBox.ForeColor = Color.White;
            this.errorTextBox.Anchor = AnchorStyles.Top| AnchorStyles.Left;

            // Set up Save ButtonS
            saveButton = new Button();
            saveButton.Text = "Save Log";
            saveButton.ForeColor = Color.White;
            saveButton.Dock = DockStyle.Bottom;
            saveButton.Click += SaveButton_Click;

            //SlimeCountLabel Set Up
            SlimeCountLabel = new Label();
            SlimeCountLabel.Location = new Point(380,20);
            SlimeCountLabel.Size = new Size(40,30);
            SlimeCountLabel.Text = $"Slime\nCounter:";
            SlimeCountLabel.BackColor = Color.FromArgb(158,90,210); //dry light purple
            SlimeCountLabel.ForeColor = Color.White;
            this.SlimeCountLabel.Anchor = AnchorStyles.Top| AnchorStyles.Left;

            //SlimeCountLabel Set Up 2
            SlimeCountLabel2 = new Label();
            SlimeCountLabel2.Location = new Point(430,20);
            SlimeCountLabel2.Size = new Size(50,30);
            SlimeCountLabel2.Text = "0";
            SlimeCountLabel2.BackColor = Color.FromArgb(158,90,210); //dry light purple
            SlimeCountLabel2.ForeColor = Color.White;
            SlimeCountLabel2.TextAlign = ContentAlignment.MiddleCenter;
            SlimeCountLabel2.Font = new Font("Lucida Console", 12);
            this.SlimeCountLabel2.Anchor = AnchorStyles.Top| AnchorStyles.Left;


            // Add controls to the form
            this.Controls.Add(errorTextBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(SlimeCountLabel);
            this.Controls.Add(SlimeCountLabel2);

            // Set up Form properties
            this.Text = "Error Log";
            this.BackColor = Color.FromArgb(25,25,25);
            this.Size = new Size(500, 700);
        }

        // Method to append errors to the log
        public void AppendToErrorLog(string error, string script)
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[{dateTime}] [{script}] {error}";
            errorTextBox.AppendText(logEntry + Environment.NewLine);

            // Store the error in the list for saving
            errorLogEntries.Add(new ErrorLogEntry
            {
                DateTime = dateTime,
                Script = script,
                Error = error
            });
            DebugAutoSave(); //Saves Errors or Debugs to chat 
        }

        public void AppendToDebugLog(string script, string debug)
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[{dateTime}] [{script}] {debug}";
            errorTextBox.AppendText(logEntry + Environment.NewLine);
        }

        // Save log entries to a JSON file
        private void SaveButton_Click(object sender, EventArgs e)
        {
            DebugAutoSave();
            System.Windows.Forms.MessageBox.Show("Error log saved to ErrorLog.json");
        }

        
        public void DebugAutoSave()
        {
            string ErrorJson = @"E:\CODES\DansbyBot\Interface\ErrorLogForm\ErrorLog.json";
            string json = JsonConvert.SerializeObject(errorLogEntries, Formatting.Indented);
            File.WriteAllText(ErrorJson, json);
        }

        public void UpdateSlimeCount(int slimeCount)
        {
            SlimeCountLabel2.Text = $"{slimeCount}";
        }

        private void Label_Paint(object sender, PaintEventArgs e)
        {
            Label label = sender as Label;
            if (label != null)
            {
                using (Pen pen = new Pen(Color.Black, 2)) // Black border with 2px thickness
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, label.Width - 1, label.Height - 1);
                }
            }
        }

    }


    // Helper class for log entries
    public class ErrorLogEntry
    {
        public string DateTime { get; set; }
        public string Script { get; set; }
        public string Error { get; set; }

    } //end of partial class ErrorLogForm

} //end of namespace