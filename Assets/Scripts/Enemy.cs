using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageTreshold = 0.2f;

    [Header("Effects")]
    [SerializeField] private GameObject enemieDeathParticle;
    [SerializeField] private AudioClip deathSound;

    private float currentHealth;
    private AudioSource audioSource;

    private void Awake()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }

    public void DamageEnemy(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.RemoveEnemy(this);

        if (enemieDeathParticle != null)
        {
            Instantiate(enemieDeathParticle, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;

        if (impactVelocity > damageTreshold)
        {
            DamageEnemy(impactVelocity);
                
        }
    }

}
