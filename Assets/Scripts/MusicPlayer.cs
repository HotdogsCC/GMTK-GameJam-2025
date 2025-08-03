using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private int pointsNeededForDoubleSpeed;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource snapSource;

    private void Start()
    {
        UpdateSpeed(0);
    }

    public void UpdateSpeed(int points)
    {
        if (points > pointsNeededForDoubleSpeed)
        {
            points = pointsNeededForDoubleSpeed;
        }

        float speed = (float)points / (float)pointsNeededForDoubleSpeed + 1.0f;

        float pitch = 1 / speed;

        musicSource.pitch = speed;
        musicSource.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", pitch);


    }

    public void PlaySnap()
    {
        snapSource.Play();
    }
}
