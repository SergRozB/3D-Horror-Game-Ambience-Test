using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [field: SerializeField]
    public Portal OtherPortal { get; private set; }

    [field: SerializeField]
    public Color PortalColour { get; private set; }

    [SerializeField]
    private LayerMask placementMask;

    [SerializeField]
    private Transform testTransform;

    private List<PortalableObject> portalObjects = new List<PortalableObject>();
    public bool IsPlaced { get; private set; } = false;

    private Collider wallCollider;

    // Components.
    public Renderer Renderer { get; private set; }
    private new BoxCollider collider;

    public bool isTriggering = false;
    [SerializeField] private Collider wallCollidingWith = null;

    public bool willBeRendered = false;    

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        Renderer = GetComponent<Renderer>();
        if (wallCollidingWith != null)
        {
            wallCollider = wallCollidingWith;
        }
    }

    private void Update()
    {
        //Debug.Log("Name: "+transform.name+"| isplaced: "+ IsPlaced);
        Renderer.enabled = OtherPortal.IsPlaced;

        for (int i = 0; i < portalObjects.Count; ++i)
        {
            Vector3 objPos = transform.InverseTransformPoint(portalObjects[i].transform.position);
            //objPos.z += -0.5f;
            //Debug.Log("Object pos: "+objPos);
            if (objPos.z >= 0.0f)
            {
                portalObjects[i].Warp();
            }
        }
    }

    private void OnRenderObject()
    {
        willBeRendered = true;
    }

    private void LateUpdate()
    {
        willBeRendered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        //Debug.Log("Object entered portal: " + obj.name);
        isTriggering = true;
        if (obj != null)
        {
            portalObjects.Add(obj);
            if(wallCollider != null)
                obj.SetIsInPortal(this, OtherPortal, wallCollider);

            else
                obj.SetIsInPortal(this, OtherPortal);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        isTriggering = false;
        if (portalObjects.Contains(obj))
        {
            portalObjects.Remove(obj);
            if (wallCollider != null)
                obj.ExitPortal(wallCollider);
            else
                obj.ExitPortal();
        }
    }

    public bool PlacePortal(Collider wallCollider, Vector3 pos, Quaternion rot)
    {
        testTransform.position = pos;
        testTransform.rotation = rot;
        testTransform.position -= testTransform.forward * 0.001f;

        FixOverhangs();
        FixIntersects();

        //this.wallCollider = wallCollider;
        transform.position = testTransform.position;
        transform.rotation = testTransform.rotation;

        gameObject.SetActive(true);
        IsPlaced = true;
        return true;

        //return false;
    }

    // Ensure the portal cannot extend past the edge of a surface.
    private void FixOverhangs()
    {
        var testPoints = new List<Vector3>
        {
            new Vector3(-1.1f,  0.0f, 0.1f),
            new Vector3( 1.1f,  0.0f, 0.1f),
            new Vector3( 0.0f, -2.1f, 0.1f),
            new Vector3( 0.0f,  2.1f, 0.1f)
        };

        var testDirs = new List<Vector3>
        {
             Vector3.right,
            -Vector3.right,
             Vector3.up,
            -Vector3.up
        };

        for(int i = 0; i < 4; ++i)
        {
            RaycastHit hit;
            Vector3 raycastPos = testTransform.TransformPoint(testPoints[i]);
            Vector3 raycastDir = testTransform.TransformDirection(testDirs[i]);

            if(Physics.CheckSphere(raycastPos, 0.05f, placementMask))
            {
                break;
            }
            else if(Physics.Raycast(raycastPos, raycastDir, out hit, 2.1f, placementMask))
            {
                var offset = hit.point - raycastPos;
                testTransform.Translate(offset, Space.World);
            }
        }
    }

    // Ensure the portal cannot intersect a section of wall.
    private void FixIntersects()
    {
        var testDirs = new List<Vector3>
        {
             Vector3.right,
            -Vector3.right,
             Vector3.up,
            -Vector3.up
        };

        var testDists = new List<float> { 1.1f, 1.1f, 2.1f, 2.1f };

        for (int i = 0; i < 4; ++i)
        {
            RaycastHit hit;
            Vector3 raycastPos = testTransform.TransformPoint(0.0f, 0.0f, -0.1f);
            Vector3 raycastDir = testTransform.TransformDirection(testDirs[i]);

            if (Physics.Raycast(raycastPos, raycastDir, out hit, testDists[i], placementMask))
            {
                var offset = (hit.point - raycastPos);
                var newOffset = -raycastDir * (testDists[i] - offset.magnitude);
                testTransform.Translate(newOffset, Space.World);
            }
        }
    }

    // Once positioning has taken place, ensure the portal isn't intersecting anything.
    private bool CheckOverlap()
    {
        var checkExtents = new Vector3(0.9f, 1.9f, 0.05f);

        var checkPositions = new Vector3[]
        {
            testTransform.position + testTransform.TransformVector(new Vector3( 0.0f,  0.0f, -0.1f)),

            testTransform.position + testTransform.TransformVector(new Vector3(-1.0f, -2.0f, -0.1f)),
            testTransform.position + testTransform.TransformVector(new Vector3(-1.0f,  2.0f, -0.1f)),
            testTransform.position + testTransform.TransformVector(new Vector3( 1.0f, -2.0f, -0.1f)),
            testTransform.position + testTransform.TransformVector(new Vector3( 1.0f,  2.0f, -0.1f)),

            testTransform.TransformVector(new Vector3(0.0f, 0.0f, 0.2f))
        };

        // Ensure the portal does not intersect walls.
        var intersections = Physics.OverlapBox(checkPositions[0], checkExtents, testTransform.rotation, placementMask);

        if(intersections.Length > 1)
        {
            return false;
        }
        else if(intersections.Length == 1) 
        {
            // We are allowed to intersect the old portal position.
            if (intersections[0] != collider)
            {
                return false;
            }
        }

        // Ensure the portal corners overlap a surface.
        bool isOverlapping = true;

        for(int i = 1; i < checkPositions.Length - 1; ++i)
        {
            isOverlapping &= Physics.Linecast(checkPositions[i], 
                checkPositions[i] + checkPositions[checkPositions.Length - 1], placementMask);
        }

        return isOverlapping;
    }

    public void RemovePortal()
    {
        gameObject.SetActive(false);
        IsPlaced = false;
    }
}
