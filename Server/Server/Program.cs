using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    class Program
    {
        private static Listener listener = new Listener();
        public static GameRoom Room = new GameRoom();

        private static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            listener.Init(endPoint, () => SessionManager.Instance.Generate());

            // FlushRoom();
            JobTimer.Instance.Push(FlushRoom);
            while (true)
            {
                JobTimer.Instance.Flush();
            }
        }
    }
}