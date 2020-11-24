using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class ColorControllerTutorial : MonoBehaviour
{
	public Material[] wallMaterial;
	Renderer rend;

	public Text displayText;

    void Start()
    {
		rend = GetComponent<Renderer>();
		rend.enabled = true;
		displayText.text = "";
    }

    private void OnCollisionEnter(Collision col) {
    	if (col.gameObject.name == "player-ball") {
    		displayText.text = "Collision detected!";
    		rend.sharedMaterial = wallMaterial[0];
    	}
    }

    private void OnCollisionExit(Collision col) {
    	if (col.gameObject.name == "player-ball") {
    		rend.sharedMaterial = wallMaterial[1];
    		displayText.text = "No collision detected.";
    	}
    }

    void Update() {}
}
