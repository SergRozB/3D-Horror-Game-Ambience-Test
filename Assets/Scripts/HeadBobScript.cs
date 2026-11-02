using UnityEngine;

public class HeadBobScript : MonoBehaviour
{
    [Range(0.001f, 0.2f)]
    [SerializeField] private float amount = 0.002f;
    [Range(1.0f, 30.0f)]
    [SerializeField] private float frequency = 10.0f;
    [Range(10f, 100.0f)]
    [SerializeField] private float smooth = 10.0f;

    private float baseFrequency;
    private float runningFrequency;

    private Vector3 StartPos;
    private PlayerMovement playerMovementScript;

    void Start()
    {
        StartPos = transform.localPosition; 
        playerMovementScript = GetComponentInParent<PlayerMovement>();
        baseFrequency = frequency;
        runningFrequency = frequency * 2;
    }

    void Update()
    {
        if (playerMovementScript.isRunning)
        {
            frequency = runningFrequency;
        }

        else 
        {
            frequency = baseFrequency;  
        }
            CheckForHeadbobTrigger();
        StopHeadBob();
    }

    private void CheckForHeadbobTrigger() 
    {
        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;

        if (inputMagnitude > 0) 
        {
            // Trigger head bobbing
            StartHeadBob();
        }
    }

    private Vector3 StartHeadBob() 
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency / 2f) * amount * 1.6f, smooth * Time.deltaTime);
        transform.localPosition += pos;

        return pos;
    }

    private void StopHeadBob() 
    {
        if (transform.localPosition == StartPos) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, StartPos, Time.deltaTime);
    }
}
