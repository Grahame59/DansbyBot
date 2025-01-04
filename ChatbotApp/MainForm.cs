using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatbotApp.Core;
using Intents;
using ChatbotApp.Utilities;

namespace ChatbotApp
{
    public partial class MainForm : Form
    {
        private DansbyCore dansbyCore;
        private IntentRecognizer intentRecognizer;
        private ResponseGenerator responseGenerator;
        private VaultManager vaultManager;
        private ErrorLogClient errorLogClient;

        // UI Controls
        private TextBox inputTextBox;
        private Button sendButton;
        private Button playButton;
        private Button pauseButton;
        private Button refreshCacheButton;
        private ComboBox soundtrackComboBox;
        private RichTextBox chatRichTextBox;
        private Timer slimeTimer;

        public MainForm()
        {
            try
            {
                InitializeComponent();
                this.StartPosition = FormStartPosition.CenterScreen;
                this.WindowState = FormWindowState.Normal;

                errorLogClient = new ErrorLogClient();

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
                AppendToChatHistory("Welcome to your chat interface. I am Dansby. How can I assist you?");

                // Run time-consuming initializations asynchronously
                await Task.Run(() => InitializeDansbyCore());
                await Task.Run(() => InitializeIntentRecognizer());
                await Task.Run(() => InitializeResponseGenerator());
                await Task.Run(() => InitializeVaultManager());

                InitializeSlimeTimer();
                InitializeRefreshCacheButton();
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error during initialization: {ex.Message}", "MainForm");
                AppendToChatHistory("Sorry, an error occurred during startup.");
            }
        }

