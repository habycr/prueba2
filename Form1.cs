using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prueba2
{
    public partial class Form1 : Form
    {
        private ChatClient _chatClient;
        private static readonly string configFilePath = "config.txt";

        public Form1()
        {
            InitializeComponent();

            int port = GetNextPort();
            _chatClient = new ChatClient(port);
            _chatClient.MessageReceived += OnMessageReceived;
            _chatClient.StartListening();

            this.Text = $"Chat Client - Port {port}"; // Opcional: muestra el puerto en el título de la ventana
        }

        private void OnMessageReceived(string message)
        {
            Invoke(new Action(() => lstMessages.Items.Add($"Received: {message}")));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text;
            int destinationPort = int.Parse(txtPort.Text);
            _chatClient.SendMessage(message, destinationPort);
            lstMessages.Items.Add($"Sent: {message}");
        }

        private static int GetNextPort()
        {
            int port;
            if (File.Exists(configFilePath))
            {
                string lastPortStr = File.ReadAllText(configFilePath);
                if (int.TryParse(lastPortStr, out port))
                {
                    port++;
                }
                else
                {
                    port = 12345; // Default port if config file is corrupt
                }
            }
            else
            {
                port = 12345; // Default starting port
            }

            File.WriteAllText(configFilePath, port.ToString());
            return port;
        }
    }

    public class ChatClient
    {
        private Socket _socket;
        private int _port;

        public event Action<string> MessageReceived;

        public ChatClient(int port)
        {
            _port = port;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, _port));
        }

        public void StartListening()
        {
            Task.Run(() => ListenForMessages());
        }

        private void ListenForMessages()
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int receivedBytes = _socket.ReceiveFrom(buffer, ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                MessageReceived?.Invoke(message);
            }
        }

        public void SendMessage(string message, int destinationPort)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Loopback, destinationPort);
            _socket.SendTo(data, remoteEndPoint);
        }
    }
}
