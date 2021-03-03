using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            C2S_PlayerInfoReq packet = new C2S_PlayerInfoReq  {playerId = 1001, name = "ABCD"};
            packet.skills.Add(new C2S_PlayerInfoReq .Skill {id = 101, level = 1, duration = 3f});
            packet.skills.Add(new C2S_PlayerInfoReq .Skill {id = 201, level = 2, duration = 4f});
            packet.skills.Add(new C2S_PlayerInfoReq .Skill {id = 301, level = 3, duration = 5f});
            packet.skills.Add(new C2S_PlayerInfoReq .Skill {id = 401, level = 4, duration = 6f});

            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> seg = packet.Write();
                if (seg != null)
                {
                    Send(seg);
                }
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"From Server >> [{recvData}]");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}