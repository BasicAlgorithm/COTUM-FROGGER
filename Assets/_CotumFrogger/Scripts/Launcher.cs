using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    #endregion

    #region Private Fields

    string gameVersion = "1";
    bool isConnecting;

    #endregion

    #region Public Fields

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    #endregion

    #region MonoBehaviour CallBacks

    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and
        // all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // If following function is executed, automatic launch will happen
        Connect();

        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion

    #region Public Methods
    
    // Start the connection process.
    // - If already connected, we attempt joining a random room
    // - if not yet connected, Connect this application instance to Photon Cloud Network
    public void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // we check if we are connected or not, we join if we are ,
        // else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room.
            // If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    #endregion

        #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("COTUM: LAUNCHER.OnConnectedToMaster() was called by PUN");

        // we don't want to do anything if we are not attempting to join a room.
        // this case where isConnecting is false is typically when you lost or quit the game,
        // when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        if (isConnecting)
        {
            // #Critical: The first we try to do is to join a potential existing room.
            // If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);

        isConnecting = false;

        Debug.LogWarningFormat("COTUM: LAUNCHER.OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("COTUM: LAUNCHER.OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("COTUM: LAUNCHER.OnJoinedRoom() I am the first, we load a FIrstLevel");

            // Load the Room Level.
            PhotonNetwork.LoadLevel("FIrstLevel");
        }
        Debug.Log("COTUM: LAUNCHER.OnJoinedRoom() I am not the first, we will enter");
    }

    #endregion
}