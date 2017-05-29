using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgCrypt
{
    public partial class Form1 : Form
    {
        public static bool ch = false;

        public static string pol;

        // Порождающая матрица
        public static int[,] G =
         {

         };

        public static int k;
        public static int n;
        public static int m;

        // Байт-массив исходного изображения
        static byte[] byte_img;

        //
        static string[] m2;

        //
        static byte[] bytes;

        // Изображение
        static Image img;

        public Form1()
        {
            InitializeComponent();
        }

        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            /*
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }*/
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        /*
        private static byte[] convertToBMP(Bitmap myImage)
        {
            byte[] rgbValues = null;

            rect = new Rectangle(0, 0, myImage.Width, myImage.Height);

            BitmapData data = myImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format48bppRgb);

            try
            {
                IntPtr ptr = data.Scan0;
                int bytes = Math.Abs(data.Stride) * myImage.Height;
                rgbValues = new byte[bytes];
                Marshal.Copy(ptr, rgbValues, 0, bytes);
                return rgbValues;
            }
            finally
            {
                myImage.UnlockBits(data);
            }
        }
        
        private static Bitmap convertToJPEG(byte[] rgbValues, int Width, int Height)
        {
            var b = new Bitmap(Width, Height, PixelFormat.Format48bppRgb);

            ColorPalette ncp = b.Palette;
            for (int i = 0; i < 256; i++)
                ncp.Entries[i] = Color.FromArgb(255, i, i, i);
            b.Palette = ncp;

            var BoundsRect = rect;//new Rectangle(0, 0, Width, Height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = bmpData.Stride * b.Height;

            Marshal.Copy(rgbValues, 0, ptr, bytes);
            b.UnlockBits(bmpData);
            return b;
        }*/

        public static string ToFileSize(string filename)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = new FileInfo(filename).Length;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                disposeAll();
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Open Image";
                    dlg.Filter = "JPG files (*.jpg)|*.jpg";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox1.Image = new Bitmap(dlg.FileName);
                        namefield.Text = dlg.SafeFileName;
                        imgInfoText.Text = "Dim: " + pictureBox1.Image.Width + "x" + pictureBox1.Image.Height + Environment.NewLine + "Size: " + ToFileSize(dlg.FileName);
                        // Считываем изображение из файла
                        img = Image.FromFile(dlg.FileName);
                        tableLayoutPanel2.Enabled = true;
                        loadButton.Enabled = true;
                        //Bitmap bt = new Bitmap(img);
                        //byte_img = convertToBMP(bt);
                    }
                }
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show("Указанный файл не найден!");
            }
            catch(OutOfMemoryException)
            {
                MessageBox.Show("Файл не является допустимым форматом изображения или GDI+ не поддерживает формат пикселей файла!");
            }
            catch(ArgumentException)
            {
                MessageBox.Show("Переданный аргумент (файл) не был успешно обработан!");
            }
        }

        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        private string encrypt(string[] input)
        {
            string result = "";
            if (hamRad.Checked)
            {
                for (int m = 0; m < input.Length; m++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        int sum = 0;
                        for (int j = 0; j < k; j++)
                        {
                            sum += input[m][j] * G[j, i];
                        }
                        if (sum % 2 == 0) result += "0";
                        else result += "1";
                    }
                }
            }
            return result;
        }

        private string CRCEncrypt(string input)
        {
            int[] crc = new int[pol.Length];
            int[] prevCrc = new int[pol.Length];

            for (int i = 0; i < crc.Length; i++)
            {
                crc[i] = 0;
                prevCrc[i] = 0;
            }

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < crc.Length - 1; j++)
                {
                    crc[j] = (prevCrc[j + 1] + ((input[i] + prevCrc[0]) * pol[j])) % 2;
                }

                crc[crc.Length - 1] = (prevCrc[0] + input[i]) % 2;

                for (int j = 0; j < crc.Length; j++)
                {
                    prevCrc[j] = crc[j];
                }
            }
            return String.Join("", crc.Select(p => p.ToString()).ToArray());
        }


        private void imgInfoText_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            disposeAll();
            G = null;
        }

        private void disposeAll()
        {
            namefield.Clear();
            imgInfoText.Clear();
            pictureBox1.Image = null;
            img = null;
            byte_img = null;
            inputBinaryField.Text = null;
            inputByteField.Text = null;
            outputBinaryField.Text = null;
            outputByteField.Text = null;
            m2 = null;
            bytes = null;
            encryptButton.Enabled = false;
            button2.Enabled = false;
            tableLayoutPanel2.Enabled = false;
        }

        private void loadButton_Click_1(object sender, EventArgs e)
        {
            // Перевод изображение в байт-массив
            byte_img = ImageToByteArray(img);

            try
            {

                // Отображаем байт-массив исходного изображения
                inputByteField.Text = HexDump(byte_img);
                // Отображаем бит-массив исходного изображения
                inputBinaryField.Text = string.Join(" ", byte_img.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка! " + ex);
            }
            finally
            {
                if (!(G == null || G.Length == 0))
                {
                    encryptButton.Enabled = true;
                }
                loadButton.Enabled = false;
            }
        }

        private void encryptButton_Click(object sender, EventArgs e)
        {

                m = n - k;
                string[] ims = byte_img.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).ToArray();
                string mg = string.Join("", ims);

                string en = "";

                if (hamRad.Checked)
                {
                    m2 = new string[mg.Length / k];

                    for (int i = 0; i < mg.Length / k; i++)
                    {
                        for (int j = 0; j < k; j++)
                        {
                            m2[i] += mg[i * k + j];
                        }
                    }
                    en = encrypt(m2);

                }
                if (CRCRad.Checked)
                {
                    en = CRCEncrypt(mg);
                }

            outputBinaryField.Text = Regex.Replace(en, ".{8}", "$0 ");

            int numOfBytes = en.Length / 8;
            bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(en.Substring(8 * i, 8), 2);
            }

            outputByteField.Text = HexDump(bytes);

            button5.Enabled = true;
                button2.Enabled = true;
                encryptButton.Enabled = false;

            /*
            Image image = byteArrayToImage(bytes);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.jpg";
            ImageFormat format = ImageFormat.Bmp;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                image.Save(sfd.FileName, ImageFormat.Bmp);
            }*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.jpg";
            ImageFormat format = ImageFormat.Bmp;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, bytes);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Matrix mx = new Matrix();
            mx.evtFrm += new ShowFrm(manageButton);
            mx.ShowDialog();
        }

        void manageButton()
        {
            encryptButton.Enabled = true;
            loadButton.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            outputBinaryField.Text = null;
            outputByteField.Text = null;
            m2 = null;
            bytes = null;
            if (G.Length != 0)
            {
                encryptButton.Enabled = true;
            }
            button2.Enabled = false;
            button5.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CRC mx = new CRC();
            mx.evtFrm2 += new ShowFrm(manageButton);
            mx.ShowDialog();
        }
    }
}
