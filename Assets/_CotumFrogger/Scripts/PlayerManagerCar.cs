using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManagerCar : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Public Fields

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    #endregion

    #region MonoBehaviour CallBacks

    public void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    #endregion
}
