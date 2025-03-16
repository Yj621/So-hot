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
            GameManager.Instance.spawnPoints = spawnPoints;
            GameManager.Instance.fireSavePoint = firePoint;
            Fire.Instance.isFireOnSP = true;

            SoundManager.Instance.PlaySound(SoundManager.AudioType.SavePoint);
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
