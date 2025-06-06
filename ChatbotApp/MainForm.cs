using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatbotApp.Core;

namespace ChatbotApp
{
    public partial class MainForm : Form
    {
        private readonly DansbyCore dansbyCore;
        private readonly ErrorLogClient errorLogClient;

        // UI Controls
        private TextBox inputTextBox;
        private Button sendButton;
        private Button playButton;
        private Button pauseButton;
        private ComboBox soundtrackComboBox;
        private RichTextBox chatRichTextBox;
        private Button returnToMainScreenButton;
        public Panel intentPanel;
        private Splitter intentSplitter;
        private Button intentTabButton;

        public MainForm()
        {
            try
            {
                InitializeComponent();
                this.StartPosition = FormStartPosition.CenterScreen;
                this.WindowState = FormWindowState.Normal;

                errorLogClient = ErrorLogClient.Instance;
                dansbyCore = new DansbyCore(this);

                // Run async Initialization
                _ = InitializeAsync();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing the form: {ex.Message}", "MainForm.cs");
            }
        }

        // Async initialization to keep the UI responsive
        private async Task InitializeAsync()
        {
            try
            {
                AppendToChatHistory("Welcome to your chat interface. I am Dansby. How can I assist you?", Color.MediumPurple);
                await dansbyCore.InitializeAsync(); //Initializes all the Managers from Core...

                // Load soundtracks into the ComboBox
                var trackNames = await dansbyCore.GetSoundtrackNamesAsync();
                soundtrackComboBox.Items.AddRange(trackNames.ToArray());

                if (soundtrackComboBox.Items.Count > 0)
                {
                    soundtrackComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error during initialization: {ex.Message}", "MainForm");
                AppendToChatHistory("Sorry, an error occurred during startup.");
            }
        }

        private void InitializeComponent()
        {

            // -------------------- KeyListener ------------------
            this.KeyPreview = true; // Allow MainForm to capture keys ***** <---- !Important

            // F9 Key to switch to Intent Panel
            this.KeyDown += async (object sender, KeyEventArgs e) =>
            {
                if (e.KeyCode == Keys.F9)
                {
                    e.SuppressKeyPress = true; // Prevent "ding" sound
                    await ToggleSlidingPanel(intentPanel, e);
                }
        
                // Allow Enter key in RichTextBoxes
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true; // Prevent the default 'ding' sound
                    await SendButton_Click(sender, EventArgs.Empty);                
                }

            };
            // -------------------- End of Key Listener ------------------

            // ------------------ UI Controls Initialization -------------------------
            inputTextBox = new TextBox
            {
                Location = new Point(35, 190),
                Size = new Size(90, 40),
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right

            };

            sendButton = new Button
            {
                Location = new Point(130, 188),
                Size = new Size(105, 26),
                Text = "Send",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            sendButton.Click += async (sender, e) => await SendButton_Click(sender, e);

            chatRichTextBox = new RichTextBox
            {
                Location = new Point(35, 50),
                Size = new Size(200, 130),
                ReadOnly = true,
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Consolas", 8)

            };

            soundtrackComboBox = new ComboBox
            {
                Location = new Point(35, 10),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            playButton = new Button
            {
                Location = new Point(350, 10),
                Size = new Size(75, 25),
                Text = "Play",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            playButton.Click += async (sender, e) => await PlayButton_Click(sender, e);

            pauseButton = new Button
            {
                Location = new Point(435, 10),
                Size = new Size(75, 25),
                Text = "Pause",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            pauseButton.Click += async (sender, e) => await PauseButton_Click(sender, e);

            returnToMainScreenButton = new Button
            {
                Location = new Point(732, 10), // Adjust location as needed
                Size = new Size(120, 25),
                Text = "Main Screen",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left 
        
            };
            returnToMainScreenButton.Click += ReturnToMainScreenButton_Click;
            returnToMainScreenButton.BringToFront();

            //------------------ Pannel UI Components for IntentEditorManagement -----------------------

            intentPanel = new Panel // Sliding Pannel for Intents - Acts as a sub form of sorts
            {
                Dock = DockStyle.Fill,
                MinimumSize = new Size(700, 0),
                Visible = false,
                BackColor = Color.FromArgb(25, 25, 25)
            };
            
            intentSplitter = new Splitter 
            {
                Dock = DockStyle.Right
                
            };

            intentTabButton = new Button //Button to Connect an Event for Keyboard Input -> "Tab"
            {
                Text = "->",
                Location = new Point(10,100),
                Size = new Size(30,50),
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
                
            };
            intentTabButton.Click += async (sender, e) => await ToggleSlidingPanel(intentPanel, e);

            //-------------------------------------------------------- End of IntentPanelUI

            // Adding controls to the form
            this.Controls.Add(inputTextBox);
            this.Controls.Add(sendButton);
            this.Controls.Add(chatRichTextBox);
            this.Controls.Add(soundtrackComboBox);
            this.Controls.Add(playButton);
            this.Controls.Add(pauseButton);
            this.Controls.Add(returnToMainScreenButton);
            this.Controls.Add(intentPanel);
            this.Controls.Add(intentSplitter);
            this.Controls.Add(intentTabButton);

            this.ClientSize = new Size(900, 580);
            this.Text = "DansbyChatBot";
            this.BackColor = Color.FromArgb(25, 25, 25);
            intentPanel.BringToFront();

            // Background Image
            //this.BackgroundImage = Image.FromFile("ChatbotApp\\Resources\\MainScreenComponents\\Background10.png");
            //this.BackgroundImageLayout = ImageLayout.Stretch;

        }

        private async Task SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputTextBox.Text.Trim();

            if (string.IsNullOrEmpty(userInput))
            {
                AppendToChatHistory("Dansby: Please enter a message.", Color.MediumPurple);
                return;
            }

            AppendToChatHistory($"You: {userInput}");
            inputTextBox.Clear();

            string response = await dansbyCore.ProcessUserInputAsync(userInput);
            AppendToChatHistory($"Dansby: {response}", Color.MediumPurple);
        }

        private async Task PlayButton_Click(object sender, EventArgs e)
        {
            if (soundtrackComboBox.SelectedItem != null)
            {
                string selectedSoundtrack = soundtrackComboBox.SelectedItem.ToString();
                await dansbyCore.PlaySoundtrackAsync(selectedSoundtrack);
            }
        }

        private async Task PauseButton_Click(object sender, EventArgs e)
        {
            await dansbyCore.PausePlaybackAsync();
        }

        // Event handler to return to MainScreenForm
        private void ReturnToMainScreenButton_Click(object sender, EventArgs e)
        {
            
            Program.mainScreenForm.Show(); //Show mainScreenForm
            this.Hide(); //hide MainForm
        }

        public void AppendToChatHistory(string message, Color? color = null)
        {
            if (chatRichTextBox.InvokeRequired)
            {
                chatRichTextBox.Invoke(new Action(() => AppendToChatHistory(message, color)));
            }
            else
            {
                int start = chatRichTextBox.TextLength;
                chatRichTextBox.AppendText(message + Environment.NewLine);
                int end = chatRichTextBox.TextLength;

                // Apply color if provided
                if (color.HasValue)
                {
                    chatRichTextBox.Select(start, end - start);
                    chatRichTextBox.SelectionColor = color.Value;
                }

                // Reset selection and scroll to the caret
                chatRichTextBox.SelectionStart = chatRichTextBox.Text.Length;
                chatRichTextBox.SelectionLength = 0;
                chatRichTextBox.ScrollToCaret();
            }
        }

        // --- Functions for the UI for IntentManagement --------
        private async Task ToggleSlidingPanel(Panel panel, EventArgs e)
        {
            if (panel.Visible)
            {
                // Hide the panel
                panel.Visible = false;
                panel.Width = 0;
            }
            else
            {
                // Show the panel
                panel.Visible = true;
                panel.Width = 500; // Set the desired width
            }
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            await dansbyCore.ShutdownAsync();
            base.OnFormClosing(e);
        }
    }
}
