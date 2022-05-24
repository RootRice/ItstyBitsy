using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static float spiderHeight;
    public static float waterHeight;
    [SerializeField] float endLevelHeight;
    WaterMnager myWater;
    bool win;
    bool lose;
    private void Start()
    {
        myWater = GameObject.FindGameObjectWithTag("WaterManager").GetComponent<WaterMnager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(win)
        {

            return;
        }
        if(lose)
        {
            return;
        }
        if(spiderHeight - waterHeight < 1.0f)
        {
            myWater.StartEndLevelSequence();
            lose = true;
        }
        if(spiderHeight -endLevelHeight > 0.0f)
        {
            win = true;
        }
    }
}
