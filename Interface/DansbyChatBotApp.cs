using System;
using Intents;
using Responses;
using UserAuthentication;
using System.Windows.Forms;
using System.Drawing;
using ChatbotApp.Interface.MinecraftBook;
using NAudio.Wave;
using System.Collections.Generic;
using System.IO;




//If I want my form to be borderless style (FormBorderStyle = None). 
//This style removes the default window frame and control box (minimize, maximize, close buttons).

//Color.FromArgb(84, 18, 194); // Set border color to purple (WANTED THEME OF PURPLE)

namespace ChatbotApp
{
    public partial class MainForm : Form
    {

        // Add a member variable to track the state of the toggle switch
        private bool sideBarStatus = false; 
        private CheckBox toggleSwitch; // Declare toggle switch at the class level
        private Panel sidebar; // Declare sidebar at the class level
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

        //All Audio Variables

        private WaveOutEvent outputDevice; //audio playback
        private AudioFileReader audioFile; //read audio files
        private bool isMuted = false; // mute/unmute state
        private List<string> soundtracks; //List of soundtrack file paths
        private ComboBox soundtrackComboBox; // Dropdown for soundtracks
        private Button playButton; // Play/Stop button
        private Button muteButton; // Mute/Unmute button
        public string SoundTrackPathGlobal = null;

        //All Sprite Variables

        private PictureBox spritePictureBox;
        private Timer animationTimer;
        private int spriteX, spriteY;
        private string currentAnimation;
        private Random random = new Random();
        private Timer jumpTimer;
        private bool isJumping;
        private int jumpHeight = 20; // Adjust the height of the jump
        private Timer SlimeTimer;
        public int SlimeCount = 0;



