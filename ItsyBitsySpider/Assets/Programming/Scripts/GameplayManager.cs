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
    GrapplingGun myGun;
    PlayerController myPlayer;
    static bool endLevel;
    static bool win;
    static bool lose;

    [SerializeField] float transitionTime;
    float transitionTimeWin = 4.0f;
    float timer;

    Image winImage;
    Image loseImage;

    private void Start()
    {
        myWater = GameObject.FindGameObjectWithTag("WaterManager").GetComponent<WaterMnager>();
        winImage = GameObject.Find("WinImage").GetComponent<Image>();
        loseImage = GameObject.Find("LoseImage").GetComponent<Image>();
        myGun = GameObject.Find("GrapplingGun").GetComponent<GrapplingGun>();
        myPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
                timer += Time.deltaTime;
                if (timer > transitionTimeWin)
                {
                    SceneManager.LoadScene("mainmenu");
                }
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
        if(spiderHeight - endLevelHeight > 0.0f && myPlayer.grounded)
        {
            myGun.SetGrapplePoint(Vector2.up);
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
