using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
    class SessionManager
    {
        private static SessionManager _session = new SessionManager();
        public static SessionManager Instance => _session;

        private List<ServerSession> _sessions = new List<ServerSession>();
        private object _lock = new object();

        public void SendForeach()
        {
            lock (_lock)
            {
                foreach (var session in _sessions)
                {
                    C2S_Chat chatPacket = new C2S_Chat();
                    chatPacket.chat = $"Hello Server !";
                    ArraySegment<byte> segment = chatPacket.Write();

                    session.Send(segment);
                }
            }
        }

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }
    }
}