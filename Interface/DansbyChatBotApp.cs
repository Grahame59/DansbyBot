using System;
using Intents;
using Responses;
using UserAuthentication;
using System.Windows.Forms;
using System.Drawing;

namespace ChatbotApp
{
      public partial class MainForm : Form
    {
        public static string CurInstanceLoginUser;
        public static string CurInstanceLoginPass;
        public static bool CurInstanceIsAdmin;
        private TextBox inputTextBox;
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

            // inputTextBox
            this.inputTextBox.Location = new System.Drawing.Point(12, 520);
            this.inputTextBox.Size = new System.Drawing.Size(600, 20);
            this.inputTextBox.BackColor = System.Drawing.Color.FromArgb(88, 86, 91);
            this.inputTextBox.ForeColor = System.Drawing.Color.White;
            this.inputTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // sendButton
            this.sendButton.Location = new System.Drawing.Point(620, 520);
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.Text = "Send";
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            this.sendButton.BackColor = System.Drawing.Color.FromArgb(88, 86, 91); // background
            this.sendButton.ForeColor = System.Drawing.Color.FromArgb(84, 18, 194); // font color
            this.sendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            // chatRichTextBox
            this.chatRichTextBox.Location = new System.Drawing.Point(12, 12);
            this.chatRichTextBox.Size = new System.Drawing.Size(683, 500);
            this.chatRichTextBox.ReadOnly = true;
            this.chatRichTextBox.BackColor = System.Drawing.Color.FromArgb(15, 15, 15);
            this.chatRichTextBox.ForeColor = System.Drawing.Color.White;
            this.chatRichTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // loggedInUserLabel
            this.loggedInUserLabel.Location = new System.Drawing.Point(12, 550);
            this.loggedInUserLabel.Size = new System.Drawing.Size(683, 20);
            this.loggedInUserLabel.Text = "Logged in as: "; // Initial text
            this.loggedInUserLabel.ForeColor = System.Drawing.Color.White;
            this.loggedInUserLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // MainForm
            this.ClientSize = new System.Drawing.Size(700, 580);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatRichTextBox);
            this.Controls.Add(this.loggedInUserLabel);

            this.BackColor = System.Drawing.Color.FromArgb(84, 18, 194); // Form background color
            this.Text = "DansbyChatBot";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private void InitializeChatbot()
        {
            // Initialize the recognizers and user manager
            intentRecognizer = new IntentRecognizer(this);
            responseRecognizer = new ResponseRecognizer(this);
            userManager = new UserManager();

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
        private void SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputTextBox.Text;
            isUserInputForColor = true;
            AppendToChatHistory($"{CurInstanceLoginUser}: {userInput}");
            isUserInputForColor = false;

            // Pass both userInput and mainform to RecognizeIntent
            string recognizedIntent = intentRecognizer.RecognizeIntent(userInput, this); // Assuming "this" refers to MainForm instance
            AppendToChatHistory($"Intent: {recognizedIntent}");
            AppendToChatHistory("");

            string returnResponse = responseRecognizer.RecognizeResponse(userInput, this);
            AppendToChatHistory($"DANSBY: {returnResponse}");
            AppendToChatHistory("");

            inputTextBox.Clear();   
        }
         public void AppendToChatHistory(string message)
        {
            chatRichTextBox.SelectionColor = isUserInputForColor ? Color.GhostWhite : Color.FromArgb(84, 18, 194);
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

    }

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