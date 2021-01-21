using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 3つに分かれているものをひとつに集約する；インスタンスを...SceneCore.csの方で操作するようにする
public class CardSelectionDetector : MonoBehaviour
{
    public GameObject[] ObjectsArr;

    private float clipSpeed; // speed to clip to selection-box
    private Vector3 targetPos;

    //public Collider envBox;



    void Start()
    {
        clipSpeed = 1.0f;
        targetPos = transform.position;
    }

    void Update()
    {
        if (CardSelectionSceneCore.selectionDict["environment"] != null)
        {
            bool isGrabbed = CardSelectionSceneCore.selectionDict["environment"].transform.GetComponent<OVRGrabbable>().isGrabbed;
            if (!isGrabbed)
            {
                float step = clipSpeed * Time.deltaTime;
                Vector3 currentPos = CardSelectionSceneCore.selectionDict["environment"].transform.position;
                Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
                CardSelectionSceneCore.selectionDict["environment"].transform.position = newPos;
            }
        }
    }

    bool IsElementOf(GameObject obj, GameObject[] arr)
    {
        foreach (GameObject elm in arr)
            if (elm == obj) return true;
        return false;
    }

    void IncreaseDrag(GameObject obj)
    {
        obj.transform.GetComponent<Rigidbody>().drag = 20;
        obj.transform.GetComponent<Rigidbody>().angularDrag = 10.0F;
    }

    void DecreaseDrag(GameObject obj)
    {
        obj.transform.GetComponent<Rigidbody>().drag = 1;
        obj.transform.GetComponent<Rigidbody>().angularDrag = 0.5F;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsElementOf(other.gameObject, ObjectsArr))
        {
            if (CardSelectionSceneCore.selectionDict["environment"] == null)
            {
                CardSelectionSceneCore.selectionDict["environment"] = other.gameObject;
                IncreaseDrag(CardSelectionSceneCore.selectionDict["environment"]);
            }
            else
            { // replace card choice
                DecreaseDrag(CardSelectionSceneCore.selectionDict["environment"]);
                CardSelectionSceneCore.selectionDict["environment"] = other.gameObject;
                IncreaseDrag(CardSelectionSceneCore.selectionDict["environment"]);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == CardSelectionSceneCore.selectionDict["environment"])
        {
            DecreaseDrag(CardSelectionSceneCore.selectionDict["environment"]);
            CardSelectionSceneCore.selectionDict["environment"] = null;
        }
    }



}
