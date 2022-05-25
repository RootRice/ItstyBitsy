using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Change_To_Level : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag("Portal"))
        {
            SceneManager.LoadScene("mainmenu");
        }

    }
}
