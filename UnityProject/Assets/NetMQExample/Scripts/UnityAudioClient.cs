using UnityEngine;

public class UnityAudioClient : MonoBehaviour
{
    private AudioRequester _audioRequester;

    private void Start()
    {   
        _audioRequester = new AudioRequester();
        _audioRequester.Start();
    }

    private void OnDestroy()
    {
        _audioRequester.Stop();
    }
}