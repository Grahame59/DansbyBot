using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatbotApp.Core;

namespace ChatbotApp.Features
{
    public class IntentEditorManager
    {
        private Panel intentPanel;
        private Button returnButton;
        private Dictionary<string, Button> intentButtons;
        private Panel leftPane, rightPane;
        private readonly ErrorLogClient errorLogger;
        private FlowLayoutPanel gridLayout;
        private Label intentTitleLabel;
        private readonly JsonFileHandler jsonFileHandler;
        private IntentMapping currentIntent;
        private RichTextBox utterancesTextBox, responsesTextBox;
        private System.Timers.Timer autoSaveTimer;
        private readonly DansbyCore dansbyCore;


        public IntentEditorManager(Panel panel, DansbyCore dansbyCore)
        {
            this.dansbyCore = dansbyCore; // Store the DansbyCore instance
            intentPanel = panel;
            errorLogger = ErrorLogClient.Instance;
            jsonFileHandler = new JsonFileHandler();
            intentButtons = new Dictionary<string, Button>();

            InitializeUI();
            AttachResizeListener();

            // Initialize AutoSave Timer
            autoSaveTimer = new System.Timers.Timer(3500);
            autoSaveTimer.Elapsed += AutoSaveTriggered;
            autoSaveTimer.AutoReset = false; // Prevent repeated saves
            autoSaveTimer.Enabled = true;
        }

        public void InitializeUI()
        {
            intentPanel.Controls.Clear();
            intentPanel.Dock = DockStyle.Fill;

            // Grid layout for intent buttons
            gridLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(25, 25, 25),
                Padding = new Padding(10),
                Margin = new Padding(10)
            };

