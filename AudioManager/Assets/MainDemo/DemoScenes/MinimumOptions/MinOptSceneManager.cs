using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MinOptSceneManager : MonoBehaviour
{
    #region Variables

    // ====== FX(Sounds) ======
    public InputField soundFxLoopValuesInput;
    int soundFxLoopValues;
    public Transform timer;
    public Transform timerAnimated;

    #endregion Variables

    // Load Variables
    private void Start()
    {
        Loads();
    }
    private void Loads()
    {
        // ======== Load Inputs Values ========
        soundFxLoopValues = PlayerPrefs.GetInt("SoundFxTimerOneValues", 1);
        soundFxLoopValuesInput.text = soundFxLoopValues.ToString();
    }

    // MUSIC
    int mus = 0;
    public void PlayMusicFade() // new music, custom fade options
    {
        switch (mus)
        {
            case 0:
                MinOptAudioManager.InstanceMin.PlayMusicFade("Music One");
                mus++;
                break;
            case 1:
                MinOptAudioManager.InstanceMin.PlayMusicFade("Music Two");
                mus--;
                break;
        }
    }
    public void StopMusic()
    {
        MinOptAudioManager.InstanceMin.StopMusic();
    }
    public void StopMusicFade()
    {
        StartCoroutine(MinOptAudioManager.InstanceMin.StopMusicFade()); // 1 = fade duration
    }

    // SOUNDS

    public void SoundFxOnceWithName(string name)
    {
        MinOptAudioManager.InstanceMin.PlaySound2D(name);
    }

    // Timer One
    #region Timer One
    public void TimerOne()
    {
        soundFxLoopValues = int.Parse(soundFxLoopValuesInput.text);
        StartCoroutine(PlayTimer("Tick", soundFxLoopValues));
    }
    IEnumerator PlayTimer(string name, float duration)
    {
        Animation anim = timer.GetComponent<Animation>();
        int dagrees = 90;
        timer.GetChild(4).transform.localRotation = Quaternion.Euler(0, 0, dagrees);
        for (int i = 0; i < duration;)
        {

            if (i == duration - 1)
            {
                dagrees -= 90;
                timer.GetChild(4).transform.localRotation = Quaternion.Euler(0, 0, dagrees);
                MinOptAudioManager.InstanceMin.PlaySound2D("Alarm");
                anim.Play();
                yield return new WaitForSeconds(3.0f);
                dagrees = 90;
                timer.GetChild(4).transform.localRotation = Quaternion.Euler(0, 0, dagrees);
                timer.transform.localRotation = Quaternion.Euler(0, 0, 0);
                anim.Stop();
                i++;
            }
            else
            {
                dagrees -= 90;
                timer.GetChild(4).transform.localRotation = Quaternion.Euler(0, 0, dagrees);
                MinOptAudioManager.InstanceMin.PlaySound2D(name);
                yield return new WaitForSeconds(1.0f);
                i++;
            }
        }
    }


    #region Sound Fx input options
    public void SoundFxInputValidate()
    {
        int minValue = 1;
        int maxValue = 10;
        int thisValue = int.Parse(soundFxLoopValuesInput.text);
        if (thisValue < 1)
        {
            thisValue = Mathf.Max(minValue, thisValue);
        }
        if (thisValue > 10)
        {
            thisValue = Mathf.Min(maxValue, thisValue);
        }
        soundFxLoopValuesInput.text = thisValue.ToString();
        PlayerPrefs.SetInt("SoundFxTimerOneValues", thisValue);
    }
    public void IncrementLoop()
    {
        int thisValue = int.Parse(soundFxLoopValuesInput.text);
        thisValue++;
        soundFxLoopValuesInput.text = thisValue.ToString();
        PlayerPrefs.SetInt("SoundFxTimerOneValues", thisValue);
    }
    public void DecrementLoop()
    {
        int thisValue = int.Parse(soundFxLoopValuesInput.text);
        thisValue--;
        soundFxLoopValuesInput.text = thisValue.ToString();
        PlayerPrefs.SetInt("SoundFxTimerOneValues", thisValue);
    }

    #endregion Sound Fx input options
    #endregion Timer One

    // Timer Two

    #region Timer Two
    public void TimerTwo()
    {
        Animation anim = timerAnimated.GetChild(4).GetComponent<Animation>();
        anim.Play();
    }
    public void CallTimer()
    {
        StartCoroutine(PlayTimer());
    }
    IEnumerator PlayTimer()
    {
        Animation anim = timerAnimated.GetComponent<Animation>();
        MinOptAudioManager.InstanceMin.PlaySound2D("Alarm");
        anim.Play();
        yield return new WaitForSeconds(3.0f);
        anim.Stop();
        timerAnimated.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    #endregion

    //SCENE MANAGMENT

    public void NextSceneCont(string sceneN)
    {
        SceneManager.LoadScene(sceneN);
    }
}
