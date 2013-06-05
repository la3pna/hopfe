using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;


namespace Curve_tracer
{

    public partial class Form1 : Form
    {
        float[] yArray;
        float[] xArray;
        int stepvalue;
        private static Bitmap bmpScreenshot;
        private static Graphics gfxScreenshot;
        int valueStep = 6;
       

        public Form1()
        {
            
            InitializeComponent();
            List<String> tList = new List<String>();

            comboBox1.Items.Clear();

            foreach (string s in SerialPort.GetPortNames())
            {
                tList.Add(s);
            }

            tList.Sort();
            comboBox1.Items.Add("Select COM port...");
            comboBox1.Items.AddRange(tList.ToArray());
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            numericUpDown1.Value = 5;
            numericUpDown2.Value = 6;
        }


        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {

            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

            serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = 9600;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Parity = Parity.None;
            serialPort1.ReadBufferSize = 4096;
            serialPort1.NewLine = "\r\n";
            serialPort1.Handshake = Handshake.None;

            try
            {
                serialPort1.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void saveVectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
             Stream myStream ; 
            SaveFileDialog saveFileDialog1 = new SaveFileDialog(); 

            saveFileDialog1.Filter = "Comma separated (*.csv)|*.csv|Graph picture (*.bmp)|*.bmp|All files (*.*)|*.*" ; 
            saveFileDialog1.FilterIndex = 1 ; 
            saveFileDialog1.RestoreDirectory = true ; 

            if(saveFileDialog1.ShowDialog() == DialogResult.OK) 
            { 

                  if((myStream = saveFileDialog1.OpenFile()) != null) 
                  { 
                                      


                                         var extension = Path.GetExtension(saveFileDialog1.FileName);

                                         switch (extension.ToLower())
                                         {
                                             case ".bmp":



                                                 // Hide the form so that it does not appear in the screenshot
                                                 this.Hide();
                                                 // Set the bitmap object to the size of the screen
                                                 bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
                                                 // Create a graphics object from the bitmap
                                                 gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                                                 // Take the screenshot from the upper left corner to the right bottom corner
                                                 gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                                                 // Save the screenshot to the specified path that the user has chosen
                                                 bmpScreenshot.Save(saveFileDialog1.FileName, ImageFormat.Png);
                                                 // Show the form again

                                                 this.Show();
                                                 break;
                                             case ".csv":
                                                 StreamWriter wText =new StreamWriter(myStream);

                                      wText.WriteLine("Data from I/V analyzer");
                                     int length = yArray.Length;
                                     for (int i = 0; i <= length-1; i++)
                                       {
                                           wText.WriteLine(Convert.ToString(yArray[i]) + ',' + Convert.ToString(xArray[i])); 
                                       }
                                         wText.Flush();
                                         wText.Close();
                                                 break;
                                             default:
                                                 throw new ArgumentOutOfRangeException(extension);
                                         }


               }   





                       // Bitmap bmp = new Bitmap(panel1.Width, panel1.Height);
                       // panel1.DrawToBitmap(bmp, panel1.Bounds);
                       // bmp.Save(@"C:\Temp\Test.bmp");

                  
            }
        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "PNP")
            {
                label5.Text = "PNP";
                int MyInt = 83; // set S for the negative supply
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 74; // set J for the positive base supply
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
            
            }
            else if (comboBox2.Text == "NPN")
            {
                label5.Text = "NPN";
                int MyInt = 74; // set J for the postive base supply
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);

            }
            else if (comboBox2.Text == "J-FET (N)")
            {
                label5.Text = "J-FET";
                int MyInt = 73; // set I for the negative base supply
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
            

            }
            else if (comboBox2.Text == "MOSFET")
            {
                label5.Text = "MOS";
                int MyInt = 74; // set J for the positive base supply
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
            
            }
            else if (comboBox2.Text == "J-FET (P)")
            {
                label5.Text = "J-FET";
                int MyInt = 74; // set J for the positive base supply
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
            
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.DiscardInBuffer();
            int MyInt = 72; // set H for the sweep
            byte[] b = BitConverter.GetBytes(MyInt);
            serialPort1.Write(b, 0, 1);
            this.Invoke(new EventHandler(rs232datamottat));

        }


