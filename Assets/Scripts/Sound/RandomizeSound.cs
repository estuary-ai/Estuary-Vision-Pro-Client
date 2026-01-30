using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSound : MonoBehaviour {

    [SerializeField] private AudioClip[] sounds;
    public bool playOnAwake;

    [SerializeField] private AudioSource audioSource;

	// Use this for initialization
	void Start () {
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
        if (playOnAwake)
        {
            PlaySound();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySound()
    {
        audioSource.PlayOneShot(sounds[Random.Range(0,sounds.Length)]);
    }
}
