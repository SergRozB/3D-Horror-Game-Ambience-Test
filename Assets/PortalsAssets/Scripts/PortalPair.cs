using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPair : MonoBehaviour
{
    public Portal[] Portals;
    [SerializeField] private Collider wallCollider;

    private void Awake()
    {
        Portals = GetComponentsInChildren<Portal>();

        if(Portals.Length != 2)
        {
            Debug.LogError("PortalPair children must contain exactly two Portal components in total.");
        }

    }

    private void Start()
    {
        foreach (Portal portal in Portals)
        {
            portal.PlacePortal(wallCollider, portal.transform.position, portal.transform.rotation);
        }
    }
}
