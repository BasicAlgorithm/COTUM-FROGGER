using Photon.Pun;
using UnityEngine;

namespace COTUM
{
	public class PlayerAnimatorManager : MonoBehaviourPun 
	{
        #region Private Fields

        [SerializeField]
	    private float directionDampTime = 0.25f;
        Animator animator;

		#endregion

		#region MonoBehaviour CallBacks

		// MonoBehaviour method called on GameObject by Unity during initialization phase.
	    void Start () 
	    {
	        animator = GetComponent<Animator>();
	    }

		// MonoBehaviour method called on GameObject by Unity on every frame.
		void Update()
		{

			// Prevent control is connected to Photon and represent the localPlayer
			if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
			{
				return;
			}

			// failSafe is missing Animator component on GameObject
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
				if (Input.GetButtonDown("Fire2")) animator.SetTrigger("Jump");
			}

			if (SystemInfo.deviceType == DeviceType.Desktop)
			{
				// deal with movement
				float h = Input.GetAxis("Horizontal");
				float v = Input.GetAxis("Vertical");

				Debug.Log("ONE MOVE");
				Debug.Log("moveforward" + h);
				Debug.Log("moveforward" + v);

				// prevent negative Speed.
				if (v < 0)
				{
					v = 0;
				}

				// set the Animator Parameters
				animator.SetFloat("Speed", h * h + v * v);
				animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
			}
			else if (SystemInfo.deviceType == DeviceType.Desktop)
			{
				//animator.SetFloat("Speed", h * h + v * v);
				//animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
			}
		}

		#endregion

	}
}