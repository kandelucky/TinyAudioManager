using System.Collections.Generic;
using UnityEngine;

public class AmbLibrary : MonoBehaviour
{
	public AmbientGroup[] ambientGroups;

	Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();

	void Awake()
	{
		foreach (AmbientGroup ambientGroups in ambientGroups)
		{
			groupDictionary.Add(ambientGroups.name, ambientGroups.clip);
		}
	}

	public AudioClip GetClipFromName(string name)
	{
		if (groupDictionary.ContainsKey(name))
		{
			AudioClip[] ambient = groupDictionary[name];
			return ambient[Random.Range(0, ambient.Length)];
		}
		return null;
	}

	[System.Serializable]
	public class AmbientGroup
	{
		public string name;
		public AudioClip[] clip;
	}
}
