using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class AllOptSceneManager : MonoBehaviour
{
    #region Variables

    // ====== FX(Sounds) ======
    public InputField soundFxLoopValuesInput;
    int soundFxLoopValues;
    // ====== AMBIENT ======
    public InputField ambientInput;
    int ambientDalayValue;
    public Text ambinetButtonText;
    int ambientSwitchInt;
    public Text ambinetDelayButtonText;
    int ambientDelaySwitch;

    #endregion Variables

    // Load Variables
    private void Start()
    {
        Loads();
    }
    private void Loads()
    {
        // ======== Load Inputs Values ========
        soundFxLoopValues = PlayerPrefs.GetInt("SoundFxLoopValues", 1);
        soundFxLoopValuesInput.text = soundFxLoopValues.ToString();
        ambientDalayValue = PlayerPrefs.GetInt("AmbientDelayValue", 1);
        ambientInput.text = ambientDalayValue.ToString();
    }

    //MUSIC

    #region Simple
    public void PlayMusicOne()
    {
        AllOptAudioManager.InstanceAll.PlayMusic("Music One", 0); // 0 = delay
    }

    public void PlayMusicTwo()
    {
        AllOptAudioManager.InstanceAll.PlayMusic("Music Two", 0); // 0 = delay
    }

    public void StopMusic()
    {
        AllOptAudioManager.InstanceAll.StopMusic();
    }
    #endregion Simple

    #region Fade
    public void PlayMusicOneFade() // new music, custom fade options
    {
        StartCoroutine(AllOptAudioManager.InstanceAll.PlayMusicFadeDelay("Music One", 2f, 0)); // 2 = fade duration, 0 = delay
    }
    public void PlayMusicTwoFade() // new music, new AudioSource - defauld fade options
    {
        AllOptAudioManager.InstanceAll.PlayMusicFade("Music Two");
    }
    public void StopMusicFade()
    {
        StartCoroutine(AllOptAudioManager.InstanceAll.StopMusicFade()); // 1 = fade duration
    }
    #endregion Fade

    //AMBIENT
    public void PlayAmbientFadeDelay()  // new ambient, custom fade and delay options
    {
        switch (ambientSwitchInt)
        {
            case 0:
                ambinetButtonText.text = "Play Street";
                StartCoroutine(AllOptAudioManager.InstanceAll.PlayAmbientDelay("Factory", 2f, 0)); // 2 = fade duration, 0 = delay
                ambientSwitchInt++;
                break;
            case 1:
                ambinetButtonText.text = "Play Rain";
                StartCoroutine(AllOptAudioManager.InstanceAll.PlayAmbientDelay("Street", 2f, 0)); // 2 = fade duration, 0 = delay
                ambientSwitchInt++;
                break;
            case 2:
                ambinetButtonText.text = "Play Factory";
                StartCoroutine(AllOptAudioManager.InstanceAll.PlayAmbientDelay("Rain", 2f, 0)); // 2 = fade duration, 0 = delay
                ambientSwitchInt = 0;
                break;
        }
    }

    public void PlayAmbientRandom()  // new ambient, new AudioSource - default fade options
    {
        AllOptAudioManager.InstanceAll.PlayAmbientFade("AmbinetGroup");
    }

    public void StopAmbientFade()
    {
        StartCoroutine(AllOptAudioManager.InstanceAll.StopAmbientFade()); // 1 = fade duration
    }

    public void PlayAmbientDelay()
    {
        ambientDalayValue = int.Parse(ambientInput.text);
        switch (ambientDelaySwitch)
        {
            case 0:
                ambinetDelayButtonText.text = "Play Street\nwith delay";
                StartCoroutine(AllOptAudioManager.InstanceAll.PlayAmbientDelay("Factory", 2f, ambientDalayValue)); // 2 = fade duration
                ambientDelaySwitch++;
                break;
            case 1:
                ambinetDelayButtonText.text = "Play Rain\nwith delay";
                StartCoroutine(AllOptAudioManager.InstanceAll.PlayAmbientDelay("Street", 2f, ambientDalayValue)); // 2 = fade duration
                ambientDelaySwitch++;
                break;
            case 2:
                ambinetDelayButtonText.text = "Play Factory\nwith delay";
                StartCoroutine(AllOptAudioManager.InstanceAll.PlayAmbientDelay("Rain", 2f, ambientDalayValue)); // 2 = fade duration
                ambientDelaySwitch = 0;
                break;
        }
    }

    #region Ambinet delay input options
    public void AmbientInputValidate()
    {
        int minValue = 1;
        int maxValue = 10;
        int thisValue = int.Parse(ambientInput.text);
        if (thisValue < 1)
        {
            thisValue = Mathf.Max(minValue, thisValue);
        }
        if (thisValue > 10)
        {
            thisValue = Mathf.Min(maxValue, thisValue);
        }
        ambientInput.text = thisValue.ToString();
        PlayerPrefs.SetInt("AmbientDelayValue", thisValue);
    }
    public void IncrementDelay()
    {
        int thisValue = int.Parse(ambientInput.text);
        thisValue++;
        ambientInput.text = thisValue.ToString();
        PlayerPrefs.SetInt("AmbientDelayValue", thisValue);
    }
    public void DecrementDelay()
    {
        int thisValue = int.Parse(ambientInput.text);
        thisValue--;
        ambientInput.text = thisValue.ToString();
        PlayerPrefs.SetInt("AmbientDelayValue", thisValue);
    }

    #endregion Ambient dalay input options

    //SOUNDS

    public void SoundFxOnceWithName(string name)
    {
        AllOptAudioManager.InstanceAll.PlaySound2D(name);
    }

    public void SoundFxOnce()
    {
        AllOptAudioManager.InstanceAll.PlaySound2D("Boink");
    }

    public void SoundFxRandom()
    {
        AllOptAudioManager.InstanceAll.PlaySound2D("Gunshot");
    }

    public void SoundFxManyTimes()
    {
        soundFxLoopValues = int.Parse(soundFxLoopValuesInput.text);
        StartCoroutine(AllOptAudioManager.InstanceAll.PlaySound2DLoop("Buzz", soundFxLoopValues)); // soundFxLoopValues = how many times You want to play fx sound
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
        PlayerPrefs.SetInt("SoundFxLoopValues", thisValue);
    }
    public void IncrementLoop()
    {
        int thisValue = int.Parse(soundFxLoopValuesInput.text);
        thisValue++;
        soundFxLoopValuesInput.text = thisValue.ToString();
        PlayerPrefs.SetInt("SoundFxLoopValues", thisValue);
    }
    public void DecrementLoop()
    {
        int thisValue = int.Parse(soundFxLoopValuesInput.text);
        thisValue--;
        soundFxLoopValuesInput.text = thisValue.ToString();
        PlayerPrefs.SetInt("SoundFxLoopValues", thisValue);
    }

    #endregion Sound Fx input options

    //SCENE MANAGMENT

    public void NextSceneCont(string sceneN)
    {
        SceneManager.LoadScene(sceneN);
    }
}
