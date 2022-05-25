using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Vector2[] patrolPoints;
    private void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach(Vector2 p in patrolPoints)
        {
            Gizmos.DrawSphere(transform.position + new Vector3(p.x, p.y, 0), 0.4f);
        }
    }
}
