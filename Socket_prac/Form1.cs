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
using Timer = System.Windows.Forms.Timer;
using Socket_prac;

namespace Server
{
	public partial class Form1 : Form
	{
        BadStudent bs;
        GoodStudent gs;
        Teacher teacher;

        public static Random rand = new Random();
        int fortimerinterval1;
        int fortimerinterval2;

        TcpListener server = null;
		TcpClient clientSocket = null;

		public Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();

        public int count = 0;

        Label[] label = new Label[2];
        private String[] UserArr = new String[2];
        private static int userCnt = 0;

        public Form1()
		{
			InitializeComponent();
            
            bs = new BadStudent();
            gs = new GoodStudent();
            teacher = new Teacher();
           
            richTextBox1.AppendText("");

            Random rand = new Random();              //  선생님 랜덤으로 뒤돌기
            fortimerinterval1 = rand.Next(7000, 11000);
            Timer timer1 = new Timer();
            timer1.Interval = fortimerinterval1;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();

            fortimerinterval2 = rand.Next(1000, 5000);
            Timer timer2 = new Timer();
            timer2.Interval = fortimerinterval2;
            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Start();

            Thread t = new Thread(initSocket);
            t.IsBackground = true;
            t.Start();
        }

        private void initSocket()
		{
			server = new TcpListener(IPAddress.Any, 9999);     //IPAddress.Any  - 모든 아이피에서 접속이 가능하다    / 특정아이피를 적을수도 있음
			clientSocket = default(TcpClient);
			server.Start();

            richTextBox1.Invoke(new Action(() => richTextBox1.AppendText("출석 시작!\n")));

            while (true)
			{
				try
                { 
                    clientSocket = server.AcceptTcpClient();     // 여기 밑에 코드부터는 서버에 연결이 안되면 실행이 안돼 여기서 멈추고 접속을 기다리는거야 접속되면 밑에 코드가 실행됩니다

                    NetworkStream stream = clientSocket.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string userName = Encoding.Unicode.GetString(buffer, 0, bytes);
                    userName = userName.Substring(0, userName.IndexOf('$'));

                    clientList.Add(clientSocket, userName);
                    UserArr[userCnt++] = userName;
                    richTextBox1.Invoke(new Action(() => richTextBox1.AppendText(userName + " 등교 완료\n")));

                    if (clientList.Count == 1)
                    {
                        pictureBox_B_normal.BringToFront();
                        label_B_BP.BringToFront();
                        label_B_GP.BringToFront();
                        Label_B_Hp.BringToFront();
                        label4.BringToFront();
                        label6.BringToFront();
                        label8.BringToFront();
                        label_B_chat.BringToFront();
                        count++;
                    }
                    else
                    {
                        pictureBox_G_normal.BringToFront();
                        pictureBox_T_teaching.BringToFront();
                        label_T_teach.BringToFront();
                        label3.BringToFront();
                        label5.BringToFront();
                        label7.BringToFront();
                        label9.BringToFront();
                        label_G_BP.BringToFront();
                        label_G_GP.BringToFront();
                        Label_G_Hp.BringToFront();
                        label_G_chat.BringToFront();
                        Label_T_Hp.BringToFront();

                        richTextBox1.Invoke(new Action(() => richTextBox1.AppendText("지각생 없음! 수업시작!!\n")));
                        
                        Label_B_Hp.Text = bs.hp + "";
                        Label_G_Hp.Text = gs.hp + "";
                        Label_T_Hp.Text = teacher.hp + "";

                    }
                    
                    numPass(count);
                    count++;
                   
                    for (int i=0; i<1000; i++)
                    {
                        startClient_Work(clientSocket, clientList);
                        i++;
                    }

                }
				catch(SocketException se)
				{
					break;
				}
				catch(Exception e)
				{
					break;
				}
            }
		}

