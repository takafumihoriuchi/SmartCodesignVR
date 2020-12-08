using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceInterface : MonoBehaviour
{

    void Start() {}
    void Update() {}

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