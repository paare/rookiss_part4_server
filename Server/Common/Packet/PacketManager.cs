using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
    #region Singleton

    private static PacketManager _instance;

    public static PacketManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PacketManager();
            }
            return _instance;
        }
    }

    #endregion

    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv =
        new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();

    private Dictionary<ushort, Action<PacketSession, IPacket>> _handler =
        new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
      _onRecv.Add((ushort) PacketId.C_PlayerInfoReq, MakePacket<C_PlayerInfoReq>);
        _handler.Add((ushort) PacketId.C_PlayerInfoReq, PacketHandler.C_PlayerInfoReqHandler);
      _onRecv.Add((ushort) PacketId.S_Test, MakePacket<S_Test>);
        _handler.Add((ushort) PacketId.S_Test, PacketHandler.S_TestHandler);

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