        public MainForm()
        {
            InitializeComponent();
            InitializeChatbot();
            LoadSoundtracks();

            //Initalize Timer    
            SlimeTimer = new Timer();
            SlimeTimer.Interval = 1000; // 1 second
            SlimeTimer.Tick += Timer_Tick;
            SlimeTimer.Start();

           
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
            this.inputTextBox.Location = new System.Drawing.Point(35, 524); // Moved to the bottom left
            this.inputTextBox.Size = new System.Drawing.Size(515, 40); // Adjusted size
            this.inputTextBox.BackColor = Color.FromArgb(88, 86, 91);//dark gray color
            this.inputTextBox.ForeColor = Color.White;
            this.inputTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; // Anchored to the bottom, right and left

            // sendButton
            this.sendButton.Location = new System.Drawing.Point(560, 522); // Adjusted position
            this.sendButton.Size = new System.Drawing.Size(105, 27); // Adjusted size
            this.sendButton.Text = "Send";
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            this.sendButton.BackColor = Color.FromArgb(88, 86, 91);//dark gray color
            this.sendButton.ForeColor = Color.White;
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
            this.loggedInUserLabel.Size = new System.Drawing.Size(150, 20); // Adjusted size
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
            this.toggleLabel.Location = new Point(530, 25); // Adjusted position
            this.toggleLabel.Size = new Size(100, 20); // Adjusted size
            this.toggleLabel.Text = "Open SideBar:"; // Set the text for the toggle button description
            this.toggleLabel.ForeColor = Color.White; // Set the text color
            this.toggleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Anchor to the top and right

            // Sidebar
            sidebar = new Panel(); // assign class level field
            sidebar.Size = new Size(200, 580);
            sidebar.Location = new Point(-200, 0); // Start off-screen
            sidebar.BackColor = Color.FromArgb(30, 30, 30);
            sidebar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(sidebar);

            // Add buttons to the sidebar
            Button appButton1 = new Button();
            appButton1.Text = "MonteCarloSIM";
            appButton1.Size = new Size(180, 40);
            appButton1.Location = new Point(10, 20);
            appButton1.BackColor = Color.FromArgb(50, 50, 50);
            appButton1.ForeColor = Color.White;
            appButton1.Click += AppButton1_Click;
            sidebar.Controls.Add(appButton1);

            Button appButton2 = new Button();
            appButton2.Text = "McBookForm(WIP)";
            appButton2.Size = new Size(180, 40);
            appButton2.Location = new Point(10, 70);
            appButton2.BackColor = Color.FromArgb(50, 50, 50);
            appButton2.ForeColor = Color.White;
            appButton2.Click += AppButton2_Click;
            sidebar.Controls.Add(appButton2);

            // Add more buttons as needed

            // Soundtrack ComboBox
            this.soundtrackComboBox = new ComboBox();
            if (this.soundtrackComboBox == null) throw new Exception("soundtrackComboBox not initialized");
            this.soundtrackComboBox.Location = new Point(680,130);
            this.soundtrackComboBox.Size = new Size(100, 20);
            this.soundtrackComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.soundtrackComboBox.SelectedIndexChanged += SoundtrackComboBox_SelectedIndexChanged;
            this.soundtrackComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Anchor to the top and right
            

            // Load soundtracks into ComboBox
            LoadSoundtracks();
            if (soundtracks == null)
            {
                throw new Exception("Soundtracks list is null.");
            }
            foreach (var track in soundtracks)
            {
                if (track == null)
                {
                    throw new Exception("A track in the soundtracks list is null.");
                }
                this.soundtrackComboBox.Items.Add(Path.GetFileName(track));
            }
            if (soundtrackComboBox.Items.Count > 0)
            {
                soundtrackComboBox.SelectedIndex = 0; // Select the first track
            }
            // Play Button
            this.playButton = new Button();
            this.playButton.Location = new Point(680, 50);
            this.playButton.Size = new Size(100, 27);
            this.playButton.Text = "Play";
            this.playButton.Click += new EventHandler(PlayButton_Click);
            this.playButton.BackColor = Color.FromArgb(88, 86, 91);
            this.playButton.ForeColor = Color.White;
            this.playButton.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Anchor to the top and right

            // Mute Button
            this.muteButton = new Button();
            this.muteButton.Location = new Point(680, 90);
            this.muteButton.Size = new Size(100, 27);
            this.muteButton.Text = "Mute";
            this.muteButton.Click += new EventHandler(MuteButton_Click);
            this.muteButton.BackColor = Color.FromArgb(88, 86, 91);
            this.muteButton.ForeColor = Color.White;
            this.muteButton.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Anchor to the top and right


            // MainForm
            this.ClientSize = new System.Drawing.Size(800, 580);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatRichTextBox);
            this.Controls.Add(this.loggedInUserLabel);
            this.Controls.Add(this.toggleSwitch); // Added the toggle switch
            this.Controls.Add(this.toggleLabel); // Added the label 
            this.Controls.Add(this.soundtrackComboBox);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.muteButton);

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
        private void SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputTextBox.Text;
            isUserInputForColor = true;
            AppendToChatHistory($"{CurInstanceLoginUser}: {userInput}");
            isUserInputForColor = false;

           
            // Process user input using intent and response recognizers
            string recognizedIntent = intentRecognizer.RecognizeIntent(userInput, this);
            AppendToChatHistory($"Intent: {recognizedIntent}");
            AppendToChatHistory("");

            string returnResponse = responseRecognizer.RecognizeResponse(userInput, this);
            AppendToChatHistory($"DANSBY: {returnResponse}");
            AppendToChatHistory("");
           
            inputTextBox.Clear();
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
            sideBarStatus = toggleSwitch.Checked;

