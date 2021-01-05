using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Server
{
	class HandleClient
	{
		TcpClient clientSocket = null;
		public Dictionary<TcpClient, string> clientList = null;

		public void startClient(TcpClient clientSocket, Dictionary<TcpClient, string> clientList)
		{
			this.clientSocket = clientSocket;
			this.clientList = clientList;

			Thread t = new Thread(doChat);
			t.IsBackground = true;
			t.Start();
		}

		public delegate void MessageDisplayHandler(string message, string userName);    //서버클래스에 있는 메서드를 여기서도 접근할수있게 선언해준거야
		public event MessageDisplayHandler OnReceived;

		public delegate void DisconnectedHandler(TcpClient clientSocket);       //튕겼을 때 
		public event DisconnectedHandler OnDisconnected;

		private void doChat()
		{
			NetworkStream stream = null;

			try                    //네트워크 사용할 때는 try catch문
			{
				byte[] buffer = new byte[1024];
				string msg = "";
				int bytes = 0;

				while (true)
				{
					stream = clientSocket.GetStream();
					bytes = stream.Read(buffer, 0, buffer.Length);       //read는 읽어오는거 write는 스트림에 쓰는거
					msg = Encoding.Unicode.GetString(buffer, 0, bytes);    //msg에 메세지 저장
					msg = msg.Substring(0, msg.IndexOf("$"));     // 이문자열을 기준으로 짜름

					if (OnReceived != null)
					{
						OnReceived(msg, clientList[clientSocket].ToString());
					}
				}
			}
			catch (SocketException ex)
			{
				if (clientSocket != null)
				{
					if (OnDisconnected != null)          //연결이 끊겼을 때
					{
						OnDisconnected(clientSocket);
					}
					 
					clientSocket.Close();     //연결이 끊겼으니까 쓰레드도 닫고 스트림도 닫아
					stream.Close();
				}
			}
			catch(Exception ex)           
			{
				if(clientSocket != null)
				{
					if(OnDisconnected != null)
					{
						OnDisconnected(clientSocket);
					}

					clientSocket.Close();
					stream.Close();
				}
			}
		}
	}
}
