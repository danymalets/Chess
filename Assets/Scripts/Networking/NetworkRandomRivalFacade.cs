using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkRandomRivalFacade : NetworkRivalProvider
{
    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Version + "r";
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
        ConnectedToServer?.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, RoomOptions);
    }

    protected override void StartGame()
    {
        if (Random.Range(0, 2) == 0)
        {
            ColorReceived?.Invoke(Color.White);
            SendColor(Color.Black);
        }
        else
        {
            ColorReceived?.Invoke(Color.Black);
            SendColor(Color.White);
        }
    }


    public override void OnEvent(EventData photonEvent)
    {
        byte code = photonEvent.Code;
        switch ((PhotonEvent)code)
        {
            default:
                {
                    base.OnEvent(photonEvent);
                }
                break;
        }
    }
}
