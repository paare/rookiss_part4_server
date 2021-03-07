using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    private MyPlayer _myPlayer;

    private Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(S2C_PlayerList packet)
    {
        Object obj = Resources.Load("Player");

        foreach (var p in packet.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;
            if (p.isSelf)
            {
                MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.PlayerId = p.playerId;
                myPlayer.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _myPlayer = myPlayer;
            }
            else
            {
                Player player = go.AddComponent<Player>();
                player.PlayerId = p.playerId;
                player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _players.Add(p.playerId, player);
            }
        }
    }

    public void EnterGame(S2C_BroadcastEnterGame pkt)
    {
        if (pkt.playerId == _myPlayer.PlayerId)
        {
            return;
        }
        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;
        
        Player player = go.AddComponent<Player>();
        player.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
        _players.Add(pkt.playerId, player);
    }

    public void LeaveGame(S2C_BroadcastLeaveGame pkt)
    {
        if (_myPlayer.PlayerId == pkt.playerId)
        {
            GameObject.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
        }
        else
        {
            if (_players.TryGetValue(pkt.playerId, out var player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(pkt.playerId);
            }
        }
    }

    public void Move(S2C_BroadcastMove pkt)
    {
        if (_myPlayer.PlayerId == pkt.playerId)
        {
            _myPlayer.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
        }   
        else
        {
            if (_players.TryGetValue(pkt.playerId, out var player))
            {
                player.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
            }
        }
    }
}