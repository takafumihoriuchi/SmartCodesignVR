using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    //RightHand
    public GameObject rightHand;
    private Transform rightHandOriginalParent;
    private bool rightHandOnWheel = false;

    //LeftHand
    public GameObject leftHand;
    private Transform leftHandOriginalParent;
    private bool leftHandOnWheel;

    public Transform[] snappPosition;

    //public GameObject Vehicle;
    //private Rigidbody VehicleRigidBody;

    public float currentWheelRotation = 0;

    public float turnDampening = 250;

    public Transform directionalObject;

    private void Start()
    {
        //VehicleRigidBody = Vehicle.GetComponent<Rigidbody>();

    }

    void Update()
    {
        ReleaseHandsFromWheel();

        ConvertHandRotationToSteeringWheelRotation();

        //TurnVehicle();

        
        
        currentWheelRotation = transform.localRotation.eulerAngles.y;
        if (currentWheelRotation > 45 && currentWheelRotation < 180)
           // currentWheelRotation = 45;
        this.gameObject.transform.localRotation = Quaternion.Euler(0, 45, 0);
        if (currentWheelRotation < 315 && currentWheelRotation >= 180)
            //  currentWheelRotation = 315;
        this.gameObject.transform.localRotation = Quaternion.Euler(0, 315, 0);


    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("PlayerHand"))
        {
            if(rightHandOnWheel == false && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                PlaceHandOnWheel(ref rightHand, ref rightHandOriginalParent, ref rightHandOnWheel);

            }

            if (leftHandOnWheel == false && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
            {
                PlaceHandOnWheel(ref rightHand, ref rightHandOriginalParent, ref leftHandOnWheel);
            }
        }

    }

    private void PlaceHandOnWheel(ref GameObject hand, ref Transform originalParent, ref bool handOnWheel)
    {
        var shortestDistance = Vector3.Distance(snappPosition[0].position, hand.transform.position);
        var bestSnapp = snappPosition[0];

        foreach (var snappPosition in snappPosition)
        {
            if (snappPosition.childCount == 0)
            {
                var distance = Vector3.Distance(snappPosition.position, hand.transform.position);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestSnapp = snappPosition;
                }
            }

        }
        originalParent = hand.transform.parent;

        hand.transform.parent = bestSnapp.transform;
        hand.transform.position = bestSnapp.transform.position;

        handOnWheel = true;
    }

    private void ConvertHandRotationToSteeringWheelRotation()
    {
        if(rightHandOnWheel == true && leftHandOnWheel == false)
        {
            Quaternion newRot = Quaternion.Euler(0, rightHandOriginalParent.transform.rotation.eulerAngles.y, 0);
            directionalObject.localRotation = newRot;
            transform.parent = directionalObject;
        }
        else if (rightHandOnWheel == false && leftHandOnWheel == true)
        {
            Quaternion newRot = Quaternion.Euler(0, leftHandOriginalParent.transform.rotation.eulerAngles.y, 0);
            directionalObject.localRotation = newRot;
            transform.parent = directionalObject;
        }
        else if(rightHandOnWheel == true && leftHandOnWheel == true)
        {
            Quaternion newRotLeft = Quaternion.Euler(0, leftHandOriginalParent.transform.rotation.eulerAngles.y, 0);
            Quaternion newRotRight = Quaternion.Euler(0, leftHandOriginalParent.transform.rotation.eulerAngles.y, 0);
            Quaternion finalRot = Quaternion.Slerp(newRotLeft, newRotRight, 1.0f / 2.0f);
            directionalObject.localRotation = finalRot;
            transform.parent = directionalObject;
        }
    }

    private void ReleaseHandsFromWheel()
    {
        if (rightHandOnWheel == true && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            rightHand.transform.parent = rightHandOriginalParent;
            rightHand.transform.position = rightHandOriginalParent.position;
            rightHand.transform.rotation = rightHandOriginalParent.rotation;
            rightHandOnWheel = false;
        }
        if(leftHandOnWheel == true && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            leftHand.transform.parent = leftHandOriginalParent;
            leftHand.transform.position = leftHandOriginalParent.position;
            leftHand.transform.rotation = leftHandOriginalParent.rotation;
            leftHandOnWheel = false;
        }

        if(leftHandOnWheel == false && rightHandOnWheel == false)
        {
            transform.parent = transform.root;
        }

    }

}
