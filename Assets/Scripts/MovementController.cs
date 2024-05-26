using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Rigidbody2D playerRigidBody { get; private set; }
    private Vector2 direction = Vector2.down;
    public float speed = 5f;
    
    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputRight = KeyCode.D;
    
    private void Awake()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKey(inputUp))
        {
            SetDirection(Vector2.up);
        } else if (Input.GetKey(inputDown))
        {
            SetDirection(Vector2.down);
        } else if (Input.GetKey(inputLeft))
        {
            SetDirection(Vector2.left);
        } else if (Input.GetKey(inputRight))
        {
            SetDirection(Vector2.right);
        }
        else
        {
            SetDirection(Vector2.zero);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = playerRigidBody.position;
        Vector2 translation = direction * speed * Time.fixedDeltaTime;
        
        playerRigidBody.MovePosition(position + translation);
    }

    private void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }
}
