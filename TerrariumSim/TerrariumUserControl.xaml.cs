using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Terrarium
{
    public partial class TerrariumUserControl : UserControl
    {
        public TerrariumUserControl()
        {
            InitializeComponent();
            Setup3DScene();
        }

        private void Setup3DScene()
        {
            Model3DGroup modelGroup = new Model3DGroup();

            GeometryModel3D cube = CreateCubeModel();
            modelGroup.Children.Add(cube);

            DirectionalLight dirLight = new DirectionalLight();
            dirLight.Color = Colors.White;
            dirLight.Direction = new Vector3D(-1, -1, -1);
            modelGroup.Children.Add(dirLight);

            ModelVisual3D modelsVisual = new ModelVisual3D();
            modelsVisual.Content = modelGroup;

            viewport.Children.Add(modelsVisual);
        }

        private GeometryModel3D CreateCubeModel()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection
            {
                new Point3D(-0.5, -0.5, -0.5),
                new Point3D(0.5, -0.5, -0.5),
                new Point3D(0.5, 0.5, -0.5),
                new Point3D(-0.5, 0.5, -0.5),
                new Point3D(-0.5, -0.5, 0.5),
                new Point3D(0.5, -0.5, 0.5),
                new Point3D(0.5, 0.5, 0.5),
                new Point3D(-0.5, 0.5, 0.5)
            };
            mesh.TriangleIndices = new Int32Collection
            {
                0, 1, 2, 2, 3, 0, // Front
                4, 5, 6, 6, 7, 4, // Back
                0, 4, 7, 7, 3, 0, // Left
                1, 5, 6, 6, 2, 1, // Right
                0, 1, 5, 5, 4, 0, // Bottom
                3, 2, 6, 6, 7, 3  // Top
            };
            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Green));
            return new GeometryModel3D(mesh, material);
        }
    }
}
