using UnityEngine;

public class BlackHoleRotationScript : MonoBehaviour
{
    [SerializeField] private float m_RotationSpeed = 10f;

    private void Update()
    {
        transform.Rotate(Vector3.forward, m_RotationSpeed * Time.deltaTime);
    }
}
