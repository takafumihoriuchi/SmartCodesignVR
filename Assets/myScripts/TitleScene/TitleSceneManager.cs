using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private OVRScreenFade screenFade = null;
    [SerializeField] private SpriteRenderer buttonHighlightRenderer = null;
    private Color buttonHighlightColor;
    private AudioSource transitionSound;
    const float blinkInterval = 0.5f;

    private AsyncOperation asyncLoad;
    private bool isTransitioning = false;

    void Start()
    {
        StartCoroutine(AsyncLoadMainScene());

        transitionSound = GetComponent<AudioSource>();
        buttonHighlightColor = buttonHighlightRenderer.color;
        buttonHighlightColor.a = 0f;
        StartCoroutine(BlinkContollerImage());
    }
    
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetKeyDown(KeyCode.A))
        {
            if (!isTransitioning)
            {
                StartCoroutine(TransitionToMainScene());
            }
        }
    }

    IEnumerator TransitionToMainScene()
    {
        isTransitioning = true;
        transitionSound.Play();
        screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeTime);
        asyncLoad.allowSceneActivation = true;
    }

    IEnumerator AsyncLoadMainScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(1);
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone) yield return null;
    }

    IEnumerator BlinkContollerImage()
    {
        while (true)
        {
            buttonHighlightColor.a = 1f - buttonHighlightColor.a;
            buttonHighlightRenderer.color = buttonHighlightColor;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

}
