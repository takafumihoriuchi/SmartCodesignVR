using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombinationManagement : MonoBehaviour
{
	public GameObject envBox;
	public GameObject inBox;
	public GameObject outBox;

	public GameObject[] envObjects;
	public GameObject[] inObjects;
	public GameObject[] outObjects;

	public float speed = 1.0f;
	private Transform targetEnv;
	private Transform targetIn;
	private Transform targetOut;

    void Start() {

    }

    void Update() {
        
        // TODO: fix inefficient computation; change to other methods such as OnTriggerEnter.
    	/*
        foreach (GameObject env in envObjects) {
        	if (Vector3.Distance(env.transform.position, envBox.transform.position) < 2.0f) {
        		//
        	}
        }

		// Move our position a step closer to the target.
		float step =  speed * Time.deltaTime; // calculate distance to move
		transform.position = Vector3.MoveTowards(transform.position, target.position, step);

		// Check if the position of the cube and sphere are approximately equal.
		if (Vector3.Distance(transform.position, target.position) < 0.001f)
		{
			// Swap the position of the cylinder.
			target.position *= -1.0f;
		}
		*/

    }

}
