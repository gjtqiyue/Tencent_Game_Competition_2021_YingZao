using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.InputSystem;

public enum GroundType
{
    None,
    Soft,
    Hard
}

public class CharacterController : BaseControlUnit
{
    readonly Vector3 flippedScale = new Vector3(-1, 1, 1);

    [Header("Character")]
    [SerializeField] Animator animator = null;
    [SerializeField] Transform puppet = null;
    [SerializeField] CharacterAudio audioPlayer = null;

    [Header("Movement")]
    [SerializeField] float acceleration = 0.0f;
    [SerializeField] float maxSpeed = 0.0f;
    [SerializeField] float jumpForce = 0.0f;
    [SerializeField] float minFlipSpeed = 0.1f;
    [SerializeField] float jumpGravityScale = 1.0f;
    [SerializeField] float fallGravityScale = 1.0f;
    [SerializeField] float groundedGravityScale = 1.0f;
    [SerializeField] bool resetSpeedOnLand = false;

    private Rigidbody2D controllerRigidbody;
    private Collider2D controllerCollider;
    private LayerMask softGroundMask;
    private LayerMask hardGroundMask;

    [SerializeField]
    private Vector2 movementInput;
    private GroundType groundType;

    private int animatorGroundedBool;
    private int animatorRunningSpeed;
    private int animatorDirection;

    public bool CanMove { get; set; }

    protected override void Start()
    {
        base.Start();

        DialogueManager.GetInstance().scenarioEventStartDelegate += DisableInputCheck;
        DialogueManager.GetInstance().scenarioEventEndDelegate += EnableInputCheck;

#if UNITY_EDITOR
        if (Keyboard.current == null)
        {
            var playerSettings = new UnityEditor.SerializedObject(Resources.FindObjectsOfTypeAll<UnityEditor.PlayerSettings>()[0]);
            var newInputSystemProperty = playerSettings.FindProperty("enableNativePlatformBackendsForNewInputSystem");
            bool newInputSystemEnabled = newInputSystemProperty != null ? newInputSystemProperty.boolValue : false;

            if (newInputSystemEnabled)
            {
                var msg = "New Input System backend is enabled but it requires you to restart Unity, otherwise the player controls won't work. Do you want to restart now?";
                if (UnityEditor.EditorUtility.DisplayDialog("Warning", msg, "Yes", "No"))
                {
                    UnityEditor.EditorApplication.ExitPlaymode();
                    var dataPath = Application.dataPath;
                    var projectPath = dataPath.Substring(0, dataPath.Length - 7);
                    UnityEditor.EditorApplication.OpenProject(projectPath);
                }
            }
        }
#endif

        controllerRigidbody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<Collider2D>();
        softGroundMask = LayerMask.GetMask("Ground Soft");
        hardGroundMask = LayerMask.GetMask("Ground Hard");

        animatorGroundedBool = Animator.StringToHash("Grounded");
        animatorRunningSpeed = Animator.StringToHash("RunningSpeed");
        animatorDirection = Animator.StringToHash("Direction");

        CanMove = true;
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        if (!inputEnabled)
            return;

        if (!CanMove || keyboard == null)
            return;

        // Horizontal movement
        float moveHorizontal = 0.0f;

        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
        {
            moveHorizontal = -1.0f;
        } 
        else if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
        {
            moveHorizontal = 1.0f;
        }
           

        movementInput = new Vector2(moveHorizontal, 0);
    }

    void FixedUpdate()
    {
        if (!inputEnabled) return;
        UpdateGrounding();
        UpdateVelocity();
        UpdateDirection();
    }

    private void UpdateGrounding()
    {
        // Use character collider to check if touching ground layers
        if (controllerCollider.IsTouchingLayers(softGroundMask))
            groundType = GroundType.Soft;
        else if (controllerCollider.IsTouchingLayers(hardGroundMask))
            groundType = GroundType.Hard;
        else
            groundType = GroundType.None;

        // Update animator
        animator.SetBool(animatorGroundedBool, groundType != GroundType.None);
    }

    private void UpdateVelocity()
    {

        Vector3 velocity = new Vector3(movementInput.x, movementInput.y, 0) * acceleration * Time.fixedDeltaTime;

        // Clamp horizontal speed.
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);

        // Assign back to the body.
        gameObject.transform.position += new Vector3(velocity.x, velocity.y, 0);

        // Update animator running speed
        var horizontalSpeedNormalized = Mathf.Abs(velocity.x) / maxSpeed;
        animator.SetFloat(animatorRunningSpeed, horizontalSpeedNormalized);

        // Play audio
        //audioPlayer.PlaySteps(groundType, horizontalSpeedNormalized);
    }

    private void UpdateDirection()
    {
        // Use scale to flip character depending on direction
        if (movementInput.x > minFlipSpeed)
        {
            animator.SetInteger(animatorDirection, 1);
        }
        else if (movementInput.x < -minFlipSpeed)
        {
            animator.SetInteger(animatorDirection, -1);
        }
        else
        {
            animator.SetInteger(animatorDirection, 0);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        UpdateInteraction(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CancelInteraction(collision.gameObject);
    }

    private void UpdateInteraction(GameObject gameObj)
    {
        if (gameObj.tag == "Interactable")
        {
            if (inputEnabled && Keyboard.current.eKey.isPressed)
            {
                gameObj.GetComponent<IInteractable>().TriggerInteraction();
            }
        }
    }

    private void CancelInteraction(GameObject gameObj)
    {
        if (gameObj.tag == "Interactable")
        {
            gameObj.GetComponent<IInteractable>().CancelInteraction();
        }
    }
}
