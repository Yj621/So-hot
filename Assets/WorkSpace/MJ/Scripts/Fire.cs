using Photon.Pun;
using UnityEngine;

public class Fire : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            GameManager.Instance.photonView.RPC("GameClear", RpcTarget.All);
        }
    }
}
