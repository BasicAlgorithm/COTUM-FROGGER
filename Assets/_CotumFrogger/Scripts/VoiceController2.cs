using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using TextSpeech;

public class VoiceController2 : MonoBehaviour
{
    const string LANG_CODE = "es-ES";

    private string current_partial_result;

    [SerializeField]
    public List<WordEvent> events;

    void Awake()
    {
        current_partial_result = "";
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

    #region Speech to Text
    public void StartListening()
    {
        SpeechToText.instance.StartRecording();
    }

    public void StopListening()
    {
        SpeechToText.instance.StopRecording();
    }

    void OnPartialSpeechResult(string result)
    {
        if (current_partial_result != result)
        {
            string last_word = result.Substring(result.LastIndexOf(' ') + 1);

            foreach (var word_event in events)
            {
                if (last_word.Contains(word_event.activation_word))
                {
                    word_event.action?.Invoke();
                }
            }
            current_partial_result = result;
        }
    }

    void OnFinalSpeechResult(string result)
    {
        current_partial_result = "";
    }

    #endregion
}