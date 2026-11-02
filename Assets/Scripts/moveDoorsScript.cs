using System.Collections.Generic;
using UnityEngine;

public class moveDoorsScript : MonoBehaviour
{
    [SerializeField] private GameObject[] leftDoors;
    [SerializeField] private GameObject[] rightDoors;
    private Train_Script trainScript;
    private float moveForwardValue = 0.2f;
    private float moveSidewaysValue = 2f;
    Dictionary<GameObject, Vector3> leftDoorOriginalPositions = new Dictionary<GameObject, Vector3>();
    Dictionary<GameObject, Vector3> rightDoorOriginalPositions = new Dictionary<GameObject, Vector3>();
    Dictionary<GameObject, Vector3> leftDoorOpenPositions = new Dictionary<GameObject, Vector3>();
    Dictionary<GameObject, Vector3> rightDoorOpenPositions = new Dictionary<GameObject, Vector3>();
    //Dictionary<GameObject, float[]> leftDoorPo
    private bool firstInstance = false;
    private float startTime = 0f;
    private int timeToOpenAndClose = 4;
    private float timeElapsed = 0f;
    [SerializeField] AnimationCurve lerpCurve;
    [SerializeField] private bool doorsOpened = false;
    public bool doorsClosed = false;

    private void Start()
    {
        trainScript = GetComponent<Train_Script>();

        foreach (GameObject door in leftDoors)
        {
            leftDoorOriginalPositions[door] = door.transform.localPosition;
            float forwardX = door.transform.position.x + moveForwardValue;
            float leftZ = door.transform.position.z - moveSidewaysValue;
            leftDoorOpenPositions[door] = door.transform.parent.InverseTransformPoint(new Vector3(forwardX, door.transform.position.y, leftZ));
        }

        foreach(GameObject door in rightDoors)
        {
            rightDoorOriginalPositions[door] = door.transform.localPosition;
            float forwardX = door.transform.position.x + moveForwardValue;
            float rightZ = door.transform.position.z + moveSidewaysValue;
            rightDoorOpenPositions[door] = door.transform.parent.InverseTransformPoint(new Vector3(forwardX, door.transform.position.y, rightZ));
        }
    }

    void Update()
    {
        if (!trainScript.trainMoving)
        {
            if (trainScript.hasArrivedAtFirstStop && !trainScript.hasArrivedAtSecondStop)
            {
                if (doorsOpened && trainScript.playerOnTrain)
                {
                    CloseDoors();
                }

                else if (!doorsOpened)
                {
                    OpenDoors();
                }
            }

            else if (trainScript.hasArrivedAtSecondStop)
            {
                if (!doorsOpened)
                {
                    OpenDoors();
                }
            }
        }    
    }

    void OpenDoors() 
    {
        if (!firstInstance)
        {
            firstInstance = true;
            startTime = Time.realtimeSinceStartup;
        }

        timeElapsed = Time.realtimeSinceStartup - startTime;
        float t = timeElapsed / timeToOpenAndClose;
        t = lerpCurve.Evaluate(t);

        foreach (GameObject door in leftDoors)
        {
            Vector3 startPosition = leftDoorOriginalPositions[door];
            Vector3 endPosition = leftDoorOpenPositions[door];
            door.transform.localPosition = new Vector3(Mathf.Lerp(startPosition.x, endPosition.x, t), door.transform.localPosition.y, Mathf.Lerp(startPosition.z, endPosition.z, t));
        }

        foreach (GameObject door in rightDoors)
        {
            Vector3 startPosition = rightDoorOriginalPositions[door];
            Vector3 endPosition = rightDoorOpenPositions[door];
            door.transform.localPosition = new Vector3(Mathf.Lerp(startPosition.x, endPosition.x, t), door.transform.localPosition.y, Mathf.Lerp(startPosition.z, endPosition.z, t));
        }

        if (timeElapsed > timeToOpenAndClose) 
        {
            doorsOpened = true;
            doorsClosed = false;
            firstInstance = false;
        }
    }

    void CloseDoors() 
    {
        if (!firstInstance)
        {
            firstInstance = true;
            Debug.Log("Doors Moving");
            startTime = Time.realtimeSinceStartup;
        }

        timeElapsed = Time.realtimeSinceStartup - startTime;
        float t = timeElapsed / timeToOpenAndClose;
        t = lerpCurve.Evaluate(t);

        foreach (GameObject door in leftDoors)
        {
            Vector3 endPosition = leftDoorOriginalPositions[door];
            Vector3 startPosition = leftDoorOpenPositions[door];
            door.transform.localPosition = new Vector3(Mathf.Lerp(startPosition.x, endPosition.x, t), door.transform.localPosition.y, Mathf.Lerp(startPosition.z, endPosition.z, t));
        }

        foreach (GameObject door in rightDoors)
        {
            Vector3 endPosition = rightDoorOriginalPositions[door];
            Vector3 startPosition = rightDoorOpenPositions[door];
            door.transform.localPosition = new Vector3(Mathf.Lerp(startPosition.x, endPosition.x, t), door.transform.localPosition.y, Mathf.Lerp(startPosition.z, endPosition.z, t));
        }

        if (timeElapsed > timeToOpenAndClose)
        {
            doorsOpened = false;
            doorsClosed = true;
            firstInstance = false;
        }
    }
}
