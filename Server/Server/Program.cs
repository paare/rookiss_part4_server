using System.Net;
using System.Text;
using ServerCore;

namespace Server
{
    class Program
    {
        private static Listener listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            listener.Init(endPoint, () => SessionManager.Instance.Generate());
            while (true)
            {
            }
        }
    }
}