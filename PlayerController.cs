using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour, ITarget
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform groundCheck;
    Vector2 direction;
    Vector3 moveDir = Vector3.zero;
    bool isShooting, isAbility, isReloading = false;
    bool isRunning, isJump = false;
    int jumpAirCount = 1;
    bool isShift = false;

    public float health { get; set; } = 100;

    //ReadCommands
    #region
    public void ReadMovementAxisCommand(MovementAxisCommand command)
    {
        direction.x = command.HorizontalAxis;
        direction.y = command.VerticalAxis;
    }
    public void ReadAttackActionCommand(AttackButtonCommand command)
    {
        isShooting = command.ShootButton;
        isAbility = command.AbilityButton;
        isReloading = command.ReloadButton;
    }
    public void ReadMovementButtonCommand(MovementButtonCommand command)
    {
        isRunning = command.RunButton;
        isJump = command.JumpButton;
    }
    public void ReadShiftButtonCommand(ShiftButtonCommand command) 
    {
        isShift = command.ShiftButton;
    }
    #endregion

    //Main
    public void UpdatePlayer()
    {
        Debug.Log(health);
        //Reloading & Shooting
        if (isReloading)
        {
            ReloadGun();
        }
        else if (isShooting)
        {
            ShootGun();
        }
        //Shifting
        if (isShift)
        {
            ShiftAbility();
        }
        //Ability
        if (isAbility)
        {
            UltimateAbility();
        }

        //Movement
        float speed = 1.0f;
        bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, 1 << 6);
        if (direction.magnitude > 0)
        {
            float velY = moveDir.y;
            Vector3 forwardDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward * direction.y, Vector3.up);
            Vector3 sideDirection = Vector3.ProjectOnPlane(Camera.main.transform.right * direction.x, Vector3.up);
            moveDir = (forwardDirection + sideDirection).normalized;
            moveDir.y = velY;
            if (isRunning)
            {
                speed = 2.25f;
            }

            //Rotation
            // Calculate the new rotation angle based on the input
            float targetAngle = Mathf.Atan2(Camera.main.transform.forward.x, Camera.main.transform.forward.z) * Mathf.Rad2Deg;
            // Rotate the character
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), 20.0f * Time.deltaTime);
        }
        else 
        {
            moveDir = new Vector3(0, moveDir.y, 0);
        }
        //Jumping
        if (isGrounded)
        {
            moveDir.y = 0f;
            jumpAirCount = 1;
            if (isJump)
            {
                Debug.Log("Jumping");
                moveDir.y = 6.0f;
            }
        }
        else
        {
            if (jumpAirCount > 0 && isJump)
            {
                jumpAirCount--;
                moveDir.y = 4.5f;
            }
            //Gravity
            moveDir.y -= 8.0f * Time.deltaTime;
        }
        characterController.Move((speed * 10.0f) * Time.deltaTime * moveDir);
    }
    private void UltimateAbility()
    {
        Debug.Log("ability active");
    }
    private void ShiftAbility()
    {
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                GameManager.Instance.UpdateWorld(hit.transform.gameObject.tag);
            }
        }
    }
    private void ShootGun()
    {
        //TODO: optimize to use gamemanager?
        InventoryController.Instance.GunEquipped.Shoot(mainCam);
    }
    private void ReloadGun()
    {
        //TODO: optimize to use gamemanager?
        InventoryController.Instance.GunEquipped.Reloading();
    }

    //ITarget
    public void OnDamaged(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        transform.position = new Vector3(0, -45.0f, 0);
        health = 100;
    }
}
