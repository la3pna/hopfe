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


namespace Curve_tracer
{

    public partial class Form1 : Form
    {
        float[] aArray;
        float[] bArray;
        int stepvalue;

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

                                                  Bitmap bmp = new Bitmap(panel1.Width, panel1.Height);
                                                  panel1.DrawToBitmap(bmp, panel1.Bounds);
                                                 // bmp.Save(@"C:\Temp\Test.bmp");
                                                 // bmp.Save(saveFileDialog1.FileName);

                                                 break;
                                             case ".csv":
                                                 StreamWriter wText =new StreamWriter(myStream);

                                      wText.WriteLine("Data from I/V analyzer");
                                     int length = aArray.Length;
                                     for (int i = 0; i <= length-1; i++)
                                       {
                                           wText.WriteLine(Convert.ToString(aArray[i]) + ',' + Convert.ToString(bArray[i])); 
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
            float[] aArrayn = lista.ToArray();
            float[] bArrayn = listb.ToArray();

            int lengthArray = Convert.ToInt32(aArrayn.Length);
            List<float> listan = new List<float>();
            List<float> listbn = new List<float>();

            for (int i = 0; i <= (lengthArray - 5); i++)
            {
                listan.Add(Convert.ToSingle((aArrayn[i] + aArrayn[i + 1] + aArrayn[i + 2] + aArrayn[i + 3]) / 4.0));
                listbn.Add(Convert.ToSingle((bArrayn[i] + bArrayn[i + 1] + bArrayn[i + 2] + bArrayn[i + 3]) / 4.0));
               // aArray[i] = ;
               // bArray[i] = ;
                
            }
                  aArray = listan.ToArray();
                  bArray = listbn.ToArray();
                  progressBar1.Value = 0;
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
            if (aArray != null)
            {


                Graphics ClientDC = panel1.CreateGraphics();
                Pen Pen1 = new Pen(System.Drawing.Color.Blue, 1);
                Pen Pen2 = new Pen(System.Drawing.Color.Red, 1);
                Pen Pen3 = new Pen(System.Drawing.Color.Gray, 1);
                float xmax = bArray.Max()+1;
                float ymax = aArray.Max()+1;
                float xscale = panel1.Width;
                float yscale = panel1.Height;
                float length = aArray.Length;

                for (int i = 0; i < length - 1; i++)
                {

                    float xvalue = (bArray[i] / xmax)*xscale;
                    SolidBrush blueBrush = new SolidBrush(Color.Blue);
                    e.Graphics.FillRectangle(blueBrush, ((bArray[i]/xmax)*xscale), (Math.Abs((aArray[i]/ymax)-1)*yscale), 4, 4);
                  //  ClientDC.DrawLine(Pen1, (1*xscale), (0*yscale), (0*xscale), (1*yscale));
                   // ClientDC.DrawLine(Pen2, ((bArray[i] / xmax) * xscale), ( Math.Abs((aArray[i] / ymax)-1) * yscale), ( (bArray[i + 1] / xmax) * xscale), ( Math.Abs((aArray[i + 1] / ymax)-1) * xscale));
               
                   

                }

                for (int i = 0; i <= 10; i++)
                {
                    ClientDC.DrawLine(Pen3, (i * xscale)/10, (0 * yscale), (i * xscale)/10, (1 * yscale));

                }

                for (int i = 0; i <= 10; i++)
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
    }
}