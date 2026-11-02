using UnityEngine;

public class GroundCheckScript : MonoBehaviour
{
    public bool isOnGround = false;
    [SerializeField] private LayerMask layerToCheck;
    [SerializeField] private float maxRayDist = 1.0f;

    private void Update()
    {
        isOnGround = Physics.Raycast(transform.position, Vector3.down, maxRayDist, layerToCheck);
    }
}
