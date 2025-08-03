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
    [SerializeField] private AudioSource pickUpSource;
    [SerializeField] private AudioSource stationCreatedSource;
    [SerializeField] private AudioSource trainSpawnSource;
    [SerializeField] private AudioSource dropOffSource;

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

    public void PlayPickUp()
    {
        pickUpSource.Play();
    }

    public void PlayStationCreated()
    {
        stationCreatedSource.Play();
    }

    public void PlayTrainSpawn()
    {
        trainSpawnSource.Play();
    }

    public void PlayPassengerDropOff()
    {
        dropOffSource.Play();
    }
}
