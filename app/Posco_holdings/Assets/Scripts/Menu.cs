using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button menuOnButton;
    public Button menuCloseButton;
    public GameObject menuPannel;

    Vector3 initPos;

    bool menuOpen;
    bool menuClose;



    public float fadeSpeed = 1.5f;
    public bool fadeInOnStart = true;
    public bool fadeOutOnExit = true;

    public GameObject fadePannel;
    Color fadePanColor;

    // Start is called before the first frame update
    void Start()
    {
        initPos = menuPannel.transform.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
        if (menuOpen)
        {
            if (Mathf.Abs(initPos.x + 810 - gameObject.transform.localPosition.x) >= 1)
            {
                menuPannel.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, initPos + Vector3.right * 810, 0.1f);
                fadePannel.GetComponent<Image>().color = fadePanColor;
            }
            else
            {
                menuPannel.transform.localPosition = initPos + Vector3.right * 810;
                menuOpen = false;
                menuOnButton.interactable = true;
                menuCloseButton.interactable = true;
                StopCoroutine(FadeIn());
            }
        }

        if (menuClose)
        {
            if (Mathf.Abs(gameObject.transform.localPosition.x - initPos.x) >= 1)
            {
                menuPannel.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, initPos, 0.1f);
                fadePannel.GetComponent<Image>().color = fadePanColor;
            }
            else
            {
                menuPannel.transform.localPosition = initPos;
                menuClose = false;
                menuOnButton.interactable = true;
                menuCloseButton.interactable = true;
                StopCoroutine(FadeOut());
                fadePannel.SetActive(false);
            }
        }
        Debug.Log(fadePanColor);
    }

    public void MenuOpen()
    {
        menuOpen = true;
        menuOnButton.interactable = false;
        menuCloseButton.interactable = false;

        fadePannel.SetActive(true);
        fadePanColor = fadePannel.GetComponent<Image>().color;
        fadePanColor.a = 0f;
        fadePannel.GetComponent<Image>().color = fadePanColor;
        StartCoroutine(FadeIn());
    }
    public void MenuClose()
    {
        menuClose = true;
        menuOnButton.interactable = false;
        menuCloseButton.interactable = false;

        fadePanColor = fadePannel.GetComponent<Image>().color;
        fadePanColor.a = 0.25f;
        fadePannel.GetComponent<Image>().color = fadePanColor;
        StartCoroutine(FadeOut());
    }



    IEnumerator FadeIn()
    {
        while (fadePanColor.a < 0.25f)
        {
            fadePanColor.a += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        while (fadePanColor.a > 0f)
        {
            fadePanColor.a -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}
