using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioClip _reveal;
    [SerializeField] AudioClip _flag;
    [SerializeField] AudioClip _flood;
    [SerializeField] AudioClip _victory;
    [SerializeField] AudioClip _defeat;

    void Awake()
    {
        Instance = this;
    }

    public void PlayTileClickSound()
    {
        if(_sfxSource.isPlaying) return;
        _sfxSource.PlayOneShot(_reveal);
    }

    public void PlayFlagSound()
    {
        if(_sfxSource.isPlaying) _sfxSource.Stop();
        _sfxSource.PlayOneShot(_flag);
    }

    public void PlayFloodSound()
    {
        if(_sfxSource.isPlaying) _sfxSource.Stop();
        _sfxSource.PlayOneShot(_flood);
    }

    public void PlayVictorySound()
    {
        if(_sfxSource.isPlaying) _sfxSource.Stop();
        _sfxSource.PlayOneShot(_victory);
    }

    public void PlayDefeatSound()
    {
        if(_sfxSource.isPlaying) _sfxSource.Stop();
        _sfxSource.PlayOneShot(_defeat);
    }
}