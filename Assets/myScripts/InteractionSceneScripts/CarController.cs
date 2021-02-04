using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attached to TukTuk
public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public GameObject SteeringWheel;
    private float horizontalInput;
    private float accelerationInput;
    private float breakInput;
    private float currentbreakForce;
    private bool isBreaking;
    public float currentSteerAngle;
    public float reverseInput;
    public CarEnter carEnterReference;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;


    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }


    private void GetInput()
    {
        horizontalInput = SteeringWheel.GetComponent<WheelController>().currentWheelRotation;
        accelerationInput = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger); //Input.GetAxis(VERTICAL);
                                                                                 // isBreaking = Input.GetKey(KeyCode.Space);
        reverseInput = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger); //Input.GetAxis(VERTICAL);
    }


    private void HandleMotor()
    {
        if (accelerationInput > 0 && carEnterReference.inCar == true)
        {
            frontLeftWheelCollider.motorTorque = accelerationInput * motorForce;
            frontRightWheelCollider.motorTorque = accelerationInput * motorForce;
        }
        else if (reverseInput > 0 && carEnterReference.inCar == true)
        {
            frontLeftWheelCollider.motorTorque = -reverseInput * motorForce;
            frontRightWheelCollider.motorTorque = -reverseInput * motorForce;
        }

        //currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }


    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }


    private void HandleSteering()
    {
        currentSteerAngle = horizontalInput;


        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }


    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }


    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

}