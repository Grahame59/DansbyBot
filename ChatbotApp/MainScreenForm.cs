using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatbotApp.UserData;

namespace ChatbotApp
{
    public partial class MainScreenForm : Form
    {
        private ProgressBar loadingBar;
        private TextBox usernameBox;
        private TextBox passwordBox;
        private Button loginButton;
        private Button guestLoginButton;
        private Button enterChatButton;
        private PictureBox logoPictureBox;
        private readonly UserManager userManager = new UserManager();
    
        public MainScreenForm()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            // Form Properties
            this.Text = "Dansby Login";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // Background Image
            this.BackgroundImage = Image.FromFile("ChatbotApp\\Resources\\MainScreenComponents\\Background5.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Logo PictureBox
            logoPictureBox = new PictureBox
            {
                Location = new Point(150, 40),
                Size = new Size(500, 150),
                Image = Image.FromFile("ChatbotApp\\Resources\\MainScreenComponents\\DansbyLogo1Transparent.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent, // Ensure the logo background is transparent
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(logoPictureBox);

            // Username TextBox
            usernameBox = new TextBox
            {
                Location = new Point(240, 150),
                Size = new Size(300, 35),
                PlaceholderText = "Enter Username",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(usernameBox);
            usernameBox.BringToFront();

            // Password TextBox
            passwordBox = new TextBox
            {
                Location = new Point(240, 180),
                Size = new Size(300, 35),
                UseSystemPasswordChar = true,
                PlaceholderText = "Enter Password",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(passwordBox);
            passwordBox.BringToFront();

            // Login Button
            loginButton = new Button
            {
                Location = new Point(240, 220),
                Size = new Size(300, 35),
                Text = "Login",
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            loginButton.Click += async (sender, e) => await LoginButton_Click(usernameBox.Text, passwordBox.Text);
            this.Controls.Add(loginButton);
            loginButton.BringToFront();

            // Guest Login Button
            guestLoginButton = new Button
            {
                Location = new Point(240, 260),
                Size = new Size(300, 35),
                Text = "Continue as Guest",
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            guestLoginButton.Click += GuestLoginButton_Click;
            this.Controls.Add(guestLoginButton);
            guestLoginButton.BringToFront();

            // Enter Chat Button (Hidden initially)
            enterChatButton = new Button
            {
                Location = new Point(240, 300),
                Size = new Size(300, 35),
                Text = "Enter Chat",
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            enterChatButton.Click += EnterChatButton_Click;
            this.Controls.Add(enterChatButton);
            enterChatButton.BringToFront();

            // Loading Bar (ProgressBar)
            loadingBar = new ProgressBar
            {
                Location = new Point(150, 350),
                Size = new Size(300, 20),
                Visible = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(loadingBar);
            loadingBar.BringToFront();
        }


        // Login Button Click Handler
        private async Task LoginButton_Click(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error");
                return;
            }

            loadingBar.Visible = true;

            // Simulate loading with a delay
            for (int i = 0; i <= 100; i += 10)
            {
                loadingBar.Value = i;
                await Task.Delay(100);
            }

            // Validate credentials
            if (await userManager.ValidateUserAsync(username, password))
            {
                //MessageBox.Show("Login Successful!", "Welcome");
                loadingBar.Visible = false;
                enterChatButton.Visible = true;
            }
            else
            {
                loadingBar.Visible = false;
                MessageBox.Show("Invalid credentials. Please try again.", "Error");
            }
        }

        // Guest Login Button Click Handler
        private void GuestLoginButton_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Continuing as Guest...", "Guest Login");
            enterChatButton.Visible = true;
        }

        // Enter Chat Button Click Handler
        private void EnterChatButton_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Hide();
        }
    }
}
