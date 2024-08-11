using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotoServiceScene()
    {
        SceneManager.LoadScene("DRT_Service");
    }
    public void GotoMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
