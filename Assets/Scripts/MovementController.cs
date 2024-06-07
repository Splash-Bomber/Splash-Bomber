using System;
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

    public AnimatedSpriteRenderer spriteRendererUp;
    public AnimatedSpriteRenderer spriteRendererDown;
    public AnimatedSpriteRenderer spriteRendererLeft;
    public AnimatedSpriteRenderer spriteRendererRight;

    public AnimatedSpriteRenderer spriteRendererDeath;

    public AudioClip deathSound; // 사망 소리
    private AudioSource audioSource;

    private AnimatedSpriteRenderer activeSpriteRenderer;
    private Vector2 respawnPosition;

    private void Awake()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (Input.GetKey(inputUp))
        {
            SetDirection(Vector2.up, spriteRendererUp);
        }
        else if (Input.GetKey(inputDown))
        {
            SetDirection(Vector2.down, spriteRendererDown);
        }
        else if (Input.GetKey(inputLeft))
        {
            SetDirection(Vector2.left, spriteRendererLeft);
        }
        else if (Input.GetKey(inputRight))
        {
            SetDirection(Vector2.right, spriteRendererRight);
        }
        else
        {
            SetDirection(Vector2.zero, activeSpriteRenderer);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = playerRigidBody.position;
        Vector2 translation = direction * speed * Time.fixedDeltaTime;

        playerRigidBody.MovePosition(position + translation);
    }

    private void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer)
    {
        direction = newDirection;

        spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
        spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
        spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
        spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;

        activeSpriteRenderer = spriteRenderer;
        activeSpriteRenderer.idle = direction == Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            DeathSequence();
        }
    }

    private void DeathSequence()
    {
        enabled = false;
        GetComponent<WaterBombController>().enabled = false;

        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = false;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;
        spriteRendererDeath.enabled = true;

        PlaySound(deathSound);

        respawnPosition = playerRigidBody.position;
        Invoke(nameof(OnDeathSequenceEnded), 1f);
    }

    private void OnDeathSequenceEnded()
    {
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), 3f);
    }

    private void Respawn()
    {
        transform.position = respawnPosition;
        gameObject.SetActive(true);

        // 초기 상태 복원
        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = true;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;
        spriteRendererDeath.enabled = false;
        activeSpriteRenderer = spriteRendererDown;

        direction = Vector2.down; // 기본 방향 설정
        enabled = true;
        GetComponent<WaterBombController>().enabled = true;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
