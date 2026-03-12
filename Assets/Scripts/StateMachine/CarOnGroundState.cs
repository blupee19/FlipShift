using UnityEngine;

/*
This script is called when the car is on the ground.
Here update state will override the default update state and add the logic for the ground state.
Enter state will be called once, you can set the variables to false or true if they are not being called 
in the update state then their value won't change.
*/
public class GroundState : CarBaseState
{
    public override void EnterState(CarMovement car)
    {
        Debug.Log("Car has entered grounded state");
        CarMotor.Instance.JumpInput = false;
    }

    public override void UpdateState(CarMovement car)
    {

        car.Move();
        car.Brake();
        car.Steer();
        car.Restart();

        if (car.jumpInput)
        {
            car.carRb.AddForce(Vector3.up * car.jumpForce, ForceMode.Impulse);
            //car.jumpInput = false;
        }

        if (!car.IsGrounded())
        {
            car.SwitchState(car.airState);
        }
    }

    public override void OnCollisionEnter(CarMovement car)
    {

    }

    public void Jump(CarMovement car)
    {
        if (CarMotor.Instance.JumpInput & car.IsGrounded())
        {
            car.carRb.AddForce(0, car.jumpForce, 0, ForceMode.Impulse);
        }
    }
}
