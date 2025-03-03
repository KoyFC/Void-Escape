using UnityEngine;
using Unity.Cinemachine;

public class FindPlayerForCamera : MonoBehaviour
{
    void Start()
    {
        GetComponent<CinemachineCamera>().Target.LookAtTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
