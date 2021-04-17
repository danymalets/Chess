using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRandomRival : NetworkRival
{
    public NetworkRandomRival(): base(new TimeSpan(0, 2, 0))
    {
    }

    protected override void ConnectToServer()
    {
        var provider = _ui.gameObject.AddComponent<NetworkRandomRivalProvider>();
        provider.ConnectToServer();
        _provider = provider;
    }
}
