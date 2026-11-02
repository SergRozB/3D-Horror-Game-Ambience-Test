using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip firstAmbientMusic;
    [SerializeField] private AudioClip mazeAmbientMusic;
    [SerializeField] private Transform mazeAmbientMusicBegin;
    private AudioSource audioSource;
    [SerializeField] private GameObject player;
    private PlayerMovement playerScript;
    private bool phaseInStarted = false;
    private bool mazeMusicStarted = false;
    [SerializeField] private float firstAmbientTimeToPhaseIn = 120f;
    [SerializeField] private float mazeMusicTimeToPhaseIn = 120f;
    private float phaseInStartTime = 0f;
    private float mazeMusicPhaseInStartTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerScript = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!mazeMusicStarted)
        {
            HandleFirstAmbientMusic();
            float distanceToMazeMusicBegin = Vector3.Distance(player.transform.position, mazeAmbientMusicBegin.position);
            if (distanceToMazeMusicBegin <= 5f)
            {
                Debug.Log("Starting maze music");
                audioSource.clip = mazeAmbientMusic;
                audioSource.volume = 0.1f;
                audioSource.Play();
                mazeMusicStarted = true;
                mazeMusicPhaseInStartTime = Time.time;
            }
        }

        else 
        {
            audioSource.volume = phaseInMusic(mazeMusicPhaseInStartTime, mazeMusicTimeToPhaseIn) * 0.1f;
        }
    }

    private void HandleFirstAmbientMusic() 
    {
        if (!playerScript.pastOldMusicPoint)
            audioSource.volume = playerScript.percentageDistanceTo;

        else 
        {
            if (!phaseInStarted)
            {
                phaseInStarted = true;
                phaseInStartTime = Time.time;
                audioSource.clip = firstAmbientMusic;
                audioSource.volume = 0f;
                audioSource.Play();
            }
            audioSource.volume = phaseInMusic(phaseInStartTime, firstAmbientTimeToPhaseIn) * 0.1f;
        }
    }

    private float phaseInMusic(float startTime, float timeToPhaseIn) 
    {
        float timeElapsed = Time.time - startTime;
        float percentageComplete = timeElapsed / timeToPhaseIn;
        if (percentageComplete >= 1f)
        {
            return 1f;
        }
        return Mathf.Clamp01(percentageComplete);
    }

}
