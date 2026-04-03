using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator anim;

    public float speed = 5f;
    public float sprintMultiplier = 2f;
    public float gravity = -9.81f;

    private Vector3 velocity;

    [HideInInspector] public bool canMove = true;

    void Update()
    {
        if (!canMove)
        {
            anim.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) && (x != 0 || z != 0))
        {
            currentSpeed *= sprintMultiplier;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (anim != null)
        {
            float speedForAnim = new Vector2(x, z).magnitude * currentSpeed;
            anim.SetFloat("Speed", speedForAnim, 0.1f, Time.deltaTime);
        }
    }
}