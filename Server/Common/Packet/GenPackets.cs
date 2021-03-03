using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketId
{
    C2S_PlayerInfoReq = 1,
	S2C_Test = 2,
	
}

interface IPacket
{
    ushort Protocol { get; }
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}

class C2S_PlayerInfoReq : IPacket
{
    public long playerId;
	public string name;
	
	public class Skill
	{
	    public int id;
		public short level;
		public float duration;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
			count += sizeof(short);
			this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
			count += sizeof(int);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
			count += sizeof(short);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
			count += sizeof(float);
	        return success;
	    }
	}
	
	public List<Skill> skills = new List<Skill>();
	

    public ushort Protocol => (ushort) PacketId.C2S_PlayerInfoReq;

    public void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(seg.Array, seg.Offset, seg.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		ushort nameLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLength));
		count += nameLength;
		skills.Clear();
		ushort skillLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);;
		for (var i = 0; i < skillLength; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(s, ref count);
		    skills.Add(skill);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(seg.Array, seg.Offset, seg.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort) PacketId.C2S_PlayerInfoReq);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(long);
		ushort nameLength = (ushort) Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, seg.Array,
		seg.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLength);
		count += sizeof(ushort);
		count += nameLength;
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (short) skills.Count);
		count += sizeof(ushort);
		
		foreach (Skill skill in skills)
		    success &= skill.Write(s, ref count);
		
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
class S2C_Test : IPacket
{
    public int TestInt;
	public byte TestByte;

    public ushort Protocol => (ushort) PacketId.S2C_Test;

    public void Read(ArraySegment<byte> seg)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(seg.Array, seg.Offset, seg.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.TestInt = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.TestByte = (byte)seg.Array[seg.Offset + count];
		count += sizeof(byte);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(seg.Array, seg.Offset, seg.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort) PacketId.S2C_Test);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.TestInt);
		count += sizeof(int);
		seg.Array[seg.Offset + count] = (byte)this.TestByte;
		count += sizeof(byte);
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}

