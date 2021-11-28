using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianHandler : MonoBehaviour
{
    // Active instance
    public static GuardianHandler active;

    // Hub instance
    public Hub hub;

    // Contains all active guardians in the scene
    [HideInInspector] public List<DefaultGuardian> guardians = new List<DefaultGuardian>();

    // Holds a reference to guardian button
    public GuardianButton guardianButton;
    [HideInInspector] public bool guardianSpawned;

    // Guardian animation components
    public AudioSource warningSound;
    public AudioSource music;
    public CanvasGroup UI;

    // Laser variables
    [HideInInspector] public bool laserFiring;
    [HideInInspector] public int laserPart;
    [HideInInspector] public float cooldown = 5f;

    // Get active instance
    public void Awake() { active = this; }

    // Start method
    public void Start()
    {
        Events.active.onStartGuardianBattle += StartGuardianBattle;
        Events.active.onGuardianDestroyed += GuardianDestroyed;
    }

    // Update guardians
    public void Update()
    {
        // Check if animation playnig
        if (laserFiring) GuardianAnimation();

        // Check if paused
        if (Settings.paused) return;

        // Move guardians each frame towards their target
        for (int i = 0; i < guardians.Count; i++)
        {
            if (guardians[i] != null)
            {
                if (guardians[i].target != null)
                {
                    guardians[i].MoveTowards(guardians[i].transform, guardians[i].target.transform);
                }
                else
                {
                    BaseTile building = InstantiationHandler.active.GetClosestBuilding(Vector2Int.RoundToInt(guardians[i].transform.position));

                    if (building != null)
                        guardians[i].target = building;
                }
            }
            else
            {
                guardians.RemoveAt(i);
                i--;
            }
        }
    }

    // Start animation
    public void StartGuardianBattle()
    {
        // Check to make sure there's not an active guardian
        if (guardians.Count > 0) return;

        // Disable UI
        UI.alpha = 0f;
        UI.interactable = false;
        UI.blocksRaycasts = false;

        // Initiate laser sequence 
        laserPart = 0;
        cooldown = 0.5f;
        laserFiring = true;
        music.Pause();

        // Reset all lasers
        hub.ResetLasers();
    }

    // Guardian animation sequence
    public void GuardianAnimation()
    {
        // Controls laser animation
        switch (laserPart)
        {
            case 0:
                cooldown -= Time.deltaTime;
                if (cooldown <= 0)
                {
                    cooldown = 8f;
                    warningSound.Play();
                    laserPart = 1;
                }
                break;
            case 1:
                cooldown -= Time.deltaTime;
                if (cooldown <= 0)
                {
                    cooldown = 2f;
                    laserPart = 2;
                    warningSound.Stop();
                }
                break;
            case 2:
                cooldown -= Time.deltaTime;
                if (cooldown <= 0) laserPart = 3;
                break;
            case 3:
                hub.PlayChargeParticle();
                cooldown = 2.3f;
                laserPart = 4;
                break;
            case 4:
                cooldown -= Time.deltaTime;
                if (cooldown <= 0)
                {
                    hub.FireLaser(Gamemode.stage.guardianDirection);
                    cooldown = 2.5f;
                    laserPart = 5;
                }
                break;
            case 5:
                cooldown -= Time.deltaTime;
                if (cooldown <= 0)
                {
                    Border.IncreaseBorderSize(Gamemode.stage.guardianDirection);
                    cooldown = 11f;
                    laserPart = 6;
                }
                break;
            case 6:
                cooldown -= Time.deltaTime;
                Border.PushBorder(Gamemode.stage.guardianDirection, 50f);
                if (cooldown <= 0)
                {
                    Border.SetBorderPositions();
                    laserFiring = false;
                    UI.alpha = 1f;
                    UI.interactable = true;
                    UI.blocksRaycasts = true;
                    music.Play();
                    laserPart = 0;
                    SpawnGuardian();
                }
                break;
        }
    }

    // Open guardian warning
    public void OpenGuardianWarning()
    {
        guardianButton.SetConfirmScreen(Gamemode.stage.guardian);
        guardianButton.gameObject.SetActive(true);
    }

    // Close guardian warning
    public void CloseGuardianWarning()
    {
        if(guardianButton != null)
            guardianButton.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    public void SpawnGuardian()
    {
        // Get active stage
        Stage stage = Gamemode.stage;

        // Create the tile
        GameObject lastObj = Instantiate(stage.guardian.obj.gameObject, stage.guardianSpawnPos, Quaternion.identity);
        lastObj.name = stage.guardian.name;

        // Move to next stage
        Resource.active.SetStorage(Resource.CurrencyType.Heat, stage.nextStage.heat);

        // Get guardian stuff
        DefaultGuardian guardian = lastObj.GetComponent<DefaultGuardian>();
        guardian.guardian = stage.guardian;
        guardian.Setup();

        // Add to active guardians
        guardians.Add(guardian);
    }

    // Guardian destroyed method
    public void GuardianDestroyed(DefaultGuardian guardian)
    {
        // Guardian destroyed
        Debug.Log("Guardian destroyed. Setting stage " + Gamemode.stage.variant.name + " to " + Gamemode.stage.nextStage.variant.name);

        // Begin transition to next stage
        Gamemode.stage = Gamemode.stage.nextStage;
        Events.active.ChangeBorderColor(Gamemode.stage.borderOutline, Gamemode.stage.borderFill);

        // Update unlockables
        Buildables.UpdateEntityUnlockables(Unlockable.UnlockType.DestroyGuardianAmount, guardian.guardian, 1);
    }

    // Destroys all active enemies
    public void DestroyAllGuardians()
    {
        for (int i = 0; i < guardians.Count; i++)
            Destroy(guardians[i].gameObject);
        guardians = new List<DefaultGuardian>();
    }
}
