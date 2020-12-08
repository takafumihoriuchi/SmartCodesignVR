using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantomLib;

public class VoiceInterface : MonoBehaviour
{
	#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidPlugin.StartSpeechRecognizer(callbackGameObject, resultCallbackMethod, errorCallbackMethod, readyCallbackMethod, beginCallbackMethod);
	#endif

    void Start() {}
    void Update() {}

    private void onReadyForSpeech() {
    	Debug.Log("this is inside onReadyForSpeech\n");
    }
    private void onBeginningOfSpeech() {}
    private void onResults() {}
    private void onError() {}

}


/*
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech; // Only for Windows 10

public class VoiceInterface : MonoBehaviour
{
	private KeywordRecognizer myKeywordRecognizer;
	private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    // Start is called before the first frame update
    void Start()
    {
        actions.Add("trash", TrashAction);
        actions.Add("motion", MotionAction);
        actions.Add("sound", SoundAction);
        myKeywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        myKeywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
       	myKeywordRecognizer.Start();
    }

    private void OnDisable() {
    	myKeywordRecognizer.Stop();
    	myKeywordRecognizer.Dispose();
    }

    // Update is called once per frame
    void Update() {}

    private void RecognizedSpeech(PhraseRecognizedEventArgs args) {
    	Debug.Log(args.text);
    	actions[args.text].Invoke();
    }

    private void TrashAction() {}
    private void MotionAction() {}
    private void SoundAction() {}

}
*/