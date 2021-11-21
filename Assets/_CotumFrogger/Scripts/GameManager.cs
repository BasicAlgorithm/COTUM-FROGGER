using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace COTUM
{
#pragma warning disable 649

    public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Public Fields

        static public GameManager Instance;

        #endregion

        #region Private Fields

        private GameObject instance;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefabDesktop;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefabMobile;

        #endregion

        #region MonoBehaviour CallBacks

        // MonoBehaviour method called on GameObject by Unity during initialization phase.
        void Start()
        {
            Instance = this;

            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.IsConnected)
            {
                if (SystemInfo.deviceType == DeviceType.Desktop)
                {
                    SceneManager.LoadScene("MainDesktop");
                }
                else if (SystemInfo.deviceType == DeviceType.Handheld)
                {
                    SceneManager.LoadScene("MainMobile");
                }
                else
                {
                    Debug.LogError("COTUM: GM.start(): We can't create a scene due to your device.", this);
                }
                return;
            }

            if (playerPrefabDesktop == null)
            { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

                Debug.LogError("COTUM: GM.START() playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {

                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("COTUM: GM.We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                    // Where instantiate frogs according device
                    if (SystemInfo.deviceType == DeviceType.Desktop)
                    {
                        Debug.Log("COTUM: GM.FROGBOT created because I am on desktop");
                        PhotonNetwork.Instantiate(this.playerPrefabDesktop.name, new Vector3(Random.Range(-10f, 10f), 5f, 0f), Quaternion.identity, 0);
                    }
                    else if (SystemInfo.deviceType == DeviceType.Handheld)
                    {
                        Debug.Log("COTUM: GM.FROGNORMAL created because I am on mobile");
                        PhotonNetwork.Instantiate(this.playerPrefabMobile.name, new Vector3(Random.Range(-10f,10f), 5f, 0f), Quaternion.identity, 0);
                    }
                    else
                    {
                        Debug.LogError("COTUM: GM.start(): We can't create a prefab due to your device.", this);
                    }
                }
                else
                {

                    Debug.LogFormat("COTUM: GM.Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }

            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
            }
        }

        #endregion

        #region Photon Callbacks

        // Called when a Photon Player got connected. We need to then load a bigger scene.
        // <param name="other">Other.</param>
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("COTUM: GM.OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("COTUM: GM.OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        // Called when a Photon Player got disconnected. We need to load a smaller scene.
        // <param name="other">Other.</param>
        public override void OnPlayerLeftRoom(Player other)
        {
            // seen when other disconnects
            Debug.Log("COTUM: GM.OnPlayerLeftRoom() " + other.NickName); 

            if (PhotonNetwork.IsMasterClient)
            {
                // called before OnPlayerLeftRoom
                Debug.LogFormat("COTUM: GM.OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

                LoadArena();
            }
        }

        // Called when the local player left the room. We need to load the launcher scene.
        public override void OnLeftRoom()
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                SceneManager.LoadScene("MainDesktop");
            }
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                SceneManager.LoadScene("MainMobile");
            }
            else
            {
                Debug.LogError("COTUM: GM.start(): We can't create a prefab due to your device.", this);
            }
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        #endregion

        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("COTUM : GM.LoadArena() Trying to Load a level but we are not the master Client");
            }

            Debug.LogFormat("COTUM:  GM.LoadArena() Loading FIrstLevel");

            PhotonNetwork.LoadLevel("FirstLevel");
        }

        #endregion

    }

}