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

        public Form1()
        {
            InitializeComponent();
            bin = new Bin();
        }
    }
}
