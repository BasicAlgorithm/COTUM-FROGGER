using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionWithSomeCar : MonoBehaviour
{


    private void Start()
    {
    }
    private void Update()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Temporal comment to test all work on scene FirstLevel
        /*
        if (collision.gameObject.tag == "SomeCar")
        {
            Debug.Log("YOU LOSE");
            SceneManager.LoadScene("LoseRoom");
        }
        if (collision.gameObject.tag == "WinObject")
        {
            Debug.Log("YOU WIN");
            SceneManager.LoadScene("MenuRoom");
        }
        */
    }
}
