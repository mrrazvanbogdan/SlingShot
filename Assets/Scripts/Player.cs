using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private AudioClip hitsound;

    private AudioSource audioSource;

    private Rigidbody2D rigidBody;
    private CircleCollider2D circleCollider;

    private bool hasBeenLaunched;
    private bool shouldFaceVelDirection;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        rigidBody.linearVelocity = Vector2.zero;
        circleCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (hasBeenLaunched && shouldFaceVelDirection)
        {
                    if (rigidBody.linearVelocity.magnitude > 0.1f) 
            {
                transform.right = rigidBody.linearVelocity.normalized;
            }
        }
    }

    public void LunchPlayer(Vector2 direction, float force)
    {
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        circleCollider.enabled = true;

        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.AddForce(direction * force, ForceMode2D.Impulse);

        hasBeenLaunched = true; 
        shouldFaceVelDirection = true; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
         shouldFaceVelDirection = false;

        if (collision.gameObject.CompareTag("Blocks"))
        {
            if (hitsound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitsound);
            }
        }

    }
}
