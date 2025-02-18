using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera followCamera;

    public void SetTarget(Transform target)
    {
        if (followCamera != null)
        {
            followCamera.Follow = target;
            followCamera.LookAt = target; 
        }
    }
}