            // Back button - Always visible
            returnButton = new Button
            {
                Text = "⬅ Back",
                Size = new Size(90, 30),
                Location = new Point(10,10),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(88, 86, 91),
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            returnButton.Click += (s, e) => ShowIntentList();

            // Left Pane (Utterances)
            leftPane = new Panel
            {
                BackColor = Color.FromArgb(88, 86, 91),
                Padding = new Padding(10),
                Visible = false
            };

            // Right Pane (Responses)
            rightPane = new Panel
            {
                BackColor = Color.FromArgb(88, 86, 91),
                Padding = new Padding(10),
                Visible = false
            };

            // Centered Intent Label
            intentTitleLabel = new Label
            {
                Text = "",  // Initially empty, updated on intent selection
                Font = new Font("Consolas", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(25, 25, 25), // Match background
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40,
                Visible = false
            };

            intentPanel.Controls.Add(intentTitleLabel);
            intentPanel.Controls.Add(returnButton);
            intentPanel.Controls.Add(leftPane);
            intentPanel.Controls.Add(rightPane);
            intentPanel.Controls.Add(gridLayout);

            _= Task.Run(() => LoadIntentButtonsAsync());
        }

        public async Task LoadIntentButtonsAsync()
        {
            try
            {
                await errorLogger.AppendToDebugLogAsync("Loading intent buttons...", "IntentEditorManager.cs");

                // Loads the Intents from the (Utterances Not responses yet) 
                var intents = await jsonFileHandler.LoadIntentsAsync();

                gridLayout.Controls.Clear();
                intentButtons.Clear();

                // Populates the buttons with the intents and their names/info
                foreach (var intent in intents)
                {
                    Button intentButton = new Button
                    {
                        Text = intent.Name,
                        Size = new Size(150, 40),
                        BackColor = Color.FromArgb(88, 86, 91),
                        ForeColor = Color.MediumPurple,
                        Font = new Font("Consolas", 15, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Margin = new Padding(10)
                    };

                    intentButton.Click += async (s, e) =>
                    {
                        gridLayout.Visible = false;
                        await LoadIntentDetails(intent);
                    };

                    intentButtons[intent.Name] = intentButton;
                    gridLayout.Controls.Add(intentButton);
                }
            }
            catch (Exception ex)
            {
                await errorLogger.AppendToErrorLogAsync($"Error loading intents: {ex.Message}", "IntentEditorManager.cs");
            }
        }

        private async Task LoadIntentDetails(IntentMapping intent)
        {
            currentIntent = intent; // Store the currently selected intent

            // Updates the UI header for the selected Intent Details
            intentTitleLabel.Text = $"Intent: {intent.Name}";
            intentTitleLabel.Visible = true;
            leftPane.Visible = true;
            rightPane.Visible = true;
            returnButton.Visible = true;

            // Clear Previous Intent Details
            leftPane.Controls.Clear();
            rightPane.Controls.Clear();

            int panelWidth = intentPanel.Width;
            int panelHeight = intentPanel.Height;

            // Manually set sizes
            int paneWidth = (int)(panelWidth * 0.45);
            int paneHeight = panelHeight - 50; // Leave space for the back button

            // Position and size left pane (utterances)
            leftPane.Size = new Size(paneWidth, paneHeight);
            leftPane.Location = new Point(10, 50); // 10px margin, below the back button
            leftPane.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left; // Resizes properly

            // Position and size right pane (responses)
            rightPane.Size = new Size(paneWidth, paneHeight);
            rightPane.Location = new Point(panelWidth - paneWidth - 10, 50); // Positioned on the right
            rightPane.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right; // Resizes properly

            returnButton.BringToFront();  // Ensure it's always visible

            string utteranceJSON = JsonSerializer.Serialize(intent.Examples, new JsonSerializerOptions { WriteIndented = true });

            // --- Utterance TextBox ---
            utterancesTextBox = new RichTextBox
            {
                Text =  utteranceJSON,
                Multiline = true,
                AcceptsTab = true,
                WordWrap = true,
                Size = new Size(leftPane.Width - 20, leftPane.Height - 20),
                Location = new Point(10, 10),
                ScrollBars = RichTextBoxScrollBars.Both,
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Font = new Font("Consolas", 8),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right // Resizes properly
            };
            leftPane.Controls.Add(utterancesTextBox);

            var responses = await jsonFileHandler.LoadResponsesAsync();
            responses.TryGetValue(intent.Name, out var responseList);
            string responseJSON = JsonSerializer.Serialize(responseList ?? new List<string>(), new JsonSerializerOptions { WriteIndented = true });
            
            // --- Response TextBox ---
            responsesTextBox = new RichTextBox
            {
                Text = responseJSON,
                Multiline = true,
                AcceptsTab = true,
                WordWrap = true,
                Size = new Size(rightPane.Width - 20, rightPane.Height - 20),
                Location = new Point(10, 10),
                ScrollBars = RichTextBoxScrollBars.Both,
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Font = new Font("Consolas", 8),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right // Resizes properly
            };
            rightPane.Controls.Add(responsesTextBox);

            // Decode any escaped characters before displaying the JSON
            utterancesTextBox.Text = System.Web.HttpUtility.HtmlDecode(utteranceJSON);
            responsesTextBox.Text = System.Web.HttpUtility.HtmlDecode(responseJSON);

            ApplyJsonSyntaxHighlighting(responsesTextBox);
            ApplyJsonSyntaxHighlighting(utterancesTextBox);

            // Attach autosave listener
            utterancesTextBox.TextChanged += HandleJsonEdit;
            responsesTextBox.TextChanged += HandleJsonEdit;
        }

        private void ShowIntentList()
        {
            leftPane.Visible = false;
            rightPane.Visible = false;
            returnButton.Visible = false;
            gridLayout.Visible = true;
            intentTitleLabel.Visible = false;
        }
        
        // Edits the Utterance/Responses Examples and makes them colorful similiar to a JSON file format
        private void ApplyJsonSyntaxHighlighting(RichTextBox textBox)
        {
            // 🔹 Preserve cursor position before making changes
            int cursorPosition = textBox.SelectionStart;

            // 🔹 Suspend layout updates to avoid flickering
            textBox.SuspendLayout();
            textBox.SelectAll();
            textBox.SelectionColor = Color.White; // Reset to default
            textBox.DeselectAll();

            string jsonText = textBox.Text;

            // 🔹 Define Regular Expressions for JSON elements
            var regexKey = new Regex(@"""(.*?)""(?=\s*:)", RegexOptions.Compiled); // Keys (Orange)
            var regexString = new Regex(@":\s*""(.*?)""", RegexOptions.Compiled); // String values (Yellow)
            var regexNumber = new Regex(@":\s*([\d\.\-]+)", RegexOptions.Compiled); // Numbers (Light Blue)
            var regexBool = new Regex(@":\s*(true|false)", RegexOptions.Compiled | RegexOptions.IgnoreCase); // Boolean values (Green)
            var regexNull = new Regex(@":\s*null", RegexOptions.Compiled | RegexOptions.IgnoreCase); // Null values (Gray)
            
            // 🔹 Apply Colors to JSON elements
            ApplyColoring(textBox, regexKey, Color.Orange); // Keys
            ApplyColoring(textBox, regexString, Color.LightGoldenrodYellow); // String Values
            ApplyColoring(textBox, regexNumber, Color.LightSkyBlue); // Numbers
            ApplyColoring(textBox, regexBool, Color.LightGreen); // Booleans
            ApplyColoring(textBox, regexNull, Color.Gray); // Null values

            // 🔹 Restore cursor position
            textBox.SelectionStart = cursorPosition;
            textBox.SelectionLength = 0;
            textBox.SelectionColor = Color.White; // Reset color
            textBox.ResumeLayout(); // Resume UI updates
        }

        // Helper function to apply regex-based syntax coloring
        private void ApplyColoring(RichTextBox textBox, Regex regex, Color color)
        {
            foreach (Match match in regex.Matches(textBox.Text))
            {
                textBox.Select(match.Index, match.Length);
                textBox.SelectionColor = color;
            }
        }

        private async Task SaveCurrentIntentChanges()
        {
            if (currentIntent == null) return; // No intent is currently loaded

            try
            {
                // Extract updated utterances from the text box
                string updatedUtterancesJson = utterancesTextBox.Text.Trim();
                var updatedExamples = JsonSerializer.Deserialize<List<ExampleUtterance>>(updatedUtterancesJson)
                                        ?? new List<ExampleUtterance>();

                // Extract updated responses from the text box
                string updatedResponsesJson = responsesTextBox.Text.Trim();
                var updatedResponses = JsonSerializer.Deserialize<List<string>>(updatedResponsesJson)
                                        ?? new List<string>();

                // Update IntentMappings.json
                var allIntents = await jsonFileHandler.LoadIntentsAsync();
                var intentToUpdate = allIntents.FirstOrDefault(i => i.Name == currentIntent.Name);
                if (intentToUpdate != null)
                {
                    intentToUpdate.Examples = updatedExamples;
                    await jsonFileHandler.SaveIntentsAsync(allIntents);
                }

                // Update ResponseMappings.json
                var allResponses = await jsonFileHandler.LoadResponsesAsync();
                if (updatedResponses.Count > 0)
                {
                    allResponses[currentIntent.Name] = updatedResponses;
                    await jsonFileHandler.SaveResponsesAsync(allResponses);
                }

                // Reload new data into DansbyCore after saving!
                await dansbyCore.ReloadIntentAndResponseDataAsync();
                await errorLogger.AppendToDebugLogAsync($"✅ Saved and reloaded changes for intent: {currentIntent.Name}", "IntentEditorManager.cs");
            }
            catch (Exception ex)
            {
                await errorLogger.AppendToErrorLogAsync($"❌ Error saving intent changes: {ex.Message}", "IntentEditorManager.cs");
            }
        }

        private void HandleJsonEdit(object sender, EventArgs e)
        {
            if (autoSaveTimer != null)
            {
                autoSaveTimer.Stop();
                autoSaveTimer.Start();
            }
        }

        private async void AutoSaveTriggered(object sender, System.Timers.ElapsedEventArgs e)
        {
            autoSaveTimer.Stop();
            await SaveCurrentIntentChanges();
        }

        // Makes the form readjust to user resizing 
        private void AttachResizeListener()
        {
            intentPanel.Resize += (s, e) =>
            {
                if (leftPane.Visible && rightPane.Visible)
                {
                    int panelWidth = intentPanel.Width;
                    int panelHeight = intentPanel.Height;

                    int paneWidth = (int)(panelWidth * 0.45); // 45% width
                    int paneHeight = panelHeight - 50; // Keep room for the Back button

                    // Apply new size and positioning
                    leftPane.Size = new Size(paneWidth, paneHeight);
                    rightPane.Size = new Size(paneWidth, paneHeight);

                    leftPane.Location = new Point(10, 50);
                    rightPane.Location = new Point(panelWidth - paneWidth - 10, 50);
                }
            };
        }

    }
}
