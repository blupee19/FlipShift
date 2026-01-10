using UnityEngine;

public abstract class CarBaseState
{
   public abstract void EnterState(CarMovement car);
   public abstract void UpdateState(CarMovement car);
   public abstract void OnCollisionEnter(CarMovement car);
}
