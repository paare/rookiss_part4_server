using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packedId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> seg);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name;

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
                count += sizeof(float);

                return success;
            }

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);

                level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);

                duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new List<SkillInfo>();

        public PlayerInfoReq()
        {
            this.packedId = (ushort) PacketId.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> seg)
        {
            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(seg.Array, seg.Offset, seg.Count);
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            // string
            ushort nameLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLength));
            count += nameLength;

            // skill list
            ushort skillLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            skills.Clear();
            for (var i = 0; i < skillLength; i++)
            {
                SkillInfo skill = new SkillInfo();
                skill.Read(s, ref count);
                skills.Add(skill);
            }
        }

        public override ArraySegment<byte> Write()
        {
            // 보낸다
            ArraySegment<byte> seg = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(seg.Array, seg.Offset, seg.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packedId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long);

            // string
            ushort nameLength = (ushort) Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, seg.Array,
                seg.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLength);
            count += sizeof(ushort);
            count += nameLength;

            // skill list
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (short) skills.Count);
            count += sizeof(ushort);

            foreach (SkillInfo skill in skills)
            {
                success &= skill.Write(s, ref count);
            }

            success &= BitConverter.TryWriteBytes(s, count);
            if (success == false)
                return null;

            return SendBufferHelper.Close(count);
        }
    }

    public enum PacketId
    {
        PlayerInfoReq = 1,
        PlayerInfoRes = 2,
    }

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq {playerId = 1001, name = "ABCD"};
            packet.skills.Add(new PlayerInfoReq.SkillInfo{id = 101, level = 1, duration = 3f});
            packet.skills.Add(new PlayerInfoReq.SkillInfo{id = 201, level = 2, duration = 4f});
            packet.skills.Add(new PlayerInfoReq.SkillInfo{id = 301, level = 3, duration = 5f});
            packet.skills.Add(new PlayerInfoReq.SkillInfo{id = 401, level = 4, duration = 6f});

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