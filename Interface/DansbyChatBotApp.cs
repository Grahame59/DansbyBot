using System;
using Intents;
using Responses;
using UserAuthentication;
using System.Windows.Forms;
using System.Drawing;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

//Color.FromArgb(84, 18, 194); // Set border color to purple (WANTED THEME OF PURPLE)

namespace ChatbotApp
{
    public partial class MainForm : Form
    {

        // Add a member variable to track the state of the toggle switch
        private bool useGPTMode = false; 
        private CheckBox toggleSwitch; // Declare toggle switch at the class level
        public static string CurInstanceLoginUser;
        public static string CurInstanceLoginPass;
        public static bool CurInstanceIsAdmin;
        private TextBox inputTextBox;
        private Label toggleLabel;
        private Label loggedInUserLabel;
        private Button sendButton;
        private RichTextBox chatRichTextBox;
        private IntentRecognizer intentRecognizer;
        private ResponseRecognizer responseRecognizer;
        private UserManager userManager;
        public bool isUserInputForColor = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeChatbot();
        }

        private void InitializeComponent()
        {
            this.inputTextBox = new TextBox();
            this.sendButton = new Button();
            this.chatRichTextBox = new RichTextBox();
            this.AcceptButton = this.sendButton;
            this.loggedInUserLabel = new Label();
            this.toggleLabel = new Label();
            this.toggleSwitch = new CircleCheckBox();
            

            // inputTextBox
            this.inputTextBox.Location = new System.Drawing.Point(50, 524); // Moved to the bottom left
            this.inputTextBox.Size = new System.Drawing.Size(500, 40); // Adjusted size
            this.inputTextBox.BackColor = Color.FromArgb(88, 86, 91);//dark gray color
            this.inputTextBox.ForeColor = Color.White;
            this.inputTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; // Anchored to the bottom, right and left

            // sendButton
            this.sendButton.Location = new System.Drawing.Point(560, 522); // Adjusted position
            this.sendButton.Size = new System.Drawing.Size(100, 27); // Adjusted size
            this.sendButton.Text = "Send";
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            this.sendButton.BackColor = Color.FromArgb(88, 86, 91);//dark gray color
            this.sendButton.ForeColor = Color.BlueViolet;
            this.sendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right; // Anchored to the bottom and right

            // chatRichTextBox
            this.chatRichTextBox.Location = new System.Drawing.Point(35, 50); // Adjusted position
            this.chatRichTextBox.Size = new System.Drawing.Size(630, 450); // Adjusted size
            this.chatRichTextBox.ReadOnly = true;
            this.chatRichTextBox.BackColor = Color.FromArgb(50,50,50);
            this.chatRichTextBox.ForeColor = Color.White;
            this.chatRichTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // loggedInUserLabel
            this.loggedInUserLabel.Location = new System.Drawing.Point(50, 25); // Moved to the top left
            this.loggedInUserLabel.Size = new System.Drawing.Size(250, 20); // Adjusted size
            this.loggedInUserLabel.Text = "Logged in as: ";
            this.loggedInUserLabel.ForeColor = Color.White;
            this.loggedInUserLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left; // Anchored to the top and left

            // ToggleSwitch
            this.toggleSwitch.Location = new Point(620, 20); // Adjusted position
            this.toggleSwitch.BackColor = Color.FromArgb(25,25,25);
            this.toggleSwitch.Size = new Size(25, 25); // Increased size for circular shape
            this.toggleSwitch.Appearance = Appearance.Button; // Use button appearance
            this.toggleSwitch.FlatStyle = FlatStyle.Flat; // Use flat style for custom appearance
            this.toggleSwitch.FlatAppearance.BorderColor = Color.FromArgb(25,25,25); // Set border color to purple
            this.toggleSwitch.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Anchor to the top and right
            this.toggleSwitch.CheckedChanged += new EventHandler(ToggleSwitch_CheckedChanged);

            // toggleLabel
            this.toggleLabel = new Label();
            this.toggleLabel.Location = new Point(550, 25); // Adjusted position
            this.toggleLabel.Size = new Size(100, 20); // Adjusted size
            this.toggleLabel.Text = "GPT Mode:"; // Set the text for the toggle button description
            this.toggleLabel.ForeColor = Color.White; // Set the text color
            this.toggleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Anchor to the top and right

            // MainForm
            this.ClientSize = new System.Drawing.Size(700, 580);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatRichTextBox);
            this.Controls.Add(this.loggedInUserLabel);
            this.Controls.Add(this.toggleSwitch); // Added the toggle switch
            this.Controls.Add(this.toggleLabel); // Added the label 

