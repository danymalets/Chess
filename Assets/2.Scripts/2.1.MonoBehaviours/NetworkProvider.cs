using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkProvider : MonoBehaviourPunCallbacks, IOnEventCallback
{
    static RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

    enum PhotonEvent : byte
    {
        Order = 1,
        Move,
        TimeIsOver
    }

    public event Action ConnectedToServer;
    public event Action Disconnected;
    public event Action RoomCreated;
    public event Action RivalFound;
    public event Action RivalDisconnected;

    public event Action<Color> ColorReceived;
    public event Action<Move> MoveReceived;
    public event Action TimeIsOverReceived;

    public void ConnectToServer()
    {
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
        ConnectedToServer?.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("player entered");
        RivalFound?.Invoke();
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

    public override void OnCreatedRoom()
    {
        RoomCreated?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("player joined");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            RivalFound?.Invoke();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RivalDisconnected?.Invoke();
    }

    public void PreDisconnect()
    {
        RivalDisconnected = () => PhotonNetwork.Disconnect();
    }

    public void OnApplicationStop() => Disconnect();
    public void OnApplicationQuit() => Disconnect();

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause != DisconnectCause.DisconnectByClientLogic)
        {
            Disconnected?.Invoke();
        }
    }

    public void SendTimerIsOver()
    {
        PhotonNetwork.RaiseEvent((byte)PhotonEvent.TimeIsOver, null, options, SendOptions.SendReliable);
    }

    public void SendColor(Color playerColor)
    {
        Debug.Log("send order " + playerColor);
        PhotonNetwork.RaiseEvent((byte)PhotonEvent.Order, (int)playerColor, options, SendOptions.SendReliable);
    }

    public void SendMove(Move move)
    {
        Debug.Log("send move " + move);
        PhotonNetwork.RaiseEvent((byte)PhotonEvent.Move, move.ToString(), options, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte code = photonEvent.Code;
        switch ((PhotonEvent)code)
        {
            case PhotonEvent.Order:
                {
                    int content = (int)photonEvent.CustomData;
                    ColorReceived?.Invoke((Color)content);
                }
                break;
            case PhotonEvent.Move:
                {
                    string content = (string)photonEvent.CustomData;
                    MoveReceived?.Invoke(Move.FromString(content));
                }
                break;
            case PhotonEvent.TimeIsOver:
                {
                    TimeIsOverReceived?.Invoke();
                }
                break;
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
