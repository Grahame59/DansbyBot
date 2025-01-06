using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ChatbotApp.Features
{ 
    public class AnimationManager
    {
        private PictureBox spritePictureBox;
        private Timer animationTimer;
        private Timer jumpTimer;
        private Random random;
        private int spriteX;
        private int groundY;
        private bool isJumping;

        private readonly Dictionary<string, string> animationPaths;
        private readonly ErrorLogClient errorLogClient;

        public AnimationManager()
        {
            random = new Random();
            spriteX = 0;
            isJumping = false;

            animationPaths = new Dictionary<string, string>
            {
                { "attack", "ChatbotApp/Resources/SlimeAnimation/SlimeAttack.gif" },
                { "running", "ChatbotApp/Resources/SlimeAnimation/SlimeRun.gif" },
                { "jump", "ChatbotApp/Resources/SlimeAnimation/SlimeJump.gif" },
                { "die", "ChatbotApp/Resources/SlimeAnimation/SlimeExplosion.gif" }
            };

            errorLogClient = ErrorLogClient.Instance;
        }

        /// <summary>
        /// Initializes the slime animation along the top edge of the RichTextBox.
        /// </summary>
        public void InitializeAnimation(RichTextBox richTextBox)
        {
            if (richTextBox.InvokeRequired)
            {
                richTextBox.Invoke(new Action(() => InitializeAnimation(richTextBox)));
                return;
            }

            // Set ground level to the top edge of the RichTextBox
            groundY = richTextBox.Top - 60; // Adjust to position the slime just above the textbox

            spritePictureBox = new PictureBox
            {
                Size = new Size(60, 60),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(spriteX, groundY),
                BackColor = Color.Transparent
            };

            richTextBox.Parent.Controls.Add(spritePictureBox);
            spritePictureBox.SendToBack();

            StartSlimeAnimation();
        }

        /// <summary>
        /// Starts the main animation loop.
        /// </summary>
        private void StartSlimeAnimation()
        {
            animationTimer = new Timer { Interval = 1500 };
            animationTimer.Tick += async (sender, e) => await AnimateSlime();
            animationTimer.Start();
        }

        /// <summary>
        /// Handles slime animation changes.
        /// </summary>
        private async Task AnimateSlime()
        {
            if (!isJumping)
            {
                spriteX += 3;

                if (spriteX > 900)
                {
                    spriteX = -80; // Reset to left side when going off screen
                }

                spritePictureBox.Location = new Point(spriteX, groundY);

                int action = random.Next(1, 101);
                string animation = action switch
                {
                    <= 20 => "attack",
                    <= 50 => "jump",
                    > 60 and <= 65 => "die",
                    _ => "running"
                };

                await PlayGifAnimationAsync(animation);

                if (animation == "die")
                {
                    spritePictureBox.Dispose();
                    animationTimer.Stop();
                }
            }
        }

        /// <summary>
        /// Plays the selected GIF animation for the slime.
        /// </summary>
        private async Task PlayGifAnimationAsync(string animation)
        {
            if (!animationPaths.TryGetValue(animation, out var imagePath))
            {
                spritePictureBox.Image = null;
                return;
            }

            if (!File.Exists(imagePath))
            {
                await errorLogClient.AppendToErrorLogAsync($"Animation file not found: {imagePath}", "AnimationManager");
                spritePictureBox.Image = null;
                return;
            }

            spritePictureBox.Image = Image.FromFile(imagePath);
            switch (animation)
            {
                case "running":
                    break;

                case "die":
                    spritePictureBox.Visible = true;
                    await Task.Delay(1400); // Wait for the GIF to finish
                    spritePictureBox.Visible = false;
                    break;

                default:
                    // Optional: Handle other cases or unknown animations
                    break;
            }
        }

        /// <summary>
        /// Handles jumping animation and movement.
        /// </summary>
        private void StartJump()
        {
            if (isJumping) return;

            isJumping = true;
            int peakY = groundY - 50;
            spriteX += 2;

            jumpTimer = new Timer { Interval = 50 };
            jumpTimer.Tick += (sender, e) =>
            {
                if (spritePictureBox.Top > peakY)
                {
                    spritePictureBox.Top -= 5;
                }
                else
                {
                    jumpTimer.Tick -= (sender, e) => { };
                    jumpTimer.Tick += (sender, e) => Fall();
                }
            };
            jumpTimer.Start();
        }

        /// <summary>
        /// Handles the falling animation.
        /// </summary>
        private void Fall()
        {
            if (spritePictureBox.Top < groundY)
            {
                spritePictureBox.Top += 5;
            }
            else
            {
                spritePictureBox.Top = groundY;
                isJumping = false;
                jumpTimer.Stop();
            }
        }

        /// <summary>
        /// Stops the animation and timers.
        /// </summary>
        public void StopAnimation()
        {
            animationTimer?.Stop();
            jumpTimer?.Stop();
        }
    }
}
