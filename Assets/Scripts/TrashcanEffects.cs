using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashcanEffects : MonoBehaviour
{
	private GameObject TrashBin;
	private AudioSource alert_sound;

    void Start() {
    	TrashBin = GameObject.Find("trashBin");
    	// get sound
    	// alert_sound = TrashBin.GetComponent<AudioSource>();
    }

    void Update() {    	
    }

    void OnCollisionEnter(Collision col) {
		// play alert sound when trash-ball collides with the gound-plane
		// if (col.gameObject.name.Equals("GroundPlane")) {
		// 	if (!alert_sound.isPlaying) alert_sound.Play();
		// }
	}

}
