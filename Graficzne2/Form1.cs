using System.Diagnostics;
using Graficzne2.Objects;
using Timer = System.Windows.Forms.Timer;

namespace Graficzne2
{
    public partial class Form1 : Form
    {
        Face[] faces;
        Graphics graphics;
        Pen pen = new Pen(Color.Black);
        DirectBitmap bitmap;
        LightSource lightSource;
        Timer timer;
        bool useNormals = true;
        Cloud cloud;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Form1()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            InitializeComponent();

            SetUpCanvas();
            SetUpLightSource();
            cloud = new Cloud(lightSource.LightLocation.Z / 3);
            SetUpTimer();
            SetUpFaces(Constants.SphereLocation);
            Draw();
        }

        private void SetUpCanvas()
        {
            canvas.Height = 700;
            canvas.Width = 700;

            var normals = new Bitmap(Constants.DefaultNormalsLocation);
            var texture = new Bitmap(Constants.DefaultTextureLocation);
            bitmap = new DirectBitmap(canvas.Width, canvas.Height, normals, texture);
            canvas.Image = bitmap.Bitmap;
            graphics = Graphics.FromImage(bitmap.Bitmap);
            graphics.Clear(Color.White);
        }

        private void SetUpLightSource() =>
            lightSource = new LightSource(0.5, 0.5, 1, Color.White, new Point3d(Constants.LightSourceX, Constants.LightSourceY, Constants.MinLightHeight));

        private void SetUpTimer()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Enabled = false;
            timer.Interval = 1000 / Constants.fps;
        }

        private void SetUpFaces(string path)
        {
            double scale = canvas.Width / 2;
            double offset = 1;
            faces = Utils.SetUpFaceArrayFromFile(path, scale, offset);
        }

        private void DrawTriangles()
        {
            foreach (var face in faces)
            {
                graphics.DrawLine(pen, face.P1.TwoDInverseY(canvas.Height), face.P2.TwoDInverseY(canvas.Height));
                graphics.DrawLine(pen, face.P1.TwoDInverseY(canvas.Height), face.P3.TwoDInverseY(canvas.Height));
                graphics.DrawLine(pen, face.P3.TwoDInverseY(canvas.Height), face.P2.TwoDInverseY(canvas.Height));
            }
            canvas.Refresh();
        }

        private void ColorFaces()
        {
            foreach (var face in faces)
            {
                face.Color(bitmap, colorDialog.Color, lightSource, useNormals, interpolateCornersButton.Checked, textureCheckBox.Checked);
            }
        }

        private void Draw()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            graphics.Clear(Color.White);

            ColorFaces();
            if (drawTrianglesCheckbox.Checked) DrawTriangles();
            if (cloudCheckBox.Checked) DrawCloud();
            canvas.Refresh();
            s.Stop();
        }

        private void DrawCloud()
        {
            cloud.DrawShadow(bitmap, lightSource);
            cloud.DrawCloud(bitmap);
        }

        private void DrawIfTimerNotRunning()
        {
            if (!timer.Enabled) Draw();
        }

        private void OnTimedEvent(object source, EventArgs e)
        {
            lightSource.Rotate(Constants.RotationDegrees, canvas.Width / 2, canvas.Height / 2);
            cloud.Move(bitmap.Width, Constants.CloudMovement);
            Draw();
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            if (timer.Enabled) timer.Enabled = false;
            else timer.Enabled = true;
        }

        private void chooseColorButton_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            DrawIfTimerNotRunning();
        }

        private void lightColorButton_Click(object sender, EventArgs e)
        {
            if (lightColorDialog.ShowDialog() == DialogResult.OK)
                lightSource.LightColor = lightColorDialog.Color;
            DrawIfTimerNotRunning();
        }

        private void kdBar_Scroll(object sender, EventArgs e)
        {
            lightSource.Kd = (double)kdBar.Value / 10;
            DrawIfTimerNotRunning();
        }

        private void ksBar_Scroll(object sender, EventArgs e)
        {
            lightSource.Ks = (double)ksBar.Value / 10;
            DrawIfTimerNotRunning();
        }

        private void kaBar_Scroll(object sender, EventArgs e)
        {
            lightSource.Ka = (double)kaBar.Value / 10;
            DrawIfTimerNotRunning();
        }

        private void mBar_Scroll(object sender, EventArgs e)
        {
            lightSource.M = mBar.Value * 10 + 1;
            DrawIfTimerNotRunning();
        }

        private void zBar_Scroll(object sender, EventArgs e)
        {
            lightSource.LightLocation.Z = Constants.MinLightHeight + zBar.Value * 50;
            cloud.Height = lightSource.LightLocation.Z / 3;
            DrawIfTimerNotRunning();
        }

        private void interpolateCornersButton_CheckedChanged(object sender, EventArgs e)
        {
            DrawIfTimerNotRunning();
        }

        private void interpolateEachButton_CheckedChanged(object sender, EventArgs e)
        {
            DrawIfTimerNotRunning();
        }

        private void drawTrianglesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            DrawIfTimerNotRunning();
        }

        private void loadNormalsButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Normals";
                dlg.InitialDirectory = Path.GetFullPath(Constants.NormalsLocation);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    bitmap.SetUpNormalMap(new Bitmap(dlg.FileName));
                    useNormals = true;
                    DrawIfTimerNotRunning();
                }
            }
        }

        private void resetNormalsButton_Click(object sender, EventArgs e)
        {
            useNormals = false;
            DrawIfTimerNotRunning();
        }

        private void loadObjectButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Object";
                dlg.InitialDirectory = Path.GetFullPath(Constants.ObjectLocation);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SetUpFaces(dlg.FileName);
                    SetUpCanvas();
                    DrawIfTimerNotRunning();
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            DrawIfTimerNotRunning();
        }

        private void textureButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Texture";
                dlg.InitialDirectory = Path.GetFullPath(Constants.TexturesLocation);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    bitmap.SetUpTexture(new Bitmap(dlg.FileName));
                    DrawIfTimerNotRunning();
                }
            }
        }

        private void cloudCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DrawIfTimerNotRunning();
        }
    }
}