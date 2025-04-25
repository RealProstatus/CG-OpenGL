using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melnik_tomogram_visualiser
{
    internal class View
    {

        int VBOTexture;
        Bitmap textureImage;

        public View() { }

        public static int clamp(int val, int min = 0, int max = 255)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }

        public static Color TransferFunc(short val, int min, int width)
        {
            int _min = min;
            int max = min + width;
            int res = View.clamp((val - _min) *255/(max - _min));
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

        public void DrawQuads(int layerNum, int min, int width)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);
            for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
            {
                for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
                {
                    short val;

                    val = Bin.array[x_coord + y_coord * Bin.X + layerNum * Bin.X * Bin.Y];
                    GL.Color3(TransferFunc(val, min, width));
                    GL.Vertex2(x_coord, y_coord);

                    val = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNum * Bin.X * Bin.Y];
                    GL.Color3(TransferFunc(val, min, width));
                    GL.Vertex2(x_coord, y_coord + 1);

                    val = Bin.array[(x_coord + 1) + (y_coord + 1) * Bin.X + layerNum * Bin.X * Bin.Y];
                    GL.Color3(TransferFunc(val, min, width));
                    GL.Vertex2(x_coord + 1, y_coord + 1);

                    val = Bin.array[(x_coord + 1) + y_coord * Bin.X + layerNum * Bin.X * Bin.Y];
                    GL.Color3(TransferFunc(val, min, width));
                    GL.Vertex2(x_coord + 1, y_coord);
                }
            }
            GL.End();
        }

        public void load2DTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, VBOTexture);
            BitmapData data = textureImage.LockBits(
                new System.Drawing.Rectangle(0,0,textureImage.Width,textureImage.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);

            textureImage.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            ErrorCode err = GL.GetError();
            string msg = err.ToString();
        }

        public void generateTextureImage(int layerNum, int min, int width)
        {
            textureImage = new Bitmap(Bin.X, Bin.Y);

            for(int i = 0; i < Bin.X; i++)
            {
                for(int j = 0; j < Bin.Y; j++)
                {
                    int pixelNumber = i + j * Bin.X + layerNum * Bin.X * Bin.Y;
                    textureImage.SetPixel(i, j, TransferFunc(Bin.array[pixelNumber], min, width));
                }
            }
        }

        public void drawTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOTexture);

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);

            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);

            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Y);

            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Y);

            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.X, 0);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
    }
}
