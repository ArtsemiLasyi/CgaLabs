using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CgaLab.Presentation
{
    public partial class FormACG : Form
    {
        public Graphics graphics;
        public BufferedGraphicsContext bufferedGraphicsContext;
        public BufferedGraphics bufferedGraphics;

        public FormACG()
        {
            InitializeComponent();
            InitPictureBox();
            DrawModel();
        }

        public void InitPictureBox()
        {
            pictureBox.Width = Size.Width;
            pictureBox.Height = Size.Height;
            graphics = pictureBox.CreateGraphics();
            bufferedGraphicsContext = new BufferedGraphicsContext();
            bufferedGraphics = bufferedGraphicsContext.Allocate(
                graphics,
                new Rectangle(
                    0,
                    0,
                    pictureBox.Width,
                    pictureBox.Height
                )
            );
            bufferedGraphics
                .Graphics
                .FillRectangle(
                    new SolidBrush(Color.Black),
                    0,
                    0,
                    pictureBox.Width,
                    pictureBox.Height
                );
            bufferedGraphics.Render();
        }

        public void DrawModel()
        {
            bufferedGraphics
                .Graphics
                .DrawRectangle(
                    new Pen(new SolidBrush(Color.Black)),
                    0,
                    0,
                    150,
                    150
                );
            bufferedGraphics.Render();
        }

        private void FormACG_Resize(object sender, EventArgs e)
        {
            pictureBox.Width = Size.Width;
            pictureBox.Height = Size.Height;
        }
    }
}
