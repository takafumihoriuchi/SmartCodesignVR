using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardCombinationManagement : MonoBehaviour
{
	public AudioSource introVoice;
	public AudioSource diveVoice;
	private bool comboComp;
    private bool playedDiveVoice;

    public GameObject OVRCamera;

    public static Dictionary<string, GameObject> selectionDict = new Dictionary<string, GameObject>() {
    	{"environment", null},
    	{"input", null},
    	{"output", null}
    };

    void Start() {
    	comboComp = false;
    	playedDiveVoice = false;
    	introVoice.PlayDelayed(3.5f);
    }

    void Update() {
        // update card completion status every frame (TODO: improve computational efficiency)
        comboComp = selectionDict["environment"]!=null && selectionDict["input"]!=null && selectionDict["output"]!=null;
        if (comboComp) {
        	if (!diveVoice.isPlaying && !playedDiveVoice) { // hasn't played voice yet
        		if (introVoice.isPlaying) introVoice.Stop();
        		diveVoice.PlayDelayed(1);
        		playedDiveVoice = true;
        		// finalize card choice:
        		CardSelectionTracker.selectionDict["environment"] = selectionDict["environment"].name;
        		CardSelectionTracker.selectionDict["input"] = selectionDict["input"].name;
        		CardSelectionTracker.selectionDict["output"] = selectionDict["output"].name;
        		Debug.Log("[env, in, out] = [" + CardSelectionTracker.selectionDict["environment"] + ", " + CardSelectionTracker.selectionDict["input"] + ", " + CardSelectionTracker.selectionDict["output"] + "]");
			}
			if (!diveVoice.isPlaying && playedDiveVoice) { // has already finished playing voice
				OVRCamera.GetComponent<OVRScreenFade>().FadeOut();
				Debug.Log("Moving on to the next scene.");
				SceneManager.LoadScene(2);
			}
        } else {
        	if (diveVoice.isPlaying) {
        		diveVoice.Stop();
        		playedDiveVoice = false;
        	}
        }
    }

}
