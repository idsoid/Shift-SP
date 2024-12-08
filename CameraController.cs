using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    float horizontalAxis, verticalAxis;
    float rotationSpeed = 0.25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Lock the cursor to the center of the screen
        Cursor.visible = false;  // Hide the cursor
    }
    public void ReadMouseAxisCommand(MouseAxisCommand command)
    {
        horizontalAxis += command.MouseX * rotationSpeed;
        verticalAxis -= command.MouseY * rotationSpeed;
    }
    public void UpdateCamera()
    {
        // Limit the vertical rotation to avoid camera flipping
        verticalAxis = Mathf.Clamp(verticalAxis, -90f, 90f);
        // Set the camera's rotation based on input
        transform.rotation = Quaternion.Euler(verticalAxis, horizontalAxis, 0);
        // Set the position of the camera behind the player
        transform.position = player.position - transform.forward * 5.0f;
        // Set camera to look at character's position with slight height offset
        transform.LookAt(player.position + new Vector3(0, 1.75f, 0));
    }
}
