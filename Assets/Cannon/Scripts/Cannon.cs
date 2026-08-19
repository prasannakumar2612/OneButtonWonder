using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private float holdThreshold = 0.2f; 
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [SerializeField] private Ball _ballPrefab;
    [SerializeField] private Transform _ballSpawn;
    [SerializeField] private ParticleSystem _muzzleFlashEffect;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip _shootSound;
    [SerializeField][Range(0f, 1f)] private float _shootVolume = 1f;

    [Header("Camera Shake Settings")]
    [SerializeField] private float _shakeDuration = 0.15f;
    [SerializeField] private float _shakeMagnitude = 0.25f;

    [SerializeField] private float _velocity = 15;

    private float spacePressedTimer = 0f;
    private bool isHolding = false;

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressedTimer = 0f;
            isHolding = false;
        }

        
        if (Input.GetKey(KeyCode.Space))
        {
            spacePressedTimer += Time.deltaTime;

            
            if (spacePressedTimer >= holdThreshold)
            {
                isHolding = true;
                transform.Rotate(-rotationAxis * rotationSpeed * Time.deltaTime);
            }
        }

        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            
            if (!isHolding)
            {
                Shoot();
            }

            
            isHolding = false;
            spacePressedTimer = 0f;
        }
    }

    void Shoot()
    {
        
        if (_muzzleFlashEffect != null)
        {
            _muzzleFlashEffect.Play();
        }

        
        if (_shootSound != null)
        {
            AudioSource.PlayClipAtPoint(_shootSound, _ballSpawn.position, _shootVolume);
        }

        
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(_shakeDuration, _shakeMagnitude);
        }

        
        var ball = Instantiate(_ballPrefab, _ballSpawn.position, _ballSpawn.rotation);
        ball.Init(_velocity);
        Destroy(ball.gameObject, 5f);
    }
}