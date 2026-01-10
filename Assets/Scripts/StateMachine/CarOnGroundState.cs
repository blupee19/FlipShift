using UnityEngine;

public class GroundState : CarBaseState
{
    public override void EnterState(CarMovement car)
    {
        Debug.Log("Car has entered grounded state");
    }

    public override void UpdateState(CarMovement car)
    {

        car.Move();
        car.Brake();
        car.Steer();

        if (car.jumpInput)
        {
            car.carRb.AddForce(Vector3.up * car.jumpForce, ForceMode.Impulse);
            car.jumpInput = false;
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
