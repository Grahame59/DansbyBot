// Manages slime animations
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ChatbotApp.Features
{
    public class AnimationManager
    {
        private PictureBox spritePictureBox;
        private Timer animationTimer;
        private Timer jumpTimer;
        private Random random;
        private int spriteX;
        private int spriteY;
        private bool isJumping;
        private string currentAnimation;

        private readonly Dictionary<string, string> animationPaths;

        public AnimationManager()
        {
            random = new Random();
            spriteX = 10;
            spriteY = 10;
            isJumping = false;
            currentAnimation = "running";

            animationPaths = new Dictionary<string, string>
            {
                { "attack", "Resources/SlimeAnimation/SlimeAttack.gif" },
                { "running", "Resources/SlimeAnimation/SlimeRun.gif" },
                { "jump", "Resources/SlimeAnimation/SlimeJump.gif" },
                { "die", "Resources/SlimeAnimation/SlimeExplosion.gif" }
            };
        }

        public void InitializeAnimation(Control parent)
        {
            spritePictureBox = new PictureBox
            {
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(spriteX, spriteY),
                Visible = true
            };

            parent.Controls.Add(spritePictureBox);

            animationTimer = new Timer { Interval = 500 };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            jumpTimer = new Timer { Interval = 150 };
            jumpTimer.Tick += JumpTimer_Tick;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            spriteX += 4;

            if (currentAnimation == "jump" && !isJumping)
            {
                isJumping = true;
                jumpTimer.Start();
            }

            if (spriteX > 800) // Example boundary
            {
                spriteX = 0;
            }

            int action = random.Next(1, 101);

            currentAnimation = action switch
            {
                <= 20 => "attack",
                <= 50 => "jump",
                > 60 and <= 65 => "die",
                _ => "running"
            };

            LoadGifAnimation(currentAnimation);

            spritePictureBox.Location = new Point(spriteX, spriteY);
        }

        private void JumpTimer_Tick(object sender, EventArgs e)
        {
            spriteY -= 3;
            spriteX += 3;

            if (spriteY <= 10 - 20) // Example jump height
            {
                jumpTimer.Tick -= JumpTimer_Tick;
                jumpTimer.Tick += FallTimer_Tick;
            }
        }

        private void FallTimer_Tick(object sender, EventArgs e)
        {
            spriteY += 3;
            spriteX += 2;

            if (spriteY >= 10)
            {
                spriteY = 10;
                isJumping = false;
                jumpTimer.Stop();
                jumpTimer.Tick -= FallTimer_Tick;
                jumpTimer.Tick += JumpTimer_Tick;
            }

            spritePictureBox.Location = new Point(spriteX, spriteY);
        }

        private void LoadGifAnimation(string animation)
        {
            if (!animationPaths.TryGetValue(animation, out var imagePath) || !System.IO.File.Exists(imagePath))
            {
                spritePictureBox.Image = null;
                return;
            }

            spritePictureBox.Image = Image.FromFile(imagePath);
        }

        public void StopAnimation()
        {
            animationTimer?.Stop();
            jumpTimer?.Stop();
        }
    }
}
