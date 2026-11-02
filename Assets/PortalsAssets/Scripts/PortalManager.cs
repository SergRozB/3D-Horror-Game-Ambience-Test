using UnityEngine;

public class PortalManager : MonoBehaviour
{
    private PortalPair[] portalPairs;
    private Portal[] portals;

    private void Awake()
    {
        portalPairs = GetComponentsInChildren<PortalPair>();
        portals = new Portal[portalPairs.Length * 2];
        foreach (PortalPair portalPair in portalPairs)
        {
            Debug.Log("Portal pair found: " + portalPair.gameObject.name);
        }

        for (int i = 0; i < portalPairs.Length; i++)
        {
            PortalPair portalPair = portalPairs[i];
            Portal[] pairPortals = portalPair.gameObject.GetComponentsInChildren<Portal>();
            if (pairPortals.Length == 2)
            {
                portals[i * 2] = pairPortals[0];
                portals[i * 2 + 1] = pairPortals[1];
            }
        }

        foreach (Portal portal in portals) 
        {
            Debug.Log("Portal found: " + portal.gameObject.name);
        }
    }

    void Update()
    {
        // Disables the portal if both portals are not visible
        for (int i = 0; i < portals.Length; i += 2)
        {
            Portal portal = portals[i];
            Portal otherPortal = portals[i + 1];
            //Debug.Log($"Checking visibility for Portal {portal.name} and Other Portal {otherPortal.name}");
            //Debug.Log($"Portal {portal.name} is visible: {portal.Renderer.isVisible}, Other Portal {otherPortal.name} is visible: {otherPortal.Renderer.isVisible}");
            /*
            if (!portal.gameObject && !otherPortal.Renderer.isVisible)
            {
                portal.enabled = false;
                otherPortal.enabled = false;
            }
            else
            {
                portal.enabled = true;
                otherPortal.enabled = true;
            }
            */
        }
    }

}
