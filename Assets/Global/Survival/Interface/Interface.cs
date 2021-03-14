﻿using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using TMPro;

public class Interface : MonoBehaviour
{
    // Survival script
    public Survival main;

    // Interace Elements
    public GameObject ResearchOverlay;
    public GameObject EngineerOverlay;
    public Transform EngineerList;
    public GameObject EngineerCooldownOverlay;
    public TextMeshProUGUI EngineerCooldown;
    public Image EngineerIcon;
    public TextMeshProUGUI EngineerTitle;
    public TextMeshProUGUI EngineerDescription;
    public TextMeshProUGUI EngineerTime;
    public TextMeshProUGUI EngineerChance;
    public TextMeshProUGUI EngineerCost;
    public Transform[] InfoPanels;
    public Transform[] HotbarUI;
    public Canvas Overlay;
    public bool MenuOpen;
    public bool BuildingOpen;
    public bool ResearchOpen;
    public bool SettingsOpen;
    public bool EngineerOpen;
    public bool ShowingInfo;
    public TextMeshProUGUI GoldAmount;
    public TextMeshProUGUI EssenceAmount;
    public TextMeshProUGUI IridiumAmount;
    public TextMeshProUGUI GoldStorage;
    public TextMeshProUGUI EssenceStorage;
    public TextMeshProUGUI IridiumStorage;
    public TextMeshProUGUI PowerUsage;
    public TextMeshProUGUI AvailablePower;
    public TextMeshProUGUI GPS;
    public TextMeshProUGUI EPS;
    public TextMeshProUGUI IPS;
    public ModalWindowManager UOL;
    public ProgressBar PowerUsageBar;
    public ProgressBar[] UpgradeProgressBars;
    public TextMeshProUGUI UpgradeProgressName;
    public ButtonManagerBasic SaveButton;
    public ButtonManagerBasicIcon ResearchButton;
    public ButtonManagerBasicIcon[] hotbarButtons;

    // Start is called before the first frame update
    private void Start()
    {
        // Set default booleans
        MenuOpen = false;
        ResearchOpen = false;
        BuildingOpen = false;
    }

    // Set the engineer cooldown
    public void SetEngineerTimer(string a)
    {
        EngineerCooldown.text = a;
    }

    // Activates an engineers UI panel
    public void OpenEngineer(bool isActive)
    {
        EngineerCooldownOverlay.SetActive(isActive);
        EngineerOverlay.SetActive(true);
        EngineerOpen = true;
    }

    // Deactivates an engineers UI panel
    public void CloseEngineer()
    {
        EngineerOverlay.SetActive(false);
        EngineerOpen = false;
    }

    // Enable cooling overlay
    public void EnableCooldown()
    {
        EngineerCooldownOverlay.SetActive(true);

    }

    // Disable cooling overlay
    public void DisableCooldown()
    {
        EngineerCooldownOverlay.SetActive(false);
    }

