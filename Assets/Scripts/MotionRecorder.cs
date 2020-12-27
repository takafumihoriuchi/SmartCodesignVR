using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionRecorder : MonoBehaviour
{
	private bool recording;

    void Start() {
		recording = false;        
    }

    void Update() {
		if (!recording) {
			if (OVRInput.GetDown(OVRInput.RawButton.B) || OVRInput.GetDown(OVRInput.RawButton.Y)) {
				recording = true;
				Debug.Log("recording started");
			}
		} else {
			if (OVRInput.GetDown(OVRInput.RawButton.B) || OVRInput.GetDown(OVRInput.RawButton.Y)) {
				recording = false;
				Debug.Log("recording ended");	
			}
		}

    }

}

// boolean of button pressed / release
// GetDown => true when pressed
// Get => true while pressed
// GetUp => true when released