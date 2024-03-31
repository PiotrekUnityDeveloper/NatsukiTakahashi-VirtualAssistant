using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NatsukiLauncher
{
    public partial class ImageViewer : Form
    {
        public Image image;
        public string filePath;

        public ImageViewer()
        {
            InitializeComponent();
        }

        private void ImageViewer_Load(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Load(filePath);
                this.Text = Path.GetFileName(filePath);
                //pictureBox1.Image = image;
                //this.Text = "Image Preview";
            }
            catch { }
        }
    }
}
