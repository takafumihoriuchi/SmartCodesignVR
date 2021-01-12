using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
	[SerializeField] OVRScreenFade fade;

	public SpriteRenderer redHighlightImage;
	private WaitForSeconds _oneSec = new WaitForSeconds(1f);
	private Color _spriteColor;

	AudioSource clickSound;

    void Start()
    {
        _spriteColor = redHighlightImage.color;
        _spriteColor.a = 0f;
        StartCoroutine(BlinkContollerImage());

        clickSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A)
        	|| OVRInput.GetDown(OVRInput.RawButton.X) ||Input.GetKeyDown("space") ) {
			clickSound.Play();
			fade.fadeColor = Color.white;
			fade.FadeOut();
			SceneManager.LoadScene(1, LoadSceneMode.Single);
		}
    }

    IEnumerator BlinkContollerImage() {
    	while (true) {
    		_spriteColor.a = 1f - _spriteColor.a;
    		redHighlightImage.color = _spriteColor;
    		yield return _oneSec;
    	}
    }

}
