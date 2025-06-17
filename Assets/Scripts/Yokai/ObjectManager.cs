using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private float objectLifespan = 20;
    [SerializeField] private List<AudioClip> collisionAudioClips = new();

    private List<Collider> colliders = new();
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        colliders.AddRange(GetComponentsInChildren<Collider>());
        StartCoroutine(DeleteObject(objectLifespan));
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioClip clip = collisionAudioClips[Random.Range(0, collisionAudioClips.Count)];
        audioSource.PlayOneShot(clip);
    }

    private IEnumerator DeleteObject(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject, seconds);
    }
}
