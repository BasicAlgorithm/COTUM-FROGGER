using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoiceEventController : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuTextObjects;

    [SerializeField]
    private GameObject nivelesTextObjects;

    private string context;
    [SerializeField]
    public string initialContext;

    void Start()
    {
        context = initialContext;
    }

    public void MenuEvent()
    {
        if (context == "juego")
        {
            SceneManager.LoadScene(0);
        }
    }

    public void JugarEvent()
    {
        if (context == "menu")
        {
            SceneManager.LoadScene(1);
        }
    }

    public void NivelesEvent()
    {
        if (context == "menu")
        {
            mainMenuTextObjects.SetActive(false);
            nivelesTextObjects.SetActive(true);
            context = "niveles";
        }
    }

    public void SalirEvent()
    {
        if (context == "menu")
        {
            Application.Quit();
        }
    }

    public void VolverEvent()
    {
        if (context == "niveles")
        {
            nivelesTextObjects.SetActive(false);
            mainMenuTextObjects.SetActive(true);
            context = "menu";
        }
    }

    public void NivelUnoEvent()
    {
        if (context == "niveles")
        {
            SceneManager.LoadScene(1);
        }
    }
}