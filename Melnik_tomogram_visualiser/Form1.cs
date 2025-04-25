using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Melnik_tomogram_visualiser
{
    public partial class Form1 : Form
    {

        Bin bin;
        View view;
        bool needReload;
        bool loaded;
        bool useTexture;
        int currentLayer;

        int frameCount;
        DateTime nextFPSUpdate = DateTime.Now.AddSeconds(1);

        public Form1()
        {
            InitializeComponent();
            bin = new Bin();
            view = new View();
            loaded = needReload = useTexture = false;
            currentLayer = 0;

            this.Load += Form1_Load1;
        }

        private void Form1_Load1(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                string name = ofd.FileName;
                bin.readBinary(name);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                trackBar1.Maximum = Bin.Z - 1;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                if (useTexture)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.load2DTexture();
                        needReload = false;
                    }
                    view.drawTexture();
                }
                else
                {
                    view.DrawQuads(currentLayer);
                }

                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
            glControl1.Invalidate();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void displayFPS()
        {
            if(DateTime.Now >= nextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps = {0})", frameCount);
                nextFPSUpdate = DateTime.Now.AddSeconds(1);
                frameCount = 0;
            }
            frameCount++;
        }

        private void rb_quads_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_quads.Checked)
            {
                useTexture = false;
                needReload = true;
            }
        }

        private void rb_textures_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_textures.Checked)
            {
                useTexture = true;
                needReload = true;
            }
        }
    }
}
