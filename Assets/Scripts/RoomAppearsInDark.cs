using UnityEngine;

public class RoomAppearsInDark : MonoBehaviour
{
    [SerializeField] private GameObject[] dissapearingWalls;
    [SerializeField] private GameObject[] appearingWalls;
    [SerializeField] private GameObject trigger;
    [SerializeField] private bool flipFlop = false;
    public bool currentlyTriggered = false;
    public string scriptName = "Room Appears In Dark";
    private EventTriggerScript eventTriggerScript;

    void Awake()
    {
        eventTriggerScript = trigger.GetComponent<EventTriggerScript>();
    }

    void Update()
    {
        if(eventTriggerScript.collidedThisFrame)
        {
            if (flipFlop && currentlyTriggered)
            {
                currentlyTriggered = false;
                foreach (GameObject wall in dissapearingWalls)
                    wall.SetActive(true);
                foreach (GameObject wall in appearingWalls)
                    wall.SetActive(false);
            }

            else if (flipFlop && !currentlyTriggered)
            {
                currentlyTriggered = true;
                foreach (GameObject wall in dissapearingWalls)
                    wall.SetActive(false);
                foreach (GameObject wall in appearingWalls)
                    wall.SetActive(true);
            }

            else if (!flipFlop)
            {
                foreach (GameObject wall in dissapearingWalls)
                    wall.SetActive(false);
                foreach (GameObject wall in appearingWalls)
                    wall.SetActive(true);
            }

            eventTriggerScript.collidedThisFrame = false;
        }
    }
}
