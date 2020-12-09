﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextSpeech;
using UnityEngine.Android;
using UnityEngine.UI;


public class VoiceController : MonoBehaviour
{
    const string LANG_CODE = "en-US";

    [SerializeField]
    Text uiText;

    private void Start() {
    	Setup(LANG_CODE);

#if UNITY_ANDROID
    	SpeechToText.instance.onPartialResultsCallback = OnPartialSpeechResult;
#endif
    	SpeechToText.instance.onResultCallback = OnFinalSpeechResult;
    	TextToSpeech.instance.onStartCallBack = OnSpeakStart;
    	TextToSpeech.instance.onDoneCallback = OnSpeakStop;
    	CheckPermission();
    }

    void CheckPermission() {
#if UNITY_ANDROID
    	if (!Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
    		Permission.RequestUserPermission(Permission.Microphone);
    	}
#endif
    }

    #region Text to Speech
    public void StartSpeaking(string message) {
    	TextToSpeech.instance.StartSpeak(message);
    }
    public void StopSpeaking() {
    	TextToSpeech.instance.StopSpeak();
    }
    void OnSpeakStart() {
    	Debug.Log("Talking started...");
    }
    void OnSpeakStop() {
    	Debug.Log("Talking stopped...");
    }
    #endregion

    #region Speech to Text
    public void StartListening() {
    	SpeechToText.instance.StartRecording();
    }
    public void StopListening() {
    	SpeechToText.instance.StopRecording();
    }
    void OnFinalSpeechResult(string result) {
    	Debug.Log("result of speech recognition: " + result);
    	uiText.text = result;
    }
    void OnPartialSpeechResult(string result) {
    	Debug.Log("result of partial speech recognition: " + result);
    	uiText.text = result;
    }
    #endregion

    private void Setup(string code) {
    	TextToSpeech.instance.Setting(code, 1, 1);
    	SpeechToText.instance.Setting(code);
    }
    
}