using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class to_delete : MonoBehaviour
{
    private float m_Speed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.transform.Translate(Camera.main.transform.forward * Time.deltaTime * m_Speed);
        //Camera.main.transform.Translate(Camera.main.transform.forward * Time.deltaTime * m_Speed);
        //transform.position += Camera.main.transform.forward * Time.deltaTime * m_Speed;
        //Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.position = Camera.main.transform.position;

        //Camera.main.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z));
        //Camera.main.transform.Rotate(0.0f, transform.rotation.y - Camera.main.transform.rotation.y, 0.0f);
    }
}
