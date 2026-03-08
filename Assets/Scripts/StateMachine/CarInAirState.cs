using UnityEditor.Experimental;
using UnityEngine;

public class AirState : CarBaseState
{
    float pitchForce = 1.5f;
    public override void EnterState(CarMovement car)
    {
        Debug.Log("Car is in the air");
        //CarMotor.Instance.JumpInput = false;
    }

    public override void UpdateState(CarMovement car)
    {
        Debug.Log("AirState Update");
        car.InAir();
        car.Steer();
        
        if (car.IsGrounded())
        {
            car.SwitchState(car.onGroundState);
        }

        if (car.jumpInput)
        {
            Debug.Log("Torque!!!");
            float pitchTorque = car.moveInput * pitchForce;
            car.carRb.AddRelativeTorque(Vector3.right * pitchTorque, ForceMode.Acceleration);
        }

    }

    public override void OnCollisionEnter(CarMovement car)
    {

    }

    //void Pitch(CarMovement car)
    //{
    //    float pitchTorque = car.moveInput * 10;
    //    car.carRb.AddRelativeTorque(Vector3.right * pitchTorque, ForceMode.Acceleration);
    //}
}
