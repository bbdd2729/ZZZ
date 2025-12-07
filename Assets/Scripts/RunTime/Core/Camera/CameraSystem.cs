using Unity.Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public static Vector3            CamPosition       { get=> ActiveCamera.State.GetCorrectedPosition();}
    public static Quaternion         CamRotation       { get=>ActiveCamera.State.GetCorrectedOrientation();}
    public static ICinemachineCamera ActiveCamera      { get => CinemachineBrain.ActiveVirtualCamera; }
    public static CinemachineBrain   CinemachineBrain  { get; set; }
    public static CinemachineCamera  CinemachineCamera { get => CinemachineBrain.ActiveVirtualCamera as CinemachineCamera;}
    public static Transform          LookAtPoint;


    private void Start()
    {
        if (Camera.main != null) CinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (CinemachineBrain == null) Debug.LogError("主相机上没有找到CinemachineBrain组件");
    }

    
    private void LateUpdate()
    {
        //LookAtPoint.position = PlayerManager.Instance.CurrentPlayer.LookAtPoint.position;


    }
}