using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSoundMonoBehaviourPasser : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Start() in MakeSoundMonoBehaviourPasser");
        MakeSoundCard coroutineAssistant = new MakeSoundCard();
        coroutineAssistant.MonoBehaviourReceiver(this);
    }
}
