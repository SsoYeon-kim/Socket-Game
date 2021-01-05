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

namespace Client
{
	public partial class Form1 : Form
	{
		TcpClient clientSocket = new TcpClient();
		NetworkStream stream = default(NetworkStream);             //데이터를 주고받을 때 사용할 스트림

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			clientSocket.Connect("127.0.0.1",9999);
			stream = clientSocket.GetStream();             // 서버에서 누가 접속이 되면 클라이언트한테 누가 접속이 됐다라고 뿌려주기위해

			string message = "Connected to Chat Server!\n";
			button1.Visible = false;
			Name_TextBox.Enabled = false;

			richTextBox1.AppendText(message);             //쓰레드 안에서 쓸때는 invoke  근데 여기는 쓰레드 안이 아니니까 그냥 AppendText

			byte[] buffer = Encoding.Unicode.GetBytes(Name_TextBox.Text + "$");
			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();

			Thread t  = new Thread(getMessage);
			t.IsBackground = true;
			t.Start();
		}

		private void getMessage()
		{
			while(true)
			{
				stream = clientSocket.GetStream();
				int size = clientSocket.ReceiveBufferSize;
				byte[] buffer = new byte[size];
				int bytes = stream.Read(buffer, 0, buffer.Length);

				string message = Encoding.Unicode.GetString(buffer, 0, bytes);
				richTextBox1.Invoke(new Action(() => richTextBox1.AppendText(message + "\n")));
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			byte[] buffer = Encoding.Unicode.GetBytes(connect_TextBox.Text.ToString() + "$");
			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
		}
	}
}