        private void InitializeComponent()
        {
            // UI Controls Initialization
            inputTextBox = new TextBox
            {
                Location = new Point(35, 524),
                Size = new Size(515, 40),
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            sendButton = new Button
            {
                Location = new Point(560, 522),
                Size = new Size(105, 27),
                Text = "Send",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            sendButton.Click += SendButton_Click;

            chatRichTextBox = new RichTextBox
            {
                Location = new Point(35, 50),
                Size = new Size(630, 450),
                ReadOnly = true,
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
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
            playButton.Click += PlayButton_Click;

            pauseButton = new Button
            {
                Location = new Point(435, 10),
                Size = new Size(75, 25),
                Text = "Pause",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            pauseButton.Click += PauseButton_Click;

            // Adding controls to the form
            this.Controls.Add(inputTextBox);
            this.Controls.Add(sendButton);
            this.Controls.Add(chatRichTextBox);
            this.Controls.Add(soundtrackComboBox);
            this.Controls.Add(playButton);
            this.Controls.Add(pauseButton);

            this.ClientSize = new Size(800, 580);
            this.Text = "DansbyChatBot";
            this.BackColor = Color.FromArgb(25, 25, 25);
        }

        private void InitializeDansbyCore()
        {
            dansbyCore = new DansbyCore();
            foreach (var trackName in dansbyCore.GetSoundtrackNames())
            {
                soundtrackComboBox.Items.Add(trackName);
            }
            if (soundtrackComboBox.Items.Count > 0)
            {
                soundtrackComboBox.SelectedIndex = 0;
            }
        }

        private void InitializeIntentRecognizer()
        {
            intentRecognizer = new IntentRecognizer();
            errorLogClient.AppendToDebugLog("IntentRecognizer initialized successfully.", "MainForm");
        }

        private void InitializeResponseGenerator()
        {
            responseGenerator = new ResponseGenerator(errorLogClient);
            errorLogClient.AppendToDebugLog("ResponseGenerator initialized successfully.", "MainForm");
        }

        private void InitializeVaultManager()
        {
            vaultManager = new VaultManager("E:\\Lorehaven\\gitconnect");
            errorLogClient.AppendToDebugLog("VaultManager initialized successfully.", "MainForm");
        }

        private void InitializeSlimeTimer()
        {
            slimeTimer = new Timer { Interval = 60000 };
            slimeTimer.Tick += (s, e) =>
            {
                if (dansbyCore.ShouldSummonSlime())
                {
                    AppendToChatHistory("A wild slime appears!");
                    dansbyCore.SummonSlime(this);
                }
            };
            slimeTimer.Start();
        }

        private void InitializeRefreshCacheButton()
        {
            refreshCacheButton = new Button
            {
                Location = new Point(520, 10),
                Size = new Size(120, 25),
                Text = "Refresh Cache",
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            refreshCacheButton.Click += async (sender, e) => await RefreshVaultCacheAsync();

            this.Controls.Add(refreshCacheButton);
        }

        private async Task RefreshVaultCacheAsync()
        {
            AppendToChatHistory("Dansby: Refreshing vault cache...");

            try
            {
                await vaultManager.RefreshCacheAsync();
                AppendToChatHistory("Dansby: Vault cache refreshed successfully.");
                errorLogClient.AppendToDebugLog("Vault cache refreshed successfully.", "MainForm");
            }
            catch (Exception ex)
            {
                AppendToChatHistory("Dansby: Sorry, an error occurred while refreshing the cache.");
                errorLogClient.AppendToErrorLog($"Error refreshing cache: {ex.Message}", "MainForm");
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputTextBox.Text.Trim();

            if (string.IsNullOrEmpty(userInput))
            {
                AppendToChatHistory("Dansby: Please enter a message.");
                return;
            }

            AppendToChatHistory($"You: {userInput}");
            string recognizedIntent = RecognizeUserIntent(userInput);

            if (recognizedIntent == "searchvault")
            {
                SearchVaultCommand(userInput);
            }
            else
            {
                GenerateAndDisplayResponse(recognizedIntent, userInput);
            }

            inputTextBox.Clear();
        }

        private string RecognizeUserIntent(string userInput)
        {
            try
            {
                string intent = intentRecognizer.RecognizeIntent(userInput);
                errorLogClient.AppendToDebugLog($"Intent recognized: {intent}", "MainForm");
                return intent;
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error recognizing intent: {ex.Message}", "MainForm");
                return "unknown_intent";
            }
        }

        private void GenerateAndDisplayResponse(string intent, string userInput)
        {
            try
            {
                string response = responseGenerator.GenerateResponse(intent, userInput);
                errorLogClient.AppendToDebugLog($"Response generated: {response}", "MainForm");
                AppendToChatHistory($"Dansby: {response}");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error generating response: {ex.Message}", "MainForm");
                AppendToChatHistory("Dansby: Sorry, an error occurred while generating my response.");
            }
        }

        private void SearchVaultCommand(string userInput)
        {
            string keyword = userInput.Replace("search vault for", "", StringComparison.OrdinalIgnoreCase).Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                AppendToChatHistory("Dansby: Please provide a keyword to search.");
                return;
            }

            AppendToChatHistory($"Dansby: Searching vault for \"{keyword}\"...");

            try
            {
                var matchingFiles = vaultManager.SearchVault(keyword);
                if (matchingFiles.Count > 0)
                {
                    AppendToChatHistory($"Dansby: Found {matchingFiles.Count} matching notes:");
                    foreach (var file in matchingFiles)
                    {
                        AppendToChatHistory($"- {Path.GetFileName(file)}");
                    }
                }
                else
                {
                    AppendToChatHistory("Dansby: No matching notes found in the vault.");
                }
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error searching vault: {ex.Message}", "MainForm");
                AppendToChatHistory("Dansby: Sorry, an error occurred while searching the vault.");
            }
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
            if (chatRichTextBox.InvokeRequired)
            {
                chatRichTextBox.Invoke(new Action(() => AppendToChatHistory(message)));
            }
            else
            {
                chatRichTextBox.AppendText(message + Environment.NewLine);
                chatRichTextBox.SelectionStart = chatRichTextBox.Text.Length;
                chatRichTextBox.ScrollToCaret();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            slimeTimer?.Stop();
            dansbyCore.Shutdown();
            base.OnFormClosing(e);
        }
    }
}
