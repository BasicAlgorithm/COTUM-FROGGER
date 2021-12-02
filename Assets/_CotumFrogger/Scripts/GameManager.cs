﻿using UnityEngine;
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
        private GameObject playerPrefabDesktopBlue;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefabDesktopRed;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefabDesktopGreen;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefabMobile;

        [SerializeField]
        private GameObject combiPrefab;
        [SerializeField]
        private GameObject taxiPrefab;
        [SerializeField]
        private GameObject policePrefab;
        [SerializeField]
        private GameObject combi2Prefab;

        const string playerCharacterColor = "Color";

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

            string defaultName = PlayerCharacter.colorCharacter;
            Debug.Log("ELEGI COLOR " + defaultName);

            if (playerPrefabDesktopRed == null)
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
                        if (PlayerCharacter.colorCharacter == "red")
                        {
                            PhotonNetwork.Instantiate(this.playerPrefabDesktopRed.name, new Vector3(34.5f, -2.3f, Random.Range(15.0f, 55.0f)), Quaternion.identity, 0);
                        }
                        else if (PlayerCharacter.colorCharacter == "blue")
                        {
                            PhotonNetwork.Instantiate(this.playerPrefabDesktopBlue.name, new Vector3(34.5f, -2.3f, Random.Range(15.0f, 55.0f)), Quaternion.identity, 0);
                        }
                        else if (PlayerCharacter.colorCharacter == "green")
                        {
                            PhotonNetwork.Instantiate(this.playerPrefabDesktopGreen.name, new Vector3(34.5f, -2.3f, Random.Range(15.0f, 55.0f)), Quaternion.identity, 0);
                        }
                        else
                        {
                            Debug.Log("COTUM: GM.FROGBOT Developer Error");
                        }
                    }
                    else if (SystemInfo.deviceType == DeviceType.Handheld)
                    {
                        Debug.Log("COTUM: GM.FROGNORMAL created because I am on mobile");
                        GameObject character_prefab = (GameObject)PhotonNetwork.Instantiate(this.playerPrefabMobile.name, new Vector3(34.5f, -2.3f, Random.Range(15.0f, 55.0f)), Quaternion.identity, 0);
                        Camera.main.transform.parent = character_prefab.transform;
                    }
                    else
                    {
                        Debug.LogError("COTUM: GM.start(): We can't create a prefab due to your device.", this);
                    }
                    LoadCars();
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

        void LoadCars()
        {
            Vector3[] cars = new[] {
                new Vector3(33.5f, -3.5f, 5f), new Vector3(31.5f, -3.5f, 70f),
                new Vector3(21.7f, -3.5f, 5f), new Vector3(19.7f, -3.5f, 70f),
                new Vector3(15.5f, -3.5f, 5f), new Vector3(13.5f, -3.5f, 70f),
                new Vector3(3f, -3.5f, 5f), new Vector3(-4.5f, -3.5f, 70f),
                new Vector3(-14.5f, -3.5f, 5f), new Vector3(-16.5f, -3.5f, 70f),
                new Vector3(-20.5f, -3.5f, 5f), new Vector3(-22.9f, -3.5f, 70f),
                new Vector3(-32.5f, -3.5f, 5f), new Vector3(-34.9f, -3.5f, 70f),
            };
            string[] prefabs = { this.combiPrefab.name, this.taxiPrefab.name, this.policePrefab.name, this.combi2Prefab.name };
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < cars.Length; ++i)
                {
                    GameObject new_car = (GameObject)PhotonNetwork.Instantiate(prefabs[i % 4], new Vector3(0, 0, 0), Quaternion.identity, 0);
                    new_car.GetComponent<VehicleGenerator>().velocity = 12f - (i % 3) * 1.5f;
                    new_car.GetComponent<VehicleGenerator>().mov_left_right = (i % 2 == 0 ? 1 : -1);
                    new_car.GetComponent<VehicleGenerator>().pos_initial_x = cars[i].x;
                    new_car.GetComponent<VehicleGenerator>().pos_initial_y = cars[i].y;
                    new_car.GetComponent<VehicleGenerator>().pos_initial_z = cars[i].z;
                    new_car.GetComponent<VehicleGenerator>().pos_end_z = cars[i].z + (i % 2 == 0 ? 65 : -65);
                }
            }
        }

        #endregion
    }
}