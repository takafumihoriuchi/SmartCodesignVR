using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionRecorder : MonoBehaviour
{
	private bool recording;
	private bool allowRecording;

 	public GameObject parentOfTrail;
 	public TrailRenderer originalTrail;
 	private TrailRenderer[] trailArr;
 	private int sizeOfArr = 100;
 	private int trailIndex;
 	// TODO: change from array to other "sustainable" data structure (e.g. linked list)
 	// must be able to delete as well; specific instance when designated (TrailRenderer.Clear)

    void Start() {
		allowRecording = true;
		recording = false;
		trailArr = new TrailRenderer[sizeOfArr];
		trailIndex = 0;
		trailArr[trailIndex] = originalTrail;
		trailArr[trailIndex].emitting = false;
		trailIndex++;
    }

    void Update() {
		if (allowRecording) {
			if (recording == false && (OVRInput.GetDown(OVRInput.RawButton.B) || OVRInput.GetDown(OVRInput.RawButton.Y))) {
				recording = true;
				Debug.Log("RECORDING IN PROGRESS...");
				trailArr[trailIndex] = Instantiate(originalTrail, parentOfTrail.transform.position, Quaternion.identity, parentOfTrail.transform) as TrailRenderer;
				trailArr[trailIndex].emitting = true;
				// TODO: change color of each trajectory
			} else if (recording == true && (OVRInput.GetUp(OVRInput.RawButton.B) || OVRInput.GetUp(OVRInput.RawButton.Y))) {
				recording = false;
				Debug.Log("RECORDING END");
				trailArr[trailIndex].emitting = false;
				trailIndex++;
			}
		}

    }


}


/*
TODO
- bring objects back to initial position when thrown outside of the platform

Notes:

- to make skybox clearFlags (similar to changing skybox material):
public Camera camera;
camera.clearFlags = CameraClearFlags.Skybox;

- boolean of button pressed / release on touch-controller
GetDown => true when pressed
Get => true while pressed
GetUp => true when released

- consider the use of the "mission card"

-

*/