using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Events;
using TextSpeech;

public class VoiceController : MonoBehaviour
{
    const string LANG_CODE = "es-ES";

    private bool is_listening;

    [SerializeField]
    TextMesh uiText;

    void Awake()
    {
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
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
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
        uiText.text = result;
        is_listening = false;
    }

    void OnFinalSpeechResult(string result)
    {
        OnPartialSpeechResult(result);
    }

    #endregion
}