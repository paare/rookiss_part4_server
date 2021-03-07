using System;
using Server;
using ServerCore;

class PacketHandler
{
    public static void C2S_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C2S_MoveHandler(PacketSession session, IPacket packet)
    {
        C2S_Move movePacket = packet as C2S_Move;
        ClientSession clientSession = session as ClientSession;
        if (clientSession.Room == null)
            return;
        
        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket));
    }
}