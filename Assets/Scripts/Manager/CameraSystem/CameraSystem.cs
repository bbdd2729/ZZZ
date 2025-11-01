using Unity.Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public  Vector3                  CamPosition;
    public  Quaternion               CamRotation;
    private ICinemachineCamera       _activeCamera;
    private CinemachineBrain         _cinemachineBrain;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;


    private void Start()
    {
        if (Camera.main != null) _cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (_cinemachineBrain == null) Debug.LogError("主相机上没有找到CinemachineBrain组件");
    }


    private void Update()
    {
        _activeCamera = _cinemachineBrain.ActiveVirtualCamera;
        CamPosition = _activeCamera.State.GetCorrectedPosition();
        CamRotation = _activeCamera.State.GetCorrectedOrientation();
    }
}