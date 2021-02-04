using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attached to OVRPlaerController/.../RightHandAnchor
public class CarEnter : MonoBehaviour
{
    public GameObject car;
    public GameObject doorLeft;
    public GameObject doorRight;
    public bool canEnter;
    public GameObject Player;
    public Transform playerDrivingTransform;
    public Transform playerOutOfCarTransform;
    public bool inCar;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == doorLeft.name || other.gameObject.name == doorRight.name)
        {
            canEnter = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == doorLeft.name || other.gameObject.name == doorRight.name)
        {
            canEnter = false;
        }
    }

    private void FixedUpdate()
    {
        if (canEnter == true && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 1f && inCar == false)
        {
            Player.GetComponentInChildren<OVRPlayerController>().enabled = false;
            Player.GetComponentInChildren<CharacterController>().enabled = false;
            Player.transform.localPosition = playerDrivingTransform.position;
            //Player.GetComponent<Rigidbody>().isKinematic = true;
            Player.transform.SetParent(car.transform);
            Player.GetComponentInChildren<OVRPlayerController>().enabled = true;

            inCar = true;
        }
        else if (canEnter == true && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 1f && inCar == true)
        {
            Player.transform.SetParent(null);
            Player.GetComponentInChildren<OVRPlayerController>().enabled = false;
            Player.GetComponentInChildren<CharacterController>().enabled = true;
            Player.transform.localPosition = playerOutOfCarTransform.position;
            //Player.GetComponent<Rigidbody>().isKinematic = false;
            Player.GetComponentInChildren<OVRPlayerController>().enabled = true;

            inCar = false;
        }
    }

}
