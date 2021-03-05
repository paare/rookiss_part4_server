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
        private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (var session in _clientSessions)
            {
                session.Send(_pendingList);
            }

            Console.WriteLine($"Flushed Item count {_pendingList.Count}");
            _pendingList.Clear();
        }

        public void BroadCast(ClientSession clientSession, string chat)
        {
            S2C_Chat packet = new S2C_Chat();
            packet.playerId = clientSession.SessionId;
            packet.chat = chat + $"{chat}I am {packet.playerId}";

            ArraySegment<byte> segment = packet.Write();
            _pendingList.Add(segment);
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