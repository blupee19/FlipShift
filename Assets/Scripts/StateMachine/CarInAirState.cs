using UnityEditor.Experimental;
using UnityEngine;

/*
This script is called when the car is in the air.
Here update state will override the default update state and add the logic for the air state.
Enter state will be called once, you can set the variables to false or true if they are not being called 
in the update state then their value won't change.
*/
public class AirState : CarBaseState
{
    public override void EnterState(CarMovement car)
    {
        //CarMotor.Instance.JumpInput = false;
    }

    public override void UpdateState(CarMovement car)
    {
        car.InAir();
        car.Steer();
        car.Roll();
        car.Pitch();
        
        if (car.IsGrounded())
        {
            car.SwitchState(car.onGroundState);
        }

        // if (car.jumpInput)
        // {
        //     float pitchTorque = car.moveInput * car.pitchForce;
        //     car.carRb.AddRelativeTorque(Vector3.right * pitchTorque, ForceMode.Acceleration);
        // }

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
