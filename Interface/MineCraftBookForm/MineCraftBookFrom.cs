using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text;

namespace ChatbotApp.Interface.MinecraftBook
{
    public partial class MineCraftBookForm : Form
    {
        private List<List<string>> chapters = new List<List<string>>();
        private List<Button> chapterButtons = new List<Button>();
        private Button prevButton;
        private Button nextButton;
        private Button returnButton;
        private Label pageNumberLabel;
        private Panel leftPagePanel;
        private Panel rightPagePanel;
        private int currentPage = 0;
        private int maxChapters = 25;
        private int pagesPerChapter = 50;
        private int chaptersPerColumn = 18;
        private int buttonWidth = 100;
        private int buttonHeight = 30;
        private int margin = 10;
        private Color borderColor = Color.FromArgb(134, 93, 63);
        private int borderWidth = 25;
        string pageDataFile = @"E:\CODES\DansbyBot\Interface\MineCraftBookForm\PageContents.txt"; //used @ to treat string as verbatim rather than \\
        public MineCraftBookForm()
        {
            InitializeComponent();
            SetupBookInterface();
            LoadChapters();
            DisplayChapterScreen();
        }

        private void InitializeComponent()
        {
            this.prevButton = new Button();
            this.nextButton = new Button();
            this.returnButton = new Button();
            this.pageNumberLabel = new Label();
            this.leftPagePanel = new Panel();
            this.rightPagePanel = new Panel();
            this.SuspendLayout();

            // Prev Button
            this.prevButton.Location = new Point(20, 670);
            this.prevButton.Size = new Size(150, 30);
            this.prevButton.Text = "Previous";
            this.prevButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.prevButton.Click += new EventHandler(this.PrevButton_Click);

            // Next Button
            this.nextButton.Location = new Point(830, 670);
            this.nextButton.Size = new Size(150, 30);
            this.nextButton.Text = "Next";
            this.nextButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.nextButton.Click += new EventHandler(this.NextButton_Click);

            // Return Button
            this.returnButton.Location = new Point(350, 670);
            this.returnButton.Size = new Size(300, 30);
            this.returnButton.Text = "Return to Chapter Screen";
            this.returnButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.returnButton.Click += new EventHandler(this.ReturnButton_Click);

            // Page Number Label
            this.pageNumberLabel.AutoSize = true;
            this.pageNumberLabel.Location = new Point(450, 20);
            this.pageNumberLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            this.pageNumberLabel.ForeColor = Color.DarkRed;
            this.pageNumberLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.pageNumberLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Left Page Panel
            this.leftPagePanel.Location = new Point(50, 50);
            this.leftPagePanel.Size = new Size(450, 600);
            this.leftPagePanel.BackColor = Color.FromArgb(252, 248, 236);
            this.leftPagePanel.BorderStyle = BorderStyle.FixedSingle;
            this.leftPagePanel.Padding = new Padding(10);
            this.leftPagePanel.Visible = false;

            // Right Page Panel
            this.rightPagePanel.Location = new Point(500, 50);
            this.rightPagePanel.Size = new Size(450, 600);
            this.rightPagePanel.BackColor = Color.FromArgb(252, 248, 236);
            this.rightPagePanel.BorderStyle = BorderStyle.FixedSingle;
            this.rightPagePanel.Padding = new Padding(10);
            this.rightPagePanel.Visible = false;

            // MineCraftBookForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(252, 248, 236);
            this.ClientSize = new Size(1000, 750);
            this.MinimumSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.returnButton);
            this.Controls.Add(this.pageNumberLabel);
            this.Controls.Add(this.leftPagePanel);
            this.Controls.Add(this.rightPagePanel);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Name = "MineCraftBookForm";
            this.Text = "Minecraft Book";
            this.ResumeLayout(false);
            this.PerformLayout();

            this.Paint += new PaintEventHandler(MineCraftBookForm_Paint);
        }

        private void SetupRichTextBox(RichTextBox richTextBox)
        {
            richTextBox.AcceptsTab = true; // Allows the RichTextBox to recognize the Tab key

            richTextBox.KeyPress += (sender, e) =>
            {
                if (e.KeyChar == (char)13) // 13 is the Enter key
                {
                    e.Handled = true;
                    richTextBox.SelectedText = Environment.NewLine; // Replace with newline
                }
            };
        }


