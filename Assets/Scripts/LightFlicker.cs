using System;
using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float minFlickerSpeed = 0.05f;
    [SerializeField] private float maxFlickerSpeed = 0.2f;
    [SerializeField] private Transform eventTrigger;
    private EventTriggerScript eventTriggerScript;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject waysOut;
    [SerializeField] private GameObject firstArea;
    private Light[] lights;
    private bool coroutineEnded = false;
    private bool coroutineStarted = false;
    [SerializeField] private AudioClip lightsFlickering;
    private AudioSource audioSource;

    private void Awake()
    {
        lights = GetComponentsInChildren<Light>();
        eventTriggerScript = eventTrigger.GetComponent<EventTriggerScript>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (eventTriggerScript.collided && !coroutineEnded && !coroutineStarted)
        {
            StartCoroutine(flicker());
        }

        if (coroutineEnded)
        {
            StopCoroutine(flicker());
            Destroy(this);
        }
    }

    private void turnOffLights()
    {
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }

    private void turnOnLights()
    {
        foreach (Light light in lights)
        {
            light.enabled = true;
        }
    }

    IEnumerator flicker() 
    {
        coroutineStarted = true;
        audioSource.clip = lightsFlickering;
        audioSource.Play();
        turnOffLights();
        yield return new WaitForSeconds(0.05f);
        turnOnLights();
        yield return new WaitForSeconds(0.05f);
        turnOffLights();
        yield return new WaitForSeconds(0.05f);
        turnOnLights();
        yield return new WaitForSeconds(0.05f);
        turnOffLights();
        yield return new WaitForSeconds(0.05f);
        turnOnLights();
        yield return new WaitForSeconds(0.05f);
        turnOffLights();
        yield return new WaitForSeconds(0.05f);
        turnOnLights();
        yield return new WaitForSeconds(0.05f);
        turnOffLights();
        yield return new WaitForSeconds(0.1f);
        turnOnLights();
        yield return new WaitForSeconds(0.15f);
        turnOffLights();
        audioSource.Stop();
        Destroy(waysOut);
        wall.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        audioSource.Play();
        yield return new WaitForSeconds(0.2f);
        audioSource.Stop();
        turnOnLights(); 
        coroutineEnded = true;
    }
}
