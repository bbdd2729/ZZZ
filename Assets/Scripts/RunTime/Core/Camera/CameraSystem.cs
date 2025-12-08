using Unity.Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public static CameraSystem             Instance;
    public        Vector3                  CamPosition;
    public        Quaternion               CamRotation;
    public        Transform                LookAtPoint;
    private       ICinemachineCamera       _activeCamera;
    private       CinemachineBrain         _cinemachineBrain;
    private       CinemachineVirtualCamera _cinemachineVirtualCamera;


    private void Start()
    {
        if (Camera.main != null) _cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (_cinemachineBrain == null) Debug.LogError("主相机上没有找到CinemachineBrain组件");
        Instance = this;
    }


    private void Update()
    {
        _activeCamera = _cinemachineBrain.ActiveVirtualCamera;
        CamPosition = _activeCamera.State.GetCorrectedPosition();
        CamRotation = _activeCamera.State.GetCorrectedOrientation();
    }

    private void LateUpdate()
    {
        //LookAtPoint.position = PlayerManager.Instance.CurrentPlayer.LookAtPoint.position;
    }
}