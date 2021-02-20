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
        if (transform.position.z > originalPosition.z) {
        	transform.position = originalPosition;
        }
        if (transform.position.z < buttonTrigger.transform.position.z) {
        	transform.position = new Vector3(transform.position.x, transform.position.y, buttonTrigger.transform.position.z);
        }
        if (transform.position.z == buttonTrigger.transform.position.z) {
        	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
