using UnityEngine;
// Para usar los audio mixers
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    /// <summary>
    /// Configura el volumen del canal Sounds.
    /// </summary>
    /// <param name="volume"></param>
    public void SetSound(float volume)
    {
        audioMixer.SetFloat("Sounds", Mathf.Log10(volume) * 20);
    }
    /// <summary>
    /// Configura el volumen del canal Volume.
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }
}