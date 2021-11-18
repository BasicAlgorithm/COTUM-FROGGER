using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(IsFiring);
            stream.SendNext(Health);
        }
        else
        {
            // Network player, receive data
            this.IsFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }

    #endregion

    #region Public Field

    [Tooltip("The current Health of frog")]
    public float Health = 1f;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    [Tooltip("The Player's UI GameObject Prefab")]
    [SerializeField]
    public GameObject PlayerUiPrefab;

    #endregion

    #region Private Fields

    [Tooltip("The Beams GameObject to control")]
    [SerializeField]
    private GameObject beams;

    //True, when the user is firing
    bool IsFiring;

    #endregion

    #region MonoBehaviour CallBacks

    // MonoBehaviour method called on GameObject by Unity during early initialization phase.
    void Awake()
    {
        if (beams == null)
        {
            Debug.LogError("COTUM: PM.awake() Beams Reference.", this);
        }
        else
        {
            beams.SetActive(false);
        }

        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
        
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);

        
    }

    // MonoBehaviour method called on GameObject by Unity during initialization phase.
    void Start()
    {
        Debug.Log("COTUM: PM.start() CALLED");

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            Debug.Log("COTUM: PM.start() CAMERA SOY DESKTOP");
            CameraDesktop camera_desktop = this.gameObject.GetComponent<CameraDesktop>();

            if (camera_desktop != null)
            {
                if (photonView.IsMine)
                {
                    camera_desktop.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("COTUM: PM.start(): Desktop_camera is null.", this);
            }
        }
        else if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            Debug.Log("COTUM: PM.start() CAMERA SOY CELULAR");
            CameraMobile camera_mobile = this.gameObject.GetComponent<CameraMobile>();

            if (camera_mobile != null)
            {
                if (photonView.IsMine)
                {
                    camera_mobile.OnStart();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Mobile Component on playerPrefab.", this);
            }
        }
        else
        {

            Debug.LogError("<Color=Red><a>Device Unknown</a></Color> We cant create a prefab due to your device.", this);
        }

        if (PlayerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("COTUM: PM.awake() PlayerUiPrefab is NULL, reference on player Prefab.", this);
        }

#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif

    }

    // MonoBehaviour method called on GameObject by Unity on every frame.
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }

        // trigger Beams active state
        if (beams != null && IsFiring != beams.activeInHierarchy)
        {
            beams.SetActive(IsFiring);
        }

        // Game over implementation, not sure of its implementation
        if (photonView.IsMine)
        {
            ProcessInputs();
            if (Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
        }

        // TODO: sync Player UI when change levels
        //GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
        //_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    // MonoBehaviour method called when the Collider 'other' enters the trigger.
    // Affect Health of the Player if the collider is a beam
    // Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
    // One could move the collider further away to prevent this or check if the beam belongs to the player.
    void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        // We are only interested in Beamers
        // we should be using tags but for the sake of distribution, let's simply check by name.
        if (!other.name.Contains("Beam"))
        {
            return;
        }
        Health -= 0.1f;
    }
    // MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
    // We're going to affect health while the beams are touching the player
    // <param name="other">Other.</param>
    void OnTriggerStay(Collider other)
    {
        // we dont' do anything if we are not the local player.
        if (!photonView.IsMine)
        {
            return;
        }
        // We are only interested in Beamers
        // we should be using tags but for the sake of distribution, let's simply check by name.
        if (!other.name.Contains("Beam"))
        {
            return;
        }
        // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
        Health -= 0.1f * Time.deltaTime;
    }

#if !UNITY_5_4_OR_NEWER
// See CalledOnLevelWasLoaded. Outdated in Unity 5.4.
void OnLevelWasLoaded(int level)
{
    this.CalledOnLevelWasLoaded(level);
}
#endif

    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }

        GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Private Methods

#if UNITY_5_4_OR_NEWER
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }
#endif

    #endregion

    #region Custom

    // Processes the inputs. Maintain a flag representing when the user is pressing Fire.
    void ProcessInputs()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!IsFiring)
            {
                IsFiring = true;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (IsFiring)
            {
                IsFiring = false;
            }
        }
    }

    #endregion
}