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

            //Console.WriteLine($"Flushed Item count {_pendingList.Count}");
            _pendingList.Clear();
        }

        public void BroadCast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            // 플레이어 추가
            _clientSessions.Add(session);
            session.Room = this;

            // 신규 플레이어에게 모든 플레이어 목록 전송
            S2C_PlayerList players = new S2C_PlayerList();
            foreach (var s in _clientSessions)
            {
                players.players.Add(new S2C_PlayerList.Player()
                {
                    isSelf = session == s,
                    playerId = s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                });
            }

            session.Send(players.Write());

            // 신규 플레이어입장을 모두에게 알린다
            S2C_BroadcastEnterGame enter = new S2C_BroadcastEnterGame();
            enter.playerId = session.SessionId;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;

            BroadCast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            // 플레이어 제고하고
            _clientSessions.Remove(session);

            // 모두에게 알린다
            S2C_BroadcastLeaveGame leave = new S2C_BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            BroadCast(leave.Write());
        }

        public void Move(ClientSession session, C2S_Move packet)
        {
            // 좌표를 바꿔주고 
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            // 모두에게 알린다
            S2C_BroadcastMove move = new S2C_BroadcastMove();
            move.playerId = session.SessionId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;

            BroadCast(move.Write());
        }
    }
}