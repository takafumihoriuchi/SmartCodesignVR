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

    void Start() {
    	comboComp = false;
    	playedDiveVoice = false;
    	introVoice.PlayDelayed(3.5f);
    	// TODO: better to wait for the user to wear the HMD (e.g. play when user comes near the boxes)
    }

    void Update() {
        // update card completion status every frame (TODO: improve computational efficiency)
        comboComp = SelectionDetectionEnv.envSelected
                 && SelectionDetectionIn.inSelected
                 && SelectionDetectionOut.outSelected;
        if (comboComp) {
        	if (!diveVoice.isPlaying && !playedDiveVoice) { // hasn't played voice yet
        		if (introVoice.isPlaying) introVoice.Stop();
        		diveVoice.PlayDelayed(1);
        		playedDiveVoice = true;
        		Debug.Log("[env, in, out] = [" + SelectionDetectionEnv.envSelectedObject.name + ", " + SelectionDetectionIn.inSelectedObject.name + ", " + SelectionDetectionOut.outSelectedObject.name + "]");
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
