using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerAnimatorManager : MonoBehaviourPun
{
    #region Private Fields

    [SerializeField]
    private float directionDampTime = 0.01f;

    private Animator animator;

    [SerializeField] 
    private float speed = 5.0f;

    #endregion

    #region MonoBehaviour CallBacks

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (!animator)
        {
            return;
        }
        // deal with Jumping
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // only allow jumping if we are running.
        if (stateInfo.IsName("Base Layer.Run"))
        {
            // When using trigger parameter
            if (Input.GetButtonDown("Fire2"))
            {
                animator.SetTrigger("Jump");
            }
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (v < 0)
        {
            v = 0;
        }
        animator.SetFloat("Speed", h * h + v * v);
        animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        //animator.SetFloat("vertical", v);
        //animator.SetFloat("horizontal", h);

    }

    #endregion
}
