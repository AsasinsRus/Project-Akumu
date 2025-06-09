using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public UnitType type;
    public float maxSpawnDistance = 0.0f;

    [Range(0.0f, 10.0f)]
    public float sphereRadius = 0.5f;
    public Color sphereColor = Color.red;
    public Color wireSphereColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = sphereColor;
        Gizmos.DrawSphere(transform.position, sphereRadius);

        Gizmos.color = wireSphereColor;
        Gizmos.DrawWireSphere(transform.position, maxSpawnDistance);
    }

    public bool CheckInNear()
    {
        return Vector3.Distance(gameObject.transform.position,
            FindFirstObjectByType<PlayerController>().transform.position) > maxSpawnDistance;
    }

    public void Spawn(Transform gameObject)
    {
        Instantiate(gameObject, 
            transform.position + Vector3.right * (Random.Range(0, maxSpawnDistance)), 
            gameObject.transform.rotation);
    }
}
