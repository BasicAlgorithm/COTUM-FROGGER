using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace COTUM
{
    public class SemaphoreController : MonoBehaviourPunCallbacks, IPunObservable
    {
        // Start is called before the first frame update
        #region Private Serializable Fields

        public int id;

        public string side;

        #endregion

        public static bool on_off_1 = false;
        public static bool on_off_2 = false;
        public static bool on_off_3 = false;

        void Start()
        {
        }

        // Update is called once per framexxxxx
        void Update()
        {
        }

        public void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (!other.tag.Contains("Player"))
            {
                return;
            }

            if (id == 1)
            {
                on_off_1 = true;

            }
            else if (id == 2)
            {
                SemaphoreController.on_off_2 = true;

            }
            else if (id == 3)
            {
                SemaphoreController.on_off_3 = true;

            }
            else
            {
                Debug.Log("COTUM: SC onTriggerStay() error on semaphore id");
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (!other.tag.Contains("Player"))
            {
                return;
            }

            if (id == 1)
            {
                SemaphoreController.on_off_1 = false;

            }
            else if (id == 2)
            {
                SemaphoreController.on_off_2 = false;

            }
            else if (id == 3)
            {
                SemaphoreController.on_off_3 = false;

            }
            else
            {
                Debug.Log("COTUM: SC onTriggerStay() error on semaphore id");
            }

        }

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                if (id == 1)
                {
                    stream.SendNext(SemaphoreController.on_off_1);

                }
                else if (id == 2)
                {
                    stream.SendNext(SemaphoreController.on_off_2);

                }
                else if (id == 3)
                {
                    stream.SendNext(SemaphoreController.on_off_3);

                }
                else
                {
                    Debug.Log("COTUM: SC OnPhotonSerializeView() error on semaphore id");
                }
            }
            else
            {
                // Network player, receive data
                if (id == 1)
                {
                    SemaphoreController.on_off_1 = (bool)stream.ReceiveNext();

                }
                else if (id == 2)
                {
                    SemaphoreController.on_off_2 = (bool)stream.ReceiveNext();

                }
                else if (id == 3)
                {
                    SemaphoreController.on_off_3 = (bool)stream.ReceiveNext();

                }
                else
                {
                    Debug.Log("COTUM: SC OnPhotonSerializeView() error on semaphore id");
                }
            }
        }

        #endregion
    }
}