        private void MineCraftBookForm_Paint(object sender, PaintEventArgs e)
        {
            // Custom drawing for form border
            using (Pen borderPen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        private void SetupBookInterface()
        {
            CreateChapterButtons();
        }

        private void DisplayChapterScreen()
        {
            pageNumberLabel.Text = "Chapter Screen";

            prevButton.Visible = false;
            nextButton.Visible = false;
            returnButton.Visible = false;

            foreach (Button button in chapterButtons)
            {
                button.Visible = true;
            }

            leftPagePanel.Visible = false;
            rightPagePanel.Visible = false;
        }

        private void CreateChapterButtons()
        {
            int currentColumn = 0;
            int startX = 20;
            int startY = 50;

            for (int i = 0; i < maxChapters; i++)
            {
                int chapterIndex = i;

                Button chapterButton = new Button
                {
                    Text = $"Chapter {chapterIndex + 1}",
                    Size = new Size(buttonWidth, buttonHeight),
                    Location = new Point(startX + currentColumn * (buttonWidth + margin), startY + (i % chaptersPerColumn) * (buttonHeight + margin)),
                    BackColor = Color.FromArgb(134, 93, 63),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance =
                    {
                        BorderColor = Color.FromArgb(175, 156, 127),
                        BorderSize = 1
                    }
                };

                chapterButton.Click += (sender, e) => NavigateToChapter(chapterIndex);
                chapterButtons.Add(chapterButton);
                this.Controls.Add(chapterButton);

                if ((i + 1) % chaptersPerColumn == 0)
                {
                    currentColumn++;
                }
            }
        }

        private void LoadChapters()
        {
            // Initialize chapters list
            for (int i = 0; i < maxChapters; i++)
            {
                List<string> chapterPages = new List<string>();
                for (int j = 0; j < pagesPerChapter; j++)
                {
                    chapterPages.Add(""); // Initialize with empty pages
                }
                chapters.Add(chapterPages);
            }

            // Load existing page contents from file
            if (File.Exists(pageDataFile))
            {
                string[] lines = File.ReadAllLines(pageDataFile);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { ',' }, 2); // Split into two parts, respecting commas in content
                    if (parts.Length == 2)
                    {
                        int page = int.Parse(parts[0]);
                        // Unescape newlines and commas
                        string content = parts[1].Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\,", ",");
                        if (page >= 1 && page <= maxChapters * pagesPerChapter)
                        {
                            int chapterIndex = (page - 1) / pagesPerChapter;
                            int localPageIndex = (page - 1) % pagesPerChapter;
                            chapters[chapterIndex][localPageIndex] = content;

                            // Debug log
                            Console.WriteLine($"Loaded Page {page}: {content}");
                        }
                    }
                }
            }
        }


        private void SavePageData()
        {
            using (StreamWriter writer = new StreamWriter(pageDataFile, false, Encoding.UTF8))
            {
                for (int i = 0; i < maxChapters; i++)
                {
                    for (int j = 0; j < pagesPerChapter; j++)
                    {
                        int globalPageNumber = i * pagesPerChapter + j + 1;
                        // Escape commas and newlines to preserve content structure
                        string sanitizedContent = chapters[i][j].Replace(",", "\\,").Replace("\n", "\\n").Replace("\r", "\\r");
                        writer.WriteLine($"{globalPageNumber},{sanitizedContent}");
                        
                        // Debug log
                        Console.WriteLine($"Saved Page {globalPageNumber}: {sanitizedContent}");
                    }
                }
            }
        }




        private void NavigateToChapter(int chapterIndex)
        {
            currentPage = chapterIndex * pagesPerChapter;
            DisplayPage();
        }

        private void DisplayPage()
        {
            if (currentPage >= 0 && currentPage < maxChapters * pagesPerChapter)
            {
                int chapterIndex = currentPage / pagesPerChapter;
                int localPageIndex = currentPage % pagesPerChapter;

                pageNumberLabel.Text = $"Chapter {chapterIndex + 1} - Page {localPageIndex + 1}";

                foreach (Button button in chapterButtons)
                {
                    button.Visible = false;
                }

                prevButton.Visible = currentPage > 0;
                nextButton.Visible = currentPage < (maxChapters * pagesPerChapter) - 1;
                returnButton.Visible = true;

                // Left Page
                leftPagePanel.Visible = true;
                leftPagePanel.Controls.Clear();
                RichTextBox leftPageTextBox = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 12, FontStyle.Regular),
                    ForeColor = Color.Black,
                    Text = chapters[chapterIndex][localPageIndex]
                };
                leftPageTextBox.TextChanged += (sender, e) =>
                {
                    chapters[chapterIndex][localPageIndex] = leftPageTextBox.Text;
                    SavePageData();
                };
                leftPagePanel.Controls.Add(leftPageTextBox);

                // Right Page
                if (localPageIndex + 1 < chapters[chapterIndex].Count)
                {
                    rightPagePanel.Visible = true;
                    rightPagePanel.Controls.Clear();
                    RichTextBox rightPageTextBox = new RichTextBox
                    {
                        Dock = DockStyle.Fill,
                        Font = new Font("Arial", 12, FontStyle.Regular),
                        ForeColor = Color.Black,
                        Text = chapters[chapterIndex][localPageIndex + 1]
                    };
                    rightPageTextBox.TextChanged += (sender, e) =>
                    {
                        chapters[chapterIndex][localPageIndex + 1] = rightPageTextBox.Text;
                        SavePageData();
                    };
                    rightPagePanel.Controls.Add(rightPageTextBox);
                }
                else
                {
                    rightPagePanel.Visible = false;
                }
            }
        }




        private void PrevButton_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                DisplayPage();
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (currentPage < (maxChapters * pagesPerChapter) - 1)
            {
                currentPage++;
                DisplayPage();
            }
        }

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            currentPage = 0;
            DisplayChapterScreen();
        }
    }
}
