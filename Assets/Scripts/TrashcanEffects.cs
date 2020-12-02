using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashcanEffects : MonoBehaviour
{
	AudioSource alert_source;

    void Start() {
    	alert_source = GameObject.Find("trashBin").GetComponent<AudioSource>();
    }

    void Update() {
    	
    }

    void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name.Equals("ground-plane"))
			if (!alert_source.isPlaying) alert_source.Play();
	}

}
