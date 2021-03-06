﻿using System;
using DummyClient;
using ServerCore;

class PacketHandler
{
    public static void S2C_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S2C_BroadcastEnterGame chatPacket = packet as S2C_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;
    }

    public static void S2C_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S2C_BroadcastLeaveGame chatPacket = packet as S2C_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;
    }

    public static void S2C_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S2C_PlayerList chatPacket = packet as S2C_PlayerList;
        ServerSession serverSession = session as ServerSession;
    }

    public static void S2C_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S2C_PlayerList chatPacket = packet as S2C_PlayerList;
        ServerSession serverSession = session as ServerSession;
    }
}