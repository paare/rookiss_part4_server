using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        private List<ClientSession> _clientSessions = new List<ClientSession>();
        private JobQueue _jobQueue = new JobQueue();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void BroadCast(ClientSession clientSession, string chat)
        {
            S2C_Chat packet = new S2C_Chat();
            packet.playerId = clientSession.SessionId;
            packet.chat = chat + $"{chat}I am {packet.playerId}";

            ArraySegment<byte> segment = packet.Write();
            foreach (var session in _clientSessions)
            {
                session.Send(segment);
            }
        }

        public void Enter(ClientSession session)
        {
            _clientSessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            _clientSessions.Remove(session);
        }
    }
}