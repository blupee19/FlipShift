using System.Collections.Generic;
using System;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public CarBaseState currentState;
    public AirState airState = new AirState();
    public GroundState onGroundState = new GroundState();
    public GameObject steeringWheel;
    public bool canJump = false;
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

    [SerializeField] public float maxAcceleration;
    [SerializeField] public float brakeAcceleration;
    [SerializeField] public float turnSensitivity;
    [SerializeField] public float maxSteerAngle;
                     
    [SerializeField] public List<Wheel> wheels;
    [SerializeField] public TMPro.TextMeshProUGUI velocityUI;
    [SerializeField] public TMPro.TextMeshProUGUI State;
    [SerializeField] public AnimationCurve torqueCurve;
    [SerializeField] public float maxRPM = 5000f;
    [SerializeField] public Vector3 customGravityDirection = new Vector3(0f, -9.81f, 0f);
    [SerializeField] public float gravityMultiplier = 1.2f;
    [SerializeField] public float jumpForce = 60000f;
    [SerializeField] public float topSpeed = 20f;
    [SerializeField] public float dampingFactor= 5f;
// hello


    public float moveInput;
    public float steerInput;
    public bool jumpInput;
    public bool brakeInput;
    // public bool airManueverInput;

    public Rigidbody carRb;

    void Start()
    {
        currentState = onGroundState;
        currentState.EnterState(this);
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
        currentState.UpdateState(this);
        CustomGravity();
        velocityUI.text = carRb.linearVelocity.magnitude.ToString("F0");
        State.text = currentState.ToString();
    }

    void GetInputs()
    {
        if (CarMotor.Instance == null) return;

        moveInput = CarMotor.Instance.AccelerateInput;
        steerInput = CarMotor.Instance.SteerInput;
        brakeInput = CarMotor.Instance.HandbrakeInput;
        jumpInput = CarMotor.Instance.JumpInput;
        // airManueverInput = CarMotor.Instance.AirManuever;
    }

    public void Move()
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

    public void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front) //remove this to make it interesting
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    public void Brake()
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
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0f;
            }
        }
    }

    public void AnimatedWheels()
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

    public void CustomGravity()
    {
        if (carRb != null)
        {
            carRb.AddForce(customGravityDirection * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    public bool IsGrounded()
    {
        foreach (Wheel wheel in wheels)
        {
            if (wheel.wheelCollider.isGrounded)
            {
                return true;
            }
        }
        //Debug.Log("In Air");
        return false;
        
    }



    public void InAir()
    {
        foreach(var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = 0f;
        }

        // Debug.Log("Angular velocity dampening");
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

    public void SwitchState(CarBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}