using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkFriend : NetworkRival
{
    private string _roomName;

    public NetworkFriend(TimeSpan moveDuration, string roomName): base(moveDuration)
    {
        _roomName = roomName;
    }

    protected override void ConnectToServer()
    {
        var provider = _ui.gameObject.AddComponent<NetworkFriendProvider>();
        provider.ConnectToServer(_roomName, _moveDuration);
        provider.MoveDurationReceived += MoveDurationReceived;
        _provider = provider;
    }

    private void MoveDurationReceived(TimeSpan moveDuration)
    {
        _moveDuration = moveDuration;
    }
}
