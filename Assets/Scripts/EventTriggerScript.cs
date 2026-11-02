using System.Collections.Generic;
using UnityEngine;

public class EventTriggerScript : MonoBehaviour
{
    public bool collided = false;
    public bool collidedThisFrame = false;
    public bool currentlyColliding = false;
    public string tagToCollideWith = "Player";
    public bool enteredZone = false;
    public List<EntityId> entitiesPastTrigger = new List<EntityId>();

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision Detected");
        //Debug.Log(other.tag);
        if (other.CompareTag(tagToCollideWith))
        {
            collidedThisFrame = true;
            entitiesPastTrigger.Add(other.gameObject.GetEntityId());
            collided = true;
            currentlyColliding = true;
            enteredZone = !enteredZone;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(tagToCollideWith))
        {
            currentlyColliding = false;
            entitiesPastTrigger.Remove(other.gameObject.GetEntityId());
        }
    }
}
