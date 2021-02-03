using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapObject : MonoBehaviour
{
    public GameObject SnapLocation;
    public GameObject car;
    public bool isSnapped;
    private bool Snapped;
   public bool grabbed;
    public GameObject snapObject;

    void Update()
    {


        grabbed = GetComponent<OVRGrabbable>().isGrabbed;
       
        Snapped = SnapLocation.GetComponent<SnapToLocation>().Snapped;

        if(Snapped == true)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.SetParent(car.transform);
            //isSnapped = true;
        }

        if(Snapped == false && grabbed == false)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
        if(Snapped == false && grabbed == true)
        {
          GetComponent<Rigidbody>().isKinematic = false;
           snapObject.transform.parent = null; 
           //isSnapped = false;
        }
    }

}
