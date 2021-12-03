using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class CollisionWithSomeCar : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("The Tombstone Prefab")]
    [SerializeField]
    private GameObject tombstonePrefab;

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Temporal comment to test all work on scene FirstLevel
        if (!photonView.IsMine)
        {
            return;
        }

        if (collision.tag.Contains("SomeCar"))
        {
            gameObject.SetActive(false);
            PhotonNetwork.Instantiate(this.tombstonePrefab.name, gameObject.transform.position, Quaternion.identity, 0);
            Debug.Log("YOU LOSE");
            //SceneManager.LoadScene("LoseRoom");
        }
        else if (collision.tag.Contains("WinObject"))
        {
            Debug.Log("YOU WIN");
            //SceneManager.LoadScene("MenuRoom");
        }
    }

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("CollisionWithSomeCar::OnPhotonSerializeView");
    }

    #endregion
}