    // Set the status of an overlay. 
    // a = name of the overlay
    // b = activate or deactive overlay
    public void SetOverlayStatus(string a, bool b)
    {
        if (b)
        {
            Overlay.transform.Find(a).GetComponent<CanvasGroup>().alpha = 1;
            Overlay.transform.Find(a).GetComponent<CanvasGroup>().interactable = true;
            Overlay.transform.Find(a).GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            Overlay.transform.Find(a).GetComponent<CanvasGroup>().alpha = 0;
            Overlay.transform.Find(a).GetComponent<CanvasGroup>().interactable = false;
            Overlay.transform.Find(a).GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    // plz dont ask me how this works :(
    public void UpdateInventoryInfo(Transform obj)
    {
        foreach (Transform panel in InfoPanels)
        {
            if (panel.parent.parent.GetComponent<CanvasGroup>().alpha == 1f)
            {
                if (obj == null)
                {
                    panel.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Undiscovered");
                    panel.Find("Name").GetComponent<TextMeshProUGUI>().text = "???";
                    panel.Find("Description").GetComponent<TextMeshProUGUI>().text = "???";
                    panel.Find("Gold").GetComponent<TextMeshProUGUI>().text = "???";
                    panel.Find("Heat").GetComponent<TextMeshProUGUI>().text = "???";
                    panel.Find("Power").GetComponent<TextMeshProUGUI>().text = "???";
                    break;
                }
                else
                {
                    panel.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + obj.name);
                    if (obj.name.Contains("Storage"))
                        panel.Find("Name").GetComponent<TextMeshProUGUI>().text = "STORAGE";
                    else
                        panel.Find("Name").GetComponent<TextMeshProUGUI>().text = obj.name.ToUpper();
                    panel.Find("Description").GetComponent<TextMeshProUGUI>().text = obj.GetComponent<TileClass>().GetDescription();
                    panel.Find("Gold").GetComponent<TextMeshProUGUI>().text = obj.GetComponent<TileClass>().GetCost().ToString();
                    panel.Find("Heat").GetComponent<TextMeshProUGUI>().text = obj.GetComponent<TileClass>().GetHeat().ToString();
                    panel.Find("Power").GetComponent<TextMeshProUGUI>().text = obj.GetComponent<TileClass>().getConsumption().ToString();
                    break;
                }
            }
        }
    }

    public void ShowTileInfo(Collider2D a)
    {
        // TODO: Fix this bullshit
        Transform b = Overlay.transform.Find("Prompt");
        TileClass c = a.GetComponent<TileClass>();
        b.transform.Find("Health").GetComponent<ProgressBar>().currentPercent = c.GetPercentage();
        b.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = a.name;
        b.transform.Find("Building").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + a.name);
        b.transform.Find("Gold Amount").GetComponent<TextMeshProUGUI>().text = (c.GetCost() - c.GetCost()/5).ToString();
        b.transform.Find("Power Amount").GetComponent<TextMeshProUGUI>().text = c.getConsumption().ToString();
        b.transform.Find("Heat Amount").GetComponent<TextMeshProUGUI>().text = c.GetHeat().ToString();
    }

    public void ShowSelectedInfo(Transform a)
    {
        Overlay.transform.Find("Selected").GetComponent<CanvasGroup>().alpha = 1;
        Transform b = Overlay.transform.Find("Selected");
        b.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = a.name;
        b.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = a.GetComponent<TileClass>().GetCost().ToString();
        b.transform.Find("Heat").GetComponent<TextMeshProUGUI>().text = a.GetComponent<TileClass>().GetHeat().ToString();
        b.transform.Find("Power").GetComponent<TextMeshProUGUI>().text = a.GetComponent<TileClass>().getConsumption().ToString();
        b.transform.Find("Building").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + a.name);
    }

    public void SetSelectedHotbar(int index)
    {
        HotbarUI[index].GetComponent<Button>().interactable = false;
    }

    // why, why is u not wokr
    public void UpdateHotbar()
    {
        for (int i = 0; i < main.hotbar.Length; i++)
        {
            if (main.hotbar[i] != null)
            {
                hotbarButtons[i].buttonIcon = Resources.Load<Sprite>("Sprites/" + main.hotbar[i].name);
            }
            else
            {
                hotbarButtons[i].buttonIcon = Resources.Load<Sprite>("Sprites/Undiscovered");
            }
            hotbarButtons[i].UpdateUI();
        }
    }

    // Disables active information
    public void DisableActiveInfo()
    {
        for (int i = 0; i < 9; i++)
            HotbarUI[i].GetComponent<Button>().interactable = true;
    }

    // Opens research overlay and pauses game
    public void OpenResearchOverlay()
    {
        if (!Research.research_unlocked) return;
        ResearchOpen = true;
        SetOverlayStatus("Research UI", true);
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
    }

    // Closes research overlay and unpauses game
    public void CloseResearchOverlay()
    {
        if (!Research.research_unlocked) return;
        ResearchOpen = false;
        SetOverlayStatus("Research UI", false);
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
    }
}
