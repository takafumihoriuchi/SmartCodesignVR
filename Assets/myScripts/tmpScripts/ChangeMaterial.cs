using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
	public Material[] material;
	Renderer rend;

    void Start() {
        rend = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
    }

    void OnCollisionEnter(Collision col) {
    	// Debug.Log("trashBin collided with " + col.gameObject.name);
    	if (col.gameObject.name.Equals("RedPaint"))
    		rend.sharedMaterial = material[1];
    	else if (col.gameObject.name.Equals("GreenPaint"))
    		rend.sharedMaterial = material[2];
    	else if (col.gameObject.name.Equals("BluePaint"))
    		rend.sharedMaterial = material[3];
    	else if (col.gameObject.name.Equals("YellowPaint"))
    		rend.sharedMaterial = material[4];
    	else if (col.gameObject.name.Equals("VioletPaint"))
    		rend.sharedMaterial = material[5];
    }

    void Update() {}

}
