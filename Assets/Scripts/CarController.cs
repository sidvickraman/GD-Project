using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private bool isBreaking;

    [SerializeField] private float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider FrontLeftCollider;
    [SerializeField] private WheelCollider FrontRightCollider;
    [SerializeField] private WheelCollider BackLeftCollider;
    [SerializeField] private WheelCollider BackRightCollider;

    [SerializeField] private Transform FrontLeftTransform;
    [SerializeField] private Transform FrontRightTransform;
    [SerializeField] private Transform BackLeftTransform;
    [SerializeField] private Transform BackRightTransform;
    
    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }
    private void HandleMotor()
    {
        FrontLeftCollider.motorTorque = verticalInput * motorForce;
        FrontRightCollider.motorTorque = verticalInput * motorForce;
        brakeForce = isBreaking ? brakeForce : 0f;
        if (isBreaking)
        {
            ApplyBreaking();
        }
    }

    private void ApplyBreaking()
    {
        FrontRightCollider.brakeTorque = currentBrakeForce;
        FrontLeftCollider.brakeTorque = currentBrakeForce;
        BackRightCollider.brakeTorque = currentBrakeForce;
        BackLeftCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        FrontLeftCollider.steerAngle = currentSteerAngle;
        FrontRightCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(FrontLeftCollider, FrontLeftTransform);
        UpdateSingleWheel(FrontRightCollider, FrontRightTransform);
        UpdateSingleWheel(BackLeftCollider, BackLeftTransform);
        UpdateSingleWheel(BackRightCollider, BackRightTransform);
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