            // Change the color of the circle
            toggleSwitch.ForeColor = sideBarStatus ? Color.Green : Color.Red;
            ToggleSidebarVisibility(sideBarStatus);
        }

        // Sidebar visibility logic
        private void ToggleSidebarVisibility(bool isVisible)
        {
            if (sidebar != null)
            {
                sidebar.Location = isVisible ? new Point(0, 0) : new Point(-200, 0);
            }
            else
            {
                // Handle case where sidebar is null (optional, for debugging)
                MessageBox.Show("Sidebar is not initialized!");
            }
        }
        //Button for MonteCarloSim
        private void AppButton1_Click(object sender, EventArgs e)
        {
             MonteCarloSimulationForm mcForm = new MonteCarloSimulationForm();
             mcForm.ShowDialog(); // Show the form as a modal dialog           
        }
        //Button for McBookRecreation
        private void AppButton2_Click(object sender, EventArgs e)
        {
            MineCraftBookForm mcForm = new MineCraftBookForm();
            mcForm.ShowDialog(); // Show the form as a modal dialog
        }


        public class CircleCheckBox : CheckBox 
        {
            private bool sideBarStatus = false;

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
                this.sideBarStatus = !this.sideBarStatus;

                // Redraw control
                this.Invalidate();
            }
        }

        private void LoadSoundtracks()
        {
            try
            {
                // Calculate the project root path
                string projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"E:\CODES\DansbyBot");

                // Navigate to Resources\Soundtracks
                string soundtrackPath = Path.Combine(projectRoot, "Resources", "Soundtracks");
                SoundTrackPathGlobal = soundtrackPath;

                //MessageBox.Show("DEBUG: " + soundtrackPath);

                // Check if the directory exists
                if (!Directory.Exists(soundtrackPath))
                {
                    MessageBox.Show("The Soundtracks folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Load the soundtracks
                soundtracks = new List<string>(Directory.GetFiles(soundtrackPath, "*.mp3"));

                for (int i = 0; i < soundtracks.Count; i++)
                {
                    Console.WriteLine(soundtracks[i]);
                }
                


                //DEBUG -----------------------------------------------------------------------------------------------------
                /*
                MessageBox.Show("Soundtrack amount: " + soundtracks.Count + "\nSoundtrack Paths: " + soundtrackPath + "\nSoundtrack Project Root: " + projectRoot);
                */
                //DEBUG -----------------------------------------------------------------------------------------------------


                if (soundtracks.Count == 0)
                {
                    MessageBox.Show("No soundtracks found in the Soundtracks folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Add soundtracks to ComboBox
                soundtrackComboBox.Items.Clear();
                foreach (var track in soundtracks)
                {
                    soundtrackComboBox.Items.Add(Path.GetFileName(track));

                    //DEBUG
                    //Console.WriteLine("Each SoundTracks Individual names: ", Path.GetFileName(track));
                    //Console.WriteLine("\n");
                }
                if (soundtrackComboBox.Items.Count > 0)
                {
                    soundtrackComboBox.SelectedIndex = 0; // Select the first track
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading soundtracks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void SoundtrackComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Change soundtrack
            string selectedSoundtrack = soundtrackComboBox.SelectedItem.ToString();
            PlaySoundtrack(selectedSoundtrack);
        }

        private void PlaySoundtrack(string filePath)
        {

            filePath = Path.Combine(SoundTrackPathGlobal, filePath);

            //MessageBox.Show("FilePath: " + filePath);
            try
            {
                Console.WriteLine("Attempting to play: " + filePath);

                StopPlayback(); // Stop any existing playback

                // Check if file exists
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File not found: " + filePath);
                }

                audioFile = new AudioFileReader(filePath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);

                // Attach the PlaybackStopped event to loop the track
                outputDevice.PlaybackStopped += (s, a) => 
                {
                    if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Stopped)
                    {
                        // Restart the track
                        audioFile.Position = 0; // Rewind the audio file
                        outputDevice.Play();
                    }
                };

                outputDevice.Play();
                Console.WriteLine("Playback started.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing soundtrack: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }



        private void StopPlayback()
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }

            if (audioFile != null)
            {
                audioFile.Dispose();
                audioFile = null;
            }
        }


        private void MuteButton_Click(object sender, EventArgs e)
        {
            if (outputDevice != null)
            {
                if (isMuted)
                {
                    outputDevice.Volume = 1.0f; // Restore volume
                    muteButton.Text = "Mute";
                }
                else
                {
                    outputDevice.Volume = 0.0f; // Mute volume
                    muteButton.Text = "Unmute";
                }

                isMuted = !isMuted;
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (outputDevice == null || outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                string selectedSoundtrack = soundtrackComboBox.SelectedItem.ToString();
                PlaySoundtrack(selectedSoundtrack);
                playButton.Text = "Stop";
            }
            else
            {
                StopPlayback();
                playButton.Text = "Play";
            }
        }

        public void SummonSlime()
        {
            spritePictureBox = new PictureBox
            {
                
                Size = new Size(40,40), //Image Size
                SizeMode = PictureBoxSizeMode.Zoom, //Image Scale
                Location = new Point(10,10) //Start Position
            };
            this.Controls.Add(spritePictureBox);

            // Initialize the Jump Timer
            jumpTimer = new Timer
            {
                Interval = 150// Adjust the interval to control the jump speed
            };
            jumpTimer.Tick += JumpTimer_Tick;


            // Initialize the Timer for animation
            animationTimer = new Timer
            {
                Interval = 300// Adjust for speed
            };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            // Set initial position
            spriteX = 10;
            spriteY = 10;
            currentAnimation = "running"; // Default animation

            // Load the initial GIF
            LoadGifAnimation(currentAnimation);

            //Layering for Slime Animation
            spritePictureBox.BringToFront();
            loggedInUserLabel.BringToFront();
            toggleLabel.BringToFront();
            toggleSwitch.BringToFront();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
        
            // Speed Logic for X and Y axis of sprite Movement
            spriteX += 4;

            // Check for animation state
            if (currentAnimation == "attack")
            {
                spriteX -= 4; //No Movement
            }
            else if (currentAnimation == "jump")
            {
                if (!isJumping)
                {
                    // Start the jump
                    isJumping = true;
                    jumpTimer.Start();
                }
            }

            // Sprite Visibility when off Screen
            spritePictureBox.Location = new Point(spriteX, spriteY);

            if (spriteX > this.ClientSize.Width || spriteY > this.ClientSize.Height)
            {
                spritePictureBox.Visible = false;
            }

            int action = random.Next(1, 101);

            if (action <= 10)
            {
                currentAnimation = "attack";
            }
            else if (action > 10 && action <= 30)
            {
                currentAnimation = "jump";
            }
            else
            {
                currentAnimation = "running";
            }

            LoadGifAnimation(currentAnimation);
        }

        private void LoadGifAnimation(string animation)
        {
            switch (animation)
            {
                case "attack":
                    spritePictureBox.Image = Image.FromFile(@"E:\CODES\DansbyBot\Resources\SlimeAnimation\SlimeAttack.gif");
                    break;
                case "running":
                    spritePictureBox.Image = Image.FromFile(@"E:\CODES\DansbyBot\Resources\SlimeAnimation\SlimeRun.gif");
                    break;
                case "jump":
                    spritePictureBox.Image = Image.FromFile(@"E:\CODES\DansbyBot\Resources\SlimeAnimation\SlimeJump.gif");
                    break;
            }
        }

        private void JumpTimer_Tick(object sender, EventArgs e)
        {
            // Move up
            spriteY -= 3; // Adjust the speed of upward movement
            spriteX += 3;

            // Check if sprite has reached the peak of the jump
            if (spriteY <= 10 - jumpHeight)
            {
                // Change direction to fall
                jumpTimer.Interval = 50; // Adjust the fall speed
                jumpTimer.Tick -= JumpTimer_Tick;
                jumpTimer.Tick += FallTimer_Tick;
            }
        }

        private void FallTimer_Tick(object sender, EventArgs e)
        {
            // Move down
            spriteY += 3; // Adjust the speed of downward movement
            spriteX += 2;

            // Check if sprite has returned to original position
            if (spriteY >= 10)
            {
                // Reset the state
                spriteY = 10;
                isJumping = false;
                jumpTimer.Stop();
                jumpTimer.Interval = 50; // Reset the interval for the next jump
                jumpTimer.Tick -= FallTimer_Tick;
                jumpTimer.Tick += JumpTimer_Tick; // Restore the original tick event
            }

            spritePictureBox.Location = new Point(spriteX, spriteY);
        }

         private void Timer_Tick(object sender, EventArgs e)
        {
            // Increment the SlimeCount every second
            SlimeCount++;
            Console.WriteLine("SlimeTimer = " + SlimeCount);

             if (SlimeCount == 5)
            {
                SummonSlime();
            } else if (SlimeCount == 457)
            {
                SummonSlime();
            } else if (SlimeCount == 1578)
            {
                SummonSlime();
            } else if (SlimeCount == 3457)
            {
                SummonSlime();
            } else if (SlimeCount == 6746)
            {
                SummonSlime();
            } else if (SlimeCount == 10000)
            {   
                SummonSlime();
            }
        }

    

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopPlayback();
            base.OnFormClosing(e);
            SlimeCount = 0; //reset Slime Timer
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