using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BrightnessContrast
{
    public partial class frmBrightness : Form
    {
        int width;
        int height;

        float gamma = 1;

        Bitmap bitmap;
        public frmBrightness()
        {
            InitializeComponent();
        }

        // Przycisk do zapisywania zdjecia
        private void BtnSave_Click(object sender, EventArgs e)
        {
            picBox.Image.Save("output.jpg");
        }

        // Przycisk do zamykania programu
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Otwieranie zdjecia
        private void BtnOpenImage_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName);
                picBox.Image = bitmap;

                width = picBox.Image.Width;
                height = picBox.Image.Height;
            }
        }

        // Regulacja jasnosci
        private void TraBrightness_Scroll(object sender, EventArgs e)
        {
            picBox.Image = AdjustBrightness(bitmap, (float)(traBrightness.Value / 100.0));
            lblBrightnessValue.Text = "Jasnosc = " + (traBrightness.Value / 100.0).ToString();
            picBox.Refresh();
        }
        private Bitmap AdjustBrightness(Image image, float brightness)
        {
            // Manioulacja RGB, ColorMatrix
            float b = brightness;
            ColorMatrix cm = new ColorMatrix(new float[][]
                {
                    new float[] {b, 0, 0, 0, 0},
                    new float[] {0, b, 0, 0, 0},
                    new float[] {0, 0, b, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1},
                });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(cm);

            // Rysuje obraz na nowej mapie bitowej, stosując nowy ColorMatrix. 
            Point[] points =
            {
                new Point(0, 0),
                new Point(image.Width, 0),
                new Point(0, image.Height),
            };
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            // Wynik bitmap
            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
            }

            // Zwraca wynik
            return bm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Pobiera wartosc pixela
                    Color p = bitmap.GetPixel(x, y);

                    // Wyodrebnia wartosci ARGB
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    // Znajduje natywna wartosc
                    r = 255 - r;
                    g = 255 - g;
                    b = 255 - b;

                    // Ustawia nowa wartosc pixelom
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            picBox.Image = bitmap;
        }
    }

}
