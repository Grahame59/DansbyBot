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
        private Timer autoSaveTimer;

        public ErrorLogForm()
        {
            InitializeComponent();
            
            // Initialize the list first
            errorLogEntries = new List<ErrorLogEntry>();

            // Load from existing JSON
            LoadErrorLog();

            //AppendToErrorLog("Open ErrorLog Test", "MainForm.cs");
            AppendToDebugLog($"Number of error entries: {errorLogEntries.Count}", "ErrorLogForm.cs");
            DebugAutoSave();

            // Initialize and set up autosave timer
            autoSaveTimer = new Timer();
            autoSaveTimer.Interval = 60000; // 1 minute in milliseconds (60000 ms)
            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            autoSaveTimer.Start();



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

            // Set the caret position to the end of the text
            errorTextBox.SelectionStart = errorTextBox.Text.Length;
            errorTextBox.ScrollToCaret();  // Scroll to the caret

            //Console.WriteLine("Error added to list. Current count: " + errorLogEntries.Count);
            
        }

        public void AppendToDebugLog(string debug, string script)
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[{dateTime}] [{script}] {debug}";
            errorTextBox.AppendText(logEntry + Environment.NewLine);

            // Store the error in the list for saving
            errorLogEntries.Add(new ErrorLogEntry
            {
                DateTime = dateTime,
                Script = script,
                Error = debug
            });

            // Set the caret position to the end of the text
            errorTextBox.SelectionStart = errorTextBox.Text.Length;
            errorTextBox.ScrollToCaret();  // Scroll to the caret

            //Console.WriteLine("Error added to list. Current count: " + errorLogEntries.Count);

        }

        // Save log entries to a JSON file
        private void SaveButton_Click(object sender, EventArgs e)
        {
            DebugAutoSave();
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            DebugAutoSave();
            AppendToDebugLog("Debug: AutoSave triggered.", "ErrorLogForm.cs");
        }

        
        public void DebugAutoSave()
        {
            try
            {
                //MessageBox.Show($"Entries count before saving: {errorLogEntries.Count}");
                
                if (errorLogEntries.Count == 0)
                {
                    AppendToDebugLog("Debug: No Entries to save", "ErrorLogForm.cs");
                    return;
                }
                
                string ErrorJson = @"E:\CODES\DansbyBot\Interface\ErrorLogForm\ErrorLog.json";    
                string json = JsonConvert.SerializeObject(errorLogEntries, Formatting.Indented); 
                
                File.WriteAllText(ErrorJson, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save error log: {ex.Message}");
                AppendToErrorLog($"Error: Failed to save error log: {ex.ToString()}", "ErrorLogForm.cs");
            }
        }       
        private void LoadErrorLog()
        {
            string ErrorJson = @"E:\CODES\DansbyBot\Interface\ErrorLogForm\ErrorLog.json";
            if (File.Exists(ErrorJson))
            {
                try
                {
                    string json = File.ReadAllText(ErrorJson);
                    errorLogEntries = JsonConvert.DeserializeObject<List<ErrorLogEntry>>(json) ?? new List<ErrorLogEntry>();
                    foreach (var entry in errorLogEntries)
                    {
                        string logEntry = $"[{entry.DateTime}] [{entry.Script}] {entry.Error}";
                        errorTextBox.AppendText(logEntry + Environment.NewLine);
                    }
                    AppendToDebugLog("Debug: ErrorLog Loaded from Json upon initialization.", "ErrorLogForm.cs");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load error log: {ex.Message}");
                }
            }
        }

        public void UpdateSlimeCount(int slimeCount)
        {
            SlimeCountLabel2.Text = $"{slimeCount}";
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