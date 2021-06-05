using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AllOptAudioManager : MonoBehaviour
{
    #region Variables
    public static AllOptAudioManager InstanceAll;
	public enum AudioChannel { Master, Music, Ambient, fx };
	// ====== SLIDERS ======
	[Header("SLIDERS")]
	[Range(0, 1)]
	public float masterVolume = 1f; // Overall volume
	public Slider masterSlider; // Master slider
	public bool musicOn = true;
	[Range(0, 1)]
	public float musicVolume = 1f; // Music volume
	public Slider musicSlider; // Music slider
	public bool ambientOn = true;
	[Range(0, 1)]
	public float ambientVolume = 1f; // Ambient volume
	public Slider ambientSlider; // Ambient slider
	public bool fxOn = true;
	[Range(0, 1)]
	public float fxVolume = 1f; // FX volume
	public Slider fxSlider; // FX slider
	// ====== TOGGLES ======
	[Header("TOGGLES")]
	public Toggle musicToggle;
	public Toggle ambientToggle;
	public Toggle fxToggle;
	// ====== SWITCHES ======
	[Header("SWITCHES")]
	public Slider musicSwitch;
	public Slider ambientSwitch;
	public Slider fxSwitch;

	// ====== Music/Ambient START PARAMETRES ======
	[Header("Start Music")]
	public string musicName; // if not Null and music was stopped (Off), when the music is turned on (On) again, music will play
	public bool startWithNewMusic = true; // if false scene starts without music, if next scene - continius with playing music
	[Header("Start Ambient")]
	public string ambientName;
	public bool StartWithNewAmbient = true;

	// ====== FADE ATTRIBUTES ======
	[Header("Fades")]
	[Range(0, 5)]
	public float musicStartFade = 2.0f;
	[Range(0, 5)]
	public float musicStopFade = 2.0f;
	[Range(0, 5)]
	public float ambientStartFade = 2.0f;
	[Range(0, 5)]
	public float ambientStopFade = 2.0f;


	//==============================================================
	// Seperate audiosources
	//==============================================================
	GameObject audios;
	AudioSource musicSource;
	AudioSource ambientSource;

	//==============================================================
	// Last played musics and ambients
	//==============================================================
	AudioName lastMusicName;
	AudioName lastAmbientName;
	bool musicIsOff;
	bool ambinetIsOff;

	//==============================================================
	// Sound libraries. All your audio clips
	//==============================================================
	FxLibrary soundLibrary;
	MusLibrary musicLibrary;
	AmbLibrary ambientLibrary;

    #endregion Variables

    //==============================================================
    // Awake/Start/Load variables
    //==============================================================
    private void Awake()
	{
		if (InstanceAll == null) InstanceAll = this;
		else if (InstanceAll != this) Destroy(gameObject);
		//==============================================================
		// Get FX, Music and Ambient sound library
		//==============================================================
		soundLibrary = GetComponent<FxLibrary>();
		musicLibrary = GetComponent<MusLibrary>();
		ambientLibrary = GetComponent<AmbLibrary>();

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

			GameObject newAmbientsource = new GameObject("Ambient source");
			ambientSource = newAmbientsource.AddComponent<AudioSource>();
			newAmbientsource.transform.parent = audios.transform;
			ambientSource.loop = true; // Ambient sound looping is true
			ambientSource.playOnAwake = false;
			lastAmbientName = newAmbientsource.AddComponent<AudioName>();
		}
		else // or just find them in next scene
		{
			ambientSource = audios.transform.Find("Ambient source").GetComponent<AudioSource>();
			lastAmbientName = audios.transform.Find("Ambient source").GetComponent<AudioName>();
			if (ambientName == "" && lastAmbientName.lastAudioName != "") ambientName = lastAmbientName.lastAudioName;
			ambinetIsOff = true;
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
		// ======== Load Sliders ========
		masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
		musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
		ambientSlider.value = PlayerPrefs.GetFloat("AmbinetVolume", 1f);
		fxSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
		// Set volume on all the channels
		SetVolume(masterSlider.value, AudioChannel.Master);
		SetVolume(musicSlider.value, AudioChannel.Music);
		SetVolume(ambientSlider.value, AudioChannel.Ambient);
		SetVolume(fxSlider.value, AudioChannel.fx);
		// ======== Load Toggles and Switches Bool ========
		musicOn = PlayerPrefs.GetInt("MusicSwitch", 1) == 0 ? false : true;
		SetOnOff("Music");
		ambientOn = PlayerPrefs.GetInt("AmbientSwitch", 1) == 0 ? false : true;
		SetOnOff("Ambient");
		fxOn = PlayerPrefs.GetInt("SoundSwitch", 1) == 0 ? false : true;
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

	public void SetSliderVolume(string name, float volume)
	{
		switch (name)
		{
			case "Master":
				masterVolume = volume;
				SetVolume(masterVolume, AudioChannel.Master);
				break;
			case "Music":
				musicVolume = volume;
				SetVolume(musicVolume, AudioChannel.Music);
				break;
			case "Ambient":
				ambientVolume = volume;
				SetVolume(ambientVolume, AudioChannel.Ambient);
				break;
			case "Sound":
				fxVolume = volume;
				SetVolume(fxVolume, AudioChannel.fx);
				break;
		}

	}
	public void SetVolume(float volumePercent, AudioChannel channel)
	{
		switch (channel)
		{
			case AudioChannel.Master:
				masterVolume = volumePercent;
				break;
			case AudioChannel.fx:
				fxVolume = volumePercent;
				break;
			case AudioChannel.Music:
				musicVolume = volumePercent;
				break;
			case AudioChannel.Ambient:
				ambientVolume = volumePercent;
				break;
		}

		// Set the audiosource volume
		musicSource.volume = musicVolume * masterVolume;
		ambientSource.volume = ambientVolume * masterVolume;
	}
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
			case "Ambient":
				if (!ambientOn && ambientSource.isPlaying)
				{
					ambientSource.Stop();
					ambinetIsOff = false;
				}
				else if (ambientName != "")
				{
					if (StartWithNewAmbient) PlayAmbientFade(ambientName);
					else if (!ambinetIsOff) PlayAmbientFade(ambientName);
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

	//====================================
	//VOLUMES MANAGER, ON/OFF MANAGER
	//====================================

	// SLIDERS
	public void SaveSlidersVolume(string name)
	{
		switch (name)
		{
			case "Master":
				if (masterSlider.value > 0.1)
				{
					// if sounds are off, switch at on
					// music
					if (!musicOn)
					{
						PlayerPrefs.SetInt("MusicSwitch", 1);
						musicOn = true;
						musicSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
						SetOnOff("Music");
					}
					// ambient
					if (!ambientOn)
					{
						PlayerPrefs.SetInt("AmbientSwitch", 1);
						ambientOn = true;
						ambientSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
						SetOnOff("Ambient");
					}
					// fx
					if (!fxOn)
					{
						PlayerPrefs.SetInt("SoundSwitch", 1);
						fxOn = true;
						fxSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
						SetOnOff("Fx");
					}
				}
				else
				{ // if master volume is too low
					masterSlider.value = 0; // off all sounds
					// off music
					musicSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color32(150, 150, 150, 255);
					PlayerPrefs.SetInt("MusicSwitch", 0);
					musicOn = false;
					SetOnOff("Music");
					// off ambient
					ambientSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color32(150, 150, 150, 255);
					PlayerPrefs.SetInt("AmbientSwitch", 0);
					ambientOn = false;
					SetOnOff("Ambient");
					// off fx
					fxSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color32(150, 150, 150, 255);
					PlayerPrefs.SetInt("SoundSwitch", 0);
					fxOn = false;
					SetOnOff("Fx");
				}
				PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
				break;
			case "Music":
				if (musicSlider.value > 0.1)
				{
					// if music is off, switch at on
					if (!musicOn)
					{
						if (masterSlider.value == 0f)
						{
							masterSlider.value = 0.5f;
							SaveSlidersVolume("Master");
						}
						else
						{
							PlayerPrefs.SetInt("MusicSwitch", 1);
							musicOn = true;
							SetOnOff("Music");
						}
					}
				}
				else
				{
					// off music
					musicSlider.value = 0;
					PlayerPrefs.SetInt("MusicSwitch", 0);
					musicOn = false;
					SetOnOff("Music");
				}
				PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
				break;
			case "Ambient":
				if (ambientSlider.value > 0.1)
				{
					// if ambient is off, switch at on
					if (!ambientOn)
					{
						if (masterSlider.value == 0f)
						{
							masterSlider.value = 0.5f;
							SaveSlidersVolume("Master");
						}
						else
						{
							PlayerPrefs.SetInt("AmbientSwitch", 1);
							ambientOn = true;
							SetOnOff("Ambient");
						}
					}
				}
				else
				{
					// off ambient
					ambientSlider.value = 0;
					PlayerPrefs.SetInt("AmbientSwitch", 0);
					ambientOn = false;
					SetOnOff("Ambient");
				}
				PlayerPrefs.SetFloat("AmbinetVolume", ambientSlider.value);
				break;
			case "Sound":
				if (fxSlider.value > 0.1)
				{
					// if fx is off, switch at on
					if (!fxOn)
					{
						if (masterSlider.value == 0f)
						{
							masterSlider.value = 0.5f;
							SaveSlidersVolume("Master");
						}
						else
						{
							PlayerPrefs.SetInt("SoundSwitch", 1);
							fxOn = true;
							SetOnOff("Fx");
						}

					}
				}
				else
				{
					// off fx
					fxSlider.value = 0;
					PlayerPrefs.SetInt("SoundSwitch", 0);
					fxOn = false;
					SetOnOff("Fx");
				}
				PlayerPrefs.SetFloat("SoundVolume", fxSlider.value);
				break;
		}
	}
	public void SetVolume(string slider)
	{
		switch (slider)
		{
			case "Master":
				SetSliderVolume("Master", masterSlider.value);
				break;
			case "Music":
				SetSliderVolume("Music", musicSlider.value);
				break;
			case "Ambient":
				SetSliderVolume("Ambient", ambientSlider.value);
				break;
			case "Sound":
				SetSliderVolume("Sound", fxSlider.value);
				break;
		}
	}

	// SWITCHES
	public void SetSwitches()
	{
		int m = musicOn ? 1 : 0;
		musicSwitch.SetValueWithoutNotify(m);
		int a = ambientOn ? 1 : 0;
		ambientSwitch.SetValueWithoutNotify(a);
		int f = fxOn ? 1 : 0;
		fxSwitch.SetValueWithoutNotify(f);
	}
	public void SetMusicOn()
	{
		int m = (int)musicSwitch.value;
		PlayerPrefs.SetInt("MusicSwitch", m);
		musicOn = m == 0 ? false : true;
		// if music become ON, play last music file, or You can play another - (AudioManager.Instance.musicName = "AnotherMusicFile";)
		if (musicOn)
		{
			if (masterSlider.value == 0) masterSlider.value = 0.5f;
			if (musicSlider.value == 0) musicSlider.value = 0.5f;
			SaveSlidersVolume("Master");
			SaveSlidersVolume("Music");
		}
		else
		{
			musicSlider.value = 0;
			SaveSlidersVolume("Music");
		}
		SetOnOff("Music");
	}
	public void SetAmbientOn()
	{
		int a = (int)ambientSwitch.value;
		PlayerPrefs.SetInt("AmbientSwitch", a);
		ambientOn = a == 0 ? false : true;
		if (ambientOn)
		{
			if (masterSlider.value == 0) masterSlider.value = 0.5f;
			if (ambientSlider.value == 0) ambientSlider.value = 0.5f;
			SaveSlidersVolume("Master");
			SaveSlidersVolume("Ambient");
		}
		else
		{
			ambientSlider.value = 0;
			SaveSlidersVolume("Ambient");
		}
		SetOnOff("Ambient");
	}
	public void SetFxOn()
	{
		int f = (int)fxSwitch.value;
		PlayerPrefs.SetInt("SoundSwitch", f);
		fxOn = f == 0 ? false : true;
		if (fxOn)
		{
			if (masterSlider.value == 0) masterSlider.value = 0.5f;
			if (fxSlider.value == 0) fxSlider.value = 0.5f;
			SaveSlidersVolume("Master");
			SaveSlidersVolume("Sound");
		}
		else
		{
			fxSlider.value = 0;
			SaveSlidersVolume("Sound");
		}
		SetOnOff("Fx");
	}

	// TOGGLES
	public void SetToggles()
	{
		musicToggle.SetIsOnWithoutNotify(musicOn);
		ambientToggle.SetIsOnWithoutNotify(ambientOn);
		fxToggle.SetIsOnWithoutNotify(fxOn);
	}
	public void SetMusicToggle()
	{
		int boolInt = musicToggle.isOn ? 1 : 0;
		PlayerPrefs.SetInt("MusicSwitch", boolInt);
		musicOn = musicToggle.isOn;
		// if music become ON, play last music file, or You can play another - (AudioManager.Instance.musicName = "AnotherMusicFile";)
		if (musicOn)
		{
			// if sliders = 0
			if (masterSlider.value == 0) masterSlider.value = 0.5f;
			if (musicSlider.value == 0) musicSlider.value = 0.5f;
			SaveSlidersVolume("Master");
			SaveSlidersVolume("Music");
		}
		else
		{
			musicSlider.value = 0;
			SaveSlidersVolume("Music");
		}
		SetOnOff("Music");
	}
	public void SetAmbientToggle()
	{
		int boolInt = ambientToggle.isOn ? 1 : 0;
		PlayerPrefs.SetInt("AmbientSwitch", boolInt);
		ambientOn = ambientToggle.isOn;

		if (ambientOn)
		{
			if (masterSlider.value == 0) masterSlider.value = 0.5f;
			if (ambientSlider.value == 0) ambientSlider.value = 0.5f;
			SaveSlidersVolume("Master");
			SaveSlidersVolume("Ambient");
		}
		else
		{
			ambientSlider.value = 0;
			SaveSlidersVolume("Ambient");
		}
		SetOnOff("Ambient");
	}
	public void SetFxToggle()
	{
		int boolInt = fxToggle.isOn ? 1 : 0;
		PlayerPrefs.SetInt("SoundSwitch", boolInt);
		fxOn = fxToggle.isOn;
		if (fxOn)
		{
			if (masterSlider.value == 0) masterSlider.value = 0.5f;
			if (fxSlider.value == 0) fxSlider.value = 0.5f;
			SaveSlidersVolume("Master");
			SaveSlidersVolume("Sound");
		}
		else
		{
			fxSlider.value = 0;
			SaveSlidersVolume("Sound");
		}
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
		float targetVolume = musicVolume*masterVolume;
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
    // Play music with delay. 0 = No delay
    //==============================================================
    public void PlayMusic(string thisMusicName, float delay)
	{
		StopMusic();
		if (musicOn && thisMusicName != "")
		{
			musicSource.clip = musicLibrary.GetClipFromName(thisMusicName);
			musicSource.PlayDelayed(delay);
		}
		// save music name for Next Scene and for Music On/Off options
		lastMusicName.lastAudioName = musicName = thisMusicName;
		musicIsOff = false;
	}

	//==============================================================
	// Play music fade in - with custom fade duration and delay. 0 = No fade, No delay
	//==============================================================
	public IEnumerator PlayMusicFadeDelay(string thisMusicName, float duration, float delay)
	{
		if (musicOn && thisMusicName != "")
		{
			if (musicSource.isPlaying)
			{
				yield return StartCoroutine(StopMusicFade());
				StartCoroutine(PlayMusicFadeDelay(thisMusicName, duration, delay));
			}
			else
			{
				float startVolume = 0;
				float targetVolume = musicSource.volume;
				float currentTime = 0;

				musicSource.clip = musicLibrary.GetClipFromName(thisMusicName);
				yield return new WaitForSeconds(delay);
				musicSource.Play();

				while (currentTime < duration)
				{
					currentTime += Time.deltaTime;
					musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
					yield return null;
				}
				// save music name for Next Scene and for Music On/Off options
				lastMusicName.lastAudioName = musicName = thisMusicName;
				musicIsOff = false;
				yield break;
			}
		}
		// save music name for Next Scene and for Music On/Off options
		lastMusicName.lastAudioName = musicName = thisMusicName;
		musicIsOff = false;
	}

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
	// Ambient
	//==============================================================

	#region Ambient

	//==================================================================
	// Play ambient fade in - with new AudioSource
	//==================================================================

	// Create new AmbientSource Gameobject and Play ambient with default fade
	public void PlayAmbientFade(string thisName)
	{
		if (ambientOn && thisName != "") // if ambient name = Null - This is new scene with no ambient
		{
			if (ambientSource.isPlaying)
			{
				StartCoroutine(StopAndDeleteAmbientSource());
				GameObject newAmbientSource = new GameObject("Ambient source");
				AudioSource thisAmbientSource = newAmbientSource.AddComponent<AudioSource>();
				newAmbientSource.transform.parent = audios.transform;
				thisAmbientSource.loop = true;
				thisAmbientSource.playOnAwake = false;
				thisAmbientSource.volume = ambientVolume * masterVolume;
				lastAmbientName = newAmbientSource.AddComponent<AudioName>();
				StartCoroutine(PlayNewAmbientFade(thisAmbientSource, thisName));
			}
			else StartCoroutine(PlayNewAmbientFade(ambientSource, thisName));
		}
	}
	IEnumerator PlayNewAmbientFade(AudioSource thisAmbientS, string thisAmbientName)
	{
		float startVolume = 0;
		float targetVolume = ambientVolume*masterVolume;
		float currentTime = 0;

		thisAmbientS.clip = ambientLibrary.GetClipFromName(thisAmbientName);
		thisAmbientS.Play();

		while (currentTime < ambientStartFade)
		{
			currentTime += Time.deltaTime;
			thisAmbientS.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / ambientStartFade);
			yield return null;
		}
		// save music name for Next Scene and for Music On/Off options
		lastAmbientName.lastAudioName = ambientName = thisAmbientName;
		ambinetIsOff = false;
		ambientSource = thisAmbientS;
		yield break;
	}
	IEnumerator StopAndDeleteAmbientSource()
	{

		Transform ASource = audios.transform.Find("Ambient source");
		AudioSource audioS = ASource.GetComponent<AudioSource>();
		float startVolume = audioS.volume;
		float targetVolume = 0;
		float currentTime = 0;

		while (currentTime < ambientStopFade)
		{
			currentTime += Time.deltaTime;
			audioS.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / ambientStopFade);
			yield return null;
		}
		audioS.Stop();
		Destroy(ASource.gameObject);
		yield break;
	}

	//==============================================================
	// Play ambient fade in with delay
	//==============================================================

	public IEnumerator PlayAmbientDelay(string thisAmbientName, float duration, float delay)
	{
		if (ambientOn && thisAmbientName != "")
		{
			if (ambientSource.isPlaying)
			{
				yield return StartCoroutine(StopAmbientFade()); //0.7f = fade duration
				StartCoroutine(PlayAmbientDelay(thisAmbientName, duration, delay));
			}
			else
			{
				float startVolume = 0;
				float targetVolume = ambientSource.volume;
				float currentTime = 0;

				ambientSource.clip = ambientLibrary.GetClipFromName(thisAmbientName);
				yield return new WaitForSeconds(delay);
				ambientSource.Play();

				while (currentTime < duration)
				{
					currentTime += Time.deltaTime;
					ambientSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
					yield return null;
				}
				lastAmbientName.lastAudioName = ambientName = thisAmbientName;
				ambinetIsOff = false;
				yield break;
			}
		}
		lastAmbientName.lastAudioName = ambientName = thisAmbientName;
		ambinetIsOff = false;
	}

	//==============================================================
	// Stop ambient sound with no fade
	//==============================================================
	public void StopAmbient()
	{
		if (ambientSource.isPlaying)
		{
			ambientSource.Stop();
			ambientName = "";
			ambinetIsOff = true;
		}
	}

	//==============================================================
	// Stop ambient sound fade out
	//==============================================================
	public IEnumerator StopAmbientFade()
	{
		if (ambientOn && ambientSource.isPlaying)
		{
			float currentVolume = ambientSource.volume;
			float startVolume = ambientSource.volume;
			float targetVolume = 0;
			float currentTime = 0;

			while (currentTime < ambientStopFade)
			{
				currentTime += Time.deltaTime;
				ambientSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / ambientStopFade);
				yield return null;
			}
			ambientSource.Stop();
			ambientSource.volume = currentVolume;
			ambientName = "";
			ambinetIsOff = true;
			yield break;
		}
	}

    #endregion Ambient

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
			fxSource.PlayOneShot(fxSource.clip, fxVolume*masterVolume);
			StartCoroutine(DeleteSound2D(newfxSource, fxSource.clip));
		}
	}
	IEnumerator DeleteSound2D(GameObject newfxSource, AudioClip clip)
	{
		yield return new WaitForSeconds(clip.length);
		Destroy(newfxSource.gameObject);
	}
	public IEnumerator PlaySound2DLoop(string soundName, int value)
	{
		if (fxOn)
		{
			GameObject newfxSource = new GameObject("2D fx source");
			AudioSource fxSource = newfxSource.AddComponent<AudioSource>();
			newfxSource.transform.parent = audios.transform;
			newfxSource.tag = ("fx");
			fxSource.playOnAwake = false;
			fxSource.clip = soundLibrary.GetClipFromName(soundName);
			
			for (int i = 0; i < value;)
			{
				fxSource.PlayOneShot(fxSource.clip, fxVolume * masterVolume);
				yield return new WaitForSeconds(fxSource.clip.length);
				i++;
			}
			Destroy(newfxSource.gameObject);
		}
	}

    #endregion Fx

}
