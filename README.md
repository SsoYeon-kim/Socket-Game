2학년 GUI프로그래밍 중 소켓통신을 이용한 게임

# 목차   
   
## 1. 시스템 소개   
1-1. 소켓통신을 이용한 게임   
   
## 2. 기능   
2-1. 쓰레드   
2-2. 파일 입출력   
2-3. 소켓 프로그래밍   
2-4. 화면 구성 
   
## 3. 동작 영상   
   
<hr>

# 1. 시스템 소개   
   
## socket-programming   
소켓통신을 이용한 게임이다.   
1. 불량학생과 모범생 총 2명이 플레이 할 수 있다.   
2. 수업하는 선생님은 무작위로 뒤를 돈다.
3. 두 플레이어는 여러가지 동작을 취할 수 있으며 선생님 몰래 성공 시 + point, 실패 시 - point를 얻는다.
   
다음 사진은 클래스 다이어그램이다. 추상화 클래스와 인터페이스 클래스를 포함한다.
<img src="https://user-images.githubusercontent.com/62587484/143852850-894821f9-ea24-41f0-91ef-f80d27c0074f.jpg" width="45%"><img src="https://user-images.githubusercontent.com/62587484/143778348-a597df3d-313f-45dd-8a74-ea536250cbc3.jpg" width="45%">      
   
# 2. 기능   
   
## 쓰레드, 파일 입출력, 소켓 프로그래밍   
   
소켓 프로그래밍을 이용한 게임으로 client와 server는 기본적으로 아래와 같은 코드이다.   

----- client -----   
* 소켓을 만든다   
* 송신/수신 buffer를 만들어 준다
* ip Address 지정
* Socket 객체 생성
* Socket의 연결
* Socket을 이용한 Data의 전송
   
<pre><code>
	public partial class Form1 : Form
	{
		TcpClient clientSocket = new TcpClient();
		NetworkStream stream = default(NetworkStream);           

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			clientSocket.Connect("127.0.0.1",9999);
			stream = clientSocket.GetStream();           

			string message = "Connected to Chat Server!\n";
			button1.Visible = false;
			Name_TextBox.Enabled = false;

			richTextBox1.AppendText(message);           

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
   
----- server -----   
* 소켓을 만든다
* IP 주소 설정
* Server Socket 생성 및 Server Listen모드
* Server에 접속한 Client 정보
* Server Accept 절차
* 수신된 자료의 대한 처리
   
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
			server = new TcpListener(IPAddress.Any, 9999);     
			clientSocket = default(TcpClient);
			server.Start();

			richTextBox1.Invoke(new Action(() => richTextBox1.AppendText("sever Start!\n")));

			while(true)
			{
				try
				{
					clientSocket = server.AcceptTcpClient();    

					NetworkStream stream = clientSocket.GetStream();    
					byte[] buffer = new byte[1024];
					int bytes = stream.Read(buffer, 0, buffer.Length);
					string userName = Encoding.Unicode.GetString(buffer, 0, bytes);   
					userName = userName.Substring(0, userName.IndexOf('$'));    
					clientList.Add(clientSocket, userName);

					SendMessageAll(userName + "님이 접속하셨습니다.\n","",false);   

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

		private void SendMessageAll(string message, string userName, bool flag)  
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

		public void Num_Enter(int num)		
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
   
## 화면 구성 및 기타   
   
Server의 첫 시작 화면이다.   
<img src="https://user-images.githubusercontent.com/62587484/143843930-e99a14ce-1967-48f2-b25e-96ecb4530dc2.png" width="45%">   
   
Client의 첫 시작 화면이다.   
<img src="https://user-images.githubusercontent.com/62587484/143844228-3eeb9abe-ded0-4e35-bbc3-eea4570f1ec2.png" width="45%">   
   
게임 방법을 눌러 point의 정보를 볼 수 있으며 이름을 입력하여 등교하기 버튼을 누른 뒤 화면이다. 10개의 랜덤한 질문이 나오게 되고 답변을 작성 후 지정폴더에 .txt형식으로 저장된다.       
<img src="https://user-images.githubusercontent.com/62587484/143844895-ab03e009-3d49-46be-b13d-7f701acb9619.png" width="45%">
<img src="https://user-images.githubusercontent.com/62587484/143846462-7bdd2f2d-7642-4a0e-8617-c81520fdcc7a.png" width="45%">
   
<pre><code>

        string[] q = new string[10];

        public static Random rnd = new Random();

        FileStream fs;
        StreamWriter sw;

        public static string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
	
	...
	
	q[0] = "무엇을 타고 등교하나요?";
        q[1] = "아침 먹었나요?";
        q[2] = "수업은 어떤가요?";
        q[3] = "피곤하나요?";
        q[4] = "잠을 얼마나 잤나요?";
        q[5] = "과제의 난이도가 어떤가요?";
	q[6] = "오늘의 기분은?";
        q[7] = "학식에 대해서 어떻게 생각하나요?";
        q[8] = "어떤 수업이 가장 재밌나요?";
        q[9] = "ㅇㅇㅇ 교수님은 어떤가요?";

	...
	
         int num = rnd.Next(0, 10);

         if (num < 11)
         {	label_question.Text = q[num];  }
	 
	 
	private void button_save_Click(object sender, EventArgs e)
        {
            fs = new FileStream(folder + @"\설문조사.txt", FileMode.Append, FileAccess.Write);
            sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            sw.WriteLine(label_question.Text + "  " + textBox_answer.Text);
            sw.Close();

           ...
        }
          
</code></pre>
   
두 명의 플레이어가 접속한 후 게임 플레이 화면이다.   
<img src="https://user-images.githubusercontent.com/62587484/143845872-b2ec7106-f3dd-4565-9f66-c3c4a3122d61.png" width="45%">   
   

   
# 3. 동작 영상   
   
다음은 게임 플레이 영상이다.   
<img src="https://user-images.githubusercontent.com/62587484/143847879-136e9a93-1f50-443a-8814-785ef5349920.gif" width="100%">   
   
두 명의 플레이어 채팅을 하나의 채팅창에 쓰는 것이 아닌 두 개로 나누었다.입력한 텍스트 끝에 남 또는 여를 붙여 구분하였다.   
   
--Client--
<pre><code>
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
</code></pre>   

--Server--
<pre><code>

...

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
</code></pre>   
   
   
이와 같은 방식으로 각 플레이어의 행동들을 서버에 보내 UI처리를 한다.예시로 아래의 코드와 같다.      

--Client--
<pre><code>
        private void button_G_eat_Click(object sender, EventArgs e)     //모범생 과자먹기 누르면 서버에 보내기
        {
            stream = clientSocket.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes("누군가 과자먹음" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }
</code></pre>   
   
--Server--   
<pre><code>
         ...
	 
         else if (msg == "누군가 과자먹음")
         {
             pictureBox_G.BringToFront();
             pictureBox_G.Image = pictureBox_G_eat.Image;
         }
         
	 ...
</code></pre>

다음은 불량학생, 모범생, 선생님의 Game Over(HP 0이하) 화면이다.   
<img src="https://user-images.githubusercontent.com/62587484/143849982-75e19802-a95a-4bda-ba99-f3e946d6a4b6.gif" width="50%"><img src="https://user-images.githubusercontent.com/62587484/143849997-d9dbd4d6-51c9-4a22-9ecd-be14aba222f5.gif" width="50%"><img src="https://user-images.githubusercontent.com/62587484/143849987-9492300f-dc2f-4c47-ad7e-7d5aeb2688e8.gif" width="50%">   
