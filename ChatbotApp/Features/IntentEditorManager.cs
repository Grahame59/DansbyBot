using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intents;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

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
        private Label utteranceLabel;
        private Label responseLabel;


        private const string IntentFilePath = "ChatbotApp/NLP_pipeline/intent_mappings.json";
        private const string ResponseFilePath = "ChatbotApp/NLP_pipeline/response_mappings.json";

        public IntentEditorManager(Panel panel)
        {
            intentPanel = panel;
            errorLogger = ErrorLogClient.Instance;
            intentButtons = new Dictionary<string, Button>();
            InitializeUI();
            AttachResizeListener();
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

            // Utterance Label (Above Left Pane)
            utteranceLabel = new Label
            {
                Text = "intent_mappings.json",
                Font = new Font("Consolas", 10, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 25,
                Visible = false
            };

            // Response Label (Above Right Pane)
            responseLabel = new Label
            {
                Text = "response_mappings.json",
                Font = new Font("Consolas", 10, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 25,
                Visible = false
            };

            leftPane.Controls.Add(utteranceLabel);
            rightPane.Controls.Add(responseLabel);
            intentPanel.Controls.Add(intentTitleLabel);
            intentPanel.Controls.Add(returnButton);
            intentPanel.Controls.Add(leftPane);
            intentPanel.Controls.Add(rightPane);
            intentPanel.Controls.Add(gridLayout);

            LoadIntentButtons();
        }

        public async void LoadIntentButtons()
        {
            try
            {
                await errorLogger.AppendToDebugLogAsync("Loading intent buttons...", "IntentEditorManager.cs");

                if (!File.Exists(IntentFilePath))
                {
                    await errorLogger.AppendToErrorLogAsync($"Intent file not found: {IntentFilePath}", "IntentEditorManager.cs");
                    return;
                }

                string json = await File.ReadAllTextAsync(IntentFilePath);
                var intents = JsonConvert.DeserializeObject<List<IntentMapping>>(json);

                if (intents == null || intents.Count == 0)
                {
                    await errorLogger.AppendToErrorLogAsync("No intents found in intent_mappings.json.", "IntentEditorManager.cs");
                    return;
                }

                gridLayout.Controls.Clear();
                intentButtons.Clear();

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

                    intentButton.Click += (s, e) =>
                    {
                        gridLayout.Visible = false;
                        LoadIntentDetails(intent);
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

        private void LoadIntentDetails(IntentMapping intent)
        {

            intentTitleLabel.Text = $"Intent: {intent.Name}";
            intentTitleLabel.Visible = true;
            utteranceLabel.Visible = true;
            responseLabel.Visible = true;
            leftPane.Visible = true;
            rightPane.Visible = true;
            returnButton.Visible = true;

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

            string utteranceJSON = "[\n  " +
                        string.Join(",\n  ", intent.Examples.Select(ex => 
                        $"{{ \"Utterance\": \"{ex.Utterance}\", \"Tokens\": [{string.Join(", ", ex.Tokens.Select(t => $"\"{t}\""))}] }}")) 
                        + "\n]";

            // --- Utterance TextBox ---
            RichTextBox utterancesTextBox = new RichTextBox
            {
                Text =  utteranceJSON,
                Multiline = true,
                Size = new Size(leftPane.Width - 20, leftPane.Height - 20),
                Location = new Point(10, 10),
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                BackColor = Color.FromArgb(88, 86, 91),
                ForeColor = Color.White,
                Font = new Font("Consolas", 8),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right // Resizes properly
            };
            leftPane.Controls.Add(utterancesTextBox);
            ApplyJsonSyntaxHighlighting(utterancesTextBox);

            // --- Response TextBox ---
            string responseJson = File.Exists(ResponseFilePath) ? File.ReadAllText(ResponseFilePath) : "{}";
            var responsesDict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(responseJson) ?? new Dictionary<string, List<string>>();
            List<string> responseList = responsesDict.ContainsKey(intent.Name) ? responsesDict[intent.Name] : new List<string> { "No response available" };
            string responseFormattedJson = JsonConvert.SerializeObject(responseList, Formatting.Indented);

            RichTextBox responsesTextBox = new RichTextBox
            {
                Text = responseFormattedJson,
                Multiline = true,
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
            ApplyJsonSyntaxHighlighting(responsesTextBox);
        }

        private void ShowIntentList()
        {
            leftPane.Visible = false;
            rightPane.Visible = false;
            returnButton.Visible = false;
            gridLayout.Visible = true;
            intentTitleLabel.Visible = false;
            utteranceLabel.Visible = false;
            responseLabel.Visible = false;
        }
        
        private void ApplyJsonSyntaxHighlighting(RichTextBox textBox)
        {
            textBox.SuspendLayout(); // Pause UI updates
            textBox.SelectAll();
            textBox.SelectionColor = Color.LightGray; // Default color
            textBox.DeselectAll();

            string jsonText = textBox.Text;

            // Regex for keys (Orange)
            var regexKey = new Regex(@"""(\w+)""(?=\s*:)"); 
            // Regex for string values (Light Yellow)
            var regexString = new Regex(@"""([^""\\]*(?:\\.[^""\\]*)*)"""); 

            // Apply Orange to JSON Keys
            foreach (Match match in regexKey.Matches(jsonText))
            {
                textBox.Select(match.Index, match.Length);
                textBox.SelectionColor = Color.Orange;
            }

            // Apply Light Yellow to String Values
            foreach (Match match in regexString.Matches(jsonText))
            {
                textBox.Select(match.Index, match.Length);
                textBox.SelectionColor = Color.LightGoldenrodYellow;
            }

            textBox.SelectionStart = textBox.Text.Length; // Move cursor to end
            textBox.SelectionColor = Color.White; // Reset to default for new text
            textBox.ResumeLayout(); // Resume UI updates
        }



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
