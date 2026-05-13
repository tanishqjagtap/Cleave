using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float laneSpeed = 10f;
    public float laneDistance = 3f;

    private int currentLane = 1; // 0 = Left, 1 = Middle, 2 = Right

    void Update()
    {
        // Constant forward movement
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Lane Switching
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLane--;
            currentLane = Mathf.Clamp(currentLane, 0, 2);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentLane++;
            currentLane = Mathf.Clamp(currentLane, 0, 2);
        }

        // Target Position
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        if (currentLane == 0)
            targetPosition += Vector3.left * laneDistance;

        else if (currentLane == 2)
            targetPosition += Vector3.right * laneDistance;

        // Smooth Movement
        Vector3 movePosition = Vector3.Lerp(
            transform.position,
            targetPosition,
            laneSpeed * Time.deltaTime
        );

        transform.position = movePosition;
    }
}