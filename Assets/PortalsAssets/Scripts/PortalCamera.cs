using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
{
    private List<Portal[]> portals;

    public List<PortalPair> portalPairs;

    [SerializeField]
    private Camera portalCameraCopy;

    [SerializeField]
    private int iterations = 7;

    private RenderTexture[] textures;

    private Dictionary<Portal, RenderTexture> portalToTextureMap = new Dictionary<Portal, RenderTexture>();
    private Dictionary<Portal, Camera> portalToCameraMap = new Dictionary<Portal, Camera>();

    private Camera mainCamera;

    private void Awake()
    {
        portals = new List<Portal[]>();
        for (int i = 0; i < portalPairs.Count; i++)
        {
            Portal[] portalGroup = portalPairs[i].GetComponentsInChildren<Portal>();
            portals.Add(portalGroup);
        }

        //portals = new Portal[][]{portalGroup1, portalGroup2, portalGroup3, portalGroup4};
        mainCamera = GetComponent<Camera>();

        int numberOfPortals = portalPairs.Count * 2;
        textures = new RenderTexture[numberOfPortals];  
        
        for (int i = 0; i < numberOfPortals; i++) 
        {
            RenderTexture tempTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            textures[i] = tempTexture;
        }
    }

    private void Start()
    {
        //portals[0].Renderer.material.mainTexture = tempTexture1;
        //portals[1].Renderer.material.mainTexture = tempTexture2;

        int portalSubgroupLength = portals[0].Length;

        /*
        foreach (Portal[] portalGroup in portals) 
        {
            Portal portal1 = portalGroup[0];
            Portal portal2 = portalGroup[1];
            portal1.Renderer.material.mainTexture = tempTexture1;
            portal2.Renderer.material.mainTexture = tempTexture2;
        }
        */
        
        CreatePortalCameras();

        ///*

        for (int i = 0; i < portals.Count; ++i)
        {
            Portal[] portalGroup = portals[i];
            for (int j = 0; j < portalGroup.Length; ++j)
            {
                Portal portal = portalGroup[j];
                RenderTexture texture = textures[(i * portalSubgroupLength) + j];
                portal.Renderer.material.mainTexture = texture;
                portalToTextureMap.Add(portal, texture);
            }
        }
        //*/

        /*
        for (int i = 0; i < portals.Length; i++) 
        {
            Portal[] portalGroup = portals[i];
            Portal portal1 = portalGroup[0];
            Portal portal2 = portalGroup[1];
            portal1.Renderer.material.mainTexture = tempTexture1;
            portal2.Renderer.material.mainTexture = tempTexture2;
        }
        */
    }

    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

    void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        /*
        if (!portals[0].IsPlaced || !portals[1].IsPlaced)
        {
            return;
        }
        */

        foreach (Portal[] portalGroup in portals) 
        {
            Portal portal1 = portalGroup[0];
            Portal portal2 = portalGroup[1];
            Camera portalCamera = portalToCameraMap[portal1];

            if (portal1.Renderer.isVisible) 
            {
                portalCamera.targetTexture = portalToTextureMap[portal1];
                for (int i = iterations - 1; i >= 0; --i)
                {
                    RenderCamera(portal1, portal2, i, SRC);
                }
            }

            if (portal2.Renderer.isVisible)
            {
                portalCamera.targetTexture = portalToTextureMap[portal2];
                for (int i = iterations - 1; i >= 0; --i)
                {
                    RenderCamera(portal2, portal1, i, SRC);
                }
            }
        }

        /*
        if (portals[0].Renderer.isVisible)
        {
            portalCamera.targetTexture = tempTexture1;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[0], portals[1], i, SRC);
            }
        }

        if(portals[1].Renderer.isVisible)
        {
            portalCamera.targetTexture = tempTexture2;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[1], portals[0], i, SRC);
            }
        }
        */
    }

    private void CreatePortalCameras() 
    {
        // Create a camera for each portal and store it in the portalToCameraMap.
        foreach (Portal[] portalGroup in portals) 
        {
            GameObject newPortalCamera = Instantiate(portalCameraCopy.gameObject, transform.parent);
            Camera cameraComponent = newPortalCamera.GetComponent<Camera>();
            foreach (Portal portal in portalGroup) 
            {
                portalToCameraMap.Add(portal, cameraComponent);
            }
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int iterationID, ScriptableRenderContext SRC)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;
        Camera portalCamera = portalToCameraMap[inPortal];
        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.

        UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
    }
}
