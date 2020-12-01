using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLimit : MonoBehaviour
{
	public GameObject buttonTrigger;
	private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (transform.position.z >= originalPosition.z) {
        	transform.position = originalPosition;
        }
        if (transform.position.z < buttonTrigger.transform.position.z) {
        	transform.position = new Vector3(transform.position.x, transform.position.y, buttonTrigger.transform.position.z);
        }
        // if (Vector3.Distance(buttonTrigger.transform.position, transform.position) == 0) {
        if (transform.position.z == buttonTrigger.transform.position.z) {
        	Debug.Log("reloading scene...");
        	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
