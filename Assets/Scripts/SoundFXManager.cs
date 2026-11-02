using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;
    public AudioClip doorOpeningClip;
    public AudioClip doorClosingClip;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1f, float inputClipLength = 0, float maxDistance = 0)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        if (maxDistance != 0) 
        {
            audioSource.spatialBlend = 1;
            audioSource.maxDistance = maxDistance;
            //audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }

        float clipLength = 0;
        if (inputClipLength == 0)
        {
            clipLength = audioSource.clip.length;
        }
        else
        {
            clipLength = inputClipLength;
        }
            Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume = 1f)
    {
        int rand = Random.Range(0, audioClips.Length);
        AudioClip audioClip = audioClips[rand];
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
