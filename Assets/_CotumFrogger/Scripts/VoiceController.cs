using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using TextSpeech;

public class VoiceController : MonoBehaviour
{
    const string LANG_CODE = "es-ES";

    [SerializeField]
    public List<WordEvent> events;

    private Transform main_camera_transform;
    private Transform player_transform;

    private Vector3 camera_forward;
    private Vector3 camera_right;

    private string current_partial_result;

    private bool is_listening;

    void Awake()
    {
        main_camera_transform = Camera.main.transform;
        player_transform = GameObject.FindWithTag("Player").transform;
        current_partial_result = "";
        is_listening = false;
    }

    void Start()
    {
        Setup(LANG_CODE);

        SpeechToText.instance.onPartialResultsCallback = OnPartialSpeechResult;
        SpeechToText.instance.onResultCallback = OnFinalSpeechResult;

        CheckPermission();
    }

    void Setup(string lang_code)
    {
        SpeechToText.instance.Setting(lang_code);
    }

    void CheckPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    void Update()
    {
        if (!is_listening)
        {
            StartListening();
        }
    }

    #region Speech to Text
    public void StartListening()
    {
        SpeechToText.instance.StartRecording();
        is_listening = true;
    }

    public void StopListening()
    {
        SpeechToText.instance.StopRecording();
        is_listening = false;
    }

    void OnPartialSpeechResult(string result)
    {
        if (current_partial_result != result)
        {
            string last_word = result.Substring(result.LastIndexOf(' ') + 1);

            foreach(var word_event in events)
            {
                if (last_word.Contains(word_event.activation_word))
                {
                    word_event.action?.Invoke();
                }
            }
            /*
            if (last_word.Contains("adelante"))
            {
                Vector2 movement = new Vector2(0, 1);

                GetCameraForwardRightVectors();

                player_transform.position += camera_forward * movement.y;
            }
            else if (last_word.Contains("atr�s"))
            {
                Vector2 movement = new Vector2(0, -1);

                GetCameraForwardRightVectors();

                player_transform.position += camera_forward * movement.y;
            }
            else if (last_word.Contains("izquierda"))
            {
                Vector2 movement = new Vector2(-1, 0);

                GetCameraForwardRightVectors();

                player_transform.position += camera_right * movement.x;
            }
            else if (last_word.Contains("derecha"))
            {
                Vector2 movement = new Vector2(1, 0);

                GetCameraForwardRightVectors();

                player_transform.position += camera_right * movement.x;
            }
            */
            StopListening();
            current_partial_result = result;
        }
    }

    void OnFinalSpeechResult(string result)
    {
        current_partial_result = "";
        is_listening = false;
    }

    #endregion

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