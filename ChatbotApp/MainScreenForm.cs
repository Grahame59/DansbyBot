using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ChatbotApp
{
    public partial class MainScreenForm : Form
    {
        private ProgressBar loadingBar;
        private Button guestLoginButton;
        private Button enterChatButton;
        private PictureBox logoPictureBox;
        private PictureBox github;
        private Timer progressTimer;
        private int progressValue = 0;
        private static bool mainFormInitSuccess = false;
        private static MainForm mainForm;
    
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
            this.BackgroundImage = Image.FromFile("ChatbotApp\\Resources\\MainScreenComponents\\Background10.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Logo PictureBox
            logoPictureBox = new PictureBox
            {
                Location = new Point(150, 40),
                Size = new Size(500, 150),
                Image = Image.FromFile("ChatbotApp\\Resources\\MainScreenComponents\\DansbyLogo2Transparent.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent, // Ensure the logo background is transparent
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(logoPictureBox);

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

            // Loading Bar (ProgressBar)
            loadingBar = new ProgressBar
            {
                Location = new Point(240, 220),
                Size = new Size(300, 20),
                Visible = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(loadingBar);
            loadingBar.BringToFront();

            // GitHub Logo PictureBox
            github = new PictureBox
            {
                Location = new Point(725, 510), // Adjust location as needed
                Size = new Size(40, 40), // Set size based on the logo dimensions
                Image = Image.FromFile("ChatbotApp\\Resources\\Github-Logo.png"), 
                SizeMode = PictureBoxSizeMode.Zoom, // Ensures the image fits the PictureBox
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent, // Ensures no background color
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            github.Click += GitHub_Click; // Attach click event
            this.Controls.Add(github);
            github.BringToFront();
        }

        // GitHub Click Event Handler
        private void GitHub_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/Grahame59", 
                    UseShellExecute = true // Opens in the default browser
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open GitHub: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Guest Login Button Click Handler
        private void GuestLoginButton_Click(object sender, EventArgs e)
        {
            progressValue = 0;
            loadingBar.Value = 0;
            loadingBar.Visible = true;
            loadingBar.Style = ProgressBarStyle.Blocks;
            guestLoginButton.Enabled = false;

            progressTimer = new Timer();
            progressTimer.Interval = 50; // 20 steps over 2 seconds
            progressTimer.Tick += ProgressTimer_Tick;
            progressTimer.Start();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            progressValue += 5;
            if (progressValue >= 100)
            {
                progressTimer.Stop();
                loadingBar.Visible = false;
                guestLoginButton.Enabled = true;

                //Proceed to MainForm after progress is completed
                if (!mainFormInitSuccess)
                {
                    mainForm = new MainForm();
                    mainForm.Show();
                    this.Hide();
                    mainFormInitSuccess = true;
                }
                else 
                {
                    mainForm.Show();
                    this.Hide();
                }
            }
            else 
            {
                loadingBar.Value = progressValue;
            }
        }
    }
}
