using System;
using ServerCore;

class PacketHandler
{
    public static void C2S_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        C2S_PlayerInfoReq p = packet as C2S_PlayerInfoReq;
        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");

        foreach (var skill in p.skills)
        {
            Console.WriteLine($"Skill {skill.id} {skill.level} {skill.duration}");
        }
    }
}