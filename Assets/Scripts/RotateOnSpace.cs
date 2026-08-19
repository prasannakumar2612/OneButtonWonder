using UnityEngine;

public class RotateOnSpace : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up; 

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Rotate(-rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }
}