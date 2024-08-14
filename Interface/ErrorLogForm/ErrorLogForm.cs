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
            this.errorTextBox.Location = new System.Drawing.Point(10,10);
            this.errorTextBox.Size = new Size(365, 620);
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

            // Add controls to the form
            this.Controls.Add(errorTextBox);
            this.Controls.Add(saveButton);

            // Set up Form properties
            this.Text = "Error Log";
            this.BackColor = Color.FromArgb(25,25,25);
            this.Size = new Size(400, 700);
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
    }


    // Helper class for log entries
    public class ErrorLogEntry
    {
        public string DateTime { get; set; }
        public string Script { get; set; }
        public string Error { get; set; }

    } //end of partial class ErrorLogForm

} //end of namespace