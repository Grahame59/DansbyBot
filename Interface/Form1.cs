using System;
using System.Windows.Forms;

namespace ChatbotApp
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.RichTextBox chatRichTextBox; // Declare the RichTextBox control

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.textBoxInput = new TextBox();
            this.textBoxOutput = new TextBox();
            this.buttonSend = new Button();
            this.SuspendLayout();
            
            // textBoxInput
            this.textBoxInput.Location = new System.Drawing.Point(10, 10);
            this.textBoxInput.Size = new System.Drawing.Size(500, 20);

            // textBoxOutput
            this.textBoxOutput.Location = new System.Drawing.Point(10, 40);
            this.textBoxOutput.Size = new System.Drawing.Size(500, 300);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.ReadOnly = true;

            // buttonSend
            this.buttonSend.Location = new System.Drawing.Point(520, 10);
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.Text = "Send";
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);

            this.chatRichTextBox = new System.Windows.Forms.RichTextBox();
            this.chatRichTextBox.Location = new System.Drawing.Point(12, 12);
            this.chatRichTextBox.Size = new System.Drawing.Size(483, 395);
            this.chatRichTextBox.Multiline = true;
            this.chatRichTextBox.ReadOnly = true;
            this.Controls.Add(this.chatRichTextBox); // Add the RichTextBox control to the form's controls
                        

            // Form1
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.buttonSend);
            this.Text = "Chatbot";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string userInput = textBoxInput.Text;
            // Process user input and display bot response in textBoxOutput
            textBoxOutput.AppendText($"You: {userInput}{Environment.NewLine}");
            // Call your chatbot logic to generate a response
            string botResponse = "Bot: Hello, world!";
            textBoxOutput.AppendText($"{botResponse}{Environment.NewLine}");
            textBoxInput.Clear();
        }

        private TextBox textBoxInput;
        private TextBox textBoxOutput;
        private Button buttonSend;
    }
}

