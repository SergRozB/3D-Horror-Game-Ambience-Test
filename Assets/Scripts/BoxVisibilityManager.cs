using UnityEngine;
using UnityEngine.UIElements;

public class BoxVisibilityManager : MonoBehaviour
{
    [SerializeField] private GameObject[] boxesToToggle;
    [SerializeField] private GameObject detectIfItemInRoomObject;
    private EventTriggerScript checkIfBoxInRoomScript;
    private RoomAppearsInDark roomTriggerScript;
    //private bool currentlyActive = false;

    void Start()
    {
        checkIfBoxInRoomScript = detectIfItemInRoomObject.GetComponent<EventTriggerScript>();
        RoomAppearsInDark[] roomScripts = GetComponents<RoomAppearsInDark>();
        foreach (RoomAppearsInDark script in roomScripts)
        {
            if (script.scriptName == "First Illusion")
            {
                roomTriggerScript = script;
                break;
            }
        }
    }

    void Update()
    {
        foreach (GameObject box in boxesToToggle)
        {
            EventTriggerScript boxTriggerScript = box.GetComponent<EventTriggerScript>();
            bool currentlyColliding = false;

            if (checkIfBoxInRoomScript.entitiesPastTrigger.Contains(box.GetEntityId())) 
            {
                currentlyColliding = true;
            }
            
            if (!boxTriggerScript.enteredZone && currentlyColliding && !roomTriggerScript.currentlyTriggered)
            {
                box.SetActive(false);
            }

            else 
            {
                box.SetActive(true);
            }
        }
    }
}
