using System;
using System.Windows.Forms;

namespace prueba2
{
    public partial class Form1 : Form
    {
        private readonly ChatClient _chatClient;

        public Form1()
        {
            InitializeComponent();
            
            int port = PortConfig.GetNextPort();
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
            if (int.TryParse(txtPort.Text, out int destinationPort))
            {
                _chatClient.SendMessage(message, destinationPort);
                lstMessages.Items.Add($"Sent: {message}");
            }
            else
            {
                MessageBox.Show("Please enter a valid port number.");
            }
        }
    }
}
