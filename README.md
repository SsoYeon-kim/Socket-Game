2학년 GUI프로그래밍 중 소켓통신을 이용한 게임

# 목차   
   
## 1. 시스템 소개   
1-1. 소켓통신을 이용한 게임   
   
## 2. 기능   
2-1. 쓰레드   
2-2. 파일 입출력   
2-3. 소켓 프로그래밍   
   
## 3. 동작 영상   
   
<hr>

# 1. 시스템 소개   
   
## socket-programming   
소켓통신을 이용한 게임이다.   
1. 불량학생과 모범생 총 2명이 플레이 할 수 있다.   
2. 수업하는 선생님은 무작위로 뒤를 돈다.
3. 두 플레이어는 여러가지 동작을 취할 수 있으며 선생님 몰래 성공 시 + point, 실패 시 - point를 얻는다.
   
다음 사진은 클래스 다이어그램이다. 추상화 클래스와 인터페이스 클래스를 포함한다.
<img src="https://user-images.githubusercontent.com/62587484/143778348-a597df3d-313f-45dd-8a74-ea536250cbc3.jpg" width="45%">   

# 2. 기능   
   
## 쓰레드   
   
## 파일 입출력   
   
## 소켓 프로그래밍   
   
-----client-----   
<pre><code>
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
</code></pre>
   
-----server-----   
<pre><code>
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

		public void Num_Enter(int num)		//먼저들어온사람 BS 나중에 들어온 사람 GS
		{
			NetworkStream stream = null;
			byte[] buffer = new byte[1024];
			stream = clientSocket.GetStream();
			string num_string = num.ToString();
			buffer = Encoding.Unicode.GetBytes(num_string);
			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
		}
	}
</code></pre>
   
# 3. 동작 영상   
