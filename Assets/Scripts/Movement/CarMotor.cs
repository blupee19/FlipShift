using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

/* 
Rules to add input
1. Add a private string to Action Name References with the names in the input action asset you'll put
2. Add a private InputAction with the name of the action followed by "Action"
3. Add a public variable (could be float, bool, etc. depending on the action) with the name of the action followed by "Input", add get & set and see if they need to be public or private
4. In the Awake method, assign the InputAction variable to the inputActions by finding the action map and then the action
5. In the RegisterInputActions method, add the performed and canceled events to the InputAction variable
6. In the OnEnable method, enable the InputAction variable
7. In the OnDisable method, disable the InputAction variable
*/
public class CarMotor : MonoBehaviour
{


    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Action Map References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string accelerate = "Accelerate";
    [SerializeField] private string steer = "Steer";
    [SerializeField] private string handbrake = "Handbrake";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string restart = "Restart";
    [SerializeField] private string roll = "Roll";
    //[SerializeField] private string airManuever = "AirManuever";

    private InputAction accelerateAction;
    private InputAction steerAction;
    private InputAction handbrakeAction;
    private InputAction jumpAction;
    private InputAction restartAction;
    private InputAction rollAction;
    //private InputAction airManueverAction;

    public float AccelerateInput { get; private set; }
    public float SteerInput { get; private set; }
    public bool HandbrakeInput { get; private set; }
    public bool JumpInput { get; set; }
    public bool RestartInput { get; set; }  
    public float RollInput { get; set; }
    //public bool AirManuever { get; set; }

    public static CarMotor Instance { get; private set; }

    public void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        accelerateAction = inputActions.FindActionMap(actionMapName).FindAction(accelerate);
        steerAction = inputActions.FindActionMap(actionMapName).FindAction(steer);
        handbrakeAction = inputActions.FindActionMap(actionMapName).FindAction(handbrake);
        jumpAction = inputActions.FindActionMap(actionMapName).FindAction(jump);
        restartAction = inputActions.FindActionMap(actionMapName).FindAction(restart);
        rollAction = inputActions.FindActionMap(actionMapName).FindAction(roll);
        //airManueverAction = inputActions.FindActionMap(actionMapName).FindAction(airManuever);
        RegisterInputActions();
    }

    public void RegisterInputActions()
    {
        accelerateAction.performed += context => AccelerateInput = context.ReadValue<float>();
        accelerateAction.canceled += context => AccelerateInput = 0;

        steerAction.performed += context => SteerInput = context.ReadValue<float>();
        steerAction.canceled += context => SteerInput = 0;

        handbrakeAction.performed += context => HandbrakeInput = true;
        handbrakeAction.canceled += context => HandbrakeInput = false;

        jumpAction.performed += context => JumpInput = true;
        jumpAction.canceled += context => JumpInput = false;

        restartAction.performed += context => RestartInput = true;
        restartAction.canceled += context => RestartInput = false;

        rollAction.performed += context => RollInput = context.ReadValue<float>();
        rollAction.canceled += context => RollInput = 0;

        //airManueverAction.performed += context => AirManuever = true;
        //airManueverAction.canceled += context => AirManuever = false;
    }

    private void OnEnable()
    {
        accelerateAction.Enable();
        steerAction.Enable();   
        handbrakeAction.Enable();
        jumpAction.Enable();
        restartAction.Enable();
        rollAction.Enable();
        //airManueverAction.Enable();
    }

    private void OnDisable()
    {
        accelerateAction.Disable(); 
        steerAction.Disable();
        handbrakeAction.Disable();
        jumpAction.Disable();
        restartAction.Disable();
        rollAction.Disable();
        //airManueverAction.Disable();
    }



}
