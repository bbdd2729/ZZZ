using UnityEngine;

public interface ICameraSystem
{
    Vector3    CamPosition  { get; }
    Quaternion CamRotation  { get; }
}







public interface ICameraQuery
{
    Vector3    Position     { get; }
    Quaternion Rotation     { get; }
    Transform  LookAtTarget { get; }
}

public interface ICameraCommand
{
    void SetLookAt(Transform target);
    void BlendTo(string cameraId, float blendTime = 0.3f);
}