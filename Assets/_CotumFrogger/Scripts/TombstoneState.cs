using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace COTUM
{
    public class TombstoneState : MonoBehaviourPunCallbacks
    {
        public int id;

        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (other.tag.Contains("Player"))
            {
                Debug.Log("Me estan reviviendo.");
                gameObject.GetComponent<PlayerManager>().PlayerAlive = true;
                Debug.Log("He revivido.");
            }
        }
    }
}