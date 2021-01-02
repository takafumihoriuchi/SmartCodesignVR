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

    void Start() {
    	comboComp = false;
    	playedDiveVoice = false;
    	introVoice.PlayDelayed(3);
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
        		diveVoice.Play(0);
        		playedDiveVoice = true;
			}
			if (!diveVoice.isPlaying && playedDiveVoice) { // has already finished playing voice
				Debug.Log("Moving on to the next scene.");
				Debug.Log("[env, in, out] = [" + SelectionDetectionEnv.envSelectedObject.name + ", " + SelectionDetectionIn.inSelectedObject.name + ", " + SelectionDetectionOut.outSelectedObject.name + "]");
				SceneManager.LoadScene("PrototypingScene");
			}
        } else {
        	if (diveVoice.isPlaying) {
        		diveVoice.Stop();
        		playedDiveVoice = false;
        	}
        }
    }

}
