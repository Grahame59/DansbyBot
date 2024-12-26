using System;
using Intents;
using Responses;
using UserAuthentication;
using System.Windows.Forms;
using System.Drawing;
using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Timers;

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
        private System.Windows.Forms.Timer animationTimer;
        private int spriteX, spriteY;
        private string currentAnimation;
        private Random random = new Random();
        private System.Windows.Forms.Timer jumpTimer;
        private bool isJumping;
        private int jumpHeight = 20; // Adjust the height of the jump
        private System.Windows.Forms.Timer SlimeTimer;
        private System.Windows.Forms.Timer LorehavenTimer;
        private const int RestartInterval = 3 *60 * 60 * 1000; // 3 hours in milliseconds.
        private System.Timers.Timer restartTimer;
        public static int SlimeCount = 0; //Will have to change from Static in future if I ever release more instances of MainForm
        private ErrorLogClient errorLogClient = new ErrorLogClient();

        public MainForm()
        {
            InitializeComponent();

            // Set up Restart Timer
            restartTimer = new System.Timers.Timer(RestartInterval);
            restartTimer.Elapsed += OnRestartTimerElapsed;
            restartTimer.Start();

            InitializeChatbot();
            LoadSoundtracks();

            //Initialize Slime Timer for SlimeCount++ and slime animation
            SlimeTimer = new System.Windows.Forms.Timer();
            SlimeTimer.Interval = 1000; // 1 second
            SlimeTimer.Tick += Timer_Tick;
            SlimeTimer.Start();

            //Initialize Lorehaven Timer
            LorehavenTimer = new System.Windows.Forms.Timer();
            LorehavenTimer.Interval = 300000; //5 minutes in ms
            LorehavenTimer.Tick += Lorehaven_Tick;
            LorehavenTimer.Start();
           
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
            this.loggedInUserLabel.Size = new System.Drawing.Size(200, 20); // Adjusted size
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
            sidebar.BringToFront();
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

            // Add more buttons as needed

            // Soundtrack ComboBox
            this.soundtrackComboBox = new ComboBox();

            this.soundtrackComboBox.Location = new Point(680, 130);
            this.soundtrackComboBox.Size = new Size(100, 20);
            this.soundtrackComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.soundtrackComboBox.SelectedIndexChanged += SoundtrackComboBox_SelectedIndexChanged;
            this.soundtrackComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Anchor to the top and right
            
            // Load soundtracks into ComboBox
            LoadSoundtracks();

        
            if (soundtracks == null)
            {
                // Log the error to the error log
                errorLogClient.AppendToErrorLog("Soundtracks list is null.", "MainForm.cs");
            }

            foreach (var track in soundtracks)
            {
                if (track == null)
                {
                    errorLogClient.AppendToErrorLog("A track in the soundtracks list is null.", "MainForm.cs");
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
            CurInstanceLoginUser = "kyler_grahame"; // TO PROMPT LOGIN DO: CurInstanceLoginUser = SaveResponse("Enter Your Username: ");
            CurInstanceLoginPass = "admin";// TO PROMPT PASS DO : CurInstanceLoginPass = SaveResponse("Enter your password:");

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

            // Removing User system : Leaving code if I want to reimplement... Only removing here unless I need to refactor some other scripts. 
        }        
        // END: OF FORM SETUP ----------------------------------------------------------------------------------------------------------------------------------------

        //Start: Restart Methods ------------------------------------------------------------------------------------------------------------------------------------
        private void OnRestartTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RestartApplication();
        }

        private void RestartApplication()
        {
            // Stop the timer to prevent multiple restarts
            restartTimer.Stop();

            // Log the restart action
            LogRestart();

            // Start a new instance of the application
            Process.Start(Application.ExecutablePath);
            Application.Exit(); // Close the current instance gracefully
        }

        private void LogRestart()
        {
            if (errorLogClient != null)
            {
                errorLogClient.AppendToDebugLog("DansbyBot is restarting.", "MainForm.cs");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Handle graceful shutdown
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Log the shutdown action
                LogShutdown();

                // Perform any cleanup needed here
                StopPlayback(); // Example cleanup action
                
                // Hide the MainForm instead of closing it
                this.Hide();
                e.Cancel = true; // Cancel the close event to keep the form open
            }
            else
            {
                SlimeCount = 0; // Reset Slime Timer
                StopPlayback(); // Ensure playback is stopped during shutdown
            }

            // Call the base class method to ensure proper closing behavior
            base.OnFormClosing(e);
        }


        private void LogShutdown()
        {
            if (errorLogClient != null)
            {
                errorLogClient.AppendToDebugLog("DansbyBot is shutting down gracefully.", "MainForm.cs");
            }
        }

        // Additional method to handle exceptions globally
        private void HandleCrash(Exception ex)
        {
            if (errorLogClient != null)
            {
                errorLogClient.AppendToErrorLog($"Unhandled exception occurred: {ex.Message}", "MainForm.cs");
            }

            // Restart the application after logging the error
            RestartApplication();
        }

        //END: Restart Methods -------------------------------------------------------------------------------------------------------------------------------------
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
        //Button for MonteCarloSimForm -----------------------------------
        private void AppButton1_Click(object sender, EventArgs e)
        {
             MonteCarloSimulationForm mcForm = new MonteCarloSimulationForm();
             mcForm.Show(); // Show the form as a non modal dialog           
        }

        // Start methods/constructors with UI/UX Logic -----------------------------------------------------------------------------------------------------------------
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

        // End: methods/constructors with UI/UX Logic ---------------------------------------------------------------------------------------------------------------

        // Start: Methods/Contructors with Soundtrack Logic ---------------------------------------------------------------------------------------------------------
        private void LoadSoundtracks()
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

            if (errorLogClient != null) //Debug
            {
                string SoundTrackDebug1 = ("Soundtrack amount: " + soundtracks.Count + "\nSoundtrack Paths: " + soundtrackPath + "\nSoundtrack Project Root: " + projectRoot);
                errorLogClient.AppendToErrorLog(SoundTrackDebug1, "MainForm.cs");
            }


            if (soundtracks.Count == 0)
            {
                //Debug
                string SoundTrackCountEmptyError = ("No soundtracks found in the Soundtracks folder. (soundtracks.Count = 0)");
                errorLogClient.AppendToErrorLog(SoundTrackCountEmptyError, "MainForm.cs");
                MessageBox.Show("No soundtracks found in the Soundtracks folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Add soundtracks to ComboBox
            soundtrackComboBox.Items.Clear();
            foreach (var track in soundtracks)
            {
                soundtrackComboBox.Items.Add(Path.GetFileName(track));
                
            }
            if (soundtrackComboBox.Items.Count > 0)
            {
                soundtrackComboBox.SelectedIndex = 0; // Select the first track
            }
            if (soundtracks == null && errorLogClient != null)
            {
                errorLogClient.AppendToErrorLog("Error loading soundtracks", "Mainform.cs");
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
            if (errorLogClient != null)
            {
                string filePathDebugString = "FilePath: " + filePath; // debugLog
                errorLogClient.AppendToDebugLog(filePathDebugString, "Mainform.cs");
            }

            StopPlayback(); // Stop any existing playback

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                if (errorLogClient != null)
                {
                    errorLogClient.AppendToErrorLog($"File not found: {filePath}", "Mainform.cs");
                }
                return;
            }

            // Initialize the audio file and output device
            try
            {
                audioFile = new AudioFileReader(filePath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);

                // Attach the PlaybackStopped event to loop the track
                outputDevice.PlaybackStopped += (s, a) =>
                {
                    // Check if the outputDevice and audioFile are still valid and not null
                    if (outputDevice != null && audioFile != null && outputDevice.PlaybackState == PlaybackState.Stopped)
                    {
                        // Rewind the audio file
                        audioFile.Position = 0;

                        // Reinitialize outputDevice in case it was disposed
                        if (outputDevice != null && audioFile != null)
                        {
                            try
                            {
                                outputDevice.Init(audioFile); // Ensure Init is called
                                outputDevice.Play();
                            }
                            catch (InvalidOperationException ex)
                            {
                                if (errorLogClient != null)
                                {
                                    errorLogClient.AppendToErrorLog("Error during re-initialization of the outputDevice: " + ex.Message, "MainForm.cs");
                                }
                            }
                        }
                        else
                        {
                            if (errorLogClient != null)
                            {
                                errorLogClient.AppendToErrorLog("OutputDevice or AudioFile for Soundtracks not initialized or disposed.", "MainForm.cs");
                            }
                        }
                    }
                };

                outputDevice.Play();
                if (errorLogClient != null)
                {
                    errorLogClient.AppendToDebugLog("Playback started for Soundtracks.", "MainForm.cs");
                }
            }
            catch (Exception ex)
            {
                if (errorLogClient != null)
                {
                    errorLogClient.AppendToErrorLog("Exception occurred during playback initialization: " + ex.Message, "MainForm.cs");
                }
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

        // End: Methods/Contructors with Soundtrack Logic -----------------------------------------------------------------------------------------------------------

        //Start: Methods/Constuctors with Slime Animation Logic -----------------------------------------------------------------------------------------------------
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
            jumpTimer = new System.Windows.Forms.Timer
            {
                Interval = 150// Adjust the interval to control the jump speed
            };
            jumpTimer.Tick += JumpTimer_Tick;


            // Initialize the Timer for animation
            animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 500// Adjust for speed {500ms = .5seconds}
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
            sidebar.BringToFront();
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
            } else if (currentAnimation == "die")
            {
                spriteX -= 4;
            }

            // Sprite Visibility when off Screen
            spritePictureBox.Location = new Point(spriteX, spriteY);

            if (spriteX > this.ClientSize.Width || spriteY > this.ClientSize.Height)
            {
                spritePictureBox.Visible = false;
            }

            int action = random.Next(1, 101);

            if (action <= 20)
            {
                currentAnimation = "attack";
                spritePictureBox.Visible = true;
            }
            else if (action > 20 && action <= 50)
            {
                currentAnimation = "jump";
                spritePictureBox.Visible = true;
            }
            else if (action > 60 && action < 65)
            {
                currentAnimation = "die";
                // Pause the AnimationTimer to let the "die" animation play out
                animationTimer.Stop();
                
                // Determine the duration of the "die" animation (in milliseconds)
                int dieAnimationDuration = 2000; // Adjust this based on the actual GIF duration
                
                // Start a timer to resume after the animation has played out
                System.Windows.Forms.Timer dieTimer = new System.Windows.Forms.Timer();
                dieTimer.Interval = dieAnimationDuration;
                dieTimer.Tick += (s, args) =>
                {
                    // Hide the sprite or remove it after the animation plays out
                    spritePictureBox.Visible = false;
                    dieTimer.Stop();
                };
                dieTimer.Start();
                
            } else
            {
                currentAnimation = "running";
                spritePictureBox.Visible = true;
            }

            LoadGifAnimation(currentAnimation);
        }

        private void LoadGifAnimation(string animation)
        {
            string imagePath = animation switch
            {
                "attack" => @"E:\CODES\DansbyBot\Resources\SlimeAnimation\SlimeAttack.gif",
                "running" => @"E:\CODES\DansbyBot\Resources\SlimeAnimation\SlimeRun.gif",
                "jump" => @"E:\CODES\DansbyBot\Resources\SlimeAnimation\SlimeJump.gif",
                "die" => @"E:\CODES\DansbyBot\Resources\SlimeAnimation\SlimeExplosion.gif",
                _ => throw new ArgumentOutOfRangeException(nameof(animation), $"Unknown animation: {animation}")
            };

            spritePictureBox.Image = Image.FromFile(imagePath);

            if (animation == "die")
            {
                int deathcount = 0;
                if (deathcount == 0)
                {
                    deathcount++;
                } else if (deathcount == 5)
                {
                    spritePictureBox.Visible = false; // Hide after resizing
                }
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
            SlimeCount++;
            string slimeSummonedDebug = "Debug: Slime Summoned.";
            
            int[] summonCounts = { 3, 33, 59, 92, 133, 212, 285, 350, 420, 555, 599, 675, 712, 795, 852, 943, 3457, 3500, 3515, 3530, 3545, 3560, 6746, 10000 };

            if (Array.IndexOf(summonCounts, SlimeCount) >= 0)
            {
                SummonSlime();
                errorLogClient.AppendToDebugLog(slimeSummonedDebug, "MainForm.cs");
            }

        }
        //End: Methods/Constructors for Slime Animation Logic ----------------------------------------------------------------------------------------------------------

        // Autosave function to autosave my Obsidian Notes onto my github
        // It access a file called autosave.bat which runs the git pull, add, commit, push commands...
        // the autosave.bat file is in cd E:/Lorehaven and the repo it pushes to is under E:/Lorehaven/gitconnect
        public void Autosave(object state)
        {
            string autosaveDebugmsg = "Autosaved excecuted for Lorehaven at: " + DateTime.Now;
            Process.Start("E:\\Lorehaven\\autosave.bat");
            errorLogClient.AppendToDebugLog($"Debug: {autosaveDebugmsg}.", "ErrorLogForm.cs" );
        }
        
        // Tick event for Autosave Timer that calls Autosave method above ^.
        private void Lorehaven_Tick(object sender, EventArgs e)
        {
            Autosave(null); //calls Autosave Method.
        }

        // Method to fully close the application when needed
        public void CloseApplication()
        {
            
            if (errorLogClient != null)
            {
                StopPlayback();
            }
            this.Close();
            Application.Exit(); // Ensure all forms are closed
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

            //MainForm Entry Point
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            int restartCount = 0; // Counter for consecutive restarts
            const int maxRestarts = 5; // Maximum allowed restarts before exiting

            while (true) // Infinite loop to keep restarting the application
            {
                try
                {
                    // Create and run the MainForm
                    using (MainForm mainForm = new MainForm())
                    {
                        Application.Run(mainForm);
                    }

                    // Reset the restart count if the application closes successfully
                    restartCount = 0;
                }
                catch (Exception ex)
                {
                    // Optionally show a message box to inform the user
                    MessageBox.Show("The application has encountered an error and will restart.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                    restartCount++; // Increment the restart count

                    // Check if the maximum restart count has been reached
                    if (restartCount >= maxRestarts)
                    {
                        MessageBox.Show("The application has encountered multiple errors and will now exit.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break; // Exit the loop

                    }
                }

                System.Threading.Thread.Sleep(1000); // Sleep for 1 second

            }
        }
    } 
}