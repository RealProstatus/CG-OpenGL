using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melnik_tomogram_visualiser
{
    internal class Bin
    {
        public static int X, Y, Z;
        public static short[] array;
        public Bin() { }

        public void readBIN(string path)
        {
            if (File.Exists(path))
            {
                BinaryReader reader =
                    new BinaryReader(File.Open(path, FileMode.Open));

                X = reader.ReadInt32();
                Y = reader.ReadInt32();
                Z = reader.ReadInt32();

                int arraySize = X * Y * Z;
                array = new short[arraySize];

                int x_offset = 6;

                for (int z = 0; z < Z; ++z)
                {
                    for (int y = 0; y < Y; ++y)
                    {
                        int ind_x = 0;
                        short[] tmp_x = new short[x_offset];

                        for (int x = 0; x < x_offset; ++x)
                        {
                            tmp_x[x] = reader.ReadInt16();
                        }

                        for (int x = 0; x < X - x_offset; ++x)
                        {
                            array[z * X * Y + y * X + ind_x] = reader.ReadInt16();
                            ind_x++;
                        }

                        for (int i = 0; i < x_offset; ++i)
                        {
                            array[z * X * Y + y * X + ind_x] = tmp_x[i];
                            ind_x++;
                        }
                    }
                }
            }
        }
    }
}
