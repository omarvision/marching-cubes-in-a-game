using UnityEngine;

public static class Globals 
{
    public static AudioSource CreateAudioSource(GameObject go, string filenamenoextension)
    {
        AudioSource audio = go.AddComponent<AudioSource>();
        audio.clip = Resources.Load<AudioClip>(filenamenoextension);
        return audio;
    }
}
