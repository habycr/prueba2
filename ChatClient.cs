using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ChatClient
{
    private readonly Socket _socket;
    private readonly int _port;

    public event Action<string> MessageReceived;

    public ChatClient(int port)
    {
        _port = port;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(new IPEndPoint(IPAddress.Any, _port));
    }

    public void StartListening()
    {
        Task.Run(ListenForMessages);
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
