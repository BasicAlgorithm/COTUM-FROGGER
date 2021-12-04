using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour
{

    private int players_colliding;

    // Start is called before the first frame update
    void Start()
    {
        this.players_colliding = 0;
    }

    // Update is called once per frame
    void Update() {
        int total = PhotonNetwork.CurrentRoom.PlayerCount;
        if (this.players_colliding >= 2 * total) {
            Debug.Log("You WIN!");
            SceneManager.LoadScene("WinRoom");   
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag.Contains("Player")) {
            this.players_colliding += 1;
        }
    }
    void OnTriggerExit(Collider other)
     {
         if (other.tag.Contains("Player")) {
            this.players_colliding -= 1;
         }
     }
}
