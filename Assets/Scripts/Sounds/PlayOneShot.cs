using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayOneShot : MonoBehaviour
{
    private AudioSource audioSource;

    public void Play(AudioClip clip) {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clip);
    }

    //when audioSource is done playing, destroy self
    private void Update() {
        if (!audioSource.isPlaying) {
            Destroy(gameObject);
        }
    }
}
