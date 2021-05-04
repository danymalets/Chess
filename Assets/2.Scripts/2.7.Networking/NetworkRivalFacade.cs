using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class NetworkRivalProvider : MonoBehaviourPunCallbacks, IOnEventCallback
{
    protected const int VERSION = 1;

    static protected RaiseEventOptions _eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
    static protected RoomOptions _roomOptions = new RoomOptions { MaxPlayers = 2 };

    protected enum PhotonEvent : byte
    {
        Order = 1,
        MoveDuration,
        Move,
        TimeIsOver
    }

    public Action ConnectedToServer;
    public Action Disconnected;
    public Action RoomCreated;
    public Action RivalFound;
    public Action RivalDisconnected;

    public Action<Color> ColorReceived;
    public Action<Move> MoveReceived;
    public Action TimeIsOverReceived;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("player entered");
        PhotonNetwork.CurrentRoom.IsOpen = false;
        RivalFound?.Invoke();
        StartGame();
    }

    protected abstract void StartGame();

    public override void OnCreatedRoom()
    {
        RoomCreated?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("on joined");
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

    public void OnApplicationPause()
    {
        Disconnected?.Invoke(); 
        Disconnect();
    }

    public void OnApplicationQuit()
    {
        Disconnect();
    }

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
        PhotonNetwork.RaiseEvent((byte)PhotonEvent.TimeIsOver, null, _eventOptions, SendOptions.SendReliable);
    }

    protected void SendColor(Color playerColor)
    {
        Debug.Log("send order " + playerColor);
        PhotonNetwork.RaiseEvent((byte)PhotonEvent.Order, (int)playerColor, _eventOptions, SendOptions.SendReliable);
    }


    public void SendMove(Move move)
    {
        Debug.Log("send move " + move);
        PhotonNetwork.RaiseEvent((byte)PhotonEvent.Move, move.ToString(), _eventOptions, SendOptions.SendReliable);
    }

    public virtual void OnEvent(EventData photonEvent)
    {
        byte code = photonEvent.Code;
        switch ((PhotonEvent)code)
        {
            case PhotonEvent.Order:
                {
                    int order = (int)photonEvent.CustomData;
                    ColorReceived?.Invoke((Color)order);
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

    public override void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    public override void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);
}
