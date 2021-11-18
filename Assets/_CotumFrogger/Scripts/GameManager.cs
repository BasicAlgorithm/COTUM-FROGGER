using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Field

    public static GameManager Instance;

    [Tooltip("The prefab to use for representing the Frog on desktop")]
    public GameObject playerPrefabDesktop;
    [Tooltip("The prefab to use for representing the Frog on mobile")]
    public GameObject playerPrefabMobile;

    void Start()
    {
        Debug.Log("COTUM: GM.start() CALLED");

        if (playerPrefabDesktop == null || playerPrefabMobile == null)
        {
            Debug.LogError("COTUM: GM.start(): playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("COTUM: GM.start(): We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                // Where instantiate frogs according device
                if (SystemInfo.deviceType == DeviceType.Desktop)
                {
                    Debug.Log("COTUM: FROGBOT created because I am on desktop");
                    PhotonNetwork.Instantiate(this.playerPrefabDesktop.name, new Vector3(6f, 5f, Random.Range(20.0f, 50.0f)), Quaternion.identity, 0);
                } else if (SystemInfo.deviceType == DeviceType.Handheld)
                {
                    Debug.Log("COTUM: FROGNORMAL created because I am on mobile");
                    PhotonNetwork.Instantiate(this.playerPrefabMobile.name, new Vector3(6f, 5f, Random.Range(20.0f, 50.0f)), Quaternion.identity, 0);
                } else
                {
                    Debug.LogError("COTUM: GM.start(): We can't create a prefab due to your device.", this);
                }
            }
            else
            {
                Debug.LogFormat("COTUM: GM.start(): Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
        // Singleton assignment
        Instance = this;
    }

    #endregion

    #region Photon Callbacks

    // Called when the local player left the room. We need to load the MenuRoom scene.
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MenuRoom");
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player other)
    {
        // not seen if you're the player connecting
        Debug.LogFormat("COTUM: GM.OnPlayerEnteredRoom() Enter to scene -> {0}", other.NickName); 

        if (PhotonNetwork.IsMasterClient)
        {
            // called before OnPlayerLeftRoom
            Debug.LogFormat("COTUM: GM.OnPlayerEnteredRoom() IsMasterClient {0}", PhotonNetwork.IsMasterClient); 

            LoadArena();
        }
    }
    public override void OnPlayerLeftRoom(Player other)
    {
        // seen when other disconnects
        Debug.LogFormat("COTUM: GM.OnPlayerLeftRoom() {0}", other.NickName); 

        if (PhotonNetwork.IsMasterClient)
        {
            // called before OnPlayerLeftRoom
            Debug.LogFormat("COTUM: GM.OnPlayerLeftRoom() IsMasterClient {0}", PhotonNetwork.IsMasterClient); 

            LoadArena();
        }
    }

    #endregion

    #region Private Methods
    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("COTUM : GM.LoadArena() Trying to Load a level but we are not the master Client");
            return;
        }
        Debug.LogFormat("COTUM:  GM.LoadArena() Loading FIrstLevel");
        PhotonNetwork.LoadLevel("FIrstLevel");
    }

    #endregion
}
