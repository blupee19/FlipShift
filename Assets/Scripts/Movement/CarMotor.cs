using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private InputAction accelerateAction;
    private InputAction steerAction;
    private InputAction handbrakeAction;
    private InputAction jumpAction;

    public float AccelerateInput { get; private set; }
    public float SteerInput { get; private set; }
    public bool HandbrakeInput { get; private set; }
    public bool JumpInput { get; set; }

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
    }

    private void OnEnable()
    {
        accelerateAction.Enable();
        steerAction.Enable();   
        handbrakeAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        accelerateAction.Disable(); 
        steerAction.Disable();
        handbrakeAction.Disable();
        jumpAction.Disable();
    }



}
