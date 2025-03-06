using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    public enum BillboardType 
    { 
        LookAtCamera, 
        CameraForward 
    }

    [SerializeField] private BillboardType m_BillboardType = BillboardType.CameraForward;

    [Header("Lock Rotation")]
    [SerializeField] private bool m_LockX = false;
    [SerializeField] private bool m_LockY = false;
    [SerializeField] private bool m_LockZ = false;

    private Vector3 m_OriginalRotation;

    private void Awake() {
        m_OriginalRotation = transform.rotation.eulerAngles;
    }

    void LateUpdate() {
        switch (m_BillboardType) 
        {
        case BillboardType.LookAtCamera:
            transform.LookAt(Camera.main.transform.position, Vector3.up);
            break;

        case BillboardType.CameraForward:
            transform.forward = Camera.main.transform.forward;
            break;

        default:
            break;
        }
        
        // Modifying the rotation in Euler space to lock certain dimensions.
        Vector3 rotation = transform.rotation.eulerAngles;
        if (m_LockX) { rotation.x = m_OriginalRotation.x; }
        if (m_LockY) { rotation.y = m_OriginalRotation.y; }
        if (m_LockZ) { rotation.z = m_OriginalRotation.z; }
        transform.rotation = Quaternion.Euler(rotation);
    }
}
