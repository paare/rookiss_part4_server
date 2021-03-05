using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
    #region Singleton

    private static PacketManager _instance = new PacketManager();

    public static PacketManager Instance => _instance;

    public PacketManager()
    {
        Register();
    }

    #endregion

    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv =
        new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();

    private Dictionary<ushort, Action<PacketSession, IPacket>> _handler =
        new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
        _onRecv.Add((ushort) PacketId.S2C_Chat, MakePacket<S2C_Chat>);
        _handler.Add((ushort) PacketId.S2C_Chat, PacketHandler.S2C_ChatHandler);

    }

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);

        if (_handler.TryGetValue(pkt.Protocol, out var action))
        {
            action?.Invoke(session, pkt);
        }
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (_onRecv.TryGetValue(id, out var action))
        {
            action?.Invoke(session, buffer);
        }
    }
}