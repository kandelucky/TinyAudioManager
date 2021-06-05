using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MinOptAudioManager : MonoBehaviour
{
	#region Variables
	public static MinOptAudioManager InstanceMin;
	
	public bool musicOn = true;
	public bool fxOn = true;
	// ====== TOGGLES ======
	[Header("TOGGLES")]
	public Toggle musicToggle;
	public Toggle fxToggle;

	// ====== Music/Ambient START PARAMETRES ======
	[Header("Start Music")]
	public string musicName; // if not Null and music was stopped (Off), when the music is turned on (On) again, music will play
	public bool startWithNewMusic = true; // if false scene starts without music, if next scene - continius with playing music

	// ====== FADE ATTRIBUTES ======
	[Header("Fades")]
	[Range(0, 5)]
	public float musicStartFade = 2.0f;
	[Range(0, 5)]
	public float musicStopFade = 2.0f;
	//==============================================================
	// Seperate audiosources
	//==============================================================
	GameObject audios;
	AudioSource musicSource;

	//==============================================================
	// Last played musics and ambients
	//==============================================================
	AudioName lastMusicName;
	bool musicIsOff;

	//==============================================================
	// Sound libraries. All your audio clips
	//==============================================================
	FxLibrary soundLibrary;
	MusLibrary musicLibrary;

	#endregion Variables

	//==============================================================
	// Awake/Start/Load variables
	//==============================================================
	private void Awake()
	{
		if (InstanceMin == null) InstanceMin = this;
		else if (InstanceMin != this) Destroy(gameObject);
		//==============================================================
		// Get FX and Music sound library
		//==============================================================
		soundLibrary = GetComponent<FxLibrary>();
		musicLibrary = GetComponent<MusLibrary>();

		//==============================================================
		// Create audio sources if we don't have
		//==============================================================
		audios = GameObject.FindGameObjectWithTag("music");
		if (audios == null)
		{
			audios = new GameObject("Audios");
			audios.AddComponent<DontDestroyOnLoad>();
			audios.tag = ("music");

			GameObject newMusicSource = new GameObject("Music source");
			musicSource = newMusicSource.AddComponent<AudioSource>();
			newMusicSource.transform.parent = audios.transform;
			musicSource.loop = true; // Music looping is true
			musicSource.playOnAwake = false;
			lastMusicName = newMusicSource.AddComponent<AudioName>();
		}
		else // or just find them in next scene
		{
			musicSource = audios.transform.Find("Music source").GetComponent<AudioSource>();
			lastMusicName = audios.transform.Find("Music source").GetComponent<AudioName>();
			if (musicName == "" && lastMusicName.lastAudioName != "") musicName = lastMusicName.lastAudioName; // if start new scene and start music Name == none; - continue playing last music
			musicIsOff = true;
		}
	}
	private void Start()
	{
		Loads();
		DestroyOldFxs();
	}
	private void Loads()
	{
		// ======== Load Toggles and Switches Bool ========
		musicOn = PlayerPrefs.GetInt("MusicMinSwitch", 1) == 0 ? false : true;
		SetOnOff("Music");
		fxOn = PlayerPrefs.GetInt("SoundMinSwitch", 1) == 0 ? false : true;
	}
	private void DestroyOldFxs()
	{
		// when You started new scene and at same time playing some fx sound, Fx Coroutine stops correctly working.
		// this funqcion find every not destroyed fxSources and destroy it when started new scene
		GameObject[] stopedfxs;
		stopedfxs = GameObject.FindGameObjectsWithTag("fx");

		if (stopedfxs.Length > 0)
		{
			foreach (GameObject fx in stopedfxs)
			{
				Destroy(fx);
			}
		}
	}

	//==============================================================
	// Set volume on all the channels
	//==============================================================

	#region Options

	public void SetOnOff(string name)
	{
		switch (name)
		{
			case "Music":
				if (!musicOn && musicSource.isPlaying)
				{
					musicSource.Stop(); // if music become OFF, stop playing
					musicIsOff = false;
				}
				else if (musicName != "") // if music become ON or if start NEW game and you have music name
				{
					// if you have start music in scene
					if (startWithNewMusic) PlayMusicFade(musicName);
					// if continuable music from another scene and music just was muted - continue last not stoped music when switch ON
					else if (!musicIsOff) PlayMusicFade(musicName);
				}
				break;
			case "Fx":
				GameObject[] fxSourceObj;
				fxSourceObj = GameObject.FindGameObjectsWithTag("fx");
				if (!fxOn && fxSourceObj.Length > 0)
				{
					foreach (GameObject fx in fxSourceObj)
					{
						Destroy(fx);
					}
				}
				break;
		}
	}
	// TOGGLES
	public void SetToggles()
	{
		musicToggle.SetIsOnWithoutNotify(musicOn);
		fxToggle.SetIsOnWithoutNotify(fxOn);
	}
	public void SetMusicToggle()
	{
		int boolInt = musicToggle.isOn ? 1 : 0;
		PlayerPrefs.SetInt("MusicMinSwitch", boolInt);
		musicOn = musicToggle.isOn;
		SetOnOff("Music");
	}
	public void SetFxToggle()
	{
		int boolInt = fxToggle.isOn ? 1 : 0;
		PlayerPrefs.SetInt("SoundMinSwitch", boolInt);
		fxOn = fxToggle.isOn;
		SetOnOff("Fx");
	}

	#endregion Options

	//==============================================================
	// Music
	//==============================================================

	#region Music

	//==================================================================
	// Play music fade in - with new AudioSource
	//==================================================================

	// Create new MusicSource Gameobject an Play music with default fade
	public void PlayMusicFade(string thisName)
	{
		if (musicOn && thisName != "") // if music name = Null - This is new scene with no music
		{
			if (musicSource.isPlaying)
			{
				StartCoroutine(StopAndDeleteMusicSource());
				GameObject newMusicSource = new GameObject("Music source");
				AudioSource thisMusicSource = newMusicSource.AddComponent<AudioSource>();
				newMusicSource.transform.parent = audios.transform;
				thisMusicSource.loop = true;
				thisMusicSource.playOnAwake = false;
				lastMusicName = newMusicSource.AddComponent<AudioName>();
				StartCoroutine(PlayNewMusicFade(thisMusicSource, thisName));
			}
			else StartCoroutine(PlayNewMusicFade(musicSource, thisName));

		}
	}

    #region Fade
    IEnumerator PlayNewMusicFade(AudioSource thisMusicS, string thisMusicName)
	{
		float startVolume = 0;
		float targetVolume = thisMusicS.volume;
		float currentTime = 0;

		thisMusicS.clip = musicLibrary.GetClipFromName(thisMusicName);
		thisMusicS.Play();

		while (currentTime < musicStartFade)
		{
			currentTime += Time.deltaTime;
			thisMusicS.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / musicStartFade);
			yield return null;
		}
		// save music name for Next Scene and for Music On/Off options
		lastMusicName.lastAudioName = musicName = thisMusicName;
		musicIsOff = false;
		musicSource = thisMusicS;
		yield break;
	}
	IEnumerator StopAndDeleteMusicSource()
	{

		Transform MSource = audios.transform.Find("Music source");
		AudioSource audioS = MSource.GetComponent<AudioSource>();
		float startVolume = audioS.volume;
		float targetVolume = 0;
		float currentTime = 0;

		while (currentTime < musicStopFade)
		{
			currentTime += Time.deltaTime;
			audioS.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / musicStopFade);
			yield return null;
		}
		audioS.Stop();
		Destroy(MSource.gameObject);
		yield break;
	}
    #endregion

    //==============================================================
    // Stop music
    //==============================================================
    public void StopMusic()
	{
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
			musicName = "";
			musicIsOff = true;
		}
	}

	//==============================================================
	// Stop music fade out
	//==============================================================
	public IEnumerator StopMusicFade()
	{
		if (musicOn && musicSource.isPlaying)
		{
			float currentVolume = musicSource.volume;
			float startVolume = musicSource.volume;
			float targetVolume = 0;
			float currentTime = 0;

			while (currentTime < musicStopFade)
			{
				currentTime += Time.deltaTime;
				musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / musicStopFade);
				yield return null;
			}
			musicSource.Stop();
			musicSource.volume = currentVolume;
			musicName = "";
			musicIsOff = true;
			yield break;
		}
	}

	#endregion Music

	//==============================================================
	// FX Audio
	//==============================================================

	#region Fx
	public void PlaySound2D(string soundName)
	{
		if (fxOn)
		{
			GameObject newfxSource = new GameObject("2D fx source");
			AudioSource fxSource = newfxSource.AddComponent<AudioSource>();
			newfxSource.transform.parent = audios.transform;
			newfxSource.tag = ("fx");
			fxSource.playOnAwake = false;
			fxSource.clip = soundLibrary.GetClipFromName(soundName);
			fxSource.PlayOneShot(fxSource.clip);
			StartCoroutine(DeleteSound2D(newfxSource, fxSource.clip));
		}
	}
	IEnumerator DeleteSound2D(GameObject newfxSource, AudioClip clip)
	{
		yield return new WaitForSeconds(clip.length);
		Destroy(newfxSource.gameObject);
	}


	#endregion Fx

}
