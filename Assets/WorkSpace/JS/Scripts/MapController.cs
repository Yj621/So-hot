using System.Collections;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Transform RockFallPoint;
    public GameObject RockPrefab;
    public float RockSpeed = 5f;
    public float RockDuration = 5f;
    public float RockSpwanTime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpwanRockRoutine());
    }

    
    private IEnumerator SpwanRockRoutine()
    {
        while (true)
        {
            RockFall();
            yield return new WaitForSeconds(RockSpwanTime);
        }
    }

    void RockFall()
    {
        GameObject rock = Instantiate(RockPrefab, RockFallPoint.position, Quaternion.identity);
        Rigidbody rb = rock.GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.down * RockSpeed;

        Destroy(rock, RockDuration);
    }
}
