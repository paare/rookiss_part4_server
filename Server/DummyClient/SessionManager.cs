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
        private Random _rand = new Random();

        public void SendForeach()
        {
            lock (_lock)
            {
                foreach (var session in _sessions)
                {
                    C2S_Move movePacket = new C2S_Move();
                    movePacket.posX = _rand.Next(-50, 50);
                    movePacket.posY = 0;
                    movePacket.posZ = _rand.Next(-50, 50);

                    session.Send(movePacket.Write());
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