using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{

    float speed = 0.2f;
    bool crawling = false;

    // Start is called before the first frame update
    void Start()
    {
        // init text here, more space to work than in the Inspector (but you could do that instead)
        //var tc = GetComponent<credits> (GUIText);
        //CameraCotum _cameraWork = gameObject.GetComponent<CameraCotum>();
        //var tc = GetComponent<text(UI.text);
        string creds = "Executive Producer:\nMr. Moneybags\n";
        creds += "Art Director:\nArt Guy\n";
        creds += "Technical Director:\nJohn Yaya\n";
        creds += "Programming:\nPoindexter Kopnik\n";
        creds += "Level Design:\nRandy Bugger\n";

        creds += "Copyright 2011 Worlds Greatest Game Co";
        //object.text = creds;
        //tc.text = creds;
        crawling = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!crawling)
            return;
        transform.Translate(Vector3.up * Time.deltaTime * speed);
        if (gameObject.transform.position.y > .8)
        {
            crawling = false;
        }
    }
}