        public void numPass(int num)            //번호 넘길거야
        {
            NetworkStream stream = null;
            byte[] buffer = new byte[1024];
            stream = clientSocket.GetStream();
            string num_string = num.ToString();
            buffer = Encoding.Unicode.GetBytes(num_string);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        public void startClient_Work(TcpClient clientSocket, Dictionary<TcpClient, string> clientList)       /////////////////////////////
        {
            this.clientSocket = clientSocket;
            this.clientList = clientList;

            Thread work = new Thread(client_Work);
            work.IsBackground = true;
            work.Start();
        }

        public void client_Work()     
        {
            NetworkStream stream = null;
            
            byte[] buffer = new byte[1024];
            string msg = "";
            int bytes = 0;

            while (true)
            {
                stream = clientSocket.GetStream();
                bytes = stream.Read(buffer, 0, buffer.Length);
                msg = Encoding.Unicode.GetString(buffer, 0, bytes);
                String[] text = msg.Split('$');
                msg = text[0];
                if (text[1].Equals("남"))
                {
                    label_B_chat.Invoke(new Action(() => label_B_chat.Text = "( " + msg + " )\n"));
                }
                else if (text[1].Equals("여"))
                {
                    label_G_chat.Invoke(new Action(() => label_G_chat.Text = "( " + msg + " )\n"));
                }

                else if (msg == "누군가 춤추는 중")
                {
                    pictureBox_B.BringToFront();
                    pictureBox_B.Image = pictureBox_B_dance_right.Image;
                }
                else if (msg == "누군가의 춤 멈춤")
                {
                    pictureBox_B.BringToFront();
                    pictureBox_B.Image = pictureBox_B_normal.Image;
                }
                else if (msg == "누군가 자는중")
                {
                    pictureBox_B.BringToFront();
                    pictureBox_B.Image = pictureBox_B_sleep.Image;
                }
                else if (msg == "누군가 일어남")
                {
                    pictureBox_B.BringToFront();
                    pictureBox_B.Image = pictureBox_B_normal.Image;
                }
                else if (msg == "누군가 과자먹음")
                {
                    pictureBox_G.BringToFront();
                    pictureBox_G.Image = pictureBox_G_eat.Image;
                }
                else if (msg == "과자먹기를 멈춤")
                {
                    pictureBox_G.BringToFront();
                    pictureBox_G.Image = pictureBox_G_normal.Image;
                }
                else if (msg == "누군가 졸음")
                {
                    pictureBox_G.BringToFront();
                    pictureBox_G.Image = pictureBox_G_sleep.Image;
                }
                else if (msg == "누군가 깨어남")
                {
                    pictureBox_G.BringToFront();
                    pictureBox_G.Image = pictureBox_G_normal.Image;
                }
            }
        }


            private void timer1_Tick(object sender, EventArgs e)        //선생님 화난거 랜덤
        {
            pictureBox_T.BringToFront();
            label_T_teach.BringToFront();
            pictureBox_T.Image = pictureBox_T_angry.Image;
            label_T_teach.Text = teacher.Angry();
            
            if(pictureBox_B.Image == pictureBox_B_dance_right.Image)        //양아치 춤추다가 걸리면
            {
                pictureBox_T.Image = pictureBox_T_bump.Image;
                label_T_teach.Text = teacher.Bump();

                bs.flag = true;
                bs.badPoint += bs.Dance();
                bs.CalHp(bs.Dance(), 0);

                if(bs.hp <= 0)
                {
                    timer1.Stop();
                    timer2.Stop();
                    pictureBox_T.Visible = false;
                    pictureBox_T_angry.Visible = false;
                    pictureBox_T_teaching.Visible = false;
                    label_T_teach.Visible = false;
                    panel1.BringToFront();
                    panel1.Dock = DockStyle.Fill;
                    panel1.Refresh();
                    pictureBox_outside.BringToFront();

                    pictureBox_Sad.BringToFront();
                    pictureBox_Sad.Image = pictureBox_B_Sad.Image;
                }

                label_B_BP.Text = bs.badPoint + "";
                label_B_GP.Text = bs.goodPoint + "";

                Label_B_Hp.Text = bs.hp + "";

                teacher.hp += 8;

                Label_T_Hp.Text = teacher.hp + "";
            }
            if(pictureBox_B.Image == pictureBox_B_sleep.Image)        //양아치 자다가 걸리면
            {
                pictureBox_T.Image = pictureBox_T_bump.Image;
                label_T_teach.Text = teacher.Bump();

                bs.flag = true;
                bs.badPoint += bs.Sleep();
                bs.CalHp(bs.Sleep(), 0);

                if (bs.hp <= 0)
                {
                    timer1.Stop();
                    timer2.Stop();
                    pictureBox_T.Visible = false;
                    pictureBox_T_angry.Visible = false;
                    pictureBox_T_teaching.Visible = false;
                    label_T_teach.Visible = false;
                    panel1.BringToFront();
                    panel1.Dock = DockStyle.Fill;
                    panel1.Refresh();
                    pictureBox_outside.BringToFront();

                    pictureBox_Sad.BringToFront();
                    pictureBox_Sad.Image = pictureBox_B_Sad.Image;
                }

                label_B_BP.Text = bs.badPoint + "";
                label_B_GP.Text = bs.goodPoint + "";

                Label_B_Hp.Text = bs.hp + "";

                teacher.hp += 8;

                Label_T_Hp.Text = teacher.hp + "";
            }
            if(pictureBox_G.Image == pictureBox_G_eat.Image)        //모범생 먹다가 걸리면
            {
                pictureBox_T.Image = pictureBox_T_bump.Image;
                label_T_teach.Text = teacher.Bump();

                gs.flag = true;
                gs.badPoint += gs.Eat();
                gs.CalHp(gs.Eat(), 0);
                if (gs.hp <= 0)
                {
                    timer1.Stop();
                    timer2.Stop();
                    pictureBox_T.Visible = false;
                    pictureBox_T_angry.Visible = false;
                    pictureBox_T_teaching.Visible = false;
                    label_T_teach.Visible = false;
                    panel1.BringToFront();
                    panel1.Dock = DockStyle.Fill;
                    panel1.Refresh();
                    pictureBox_outside.BringToFront();

                    pictureBox_Sad.BringToFront();
                    pictureBox_Sad.Image = pictureBox_G_Sad.Image;
                }

                label_G_BP.Text = gs.badPoint + "";
                label_G_GP.Text = gs.goodPoint + "";

                Label_G_Hp.Text = gs.hp + "";

                teacher.hp += 8;

                Label_T_Hp.Text = teacher.hp + "";
            }
            if(pictureBox_G.Image == pictureBox_G_sleep.Image)        //모범생 졸다가 걸리면
            {
                pictureBox_T.Image = pictureBox_T_bump.Image;
                label_T_teach.Text = teacher.Bump();

                gs.flag = true;
                gs.badPoint += gs.Sleep();
                gs.CalHp(gs.Sleep(), 0);

                if (gs.hp <= 0)
                {
                    timer1.Stop();
                    timer2.Stop();
                    pictureBox_T.Visible = false;
                    pictureBox_T_angry.Visible = false;
                    pictureBox_T_teaching.Visible = false;
                    label_T_teach.Visible = false;
                    panel1.BringToFront();
                    panel1.Dock = DockStyle.Fill;
                    panel1.Refresh();
                    pictureBox_outside.BringToFront();

                    pictureBox_Sad.BringToFront();
                    pictureBox_Sad.Image = pictureBox_G_Sad.Image;
                }

                label_G_BP.Text = gs.badPoint + "";
                label_G_GP.Text = gs.goodPoint + "";

                Label_G_Hp.Text = gs.hp + "";

                teacher.hp += 8;

                Label_T_Hp.Text = teacher.hp + "";
            }

            if (teacher.hp < 0)
            {
                if (teacher.hp <= 0)
                {
                    pictureBox_T.Visible = false;
                    pictureBox_T_angry.Visible = false;
                    pictureBox_T_teaching.Visible = false;
                    label_T_teach.Visible = false;
                    panel1.BringToFront();
                    panel1.Dock = DockStyle.Fill;
                    panel1.Refresh();
                    pictureBox_outside.BringToFront();

                    pictureBox_T_null.BringToFront();
                    pictureBox_T_null.Image = pictureBox_T_Sad.Image;
                }

                timer1.Stop();
            }

        }

        private void timer2_Tick(object sender, EventArgs e)        //선생님 뒷모습 랜덤
        {
            pictureBox_T.BringToFront();
            label_T_teach.BringToFront();
            pictureBox_T.Image = pictureBox_T_teaching.Image;
            label_T_teach.Text = teacher.Teaching();
            
            if(pictureBox_B.Image == pictureBox_B_dance_right.Image)        //양아치 춤춘거 성공
            {
                bs.flag = false;
                bs.goodPoint += bs.Dance();
                bs.CalHp(0, bs.Dance());

                label_B_BP.Text = bs.badPoint + "";
                label_B_GP.Text = bs.goodPoint + "";

                Label_B_Hp.Text = bs.hp + "";

                teacher.hp -= 5;

                Label_T_Hp.Text = teacher.hp + "";
            }
            if(pictureBox_B.Image == pictureBox_B_sleep.Image)      //양아치 자는거 성공
            {
                bs.flag = false;
                bs.goodPoint += bs.Sleep();
                bs.CalHp(0, bs.Sleep());

                label_B_BP.Text = bs.badPoint + "";
                label_B_GP.Text = bs.goodPoint + "";

                Label_B_Hp.Text = bs.hp + "";

                teacher.hp -= 5;

                Label_T_Hp.Text = teacher.hp + "";
            }
            if(pictureBox_G.Image == pictureBox_G_eat.Image)        //모범생 먹기 성공
            {
                gs.flag = false;
                gs.goodPoint += gs.Eat();
                gs.CalHp(0, gs.Eat());

                label_G_BP.Text = gs.badPoint + "";
                label_G_GP.Text = gs.goodPoint + "";

                Label_G_Hp.Text = gs.hp + "";

                teacher.hp -= 5;

                Label_T_Hp.Text = teacher.hp + "";
            }
            if(pictureBox_G.Image == pictureBox_G_sleep.Image)      //모범생 졸기 성공
            {
                gs.flag = false;
                gs.goodPoint += gs.Sleep();
                gs.CalHp(0, gs.Sleep());

                label_G_BP.Text = gs.badPoint + "";
                label_G_GP.Text = gs.goodPoint + "";

                Label_G_Hp.Text = gs.hp + "";

                teacher.hp -= 5;

                Label_T_Hp.Text = teacher.hp + "";
            }
            
            if (teacher.hp < 0)
            {
                if(teacher.hp <= 0)
                {
                    pictureBox_T.Visible = false;
                    pictureBox_T_angry.Visible = false;
                    pictureBox_T_teaching.Visible = false;
                    label_T_teach.Visible = false;
                    panel1.BringToFront();
                    panel1.Dock = DockStyle.Fill;
                    panel1.Refresh();
                    pictureBox_outside.BringToFront();

                    pictureBox_T_null.BringToFront();
                    pictureBox_T_null.Image = pictureBox_T_Sad.Image;
                }

                timer2.Stop();
            }
            
        }
        
       
    }
}
