using System.Windows.Forms;
using System.Windows.Forms.Integration; // Ensure this is added
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Terrarium
{
    public partial class WpfHostForm : Form
    {
        public WpfHostForm()
        {
            InitializeComponent();
            LoadWpfControl();
        }

        private void LoadWpfControl()
        {
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;
            TerrariumUserControl wpfControl = new TerrariumUserControl();
            host.Child = wpfControl;
            this.Controls.Add(host);
        }
    }
}
