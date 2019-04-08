using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
    public float timeBeforeMainPage;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GamePreloader());   
    }

    IEnumerator GamePreloader()
    {
        yield return new WaitForSeconds(timeBeforeMainPage);

        yield return SceneManager.LoadSceneAsync("Main");


    }
}
