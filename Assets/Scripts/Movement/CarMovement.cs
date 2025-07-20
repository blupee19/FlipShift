using System.Collections.Generic;
using System;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    [SerializeField] private float maxAcceleration;
    [SerializeField] private float brakeAcceleration;
    [SerializeField] private float turnSensitivity;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private List<Wheel> wheels;
    [SerializeField] private TMPro.TextMeshProUGUI velocityUI;
    [SerializeField] private AnimationCurve torqueCurve;
    [SerializeField] private float maxRPM = 5000f;

    [SerializeField] private Vector3 customGravityDirection = new Vector3(0f, -9.81f, 0f);
    [SerializeField] private float gravityMultiplier = 1.2f;
    [SerializeField] private float jumpForce = 60000f;
    [SerializeField] private float topSpeed = 20f;
    [SerializeField] private float dampingFactor= 5f;



    float moveInput;
    float steerInput;
    bool jumpInput;
    bool brakeInput;

    private Rigidbody carRb;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.useGravity = false;
    }

    void Update()
    {
        GetInputs();
        AnimatedWheels();
    }

    void FixedUpdate()
    {
        Move();
        Jump();
        Steer();
        Brake();
        CustomGravity();
        InAir();

        velocityUI.text = carRb.linearVelocity.magnitude.ToString("F0");
    }

    void GetInputs()
    {
        moveInput = CarMotor.Instance.AccelerateInput;
        steerInput = CarMotor.Instance.SteerInput;
        brakeInput = CarMotor.Instance.HandbrakeInput;
        jumpInput = CarMotor.Instance.JumpInput;
    }

    void Move()
    {
        float totalWheelRPM = 0;
        int driveWheelCount = 0;

        foreach (var wheel in wheels)
        {
            totalWheelRPM += wheel.wheelCollider.rpm;
            driveWheelCount++;

        }

        float avgWheelRPM = (driveWheelCount > 0) ? totalWheelRPM / driveWheelCount : 0;

        float engineRPM = Mathf.Abs(avgWheelRPM) + 1000f;

        if (carRb.linearVelocity.magnitude > topSpeed)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.motorTorque = 0;
            }
            return;
        }

        float torqueCurveValue = torqueCurve.Evaluate(engineRPM / maxRPM);

        float currentTorque = moveInput * maxAcceleration * torqueCurveValue;

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = currentTorque;
        }

    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void Brake()
    {
        if (brakeInput)
        {
            foreach (var wheel in wheels)
            {
                if (wheel.axel == Axel.Rear)
                {
                    wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
                }
            }
            Debug.Log("brakes!");
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0f;
            }
        }
    }

    void AnimatedWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;

        }
    }

    void CustomGravity()
    {
        if (carRb != null)
        {
            carRb.AddForce(customGravityDirection * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    bool IsGrounded()
    {
        foreach (Wheel wheel in wheels)
        {
            if (wheel.wheelCollider.isGrounded)
            {
                return true;
            }
        }
        return false;
    }

    void Jump()
    {
        if (jumpInput & IsGrounded())
        {
            carRb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
            Debug.Log("jump!");
            jumpInput = false;
        }
    }

    //void InAir()
    //{
    //    if (!IsGrounded())
    //    {
    //        carRb.constraints = RigidbodyConstraints.FreezeRotationZ;
    //    }
    //    else
    //    {
    //        carRb.freezeRotation = false;
    //    }

    //}

    void InAir()
    {
        // We only want to dampen the Z-axis rotation
        // Get the current angular velocity on the Z axis
        float currentZVelocity = carRb.angularVelocity.z;
        float currentXVelocity = carRb.angularVelocity.x;

        // Calculate the damping torque. It's the negative of the current velocity
        // multiplied by our damping factor.
        float dampingTorqueZ = -currentZVelocity * dampingFactor;
        float dampingTorqueX = -currentXVelocity * dampingFactor;

        // Apply this torque on the Z axis.
        // We use ForceMode.Acceleration to ignore the object's mass for a more direct damping effect.
        carRb.AddTorque(dampingTorqueX, 0, dampingTorqueZ, ForceMode.Acceleration);
    }
}