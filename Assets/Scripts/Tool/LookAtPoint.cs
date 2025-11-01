using UnityEngine;

public class LookAtPoint  : MonoBehaviour
{
    public void LateUpdate()
    {
        transform.position = PlayerManager.Instance.CurrentPlayer.transform.position;
    }
}