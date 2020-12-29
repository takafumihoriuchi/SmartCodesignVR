﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBallCollisionDetection : MonoBehaviour
{
    void Start() {
	}

    void Update() {
    }

    void OnCollisionEnter(Collision col) {
    	Debug.Log("Trash-bin collided with: " + col.gameObject.name + " at " + col.contacts[0].thisCollider.name);
    	Collider collidedFace = col.contacts[0].thisCollider;
    	if (collidedFace.name.Equals("BaseFace")) {
    		Debug.Log("paper-ball collided with BASE-FACE");
    	} else if (collidedFace.name.Equals("FrontFace")) {
    		Debug.Log("paper-ball collided with FRONT-FACE");
    	} else if (collidedFace.name.Equals("BackFace")) {
    		Debug.Log("paper-ball collided with BACK-FACE");
    	} else if (collidedFace.name.Equals("RightFace")) {
    		Debug.Log("paper-ball collided with RIGHT-FACE");
    	} else if (collidedFace.name.Equals("LeftFace")) {
    		Debug.Log("paper-ball collided with LEFT-FACE");
    	}
    }

}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashcanEffects : MonoBehaviour
{
	// private GameObject TrashBin;
	// private AudioSource alert_sound;

    void Start() {
    	// TrashBin = GameObject.Find("trashBin");
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

*/