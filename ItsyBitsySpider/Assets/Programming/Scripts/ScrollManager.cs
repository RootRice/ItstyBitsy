using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    public static float startScrollHeight;
    public static float endScrollHeight;
    float spiderHeight;
    Transform myCamera;
    float origHeight;

    [SerializeField] float minScrollHeight;
    [SerializeField] float maxScrollHeight;
    [SerializeField] Transform spider;

    private void Start()
    {
        endScrollHeight = maxScrollHeight;
        startScrollHeight = minScrollHeight;
        myCamera = Camera.main.transform;
        origHeight = myCamera.position.y;

    }
    private void LateUpdate()
    {
        spiderHeight = spider.transform.position.y;
        float scrollAmount = GetScrollAmount();
        myCamera.position = new Vector3(myCamera.position.x, origHeight + scrollAmount, myCamera.position.z);
        
    }

    public float GetScrollAmount()
    {
        return Mathf.Clamp(spiderHeight, minScrollHeight, maxScrollHeight);
    }
}
