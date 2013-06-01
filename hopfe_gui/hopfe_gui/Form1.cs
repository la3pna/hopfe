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

            saveFileDialog1.Filter = "Comma separated (*.csv)|*.csv|Graph picture (*.jpg)|*.jpg|All files (*.*)|*.*" ; 
            saveFileDialog1.FilterIndex = 1 ; 
            saveFileDialog1.RestoreDirectory = true ; 

            if(saveFileDialog1.ShowDialog() == DialogResult.OK) 
            { 
                  if((myStream = saveFileDialog1.OpenFile()) != null) 
                  { 
                        StreamWriter wText =new StreamWriter(myStream);

                        wText.WriteLine("Version 4");
                        wText.Flush();
                        wText.Close();
                        
                  }
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
            for (int i = 1; i <= 1024; i++)
            {
                string servaluestrn = serialPort1.ReadLine();
                textBox1.Text = servaluestrn;
                if (Regex.IsMatch(servaluestrn, @"\d"))
                {
                    string[] servalspl = servaluestrn.Split(' ');

                    string a = servalspl[0];
                    string b = servalspl[1];
                    int aint = Convert.ToInt32(a);
                    int bint = Convert.ToInt32(b);
                    progressBar1.Value = Convert.ToInt32(b);
                }
            }
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
    }
}