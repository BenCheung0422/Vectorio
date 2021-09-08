using System.Collections.Generic;
using UnityEngine;

[HideInInspector]
public class BaseTurret : BaseBuilding, IAudible
{
    // IAudible interface variables
    public AudioClip sound { get; set; }

    // Sets stats and registers itself under the turret handler
    public void Start()
    {
        SetBuildingStats();
    }

    // IAudible sound method
    public void PlaySound()
    {
        float audioScale = CameraScroll.getZoom() / 1400f;
        AudioSource.PlayClipAtPoint(sound, gameObject.transform.position, Settings.soundVolume - audioScale);
    }
}
