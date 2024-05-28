using System;
using System.Windows.Forms;

namespace ChatbotApp
{
    public class MainForm : Form
    {
        private TextBox? inputTextBox;
        private Button? sendButton;
        private TextBox? chatTextBox;


        
        public MainForm()
        {
            InitializeComponent();
        }

        

        private void InitializeComponent()
        {
            this.inputTextBox = new TextBox();
            this.sendButton = new Button();
            this.chatTextBox = new TextBox();

            // inputTextBox
            this.inputTextBox.Location = new System.Drawing.Point(12, 415);
            this.inputTextBox.Size = new System.Drawing.Size(400, 20);

            // sendButton
            this.sendButton.Location = new System.Drawing.Point(420, 413);
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.Text = "Send";
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);

            // chatTextBox
            this.chatTextBox.Location = new System.Drawing.Point(12, 12);
            this.chatTextBox.Size = new System.Drawing.Size(483, 395);
            this.chatTextBox.Multiline = true;
            this.chatTextBox.ReadOnly = true;

            // MainForm
            this.ClientSize = new System.Drawing.Size(507, 450);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatTextBox);
            this.Text = "Chatbot";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputTextBox.Text;
            
            // Append the user input to the chat history TextBox
            chatTextBox.AppendText("You said: " + userInput + Environment.NewLine);

            // Clear the input TextBox
            inputTextBox.Clear();
        }


        private string ProcessUserInput(string input)
        {
            return "You said: " + input;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

