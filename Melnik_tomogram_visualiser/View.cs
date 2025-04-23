using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melnik_tomogram_visualiser
{
    internal class View
    {

        public View() { }

        public static int clamp(int val, int min = 0, int max = 255)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }

        public static Color TransferFunc(short val)
        {
            int min = 0;
            int max = Bin.Z;
            int res = View.clamp((val - min)*255/(max - min));
            return Color.FromArgb(255, res, res, res);
        }

        public void SetupView(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        public void DrawQuads(int layerNum)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);
            for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
            {
                for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
                {
                    short val;

                    val = Bin.array[x_coord + y_coord * Bin.X + layerNum * Bin.X * Bin.Y];
                    GL.Color3(TransferFunc(val));
                    GL.Vertex2(x_coord, y_coord);

                    val = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNum * Bin.X * Bin.Y];
                    GL.Color3(TransferFunc(val));
                    GL.Vertex2(x_coord, y_coord + 1);

                    val = Bin.array[(x_coord + 1) + (y_coord + 1) * Bin.X + layerNum * Bin.X * Bin.Y];
                    GL.Color3(TransferFunc(val));
                    GL.Vertex2(x_coord + 1, y_coord + 1);

                    val = Bin.array[(x_coord + 1) + y_coord * Bin.X + layerNum * Bin.X * Bin.Y];
                    GL.Color3(TransferFunc(val));
                    GL.Vertex2(x_coord + 1, y_coord);
                }
            }
            GL.End();
        }
    }
}
