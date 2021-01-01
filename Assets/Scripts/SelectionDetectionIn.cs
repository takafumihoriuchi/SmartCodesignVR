using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionDetectionIn : MonoBehaviour
{
	public GameObject[] inObjectsArr;
	public static GameObject inSelectedObject;
	public static bool inSelected;

	public float clipSpeed = 1.0f; // speed to clip to selection-box
	private Vector3 targetPos;

    void Start() {
        inSelected = false;
        targetPos = transform.position;
    }

    void Update() {
        if (inSelected) {
        	bool isGrabbed = inSelectedObject.transform.GetComponent<OVRGrabbable>().isGrabbed;
        	if (!isGrabbed) {
        		float step = clipSpeed * Time.deltaTime;
		        Vector3 currentPos = inSelectedObject.transform.position;
		        Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
		        inSelectedObject.transform.position = newPos;
        	}
        }
    }

    bool IsElementOf(GameObject obj, GameObject[] arr) {
    	foreach (GameObject elm in arr)
    		if (elm == obj) return true;
    	return false;
    }

    void OnTriggerEnter(Collider other) {
    	if (inSelected == false && IsElementOf(other.gameObject, inObjectsArr)) {
    		Debug.Log("Input Card choice: " + other.name);
    		inSelectedObject = other.gameObject;
    		inSelected = true;
    		inSelectedObject.transform.GetComponent<Rigidbody>().drag = 20;
    		inSelectedObject.transform.GetComponent<Rigidbody>().angularDrag = 10.0F;
    	}
    }

    void OnTriggerExit(Collider other) {
    	if (other.gameObject == inSelectedObject) {
			Debug.Log("Input Card is cancelled:" + other.name);
    		inSelectedObject.transform.GetComponent<Rigidbody>().drag = 1;
    		inSelectedObject.transform.GetComponent<Rigidbody>().angularDrag = 0.5F;
    		inSelectedObject = null;
    		inSelected = false;
    	}
    }

}