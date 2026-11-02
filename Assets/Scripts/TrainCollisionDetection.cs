using NUnit.Framework.Constraints;
using UnityEngine;

public class TrainCollisionDetection : MonoBehaviour
{
    public bool isCollidingWithPlayer = false;
    public Transform trainCar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("ENTERED");
            isCollidingWithPlayer = true;;
            other.transform.SetParent(trainCar, true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isCollidingWithPlayer = false;
            Debug.Log("EXITED");
            Vector3 currentPosition = other.transform.position;
            other.transform.parent = null;
        }
    }
}
