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

namespace Server
{
	public partial class Form1 : Form
	{
		TcpListener server = null;
		TcpClient clientSocket = null;

		public Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();

		public Form1()
		{
			InitializeComponent();
			richTextBox1.AppendText("");

			Thread t = new Thread(initSocket);
			t.IsBackground = true;
			t.Start();
		}

		private void initSocket()
		{
			server = new TcpListener(IPAddress.Any, 9999);     //IPAddress.Any  - 모든 아이피에서 접속이 가능하다    / 특정아이피를 적을수도 있음
			clientSocket = default(TcpClient);
			server.Start();

			richTextBox1.Invoke(new Action(() => richTextBox1.AppendText("sever Start!\n")));

			while(true)
			{
				try
				{
					clientSocket = server.AcceptTcpClient();     // 여기 밑에 코드부터는 서버에 연결이 안되면 실행이 안돼 여기서 멈추고 접속을 기다리는거야 접속되면 밑에 코드가 실행됩니다

					NetworkStream stream = clientSocket.GetStream();    //보내기위한 스트림
					byte[] buffer = new byte[1024];
					int bytes = stream.Read(buffer, 0, buffer.Length);
					string userName = Encoding.Unicode.GetString(buffer, 0, bytes);    //클라이언트 이름을 여기에 저장
					userName = userName.Substring(0, userName.IndexOf('$'));    //편법이야 EOF안쓰고 

					clientList.Add(clientSocket, userName);

					SendMessageAll(userName + "님이 접속하셨습니다.\n","",false);    // false인 이유 : 그냥 누가 접속했다는 것만 알려주기위해

					HandleClient handleClient = new HandleClient();
					handleClient.OnReceived += new HandleClient.MessageDisplayHandler(OnRecieved);
					handleClient.OnDisconnected += new HandleClient.DisconnectedHandler(OnSidconnected);
					handleClient.startClient(clientSocket, clientList);

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

		private void OnRecieved(string message, string userName)
		{
			string displayMessage = "From client : " + userName + " : " + message + "\n";
			richTextBox1.Invoke(new Action(() => richTextBox1.AppendText(displayMessage)));
			SendMessageAll(message, userName, true);

		}

		private void OnSidconnected(TcpClient clinetSocket)
		{
			if(clientList.ContainsKey(clientSocket))
			{
				clientList.Remove(clientSocket);
			}
		}

		private void SendMessageAll(string message, string userName, bool flag)   //접속되어 있는 모든 사용자들한테 소켓을 통해서 다시 메세지를 쏴줌
		{
			foreach(var pair in clientList)
			{
				TcpClient client = pair.Key as TcpClient;
				NetworkStream stream = client.GetStream();
				byte[] buffer = null;

				if(flag)    //true 일때
				{
					buffer = Encoding.Unicode.GetBytes(userName + " : " + message);
				}
				else    //false 일때
				{
					buffer = Encoding.Unicode.GetBytes(message);
				}

				stream.Write(buffer, 0, buffer.Length);
				stream.Flush();
			}
		}
	}
}
