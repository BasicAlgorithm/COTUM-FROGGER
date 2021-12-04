using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COTUM
{
    public class VehicleGenerator : MonoBehaviour
    {
        // Start is called before the first frame update
        public float velocity;
        public float mov_left_right;
        public float pos_initial_x;
        public float pos_initial_y;
        public float pos_initial_z;
        public float pos_end_z;


        public bool s_1 = false;
        public bool s_2 = false;
        public bool s_3 = false;

        public bool space_1 = false;
        public bool space_2 = false;
        public bool space_3 = false;
        void Start()
        {
            if (mov_left_right == -1)
            {
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }
            else
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            transform.position = new Vector3(pos_initial_x, pos_initial_y, pos_initial_z);
        }

        // Update is called once per frame
        // Update is called once per frame
        void Update()
        {
            s_1 = SemaphoreController.on_off_1;
            s_2 = SemaphoreController.on_off_2;
            s_3 = SemaphoreController.on_off_3;

            space_1 = StopSemaphore1();
            space_2 = StopSemaphore2();
            space_3 = StopSemaphore3();
            // semaphore_1
            if (SemaphoreController.on_off_1 && StopSemaphore1())
            {
                return;
            }

            if (SemaphoreController.on_off_2 && StopSemaphore2())
            {
                return;
            }

            if (SemaphoreController.on_off_3 && StopSemaphore3())
            {
                return;
            }


            transform.Translate(0.0f, 0.0f, velocity * Time.deltaTime);
            if ((mov_left_right * transform.position.z) > (pos_end_z * mov_left_right))
            {
                Start();
            }

        }

        bool StopSemaphore1()
        {
            if (transform.position.x >= 12.5f && transform.position.x <= 16.5f &&
                transform.position.z >= 18.2f && transform.position.z <= 35.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool StopSemaphore2()
        {
            if (transform.position.x >= -5.3f && transform.position.x <= -2.8f &&
                transform.position.z >= 37.75f && transform.position.z <= 47.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool StopSemaphore3()
        {
            if (transform.position.x >= -23.5f && transform.position.x <= -20.0f &&
                transform.position.z >= 50.52f && transform.position.z <= 67.3f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}