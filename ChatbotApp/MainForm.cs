// Handles the UI Interactions and Form Lifecycles
using System;
using System.Drawing;
using System.Windows.Forms;
using ChatbotApp.Core;

namespace ChatbotApp
{
    public partial class MainForm : Form
    {
        private DansbyCore dansbyCore;

        private TextBox inputTextBox;
        private Button sendButton;
        private Button playButton;
        private Button pauseButton;
        private ComboBox soundtrackComboBox;
        private RichTextBox chatRichTextBox;
        private Timer slimeTimer; // Added SlimeTimer

        public MainForm()
        {
            InitializeComponent();
            InitializeDansbyCore();
            InitializeSlimeTimer(); // Initialize SlimeTimer
            AppendToChatHistory("Welcome to your chat interface. I am Dansby. How can I assist you?");
        }

        private void InitializeComponent()
        {
            // UI Initialization
            this.inputTextBox = new TextBox
            {
                Location = new Point(35, 524),
                Size = new Size(515, 40),
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            this.sendButton = new Button
            {
                Location = new Point(560, 522),
                Size = new Size(105, 27),
                Text = "Send",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            this.sendButton.Click += SendButton_Click;

            this.chatRichTextBox = new RichTextBox
            {
                Location = new Point(35, 50),
                Size = new Size(630, 450),
                ReadOnly = true,
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            this.soundtrackComboBox = new ComboBox
            {
                Location = new Point(35, 10),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            this.playButton = new Button
            {
                Location = new Point(350, 10),
                Size = new Size(75, 25),
                Text = "Play",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            this.playButton.Click += PlayButton_Click;

            this.pauseButton = new Button
            {
                Location = new Point(435, 10),
                Size = new Size(75, 25),
                Text = "Pause",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            this.pauseButton.Click += PauseButton_Click;

            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatRichTextBox);
            this.Controls.Add(this.soundtrackComboBox);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.pauseButton);

            this.ClientSize = new Size(800, 580);
            this.Text = "DansbyChatBot";
            this.BackColor = Color.FromArgb(25, 25, 25);
        }

        private void InitializeDansbyCore()
        {
            // Initialize DansbyCore
            dansbyCore = new DansbyCore();

            // Populate the ComboBox with soundtracks
            foreach (var trackName in dansbyCore.GetSoundtrackNames())
            {
                soundtrackComboBox.Items.Add(trackName);
            }

            if (soundtrackComboBox.Items.Count > 0)
            {
                soundtrackComboBox.SelectedIndex = 0; // Default to the first soundtrack
            }
        }

        private void InitializeSlimeTimer()
        {
            slimeTimer = new Timer
            {
                Interval = 60000 // Every 60 seconds
            };
            slimeTimer.Tick += (s, e) =>
            {
                // Use DansbyCore to decide whether to summon a slime
                if (dansbyCore.ShouldSummonSlime())
                {
                    AppendToChatHistory("A wild slime appears!");
                    dansbyCore.SummonSlime(this); // Pass `this` as the parent for animations
                }
            };
            slimeTimer.Start();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputTextBox.Text;
            AppendToChatHistory($"You: {userInput}");

            // Process input through DansbyCore
            string response = dansbyCore.ProcessUserInput(userInput);
            AppendToChatHistory($"Dansby: {response}");

            inputTextBox.Clear();
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (soundtrackComboBox.SelectedItem != null)
            {
                string selectedSoundtrack = soundtrackComboBox.SelectedItem.ToString();
                dansbyCore.PlaySoundtrack(selectedSoundtrack);
            }
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            dansbyCore.PausePlayback();
        }

        public void AppendToChatHistory(string message)
        {
            chatRichTextBox.AppendText(message + Environment.NewLine);
            chatRichTextBox.SelectionStart = chatRichTextBox.Text.Length;
            chatRichTextBox.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up resources in DansbyCore and stop slime timer
            slimeTimer?.Stop();
            dansbyCore.Shutdown();
            base.OnFormClosing(e);
        }
    }
}
