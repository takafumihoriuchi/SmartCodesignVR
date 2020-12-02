using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashcanEffects : MonoBehaviour
{
	private GameObject TrashBin;
	private AudioSource alert_sound;
	// public Material[] mat;
	// private Renderer rend;

    void Start() {
    	TrashBin = GameObject.Find("trashBin");
    	alert_sound = TrashBin.GetComponent<AudioSource>();
    	// rend = TrashBin.GetComponent<Renderer>();
    	// rend.enabled = true;
    	// rend.shareMaterial = mat[0];
    }

    void Update() {
    	
    }

    void OnCollisionEnter(Collision col) {
		if (col.gameObject.name.Equals("ground-plane")) {
			if (!alert_sound.isPlaying) {
				alert_sound.Play();
				// rend.shareMaterial = material;
			}
		}
	}

}
