using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{

    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] Transform firePoint;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            if (!Fire.Instance.isFireOnSP) // Fire가 SavePoint에 닿아있지 않은 경우에만 실행
            {
                GameManager.Instance.spawnPoints = spawnPoints;
                GameManager.Instance.fireSavePoint = firePoint;

                Vector3 pos = Fire.Instance.gameObject.transform.position;
                pos.x = firePoint.position.x;
                pos.z = firePoint.position.z;
                Fire.Instance.gameObject.transform.position = pos;

                Fire.Instance.isFireOnSP = true;

                SoundManager.Instance.PlaySound(SoundManager.AudioType.SavePoint);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            Fire.Instance.isFireOnSP = false;
        }
    }
}
