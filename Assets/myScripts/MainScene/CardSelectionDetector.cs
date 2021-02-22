using UnityEngine;
using EventBridge;

public class CardSelectionDetector
{
    private GameObject selectedCardObj = null;
    public GameObject SelectedCardObj
    {
        get { return selectedCardObj; }
        set { selectedCardObj = value; triggerFlag = true; }
    }

    private bool triggerFlag = false;
    public bool TriggerFlag
    {
        get {
            if (triggerFlag) { triggerFlag = false; return true; }
            else return false;
        }
    }

    private GameObject selectionContainer;
    private GameObject[] candidateObjArr;
    private IComponentEventHandler eventBridgeHandler;
    private Vector3 targetPos;

    readonly float attachmentSpeed = 1.0f;
    const float DRAG_L = 20f;
    const float DRAG_S = 1f;
    const float ANGDRAG_L = 10.0f;
    const float ANGDRAG_S = 0.5f;

    public CardSelectionDetector(
        GameObject selectionContainer, GameObject[] candidateObjArr)
    {
        this.selectionContainer = selectionContainer;
        this.candidateObjArr = candidateObjArr;
        targetPos = this.selectionContainer.transform.position;
        eventBridgeHandler = this.selectionContainer.RequestEventHandlers();
        eventBridgeHandler.TriggerEnter += OnTriggerEnterEB;
        eventBridgeHandler.TriggerExit += OnTriggerExitEB;
    }

    private void OnTriggerEnterEB(Collider other)
    {
        if (!IsElementOf(other.gameObject, candidateObjArr)) return;
        if (selectedCardObj == null)
        {
            // put object in empty container
            SelectedCardObj = other.gameObject;
            IncreaseDrag(selectedCardObj);
        }
        else
        {
            // kickout current object and replace object in container
            DecreaseDrag(selectedCardObj);
            SelectedCardObj = other.gameObject;
            IncreaseDrag(selectedCardObj);
        }
    }

    private void OnTriggerExitEB(Collider other)
    {
        if (other.gameObject == selectedCardObj)
        {
            // release object from container
            DecreaseDrag(selectedCardObj);
            SelectedCardObj = null;
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

    public void CenterDragMotion()
    {
        if (selectedCardObj == null) return;
        if (!selectedCardObj.transform.GetComponent<OVRGrabbable>().isGrabbed)
        {
            float step = attachmentSpeed * Time.deltaTime;
            Vector3 currentPos = selectedCardObj.transform.position;
            Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
            selectedCardObj.transform.position = newPos;
        }
    }

}
