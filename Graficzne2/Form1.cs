using System.Windows.Forms;

namespace Graficzne2
{
    public partial class Form1 : Form
    {
        Obj obj;
        public Form1()
        {
            InitializeComponent();

            Bitmap bitmap = new Bitmap(canvas.Width, canvas.Height);
            canvas.Image = bitmap;
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            double scale = canvas.Width / 2;
            double offset = 1;
            obj = new Obj(Constants.PathToSphere, scale, offset);

            Pen pen = new Pen(Color.Black);
            //Graphics graphics = canvas.CreateGraphics();
            foreach(var face in obj.Faces)
            {
                graphics.DrawLine(pen, obj.Points[face.P1].X, obj.Points[face.P1].Y, obj.Points[face.P2].X, obj.Points[face.P2].Y);
                graphics.DrawLine(pen, obj.Points[face.P1].X, obj.Points[face.P1].Y, obj.Points[face.P3].X, obj.Points[face.P3].Y);
                graphics.DrawLine(pen, obj.Points[face.P3].X, obj.Points[face.P3].Y, obj.Points[face.P2].X, obj.Points[face.P2].Y);
            }
            canvas.Refresh();
        }

        private void canvas_Click(object sender, EventArgs e)
        {

        }
    }
}