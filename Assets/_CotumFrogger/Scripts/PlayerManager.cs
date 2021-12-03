using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

namespace COTUM
{
	#pragma warning disable 649

    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The current Health of our player")]
        public bool PlayerAlive = true;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion

        #region Private Fields

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        private GameObject playerUiPrefab;

        [Tooltip("The Tombstone Prefab")]
        [SerializeField]
        private GameObject tombstonePrefab;

        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;

        //True, when the user is firing
        bool IsFiring;

        #endregion

        #region MonoBehaviour CallBacks

        // MonoBehaviour method called on GameObject by Unity during early initialization phase.
        public void Awake()
        {
            if (this.beams == null)
            {
                Debug.LogError("COTUM: PM.awake() Beams Reference.", this);
            }
            else
            {
                this.beams.SetActive(false);
            }

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization,
            // thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }

        // MonoBehaviour method called on GameObject by Unity during initialization phase.
        public void Start()
        {
        Debug.Log("COTUM: PM.start() CALLED");

            CameraCotum _cameraWork = gameObject.GetComponent<CameraCotum>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("COTUM: PM.start() CameraDesktop Component on player Prefab.", this);
            }

            // Create the UI
            if (this.playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(this.playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("COTUM: PM.start() PlayerUiPrefab reference on player Prefab.", this);
            }

            #if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            #endif
        }


		public override void OnDisable()
		{
			// Always call the base to remove callbacks
			base.OnDisable ();

			#if UNITY_5_4_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
			#endif
		}


        public void Update()
        {
            // we only process Inputs and check health if we are the local player
            if (photonView.IsMine)
            {
                this.ProcessInputs();

                if (this.Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
                gameObject.SetActive(this.PlayerAlive);
            }

            if (this.beams != null && this.IsFiring != this.beams.activeInHierarchy)
            {
                this.beams.SetActive(this.IsFiring);
            }
        }

        // MonoBehaviour method called when the Collider 'other' enters the trigger.
        // Affect Health of the Player if the collider is a beam
        // Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        // One could move the collider further away to prevent this or check if the beam belongs to the player.
        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (other.name.Contains("Beam"))
            {
                this.Health -= 0.1f;
            }
            if (other.tag.Contains("SomeCar"))
            {
                this.PlayerAlive = false;
                GameObject tombstone = PhotonNetwork.Instantiate(this.tombstonePrefab.name, this.transform.position, Quaternion.identity, 0);
                tombstone.GetComponent<TombstoneState>().id = photonView.ViewID;
                Debug.Log("YOU LOSE");
                Debug.Log(tombstone.GetComponent<TombstoneState>().id);
                //SceneManager.LoadScene("LoseRoom");
            }

            else if (other.tag.Contains("Tombstone"))
            {
                Debug.Log("Reviviendo al jugador");
                int playerToRevivePrefabId = other.GetComponent<TombstoneState>().id;
                PhotonView.Find(playerToRevivePrefabId).RPC("Revive", RpcTarget.AllBuffered);
                Debug.Log("Jugador revivido");
            }
        }

        /*[PunRPC]
        public void Revive()
        {
            this.PlayerAlive = true;
        }*/

        // MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        // We're going to affect health while the beams are touching the player
        // <param name="other">Other.</param>
        public void OnTriggerStay(Collider other)
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
            this.Health -= 0.1f*Time.deltaTime;
        }


        #if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
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

            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        #endregion

        #region Private Methods


		#if UNITY_5_4_OR_NEWER
		void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
		{
			this.CalledOnLevelWasLoaded(scene.buildIndex);
		}
		#endif

        // Processes the inputs. This MUST ONLY BE USED when the player has authority over
        // this Networked GameObject (photonView.isMine == true)
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // we don't want to fire when we interact with UI buttons for example.
                // IsPointerOverGameObject really means IsPointerOver*UI*GameObject
                // notice we don't use on on GetbuttonUp() few lines down, because one can mouse down,
                // move over a UI element and release, which would lead to not lower the isFiring Flag.
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //	return;
                }

                if (!this.IsFiring)
                {
                    this.IsFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1")) 
            {
                if (this.IsFiring)
                {
                    this.IsFiring = false;
                }
            }
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.IsFiring);
                stream.SendNext(this.Health);
                stream.SendNext(this.PlayerAlive);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                this.PlayerAlive = (bool)stream.ReceiveNext();
            }
        }

        #endregion
    }
}