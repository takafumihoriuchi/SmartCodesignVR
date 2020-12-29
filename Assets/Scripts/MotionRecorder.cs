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
B|Yボタンを押しているときは、新しい線を描画できるようにしたい。
話している時は、描画できなくしたい。

*/

// boolean of button pressed / release
// GetDown => true when pressed
// Get => true while pressed
// GetUp => true when released