using UnityEngine;

public class PlayHitSoundOnCollision : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip _hitSound;
    [SerializeField][Range(0f, 1f)] private float _volume = 1f;

    [Header("Impact Threshold")]
    [SerializeField] private float _minVelocityForSound = 1.5f;

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Check if the collision has enough force to produce a sound
        if (collision.relativeVelocity.magnitude < _minVelocityForSound)
            return;

        // 2. Check if the colliding object is the Cannonball (or tagged as Ball)
        bool isBall = collision.gameObject.TryGetComponent<Ball>(out _) || collision.gameObject.CompareTag("Ball");

        if (isBall)
        {
            if (_hitSound != null)
            {
                // Plays sound at the exact contact point in 3D space
                Vector3 contactPoint = collision.contacts[0].point;
                AudioSource.PlayClipAtPoint(_hitSound, contactPoint, _volume);
            }
        }
    }
}