using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clips;
    private AudioSource _audio;
    private bool _audioActive;
    private float _volume = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        _audioActive = true;
        _audio.clip = clips[Random.Range(0, clips.Count)];
        _audio.Play();
        SetVolume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (_audioActive)
            {
                _audioActive = false;
                _audio.volume = 0f;
            }
            else
            {
                _audioActive = true;
                _audio.volume = 0.25f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            ChangeVolume(false);
        }

        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            ChangeVolume(true);
        }
    }

    public void SetVolume()
    {
        _volume = PlayerPrefs.GetFloat("Volume", 0.25f);
        _audio.volume = _volume;
    }

    public void ChangeVolume(bool addVolume)
    {
        if (addVolume)
        {
            _volume += 5f;
        }
        else
        {
            _volume -= 5f;
        }

        _audio.volume = _volume;
        PlayerPrefs.SetFloat("Volume", _volume);
    }
}
