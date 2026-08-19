using Mono.Cecil.Cil;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RotatingCannon : MonoBehaviour
{
    [Header("Rotation Target")]
    [Tooltip("The Transform that rotates left and right (Y-axis).")]
    public Transform cannonSwivel;

    [Header("Aim Settings")]
    [Tooltip("Maximum sweep angle to the left and right in degrees.")]
    [Range(10f, 85f)]
    public float maxSweepAngle = 45f;

    [Tooltip("Speed of the sweep oscillation.")]
    public float sweepSpeed = 2f;

    [Tooltip("Fixed upward tilt for the barrel in degrees (X-axis).")]
    public float barrelElevation = 10f;

    [Header("Projectile & Firing")]
    public GameObject bombPrefab;
    public Transform firePoint;
    public float launchForce = 30f;

    [Header("Trajectory / Laser Sight")]
    public LineRenderer aimLine;
    public float maxLaserDistance = 25f;
    public LayerMask hitLayers;

    private float timer = 0f;
    private bool isAiming = false;

    void Start()
    {
        if (aimLine != null)
        {
            aimLine.positionCount = 2;
            aimLine.enabled = false;
        }

        // Initialize default forward rotation
        ApplyRotation(0f);
    }

    void Update()
    {
        // 1. HOLD SPACEBAR -> Sweep back and forth & show aim line
        if (Input.GetKey(KeyCode.Space))
        {
            isAiming = true;

            // Advance timer for smooth oscillation
            timer += Time.deltaTime * sweepSpeed;

            // Mathf.Sin gives smooth deceleration near edges (-1 to +1)
            float currentYaw = Mathf.Sin(timer) * maxSweepAngle;

            ApplyRotation(currentYaw);
            RenderAimLine();
        }

        // 2. RELEASE SPACEBAR -> Lock angle and fire bomb
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isAiming)
            {
                Fire();
            }

            isAiming = false;

            if (aimLine != null)
            {
                aimLine.enabled = false;
            }
        }
    }

    private void ApplyRotation(float yawAngle)
    {
        if (cannonSwivel == null) return;

        // X = Barrel elevation/tilt, Y = Left/Right sweep, Z = 0
        Quaternion targetRotation = Quaternion.Euler(-barrelElevation, yawAngle, 0f);
        cannonSwivel.localRotation = targetRotation;
    }

    private void RenderAimLine()
    {
        if (aimLine == null || firePoint == null) return;

        aimLine.enabled = true;
        aimLine.SetPosition(0, firePoint.position);

        Ray ray = new Ray(firePoint.position, firePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxLaserDistance, hitLayers))
        {
            aimLine.SetPosition(1, hit.point);
        }
        else
        {
            aimLine.SetPosition(1, firePoint.position + (firePoint.forward * maxLaserDistance));
        }
    }

    private void Fire()
    {
        if (bombPrefab == null || firePoint == null) return;

        // Instantiate and apply forward impulse
        GameObject bomb = Instantiate(bombPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * launchForce, ForceMode.Impulse);
        }
    }
}

