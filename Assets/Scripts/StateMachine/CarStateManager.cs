/*using UnityEngine;

public class CarStateManager : MonoBehaviour
{
    public CarBaseState currentState;
    public CarInAirState airState = new CarInAirState();
    public CarOnGroundState onGroundState = new CarOnGroundState();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //starting state fpr the state machine
        currentState = onGroundState;
        // "this" is a reference to the context (this EXACT monobehaviour script)
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(CarBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
*/