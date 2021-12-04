using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


namespace COTUM
{
    public class CheckpointController : MonoBehaviour
    {

        private int players_colliding;

        // Start is called before the first frame update
        void Start()
        {
            this.players_colliding = 0;
        }

        // Update is called once per frame
        void Update()
        {
            int total = PhotonNetwork.CurrentRoom.PlayerCount;
            if (this.players_colliding >= 2 * total)
            {
                Debug.Log("You WIN!");
                //PhotonNetwork.Destroy("Semaphore");
                //int a = GameManager.Instance.ViewsOfSemaphores.Count;
                for (int i = 0; i< GameManager.Instance.ViewsOfSemaphores.Count; i++)
                {
                    PhotonNetwork.Destroy(GameManager.Instance.ViewsOfSemaphores[i]);
                }
                SceneManager.LoadScene("WinRoom");
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.tag.Contains("Player"))
            {
                this.players_colliding += 1;
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.tag.Contains("Player"))
            {
                this.players_colliding -= 1;
            }
        }
    }
}