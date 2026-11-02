using TreeEditor;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool swing = false;
    [SerializeField] private bool isOpen = false; // This bool must 
    private float closedRotation = 0;
    private float openRotation = 120;
    private float startYRot = 0;
    private bool firstTick = true;
    private float startTime = 0;
    [SerializeField] float doorSwingDuration = 1f;
    private Transform parent;
    private bool finished = false;
    [SerializeField] private AnimationCurve lerpCurve;
    [SerializeField] private AudioClip doorOpenAudio;
    [SerializeField] private AudioClip doorClosedAudio;

    private void Start()
    {
        parent = transform.parent;
        startYRot = parent.rotation.eulerAngles.y;
        doorOpenAudio = SoundFXManager.instance.doorOpeningClip;
        doorClosedAudio = SoundFXManager.instance.doorClosingClip;
    }
    private void Update()
    {
        if (swing) 
        {
            if (firstTick) 
            {
                startTime = Time.time;
                firstTick = false;

                if(!isOpen)
                {
                    SoundFXManager.instance.PlaySoundFXClip(doorOpenAudio, transform, 1);
                }
                else
                {
                    SoundFXManager.instance.PlaySoundFXClip(doorClosedAudio, transform, 1);
                }
            }

            SwingDoor();

            if (finished)
            {
                swing = false;
                isOpen = !isOpen;
                firstTick = true;
                finished = false;
            }
        }
    }

    public void SwingDoor() 
    {
        float yRotation = 0;
        float time = Time.time - startTime;
        float t = Mathf.Clamp01(time/doorSwingDuration);
        t = lerpCurve.Evaluate(t);

        if (!isOpen)
        {
            yRotation = Mathf.Lerp(closedRotation, openRotation, t);
        }

        else 
        {
            yRotation = Mathf.Lerp(openRotation, closedRotation, t);
        }

        parent.rotation = Quaternion.Euler(parent.rotation.eulerAngles.x, startYRot+yRotation, parent.rotation.eulerAngles.z);

        if (t == 1)
        {
            finished = true;
        }
    }
}
