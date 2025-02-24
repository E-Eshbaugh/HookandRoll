using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play(0);
    }
}
