using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatbotApp
{
    public partial class MonteCarloSimulationForm : Form
    {
        private Random random = new Random();
        private int numOfPoints = 50000; // Number of points to sample, default set to 50,000
        private int pointsWithinSphere = 0; // Counter for points inside the sphere
        private double cubeSideLength = 400.0; // Side length of the cube (pixels)
        private double sphereRadius; // Radius of the sphere (calculated based on cube size)
        private double cubeVolume; // Volume of the cube
        private bool simulationRunning = false;

        private Label volumeEstimationLabel;
        private Label trueVolumeLabel;
        private PictureBox simulationPictureBox;
        private NumericUpDown numOfPointsInput; // Input control for numOfPoints

        public MonteCarloSimulationForm()
        {
            InitializeSimulation();
        }

        private void InitializeSimulation()
        {
            this.Text = "Monte Carlo Simulation";
            this.Size = new Size(650, 600); // Increased height to accommodate the input control
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.LightPink;

            volumeEstimationLabel = new Label();
            volumeEstimationLabel.Text = "Estimated sphere volume: ";
            volumeEstimationLabel.ForeColor = Color.LightPink;
            volumeEstimationLabel.AutoSize = true;
            volumeEstimationLabel.Location = new Point(20, 20);
            this.Controls.Add(volumeEstimationLabel);

            trueVolumeLabel = new Label();
            trueVolumeLabel.Text = "True sphere volume: ";
            trueVolumeLabel.ForeColor = Color.LightPink;
            trueVolumeLabel.AutoSize = true;
            trueVolumeLabel.Location = new Point(20, 40); // Adjust the vertical position
            this.Controls.Add(trueVolumeLabel);

            simulationPictureBox = new PictureBox();
            simulationPictureBox.Location = new Point(20, 70); // Adjusted vertical position to make space for labels
            simulationPictureBox.Size = new Size(400, 400);
            simulationPictureBox.BackColor = Color.FromArgb(88, 86, 91); //dark gray color;
            simulationPictureBox.Paint += SimulationPictureBox_Paint;
            this.Controls.Add(simulationPictureBox);

            sphereRadius = cubeSideLength / 2.0;
            cubeVolume = Math.Pow(cubeSideLength, 3);

            // Numeric up-down control for inputting numOfPoints
            numOfPointsInput = new NumericUpDown();
            numOfPointsInput.Minimum = 1000; // Minimum value for numOfPoints
            numOfPointsInput.Maximum = 1000000; // Maximum value for numOfPoints
            numOfPointsInput.Value = numOfPoints; // Default value
            numOfPointsInput.Location = new Point(450, 100); // Adjusted position
            numOfPointsInput.Size = new Size(100, 30);
            this.Controls.Add(numOfPointsInput);

            Button startButton = new Button();
            startButton.Text = "Start Simulation";
            startButton.ForeColor = Color.LightPink;
            startButton.Size = new Size(150, 30);
            startButton.Location = new Point(450, 140); // Adjusted position
            startButton.Click += StartButton_Click;
            this.Controls.Add(startButton);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            // Update numOfPoints from the input control
            numOfPoints = (int)numOfPointsInput.Value;

            if (!simulationRunning)
            {
                simulationRunning = true;
                Task.Run(() => RunMonteCarloEstimation());
            }
        }

        private void RunMonteCarloEstimation()
        {
            pointsWithinSphere = 0;

            for (int i = 0; i < numOfPoints; i++)
            {
                double x = random.NextDouble() * cubeSideLength - cubeSideLength / 2.0;
                double y = random.NextDouble() * cubeSideLength - cubeSideLength / 2.0;
                double z = random.NextDouble() * cubeSideLength - cubeSideLength / 2.0;

                if (IsPointInsideSphere(x, y, z))
                {
                    pointsWithinSphere++;
                }

                // Update UI every 1000 points (optional for smoother performance)
                if (i % 100 == 0)
                {
                    UpdateUI();
                }
            }

            // Final update after all points have been processed
            UpdateUI();
            simulationRunning = false;
        }

        private void UpdateUI()
        {
            // Update labels with estimations
            double estimatedSphereVolume = (double)pointsWithinSphere / numOfPoints * cubeVolume;
            double trueSphereVolume = (4.0 / 3.0) * Math.PI * Math.Pow(sphereRadius, 3);

            this.Invoke((Action)(() =>
            {
                volumeEstimationLabel.Text = $"Estimated sphere volume: {estimatedSphereVolume:F2}";
                trueVolumeLabel.Text = $"True sphere volume: {trueSphereVolume:F2}";
                simulationPictureBox.Invalidate(); // Redraw the PictureBox
            }));
        }

        private void SimulationPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw the cube
            g.DrawRectangle(Pens.Black, 0, 0, simulationPictureBox.Width - 1, simulationPictureBox.Height - 1);

            // Draw the sphere inside the cube
            double sphereDiameter = sphereRadius * 2.0;
            double spherePosition = (cubeSideLength - sphereDiameter) / 2.0;
            g.FillEllipse(Brushes.LightPink, (float)spherePosition, (float)spherePosition, (float)sphereDiameter, (float)sphereDiameter);

            // Draw sampled points
            for (int i = 0; i < pointsWithinSphere; i++)
            {
                double x = random.NextDouble() * cubeSideLength;
                double y = random.NextDouble() * cubeSideLength;
                g.FillRectangle(Brushes.Black, (float)x, (float)y, 1, 1);
            }
        }

        private bool IsPointInsideSphere(double x, double y, double z)
        {
            return x * x + y * y + z * z <= sphereRadius * sphereRadius;
        }
    }
}
