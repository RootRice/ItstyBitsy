using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    float colliderHeight;
    bool shouldFall;
    float timer;
    Vector2 origPos;
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
            transform.position = origPos + new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            timer += Time.deltaTime;
            if(timer > timeBeforeFall)
            {
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                GetComponent<BoxCollider2D>().enabled = false;
                shouldFall = false;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.position.y > transform.position.y+colliderHeight*0.9f && collision.gameObject.CompareTag("Player"))
        {
            shouldFall = true;
        }
    }

  

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && fallAfterPlayerLeaves && shouldFall)
        {
            Rigidbody2D mRigidbody = GetComponent<Rigidbody2D>();
            mRigidbody.bodyType = RigidbodyType2D.Dynamic;
            mRigidbody.AddForceAtPosition(collision.gameObject.transform.position, Vector2.down* 0.3f);
            GetComponent<BoxCollider2D>().enabled = false;
            shouldFall = false;
        }
    }
}
