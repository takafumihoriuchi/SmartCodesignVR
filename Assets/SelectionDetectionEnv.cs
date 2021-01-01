using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionDetectionEnv : MonoBehaviour
{
	public GameObject[] envObjectsArr;
	private GameObject envSelectedObject;
	private bool envSelected;

	public float speed = 0.2f; // speed to clip to selection-box
	private Vector3 currentPos;
	private Vector3 targetPos;
	// private bool atBoxCenter;

    void Start() {
        envSelected = false;
        // atBoxCenter = false;
        targetPos = transform.position;
    }

    void Update() {
        if (envSelected) {
        	// bool isGrabbed = OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger)
        	// 				 || OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)
        	// 				 || OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger)
        	// 				 || OVRInput.GetDown(OVRInput.RawButton.LHandTrigger);
        	bool isGrabbed = envSelectedObject.transform.GetComponent<OVRGrabbable>().isGrabbed;
        	if (!isGrabbed) {
        		float step = speed * Time.deltaTime;
		        Vector3 currentPos = envSelectedObject.transform.position;
		        Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
		        envSelectedObject.transform.position = newPos;
		        // if (Vector3.Distance(newPos, targetPos) < 0.001f) atBoxCenter = true;
		        // else atBoxCenter = false;
        	}
        }
    }

    bool IsElementOf(GameObject obj, GameObject[] arr) {
    	foreach (GameObject elm in arr)
    		if (elm == obj) return true;
    	return false;
    }

    void OnTriggerEnter(Collider other) {
    	if (envSelected == false && IsElementOf(other.gameObject, envObjectsArr)) {
    		Debug.Log("Environment Card choice: " + other.name);
    		envSelectedObject = other.gameObject;
    		envSelected = true;
    	}
    }

    void OnTriggerExit(Collider other) {
    	if (other.gameObject == envSelectedObject) {
    		envSelectedObject = null;
    		envSelected = false;
    	}
    }

}

/*
example usage of Distance():
if (Vector3.Distance(envSelectedObject.transform.position, targetPos) < 0.001f) { ... }
*/