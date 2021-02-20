using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneCore : MonoBehaviour
{
	[SerializeField] private OVRScreenFade screenFade = null;
	[SerializeField] private SpriteRenderer buttonRedHighlightImage = null;
    // initializing with 'null' to avoid compilar warnings
	private WaitForSeconds oneSec = new WaitForSeconds(1f);
	private Color spriteColor;
	private AudioSource clickSound;

    void Start()
    {
        clickSound = GetComponent<AudioSource>();

        spriteColor = buttonRedHighlightImage.color;
        spriteColor.a = 0f;
        StartCoroutine(BlinkContollerImage());
    }
    
    void Update()
    {
        // accept keyboard input for development purpose
        if (OVRInput.GetDown(OVRInput.RawButton.A)
            || Input.GetKeyDown(KeyCode.A)) {
			clickSound.Play();
            //screenFade.fadeColor = Color.white;
            screenFade.fadeColor = Color.black;
            screenFade.FadeOut();
			SceneManager.LoadScene(1, LoadSceneMode.Single);
		}
    }

    IEnumerator BlinkContollerImage() {
    	while (true) {
    		spriteColor.a = 1f - spriteColor.a;
            buttonRedHighlightImage.color = spriteColor;
    		yield return oneSec;
    	}
    }

}
