using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionDetectionEnv : MonoBehaviour
{
	public GameObject[] envObjectsArr;
	public static GameObject envSelectedObject;
	public static bool envSelected;

	public float clipSpeed = 1.0f; // speed to clip to selection-box
	private Vector3 targetPos;

    void Start() {
        envSelected = false;
        targetPos = transform.position;
    }

    void Update() {
        if (envSelected) {
        	bool isGrabbed = envSelectedObject.transform.GetComponent<OVRGrabbable>().isGrabbed;
        	if (!isGrabbed) {
        		float step = clipSpeed * Time.deltaTime;
		        Vector3 currentPos = envSelectedObject.transform.position;
		        Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
		        envSelectedObject.transform.position = newPos;
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
    		envSelectedObject.transform.GetComponent<Rigidbody>().drag = 20;
    		envSelectedObject.transform.GetComponent<Rigidbody>().angularDrag = 10.0F;
    	}
    }

    void OnTriggerExit(Collider other) {
    	if (other.gameObject == envSelectedObject) {
    		Debug.Log("Environment Card is cancelled:" + other.name);
    		envSelectedObject.transform.GetComponent<Rigidbody>().drag = 1;
    		envSelectedObject.transform.GetComponent<Rigidbody>().angularDrag = 0.5F;
    		envSelectedObject = null;
    		envSelected = false;
    	}
    }

}

/*
- example usage of Distance():
if (Vector3.Distance(newPos, targetPos) < 0.001f) atBoxCenter = true;
- isKinematic = true is similart to disableing a rigidbody object
envSelectedObject.transform.GetComponent<Rigidbody>().isKinematic = true;
*/