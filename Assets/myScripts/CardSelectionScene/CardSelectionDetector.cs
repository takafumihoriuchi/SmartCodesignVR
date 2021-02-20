using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBridge;

public class CardSelectionDetector
{
    const float DRAG_L = 20f;
    const float DRAG_S = 1f;
    const float ANGDRAG_L = 10.0f;
    const float ANGDRAG_S = 0.5f;

    private GameObject selectionContainer;
    private GameObject[] candidateObjArr;
    private IComponentEventHandler eventBridgeHandler;
    private Vector3 targetPos;
    private string cardType;

    [SerializeField] private float attachmentSpeed = 1.0f;

    public CardSelectionDetector(GameObject obj, GameObject[] objArr, string typStr)
    {
        selectionContainer = obj;
        candidateObjArr = objArr;
        targetPos = obj.transform.position;
        eventBridgeHandler = selectionContainer.RequestEventHandlers();
        eventBridgeHandler.TriggerEnter += OnTriggerEnterEB;
        eventBridgeHandler.TriggerExit += OnTriggerExitEB;
        cardType = typStr;
    }

    private void OnTriggerEnterEB(Collider other)
    {
        if (!IsElementOf(other.gameObject, candidateObjArr)) return;
        if (CardSelectionSceneCore.selectionDict[cardType] == null)
        { // put object in empty container
            CardSelectionSceneCore.selectionDict[cardType] = other.gameObject;
            IncreaseDrag(CardSelectionSceneCore.selectionDict[cardType]);
        }
        else
        { // kickout current object and replace object in container
            DecreaseDrag(CardSelectionSceneCore.selectionDict[cardType]);
            CardSelectionSceneCore.selectionDict[cardType] = other.gameObject;
            IncreaseDrag(CardSelectionSceneCore.selectionDict[cardType]);
        }
    }

    private void OnTriggerExitEB(Collider other)
    {
        if (other.gameObject == CardSelectionSceneCore.selectionDict[cardType])
        { // release object from container
            DecreaseDrag(CardSelectionSceneCore.selectionDict[cardType]);
            CardSelectionSceneCore.selectionDict[cardType] = null;
        }
    }

    private void IncreaseDrag(GameObject obj)
    {
        obj.transform.GetComponent<Rigidbody>().drag = DRAG_L;
        obj.transform.GetComponent<Rigidbody>().angularDrag = ANGDRAG_L;
    }

    private void DecreaseDrag(GameObject obj)
    {
        obj.transform.GetComponent<Rigidbody>().drag = DRAG_S;
        obj.transform.GetComponent<Rigidbody>().angularDrag = ANGDRAG_S;
    }

    private bool IsElementOf(GameObject obj, GameObject[] arr)
    {
        foreach (GameObject elm in arr)
            if (elm == obj) return true;
        return false;
    }

    public void CenterGravityMotion()
    {
        if (CardSelectionSceneCore.selectionDict[cardType] == null) return;
        bool isGrabbed = CardSelectionSceneCore.selectionDict[cardType].
            transform.GetComponent<OVRGrabbable>().isGrabbed;
        if (!isGrabbed)
        {
            float step = attachmentSpeed * Time.deltaTime;
            Vector3 currentPos
                = CardSelectionSceneCore.selectionDict[cardType].transform.position;
            Vector3 newPos
                = Vector3.MoveTowards(currentPos, targetPos, step);
            CardSelectionSceneCore.selectionDict[cardType].transform.position
                = newPos;
        }
    }

    private void OnDestroy()
    {
        eventBridgeHandler.TriggerEnter -= OnTriggerEnterEB;
        eventBridgeHandler.TriggerExit -= OnTriggerExitEB;
    }

}