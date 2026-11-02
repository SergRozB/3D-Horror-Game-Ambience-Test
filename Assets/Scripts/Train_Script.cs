using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Train_Script : MonoBehaviour
{
    [SerializeField] private GameObject firstStop;
    [SerializeField] private GameObject secondStop;
    public bool playerOnTrain = false;
    public bool hasArrivedAtFirstStop = false;
    public bool hasArrivedAtSecondStop = false;
    public bool trainMoving = true;
    [SerializeField] private bool hasStartedSecondStop = false;
    [SerializeField] private float timeToArriveFirstStop;
    [SerializeField] private float timeToArriveSecondStop;
    private float currentTimeToArrive => hasArrivedAtFirstStop ? timeToArriveSecondStop : timeToArriveFirstStop;
    private float startTime;
    private Vector3 startPosition;
    [SerializeField] private TrainCollisionDetection collisionDetection;
    [SerializeField] private AudioClip startAudioClip;
    [SerializeField] private AudioClip movingAudioClip;
    [SerializeField] private AudioClip arrivingAudioClip;
    private bool hasStartedSoundCoroutine = false;
    [SerializeField] private AnimationCurve firstLerpCurve;
    [SerializeField] private AnimationCurve secondLerpCurve;
    private AudioSource trainAudioSource;
    private moveDoorsScript moveDoorsScript;

    void Start()
    {
        //collisionDetection = GetComponentInChildren<TrainCollisionDetection>();
        startTime = Time.realtimeSinceStartup;
        startPosition = transform.position;
        trainAudioSource = GetComponent<AudioSource>();
        moveDoorsScript = GetComponent<moveDoorsScript>();
    }

    void Update()
    {
        playerOnTrain = collisionDetection.isCollidingWithPlayer;
        //Debug.Log(Time.realtimeSinceStartup);
        //Debug.Log(howFarAlong);
        //Debug.Log(playerOnTrain);
        //Debug.Log("Doors Closed:" + moveDoorsScript.doorsClosed);
        if (!hasArrivedAtFirstStop) 
        {
            if (!hasStartedSoundCoroutine)
                StartCoroutine(StartTrainAudio());
            trainMoving = true;
            GoToFirstStop();
        }
        else if (playerOnTrain && !hasArrivedAtSecondStop && moveDoorsScript.doorsClosed) 
        {
            if (!hasStartedSecondStop)
            {
                startPosition = transform.position;
                startTime = Time.realtimeSinceStartup;
                //Debug.Log("Happened");
            }
            if (!hasStartedSoundCoroutine)
                StartCoroutine(StartTrainAudio());
            trainMoving = true;
            GoToSecondStop();  
            hasStartedSecondStop = true;
        }
    }

    private void GoToFirstStop()
    {
        if (!hasArrivedAtFirstStop)
        {
            if (Mathf.Abs(transform.position.z - firstStop.transform.position.z) > 1)
            {
                float time = Time.realtimeSinceStartup - startTime;
                float t = time / timeToArriveFirstStop;
                //t = t * t * t * (t * (6f * t - 15f) + 10f);
                t = firstLerpCurve.Evaluate(t);
                //Debug.Log("T:" + t);
                transform.position = new Vector3 (transform.position.x, transform.position.y, Mathf.Lerp(startPosition.z, firstStop.transform.position.z, t));
            }
            else
            {
                hasArrivedAtFirstStop = true;
                trainMoving = false;
            }
        }
    }

    private void GoToSecondStop()
    {
        if (!hasArrivedAtSecondStop)
        {
            if (Mathf.Abs(transform.position.z - secondStop.transform.position.z) > 1)
            {
                float time = Time.realtimeSinceStartup - startTime;
                float t = time / timeToArriveSecondStop;
                //t = t * t * t * (t * (6f * t - 15f) + 10f);
                t = secondLerpCurve.Evaluate(t);
                //Debug.Log("T:" + t);
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(startPosition.z, secondStop.transform.position.z, t));
            }
            else
            {
                hasArrivedAtSecondStop = true;
                trainMoving = false;
            }
        }
    }

    private void PlaySoundFXClip(AudioClip audioClip)
    {
        trainAudioSource.clip = audioClip;
        trainAudioSource.Play();
    }

    IEnumerator StartTrainAudio()
    {
        hasStartedSoundCoroutine = true;
        PlaySoundFXClip(startAudioClip);
        yield return new WaitForSeconds(startAudioClip.length*0.7f);
        float currentTime = Time.realtimeSinceStartup;
        float arrivalTime = startTime + currentTimeToArrive;
        float timeLeftBeforeArrivalAudio = arrivalTime - currentTime - arrivingAudioClip.length;
        trainAudioSource.loop = true;
        PlaySoundFXClip(movingAudioClip);
        yield return new WaitForSeconds (timeLeftBeforeArrivalAudio);
        trainAudioSource.loop = false;
        PlaySoundFXClip(arrivingAudioClip);  
        yield return new WaitForSeconds(arrivingAudioClip.length);
        hasStartedSoundCoroutine = false;
    }
}
