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
        private TextBox inputTextBox; // Added inputTextBox
        private Button sendButton;
        private System.Windows.Forms.RichTextBox chatRichTextBox; // Declare the RichTextBox control

        private TextBox chatTextBox;
        private IntentRecognizer intentRecognizer;
        private ResponseRecognizer responseRecognizer;
        private UserManager userManager;
        public bool isUserInputForColor = false; //for chatbox color

        public MainForm()
        {
            InitializeComponent();
            InitializeChatbot();
        }

        private void InitializeComponent()
        {
            this.inputTextBox = new TextBox();
            this.sendButton = new Button();
            this.chatRichTextBox = new RichTextBox(); // Change from TextBox to RichTextBox
            this.AcceptButton = this.sendButton;

            // inputTextBox
            this.inputTextBox.Location = new System.Drawing.Point(12, 415);
            this.inputTextBox.Size = new System.Drawing.Size(400, 20);

            // sendButton
            this.sendButton.Location = new System.Drawing.Point(420, 413);
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.Text = "Send";
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            this.KeyDown += Form_KeyDown;

            // chatRichTextBox
            this.chatRichTextBox.Location = new System.Drawing.Point(12, 12);
            this.chatRichTextBox.Size = new System.Drawing.Size(483, 395);
            this.chatRichTextBox.Multiline = true;
            this.chatRichTextBox.ReadOnly = true;
            this.KeyDown += Form_KeyDown;

            // MainForm
            this.ClientSize = new System.Drawing.Size(507, 450);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatRichTextBox); // Add chatRichTextBox to controls
            this.Text = "DansbyChatBot";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the pressed key is Enter
            if (e.KeyCode == Keys.Enter)
            {
                // Trigger the action same as the click event of the send button
                SendButton_Click(sender, e);
                
                // Optionally, prevent the beep sound when Enter key is pressed
                e.SuppressKeyPress = true;
            }
        }

        private void InitializeChatbot()
        {
            // Create and initialize IntentRecognizer, ResponseRecognizer, & UserManager instance
            intentRecognizer = new IntentRecognizer();
            responseRecognizer = new ResponseRecognizer();
            userManager = new UserManager();

            // Prompt user for login credentials
            string username = PromptForInput("Enter your username:");
            CurInstanceLoginUser = username;
            string password = PromptForInput("Enter your password:");
            CurInstanceLoginPass = password;

            // Attempt to login
            bool loginSuccess = userManager.Login(username, password);
            if (loginSuccess)
            {
                CurInstanceIsAdmin = userManager.IsCurrentUserAdmin();
                AppendToChatHistory($"Logged in as: {username}");
            }
            else
            {
                AppendToChatHistory("Login failed. Logging in as Guest.");
                userManager.Login("guest", "guest");
            }

            AppendToChatHistory("Welcome to your chat interface. I am Dansby also known as Dansby bot. May I assist you?");
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputTextBox.Text;
            isUserInputForColor = true; //sets it to true for user
            AppendToChatHistory("");
            AppendToChatHistory(CurInstanceLoginUser + $": {userInput}");
            AppendToChatHistory("");
            isUserInputForColor = false; //reverts to false for following Dansby Output

            string recognizedIntent = intentRecognizer.RecognizeIntent(userInput);
            AppendToChatHistory($"Intent: {recognizedIntent}");
            AppendToChatHistory("");

            string returnResponse = responseRecognizer.RecognizeResponse(userInput);
            AppendToChatHistory($"DANSBY: {returnResponse}");
            AppendToChatHistory("");

            inputTextBox.Clear();
        }

        public void AppendToChatHistory(string message)
        {   
            
            // Set the color of the text based on whether it's user input or Dansby's response
            if (isUserInputForColor)
            {
                chatRichTextBox.SelectionColor = Color.Blue; // Change to your desired color for user input
            }
            else
            {
                chatRichTextBox.SelectionColor = Color.Red; // Change to your desired color for Dansby's response
            }
            chatRichTextBox.AppendText(message + Environment.NewLine);
        }

        private string PromptForInput(string prompt)
        {
            return Prompt.ShowDialog(prompt);
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
