using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    float colliderHeight;
    bool shouldFall;
    float timer;
    Vector2 origPos;
    float timeOfImpact;
    [SerializeField] bool fallAfterPlayerLeaves;
    [SerializeField] float timeBeforeFall;
    private void Start()
    {
        colliderHeight = GetComponent<BoxCollider2D>().bounds.extents.y;
        origPos = transform.position;
    }

    private void Update()
    {
        if (shouldFall && !fallAfterPlayerLeaves)
        {
            transform.position = origPos + new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.1f, 0.1f));
            timer += Time.deltaTime;
            if(timer > timeBeforeFall)
            {
                transform.position += Vector3.forward * 0.1f;
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                foreach (BoxCollider2D c in GetComponentsInChildren<BoxCollider2D>())
                {
                    c.enabled = false;
                }
                shouldFall = false;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.position.y > transform.position.y+colliderHeight*0.9f && collision.gameObject.CompareTag("Player"))
        {
            if(!shouldFall)
            {
                timeOfImpact = Time.timeSinceLevelLoad;
            }
            shouldFall = true;
        }
    }

  

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && fallAfterPlayerLeaves && shouldFall && Time.timeSinceLevelLoad - timeOfImpact > 0.05f)
        {
            Rigidbody2D mRigidbody = GetComponent<Rigidbody2D>();
            mRigidbody.bodyType = RigidbodyType2D.Dynamic;
            mRigidbody.AddForceAtPosition(collision.gameObject.transform.position, Vector2.down* 0.3f);
            transform.position += Vector3.forward * -0.1f;
            foreach (BoxCollider2D c in GetComponentsInChildren<BoxCollider2D>())
            {
                c.enabled = false;
            }
            shouldFall = false;
        }
    }
}
