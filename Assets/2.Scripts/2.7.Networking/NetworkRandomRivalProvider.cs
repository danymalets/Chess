using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkRandomRivalProvider : NetworkRivalProvider
{
    public void ConnectToServer()
    {
        PhotonNetwork.GameVersion = "1r";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("join random");
        ConnectedToServer?.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, roomOptions);
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
        Debug.Log("On Event " + photonEvent);
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
