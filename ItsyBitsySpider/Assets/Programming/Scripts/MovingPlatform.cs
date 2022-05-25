using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Vector2[] patrolPoints;
    [SerializeField] Transform snailTransform;
    [SerializeField] float speed;

    int curTarget = 1;
    private void Start()
    {
        snailTransform.localPosition = patrolPoints[0];
    }
    private void Update()
    {
        
        snailTransform.localPosition = Vector2.MoveTowards(snailTransform.localPosition, patrolPoints[curTarget], speed * Time.deltaTime);
        float angle = Mathf.Atan2(patrolPoints[curTarget].y - snailTransform.localPosition.y, patrolPoints[curTarget].x - snailTransform.localPosition.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle-90));
        snailTransform.rotation = Quaternion.RotateTowards(snailTransform.rotation, targetRotation, 40*speed * Time.deltaTime);
        if (new Vector2(snailTransform.transform.localPosition.x, snailTransform.transform.localPosition.y) == patrolPoints[curTarget])
        {
            curTarget += 1;
            if(curTarget == patrolPoints.Length)
            {
                curTarget = 0;
            }
        }
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
