using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private GameManager gm = GameManager.Instance;
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    Fire fire = Fire.Instance;
    [SerializeField] Transform firePoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            gm.spawnPoints = spawnPoints;
            gm.fireSavePoint = firePoint;
            fire.gameObject.transform.position = firePoint.position;
        }
    }
}
