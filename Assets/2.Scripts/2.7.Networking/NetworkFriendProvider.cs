using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkFriendProvider : NetworkRivalProvider
{
    public Action<TimeSpan> MoveDurationReceived;

    private string _roomName;
    private TimeSpan _moveDuration;

    public void ConnectToServer(string roomName, TimeSpan moveDuration)
    {
        _moveDuration = moveDuration;
        _roomName = roomName;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "1f";
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, null);
        ConnectedToServer?.Invoke();
    }

    protected override void StartGame()
    {
        Debug.Log("start" + _moveDuration);
        if (Random.Range(0, 2) == 0)
        {
            MoveDurationReceived?.Invoke(_moveDuration);
            ColorReceived?.Invoke(Color.White);

            SendMoveDuration(_moveDuration);
            SendColor(Color.Black);
        }
        else
        {
            MoveDurationReceived?.Invoke(_moveDuration);
            ColorReceived?.Invoke(Color.Black);

            SendMoveDuration(_moveDuration);
            SendColor(Color.White);
        }
    }

    private void SendMoveDuration(TimeSpan moveDuration)
    {
        PhotonNetwork.RaiseEvent(
            (byte)PhotonEvent.MoveDuration,
            (int)moveDuration.TotalSeconds,
            eventOptions,
            SendOptions.SendReliable);
    }

    public override void OnEvent(EventData photonEvent)
    {
        
        byte code = photonEvent.Code;
        switch ((PhotonEvent)code)
        {
            case PhotonEvent.MoveDuration:
                {
                    Debug.Log("dur r");
                    int duration = (int)photonEvent.CustomData;
                    MoveDurationReceived?.Invoke(new TimeSpan(0, 0, duration));
                }
                break;
            default:
                {
                    base.OnEvent(photonEvent);
                }
                break;
        }
    }
}