            this.BackColor = Color.FromArgb(25,25,25);
            this.Text = "DansbyChatBot";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private void InitializeChatbot()
        {
            // Initialize the recognizers and user manager
            intentRecognizer = new IntentRecognizer(this);
            responseRecognizer = new ResponseRecognizer(this);
            userManager = new UserManager(this);

            // Prompt user for login credentials
            CurInstanceLoginUser = SaveResponse("Enter your username:");
            CurInstanceLoginPass = SaveResponse("Enter your password:");

            // Attempt to login
            bool loginSuccess = userManager.Login(CurInstanceLoginUser, CurInstanceLoginPass);
            if (loginSuccess)
            {
                CurInstanceIsAdmin = userManager.IsCurrentUserAdmin();
                loggedInUserLabel.Text = $"Logged in as: {CurInstanceLoginUser}"; // Update the label
            }
            else
            {
                //AppendToChatHistory("Login failed. Logging in as Guest.");
                userManager.Login("guest", "guest");
                loggedInUserLabel.Text = "Logged in as: Guest"; // Update the label
            }

            AppendToChatHistory("Welcome to your chat interface. I am Dansby also known as Dansby bot. May I assist you?");
        }        
        private async void SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputTextBox.Text;
            isUserInputForColor = true;
            AppendToChatHistory($"{CurInstanceLoginUser}: {userInput}");
            isUserInputForColor = false;

            if (!useGPTMode)
            {
                // Process user input using intent and response recognizers
                string recognizedIntent = intentRecognizer.RecognizeIntent(userInput, this);
                AppendToChatHistory($"Intent: {recognizedIntent}");
                AppendToChatHistory("");

                string returnResponse = responseRecognizer.RecognizeResponse(userInput, this);
                AppendToChatHistory($"DANSBY: {returnResponse}");
                AppendToChatHistory("");
            }
            else
            {
                // Send user input to GPT API and receive response
                string gptResponse = await SendToGPTAPI(userInput);

                // Display GPT-generated response
                AppendToChatHistory($"DansbyGPT: {gptResponse}");
                AppendToChatHistory("");
            }

            inputTextBox.Clear();
        }

        private async Task<string> SendToGPTAPI(string userInput)
        {
            // Read the API key from the configuration file
            string apiKey;
            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                apiKey = configuration.GetSection("OpenAI")["ApiKey"];
            }
            catch (Exception ex)
            {
                return $"Error reading API key from configuration: {ex.Message}";
                
            }
            // Define API endpoint URL
            string gptApiUrl = "https://api.openai.com/v1/chat/completions";

            // Prepare request data
            var requestData = new { text = userInput };
            var jsonData = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Send request to GPT API
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(gptApiUrl, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
        

        public void AppendToChatHistory(string message)
        {
            chatRichTextBox.SelectionColor = isUserInputForColor ? Color.BlueViolet : Color.WhiteSmoke;
            chatRichTextBox.AppendText(message + Environment.NewLine);

            // Scroll to the bottom
            chatRichTextBox.SelectionStart = chatRichTextBox.Text.Length;
            chatRichTextBox.ScrollToCaret();
        }

        public string SaveResponse(string prompt)
        {
            //AppendToChatHistory(prompt);
            string userInput = Prompt.ShowDialog(prompt);
            return userInput;
        }
        // Toggle Switch CheckedChanged event handler
        private void ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Update the state of the toggle switch
            useGPTMode = toggleSwitch.Checked;

            // Change the color of the circle
            toggleSwitch.ForeColor = useGPTMode ? Color.Green : Color.Red;
        }

        public class CircleCheckBox : CheckBox
        {
            private bool useGPTMode = false;

           public CircleCheckBox()
            {
                
                this.Size = new Size(20, 20); // Set the size for circular shape
                this.Appearance = Appearance.Button; // Use button appearance
                this.FlatStyle = FlatStyle.Flat; // Use flat style for custom appearance
                this.BackColor = Color.FromArgb(25,25,25); // Set background color to transparent
                
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                
                base.OnPaint(e);

                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int diameter = Math.Min(this.Width, this.Height);
                Rectangle rect = new Rectangle(0, 0, diameter, diameter);

                // Fill the background with the desired color
                if (this.Checked)
                    g.FillEllipse(new SolidBrush(Color.Green), rect); // Green color when checked
                else
                    g.FillEllipse(new SolidBrush(Color.Red), rect); // Purple color when unchecked

                // Draw the circle outline with a thick border
                int borderWidth = 2; // Adjust the thickness as needed
                using (Pen borderPen = new Pen(Color.Black, borderWidth))
                {
                    g.DrawEllipse(borderPen, rect);
                }

            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);

                // Toggle state
                this.useGPTMode = !this.useGPTMode;

                // Redraw control
                this.Invalidate();
            }
        }


    } //end of MainForm

    public static class Prompt
    {
        public static string ShowDialog(string text)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = text,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}