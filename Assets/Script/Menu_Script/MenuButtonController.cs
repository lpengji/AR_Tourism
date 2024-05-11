using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{
    private String previousScene;
    // Start is called before the first frame update
    void Start()
    {
        previousScene = "Map_Scene";
    }
    public void BackToMapScene()
    {
        PlayerPrefs.DeleteKey("locationInfo");
        SceneManager.LoadScene(previousScene);
    }
}
