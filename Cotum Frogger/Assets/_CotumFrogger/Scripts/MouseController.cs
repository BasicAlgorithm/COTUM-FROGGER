using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Update is called once per frame

    private Transform main_camera_transform;
    private Vector3 camera_forward;
    private Vector3 camera_right;

    void Awake()
    {
        main_camera_transform = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 movement = new Vector2(0, 5);

            GetCameraForwardRightVectors();

            main_camera_transform.position += camera_forward * movement.y;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 movement = new Vector2(0, -5);

            GetCameraForwardRightVectors();

            main_camera_transform.position += camera_forward * movement.y;
        }
    }

    private void GetCameraForwardRightVectors()
    {
        camera_forward = main_camera_transform.forward;
        camera_right = main_camera_transform.right;

        camera_forward.y = 0;
        camera_right.y = 0;

        camera_forward = camera_forward.normalized;
        camera_right = camera_right.normalized;
    }
}
