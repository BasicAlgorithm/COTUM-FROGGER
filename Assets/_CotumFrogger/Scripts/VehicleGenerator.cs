using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public float velocity;
    public float mov_left_right;
    public float pos_initial_x;
    public float pos_initial_y;
    public float pos_initial_z;
    public float pos_end_z;
    void Start()
    {
         if (mov_left_right == -1) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
         } else {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
         }
        transform.position = new Vector3(pos_initial_x, pos_initial_y, pos_initial_z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0.0f, 0.0f, velocity * Time.deltaTime);
        if ((mov_left_right * transform.position.z) > (pos_end_z * mov_left_right)) {
            Start();
        }
    }
}
