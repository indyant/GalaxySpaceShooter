using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Audio;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource _audioSource;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("_audioSource is null");
        }
        else
        {
            _audioSource.Play();
        }

        Destroy(gameObject, 2.7f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
