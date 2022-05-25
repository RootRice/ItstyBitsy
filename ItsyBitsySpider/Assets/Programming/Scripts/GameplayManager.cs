using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static float spiderHeight;
    public static float waterHeight;
    [SerializeField] float endLevelHeight;
    WaterMnager myWater;
    static bool endLevel;
    static bool win;
    static bool lose;

    [SerializeField] float transitionTime;
    float timer;

    Image winImage;
    Image loseImage;

    private void Start()
    {
        myWater = GameObject.FindGameObjectWithTag("WaterManager").GetComponent<WaterMnager>();
        winImage = GameObject.Find("WinImage").GetComponent<Image>();
        loseImage = GameObject.Find("LoseImage").GetComponent<Image>();
        endLevel = false;
        win = false;
        lose = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(endLevel)
        {
            if(win)
            {
                winImage.color += new Color(0, 0, 0, 1 * Time.deltaTime*2);
            }
            if(lose)
            {
                loseImage.color += new Color(0, 0, 0, 1 * Time.deltaTime*2);
                timer += Time.deltaTime;
                if (timer > transitionTime)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
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
            myWater.StartEndLevelSequence();
            win = true;
        }
    }

    public static void EndLevel()
    {
        if(win)
        {

        }
        endLevel = true;
    }


}
