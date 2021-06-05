using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopOpener : MonoBehaviour
{
    public Transform slidersPop;
    public Transform togglePop;
    public Transform switchesPop;
    public GameObject popBg;
    public Image img;
    Animator anim;
    string popName;

    #region Sliders Pop
    public void OpenSliderPop()
    {
        popName = "Slider";
        popBg.SetActive(true);
        slidersPop.gameObject.SetActive(true);
        anim = slidersPop.GetComponent<Animator>();
        StartCoroutine(FadeIn());
        anim.Play("PopAnimation");

    }
    public void CloseSliderPop()
    {
        anim = slidersPop.GetComponent<Animator>();
        StartCoroutine(FadeOut());
        anim.Play("ClosePopAnimation");
    }
    #endregion

    #region Toggles Pop
    public void OpenTogglePop()
    {
        popName = "Toggle";
        popBg.SetActive(true);
        togglePop.gameObject.SetActive(true);
        anim = togglePop.GetComponent<Animator>();
        StartCoroutine(FadeIn());
        anim.Play("PopAnimation");

    }
    public void CloseTogglePop()
    {
        anim = togglePop.GetComponent<Animator>();
        StartCoroutine(FadeOut());
        anim.Play("ClosePopAnimation");
    }
    #endregion

    #region Switches Pop
    public void OpenSwitchesPop()
    {
        popName = "Switch";
        popBg.SetActive(true);
        switchesPop.gameObject.SetActive(true);
        anim = switchesPop.GetComponent<Animator>();
        StartCoroutine(FadeIn());
        anim.Play("PopAnimation");

    }
    public void CloseSwitchesPop()
    {
        anim = switchesPop.GetComponent<Animator>();
        StartCoroutine(FadeOut());
        anim.Play("ClosePopAnimation");
    }
    #endregion

    #region BackGround Fade
    // fade from transparent to opaque
    IEnumerator FadeIn()
    {

        // loop over 1 second
        for (float i = 0; i <= 0.83f; i += Time.deltaTime*5)
        {
            // set color with i as alpha
            img.color = new Color(0.13f, 0.13f, 0.13f, i);
            yield return null;
        }

    }

    // fade from opaque to transparent
    IEnumerator FadeOut()
    {
        // loop over 1 second backwards
        for (float i = 0.83f; i >= 0; i -= Time.deltaTime*5)
        {
            // set color with i as alpha
            img.color = new Color(0.13f, 0.13f, 0.13f, i);
            yield return null;
        }
        popBg.SetActive(false);
        switch (popName)
        {
            case "Slider":
                slidersPop.gameObject.SetActive(false);
                break;
            case "Toggle":
                togglePop.gameObject.SetActive(false);
                break;
            case "Switch":
                switchesPop.gameObject.SetActive(false);
                break;
        }
    }
    #endregion
}
