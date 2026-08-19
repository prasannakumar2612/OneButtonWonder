using UnityEngine;

public class ExplosiveObstacle : MonoBehaviour
{
    [SerializeField] private float _triggerForce = 5f; // Raised to avoid light touches
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionForce = 500f;
    [SerializeField] private GameObject _particles;

    private bool _hasExploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasExploded) return;

        // 1. Check if the object hitting this is the Cannon Ball
        bool isBall = collision.gameObject.TryGetComponent<Ball>(out _);

        // 2. Explode if it's the ball, OR if another object hits it with high enough impact force
        if (isBall || collision.relativeVelocity.magnitude >= _triggerForce)
        {
            // Ignore low-velocity floor / static environment contacts
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Floor"))
                return;

            Explode();
        }
    }

    private void Explode()
    {
        _hasExploded = true;

        var surroundingObjects = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (var obj in surroundingObjects)
        {
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, 1f);
        }

        if (_particles != null)
        {
            GameObject fx = Instantiate(_particles, transform.position, Quaternion.identity);
            Destroy(fx, 3f);
        }

        Destroy(gameObject);
    }
}