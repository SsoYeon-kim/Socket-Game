using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using WMPLib;
using System.IO;

namespace Client
{
    public partial class ClientForm : Form
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream stream = default(NetworkStream);
        WindowsMediaPlayer player_bell = new WindowsMediaPlayer();

        string[] q = new string[10];

        public static Random rnd = new Random();

        FileStream fs;
        StreamWriter sw;

        public static string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public ClientForm()
        {
            q[0] = "무엇을 타고 등교하나요?";
            q[1] = "아침 먹었나요?";
            q[2] = "수업은 어떤가요?";
            q[3] = "피곤하나요?";
            q[4] = "잠을 얼마나 잤나요?";
            q[5] = "과제의 난이도가 어떤가요?";
            q[6] = "오늘의 기분은?";
            q[7] = "학식에 대해서 어떻게 생각하나요?";
            q[8] = "어떤 수업이 가장 재밌나요?";
            q[9] = "왕한호 교수님은 어떤가요?";

            InitializeComponent();

            int num = rnd.Next(0, 10);

            if (num < 11)
            {
                label_question.Text = q[num];
            }
           
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            fs = new FileStream(folder + @"\설문조사.txt", FileMode.Append, FileAccess.Write);
            sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            sw.WriteLine(label_question.Text + "  " + textBox_answer.Text);
            sw.Close();

            panel_Survey.SendToBack();
            player_bell.URL = "종소리2.wav";
            player_bell.controls.play();
        }

        private void button_First_Enter_Click(object sender, EventArgs e)
        {
            player_bell.controls.stop();

            clientSocket.Connect("127.0.0.1", 9999);
            stream = clientSocket.GetStream();             

            byte[] buffer = Encoding.Unicode.GetBytes(textBox_Name.Text + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            client_Panel();

            Thread t = new Thread(getMessage);
            t.IsBackground = true;
            t.Start();
        }
        private void getMessage()
        {
            while (true)
            {
                stream = clientSocket.GetStream();
                int size = clientSocket.ReceiveBufferSize;
                byte[] buffer = new byte[size];
                int bytes = stream.Read(buffer, 0, buffer.Length);

                string message = Encoding.Unicode.GetString(buffer, 0, bytes);

            }
        }
        private void client_Panel()
        {
            while (true)
            {
                stream = clientSocket.GetStream();
                int size = clientSocket.ReceiveBufferSize;
                byte[] buffer = new byte[size];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.Unicode.GetString(buffer, 0, bytes);

                if (message == "1")
                {
                    panel_B.Visible = true;
                    panel_B.BringToFront();
                    panel_B.Dock = DockStyle.Fill;
                    panel_B.Refresh();
                    break;
                }
                else
                {
                    panel_G.Visible = true;
                    panel_G.BringToFront();
                    panel_G.Dock = DockStyle.Fill;
                    panel_G.Refresh();
                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bad Point 3점 당 -5 Hp\n\nGood Point 4점 당 +3Hp\n\n선생님 :  발견하면 +8 Hp   모르면 -5 Hp");
        }

        private void button_B_dance_Click(object sender, EventArgs e)   //양아치 춤추기 누르면 서버에 보내기
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes("누군가 춤추는 중" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void button_B_dancestop_Click(object sender, EventArgs e)       //양아치 멈추기 누르면 서버에 보내기
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes("누군가의 춤 멈춤" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void button_B_sleep_Click(object sender, EventArgs e)        //양아치 자기 누르면 서버에 보내기
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes("누군가 자는중" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void button_B_wake_Click(object sender, EventArgs e)            //양아치 깨우기 누르면 서버에 보내기
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes("누군가 일어남" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void button_G_eat_Click(object sender, EventArgs e)     //모범생 과자먹기 누르면 서버에 보내기
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes("누군가 과자먹음" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void button_G_eatstop_Click(object sender, EventArgs e)
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes("과자먹기를 멈춤" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void button_G_sleep_Click(object sender, EventArgs e)        //모범생 졸기 누르면 서버에 보내기
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes("누군가 졸음" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void button_G_wake_Click(object sender, EventArgs e)
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes("누군가 깨어남" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void button_B_send_Click(object sender, EventArgs e)
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes(textBox_B.Text.ToString() + "$남");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();

            textBox_B.Clear();
        }

        private void button_G_send_Click(object sender, EventArgs e)
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes(textBox_G.Text.ToString() + "$여");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();

            textBox_G.Clear();
        }

        
    }
}
