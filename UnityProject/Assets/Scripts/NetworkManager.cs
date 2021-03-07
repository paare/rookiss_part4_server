using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using DummyClient;
using ServerCore;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private ServerSession _session = new ServerSession();

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    private void Start()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => _session, 1);
    }

    private void Update()
    {
        var list = PacketQueue.Instance.PopAll();
        foreach (var pk in list)
        {
            if (pk != null)
            {
                PacketManager.Instance.HandlePacket(_session, pk);
            }
        }
    }

    private void OnDestroy()
    {
        _session.Disconnect();
    }
}