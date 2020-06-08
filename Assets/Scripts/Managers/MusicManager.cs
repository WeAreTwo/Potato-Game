using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

public class MusicManager : MonoBehaviour
{
    // public variables -------------------------
    [Title("Audio Source for the musics")] 
    public AudioSource m_audio; // Instanc eof the audio source responsible for the music player
    [Title("Music Tracks")]
    public AudioClip m_mainTheme; // Main theme to play when no other actions in progress

    [Title("Active Track")] [ReadOnly] 
    public AudioClip m_activeTrack; // Track currently playing


    // private variables ------------------------


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Play the default theme 
        SelectTrack(m_mainTheme);
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Select the next track to play -----------------------------------------------
    private void SelectTrack(AudioClip nextTrack)
    {
        // Set the new active track
        m_activeTrack = nextTrack;
        
        // Play the new track
        m_audio.clip = m_activeTrack;
        m_audio.Play();
    }
}
