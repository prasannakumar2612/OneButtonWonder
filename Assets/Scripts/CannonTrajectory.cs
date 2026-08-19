using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CannonTrajectory : MonoBehaviour
{
    [Header("Cannon Settings")]
    [SerializeField] private Transform firePoint;     
    [SerializeField] private float launchForce = 20f;  

    [Header("Trajectory Line Settings")]
    [SerializeField] private int pointCount = 30;     
    [SerializeField] private float timeStep = 0.05f;   
    [SerializeField] private LayerMask hitLayers;     

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawTrajectory();
    }

    void DrawTrajectory()
    {
        Vector3 startPosition = firePoint.position;
        Vector3 initialVelocity = firePoint.forward * launchForce;
        Vector3 gravity = Physics.gravity;

        lineRenderer.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i * timeStep;

            // Kinematic formula: P = P0 + V0*t + 0.5*g*t^2
            Vector3 point = startPosition + (initialVelocity * t) + (0.5f * gravity * (t * t));
            lineRenderer.SetPosition(i, point);

            // Optional: Raycast check to stop the line when it hits an obstacle
            if (i > 0)
            {
                Vector3 prevPoint = lineRenderer.GetPosition(i - 1);
                if (Physics.Linecast(prevPoint, point, out RaycastHit hit, hitLayers))
                {
                    lineRenderer.SetPosition(i, hit.point);
                    lineRenderer.positionCount = i + 1; // Trim remaining points
                    break;
                }
            }
        }
    }
}