        private void rs232datamottat(object s, EventArgs e)
        {
            int steps =6;
            int length = steps * 125;
          //  int length = _serialPort.ReadBufferSize
            List<float> lista = new List<float>();
            List<float> listb = new List<float>();
            for (int i = 1; i <= (length-1); i++)
            {
                string servaluestrn = serialPort1.ReadLine();
                
                if (Regex.IsMatch(servaluestrn, @"\d"))
                {
                    string[] servalspl = servaluestrn.Split(' ');

                    string a = servalspl[0];
                    string b = servalspl[1];
                    
                    float aint = Convert.ToSingle(a);
                    float bint = Convert.ToSingle(b);
                    progressBar1.Value = Convert.ToInt32(b);
                    lista.Add(aint);
                    listb.Add(bint);

                   // int[] alist = lista.ToArray();
                   // int[] blist = listb.ToArray();

                }
            }
            float[] yArrayn = lista.ToArray();
            float[] xArrayn = listb.ToArray();

            int lengthArray = Convert.ToInt32(yArrayn.Length);
            List<float> listan = new List<float>();
            List<float> listbn = new List<float>();

            for (int i = 0; i <= (lengthArray - 5); i++)
            {
                listan.Add(Convert.ToSingle((yArrayn[i] + yArrayn[i + 1] + yArrayn[i + 2] + yArrayn[i + 3]) / 4.0));
                listbn.Add(Convert.ToSingle((xArrayn[i] + xArrayn[i + 1] + xArrayn[i + 2] + xArrayn[i + 3]) / 4.0));
               // yArray[i] = ;
               // xArray[i] = ;
                
            }
                  yArray = listan.ToArray();
                  xArray = listbn.ToArray();
                  progressBar1.Value = 0;
                  serialPort1.DiscardInBuffer();
                this.panel1.Invalidate();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox3_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text == "0")
            {
                label7.Text = "0";
                int MyInt = 65; // set switch 1
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 66; // set switch 2
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 67; // set switch 3
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 68; // set switch 4
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);

            }
            else if (comboBox3.Text == "10k")
            {
                label7.Text = "10k";
                int MyInt = 66; // set switch 2
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 67; // set switch 3
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 68; // set switch 4
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
             //   label8.Text = Convert.ToString((5 / stepvalue) / 10000);


            }
            else if (comboBox2.Text == "50k")
            {
                label7.Text = "50k";
                int MyInt = 65; // set switch 1
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 67; // set switch 3
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 68; // set switch 4
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);

            }
            else if (comboBox3.Text == "100k")
            {
                label7.Text = "100k";
                int MyInt = 65; // set switch 1
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 66; // set switch 2
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 68; // set switch 4
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);

            }
            else if (comboBox3.Text == "200k")
            {
                label7.Text = "200k";
                int MyInt = 65; // set switch 1
                byte[] b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 67; // set switch 3
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);
                MyInt = 66; // set switch 2
                b = BitConverter.GetBytes(MyInt);
                serialPort1.Write(b, 0, 1);

            }
        }

        private void panel1_paint(object sender, PaintEventArgs e)
        {
            if (yArray != null)
            {
                Pen Pen1;
                SolidBrush blueBrush;

                Graphics ClientDC = panel1.CreateGraphics();

                if (blueToolStripMenuItem.Checked == true)
                {
                     Pen1 = new Pen(System.Drawing.Color.Blue, 1);
                     blueBrush = new SolidBrush(Color.Blue);
                }
                else if (redToolStripMenuItem.Checked == true)
                {
                     Pen1 = new Pen(System.Drawing.Color.Red, 1);
                     blueBrush = new SolidBrush(Color.Red);
                }
                else
                {
                    Pen1 = new Pen(System.Drawing.Color.Green, 1);
                     blueBrush = new SolidBrush(Color.Green);
                }

               // Pen Pen1 = new Pen(System.Drawing.Color.Blue, 1);
               Pen Pen2 = new Pen(System.Drawing.Color.Black, 1);
             //   SolidBrush blueBrush = new SolidBrush(Color.Blue);


                Pen Pen3 = new Pen(System.Drawing.Color.Gray, 1);
                float xmax = xArray.Max()+1;
                float ymax = yArray.Max()+1;
                float xscale = panel1.Width;
                float yscale = panel1.Height;
                float length = yArray.Length;

                for (int i = 0; i < length - 1; i++)
                {

                    float xvalue = (xArray[i] / xmax)*xscale;
                
                  //  ClientDC.DrawLine(Pen1, (1*xscale), (0*yscale), (0*xscale), (1*yscale));

                    if (pointsToolStripMenuItem.Checked == true)
                    {

                        ClientDC.DrawLine(Pen1, ((xArray[i] / xmax) * xscale), (Math.Abs((yArray[i] / ymax) - 1) * yscale), ((xArray[i + 1] / xmax) * xscale), (Math.Abs((yArray[i + 1] / ymax) - 1) * yscale));
                    }
                    else
                    {
                        e.Graphics.FillRectangle(blueBrush, ((xArray[i] / xmax) * xscale), (Math.Abs((yArray[i] / ymax) - 1) * yscale), 4, 4);

                    }
                   

                }

                for (int i = 0; i <= 11; i++)
                {
                    ClientDC.DrawLine(Pen3, (i * xscale)/10, (0 * yscale), (i * xscale)/10, (1 * yscale));

                }

                for (int i = 0; i <= 11; i++)
                {
                    ClientDC.DrawLine(Pen3, (0 * xscale), (i * yscale)/10, (1 * xscale), (i * yscale)/10);
                }

                label9.Text = Convert.ToString(Math.Round((xmax / 10) * 0.00488,2)) + 'V';
                label10.Text = Convert.ToString(Math.Round((ymax / 10) * 0.00488,2)) ;

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pointsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void blueToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            redToolStripMenuItem.Checked = false;
            greenToolStripMenuItem.Checked = false;

        }

        private void redToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
           
            blueToolStripMenuItem.Checked = false;
            greenToolStripMenuItem.Checked = false;
        }

        private void greenToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            
            blueToolStripMenuItem.Checked = false;
            redToolStripMenuItem.Checked = false;
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
