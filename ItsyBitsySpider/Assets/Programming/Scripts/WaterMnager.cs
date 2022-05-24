using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMnager : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField]  WaterEditor[] waters;
    [SerializeField] float[] maxOffsets;
    [SerializeField] float[] minOffsets;
    [SerializeField] float[] changeSpeed;
    [SerializeField] float timerRange;
    float[] waveOffsets;
    float[] originalHeights;
    int[] dir;
    float[] timers;
    float[] timeLimits;
    [Header("Heights")]
    [SerializeField] float[] maxHeightOffsetRange;
    [SerializeField] float[] speedRange;
    float targetOffset;
    float speed;
    Transform[] waterTransforms;
    Vector3[] waterPositions;
    float heightOffset;
    int heightDir = 1;
    [Header("TimeOffsets")]
    [SerializeField] float[] maxTimeOffsets;
    [SerializeField] float[] minTimeOffsets;
    [SerializeField] float[] timeChangeSpeed;
    float[] timeOffsets;

    float vel;

    private void Start()
    {
        timeLimits = new float[2];
        waveOffsets = new float[2];
        timers = new float[2];
        originalHeights = new float[2];
        dir = new int[2];
        waterTransforms = new Transform[2];
        waterPositions = new Vector3[2];
        timeOffsets = new float[2];
        for (int i = 0; i < waters.Length; i++)
        {
            originalHeights[i] = waters[i].waveHeight;
            dir[i] = Random.Range(0, 2) * 2 - 1;
            timeLimits[i] = Random.Range(0f, timerRange);
            waterTransforms[i] = waters[i].transform;
            waterPositions[i] = waterTransforms[i].localPosition;
            speed = Random.Range(speedRange[0], speedRange[1]);
            targetOffset = Random.Range(maxHeightOffsetRange[0], maxHeightOffsetRange[1]);
        }

    }
    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < waters.Length; i++)
        {
            //Wave height
            timers[i] += Time.deltaTime;
            if (timers[i] > timeLimits[i])
            {
                dir[i] *= -1;
                timers[i] = 0;
                timeLimits[i] = Random.Range(0f, timerRange);
            }
            waveOffsets[i] = Mathf.Clamp(waveOffsets[i] + Time.deltaTime * dir[i] * changeSpeed[i], minOffsets[i], maxOffsets[i]);
            waters[i].waveHeight = originalHeights[i] + waveOffsets[i];
            timeOffsets[i] = Mathf.Clamp(timeOffsets[i] + Time.deltaTime * dir[i] * timeChangeSpeed[i], minTimeOffsets[i], maxTimeOffsets[i]);
            waters[i].timeOffset = 0 + timeOffsets[i];


        }
        //Water height
        heightOffset = Mathf.SmoothDamp(heightOffset, targetOffset * heightDir, ref vel, Time.deltaTime*speed);
        if (Mathf.Abs(heightOffset - targetOffset * heightDir) < 0.1f)
        {
            speed = Random.Range(speedRange[0], speedRange[1]);
            targetOffset = Random.Range(maxHeightOffsetRange[0], maxHeightOffsetRange[1]);
            heightDir *= -1;
        }
        waterTransforms[0].localPosition = waterPositions[0] + Vector3.up * heightOffset;
        waterTransforms[1].localPosition = waterPositions[1] + Vector3.up * -heightOffset;
    }

    void Water1()
    {

    }

    void Water2()
    {

    }
}
