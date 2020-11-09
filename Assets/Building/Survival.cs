﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using TMPro;

public class Survival : MonoBehaviour
{

    // Player stats
    public int gold = 100;
    protected int essence = 0;
    protected int iridium = 0;

    // Placement sprites
    private SpriteRenderer Selected;
    private float Adjustment = 1f;
    private int AdjustLimiter = 0;
    private bool AdjustSwitch = false;

    // Object placements
    [SerializeField] private GameObject SniperObj;
    [SerializeField] private GameObject TurretObj;
    [SerializeField] private GameObject BoltObj;
    [SerializeField] private GameObject ShotgunObj;
    [SerializeField] private GameObject WallObj;
    [SerializeField] private GameObject SMGObj;
    [SerializeField] private GameObject CollectorObj;

    // Object variables
    private GameObject SelectedObj;
    private GameObject LastObj;
    private float rotation = 0f;

    // UI Elements
    public Canvas Overlay;
    private bool MenuOpen;
    private bool BuildingOpen;
    public TextMeshProUGUI GoldAmount;

    // Internal placement variables
    [SerializeField]
    private LayerMask TileLayer;
    private Vector2 MousePos;
    delegate void HotbarItem();
    protected float distance = 10;
    List<HotbarItem> hotbar = new List<HotbarItem>();
    List<GameObject> unlocked = new List<GameObject>();

    private void Start()
    {
        Selected = GetComponent<SpriteRenderer>();
        MenuOpen = false;
        BuildingOpen = false;

        // Temporary
        hotbar.Add(SetTurret);
        hotbar.Add(SetWall);
        hotbar.Add(SetCollector);
        unlocked.Add(TurretObj);
        unlocked.Add(WallObj);
        unlocked.Add(CollectorObj);

        InvokeRepeating("UpdateGui", 0f, 1f);
    }

    private void Update()
    {
        // Get mouse position and round to middle grid coordinate
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(5*Mathf.Round(MousePos.x/5), 5*Mathf.Round(MousePos.y/5));

        // Make color flash
        Color tmp = this.GetComponent<SpriteRenderer>().color;
        tmp.a = Adjustment;
        this.GetComponent<SpriteRenderer>().color = tmp;
        AdjustAlphaValue();

        if (Input.mousePosition.y > 170f)
        {
            //if (SelectedObj == null)
            //{
            //    if (CurrentCache >= CacheAmount)
            //    {
            //        RaycastHit2D rayHit = Physics2D.Raycast(MousePos, Vector2.zero, Mathf.Infinity, TileLayer);
            //        CurrentCache = 0;
            //        if (rayHit.collider != null)
            //        {
            //            ShowTileInfo(rayHit.collider);
            //       } 
            //        else
            //        {
            //            Overlay.transform.Find("Hovering Stats").GetComponent<CanvasGroup>().alpha = 0;
            //        }
            //    }
            //   CurrentCache += 1;
            //}
            // If user left clicks, place object
            if (Input.GetButton("Fire1") && !BuildingOpen && transform.position.x <= distance && transform.position.y <= distance)
            {
                //Overlay.transform.Find("Hovering Stats").GetComponent<CanvasGroup>().alpha = 0;
                RaycastHit2D rayHit = Physics2D.Raycast(MousePos, Vector2.zero, Mathf.Infinity, TileLayer);

                // Raycast tile to see if there is already a tile placed
                if (rayHit.collider == null && SelectedObj != CollectorObj)
                {
                    int cost = SelectedObj.GetComponent<TileClass>().GetCost();
                    if (cost <= gold)
                    {
                        gold -= cost;
                        UpdateGui();
                        if (SelectedObj == WallObj)
                        {
                            LastObj = Instantiate(SelectedObj, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                        }
                        else
                        {
                            LastObj = Instantiate(SelectedObj, transform.position, Quaternion.Euler(new Vector3(0, 0, rotation)));
                        }
                        LastObj.name = SelectedObj.name;
                        float dist = Vector2.Distance(LastObj.transform.position, GameObject.Find("Hub").GetComponent<Transform>().position);
                        if(dist > distance)
                        {
                            distance = dist;
                        }
                    }
                } 
                else
                {
                    if (rayHit.collider.name.ToLower() == "goldtile") {
                        LastObj = Instantiate(SelectedObj, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                        LastObj.name = SelectedObj.name;
                    }
                }
            }
            // If user right clicks, place object
            else if (Input.GetButton("Fire2") && !BuildingOpen)
            {
                //Overlay.transform.Find("Hovering Stats").GetComponent<CanvasGroup>().alpha = 0;
                RaycastHit2D rayHit = Physics2D.Raycast(MousePos, Vector2.zero, Mathf.Infinity, TileLayer);

                // Raycast tile to see if there is already a tile placed
                if (rayHit.collider != null && rayHit.collider.name != "Hub")
                {
                    int cost = rayHit.collider.GetComponent<TileClass>().GetCost();
                    gold += cost - cost / 5;
                    UpdateGui();
                    Destroy(rayHit.collider.gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SelectHotbar(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SelectHotbar(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SelectHotbar(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SelectHotbar(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            SelectHotbar(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            SelectHotbar(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            SelectHotbar(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            SelectHotbar(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            SelectHotbar(8);
        }
        else if (Input.GetKeyDown(KeyCode.R) && BuildingOpen == false && MenuOpen == false && SelectedObj != null)
        {
            rotation = rotation -= 90f;
            if (rotation == -360f)
            {
                rotation = 0;
            }
            Selected.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
        }
        if (Input.GetKeyDown(KeyCode.E) && BuildingOpen == false)
        {
            BuildingOpen = true;
            Overlay.transform.Find("Survival Menu").GetComponent<CanvasGroup>().alpha = 1;
            Overlay.transform.Find("Survival Menu").GetComponent<CanvasGroup>().interactable = true;
            Overlay.transform.Find("Survival Menu").GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) && BuildingOpen == true)
        {
            BuildingOpen = false;
            Overlay.transform.Find("Survival Menu").GetComponent<CanvasGroup>().alpha = 0;
            Overlay.transform.Find("Survival Menu").GetComponent<CanvasGroup>().interactable = false;
            Overlay.transform.Find("Survival Menu").GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && SelectedObj != null)
        {
            Selected.sprite = null;
            SelectedObj = null;
            rotation = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && MenuOpen == false)
        {
            MenuOpen = true;
            Overlay.transform.Find("Return").GetComponent<CanvasGroup>().alpha = 1;
            Overlay.transform.Find("Return").GetComponent<CanvasGroup>().blocksRaycasts = true;
            Overlay.transform.Find("Return").GetComponent<CanvasGroup>().interactable = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOpen = false;
            Overlay.transform.Find("Return").GetComponent<CanvasGroup>().alpha = 0;
            Overlay.transform.Find("Return").GetComponent<CanvasGroup>().blocksRaycasts = false;
            Overlay.transform.Find("Return").GetComponent<CanvasGroup>().interactable = false;
        }
    }

    //void ShowTileInfo(Collider2D a)
    //{
    //    Overlay.transform.Find("Hovering Stats").GetComponent<CanvasGroup>().alpha = 1;
    //    Transform b = Overlay.transform.Find("Hovering Stats");
    //    b.transform.Find("Health").GetComponent<ProgressBar>().currentPercent = a.GetComponent<TileClass>().GetPercentage();
    //    b.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = a.name + " (Level " + a.GetComponent<TileClass>().GetLevel() + ")";
    //    b.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + a.name);
    //}

    //void ShowSelectedInfo(GameObject a)
    //{
    //    Overlay.transform.Find("Selected Info").GetComponent<CanvasGroup>().alpha = 1;
    //    Transform b = Overlay.transform.Find("Selected Info");
    //    b.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = a.name + " (Level " + a.GetComponent<TileClass>().GetLevel() + ")";
    //    b.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "Cost:      " + a.GetComponent<TileClass>().GetCost();
    //    b.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + a.name);
    //}

    private void UpdateGui()
    {
        GoldAmount.text = ""+gold;
    }

    public void AddGold(int a)
    {
        gold += a;
    }

    public void RemoveGold(int a)
    {
        gold -= a;
    }

    public void AdjustAlphaValue()
    {
        if (AdjustLimiter == 10)
        {
            if (AdjustSwitch == false)
            {
                Adjustment -= 0.1f;
            }
            else if (AdjustSwitch == true)
            {
                Adjustment += 0.1f;
            }
            if (AdjustSwitch == false && Adjustment <= 0f)
            {
                AdjustSwitch = true;
            }
            else if (AdjustSwitch == true && Adjustment >= 1f)
            {
                AdjustSwitch = false;
            }
            AdjustLimiter = 0;
        }
        else
        {
            AdjustLimiter += 1;
        }
    }

    public void SelectHotbar(int index)
    {
        try
        {
            hotbar[index]();
        } catch { return; }
        if (index == 0)
        {
            Overlay.transform.Find("One").GetComponent<Button>().interactable = false;
        }
        else if (index == 1)
        {
            Overlay.transform.Find("Two").GetComponent<Button>().interactable = false;
        }
        else if (index == 2)
        {
            Overlay.transform.Find("Three").GetComponent<Button>().interactable = false;
        }
        else if (index == 3)
        {
            Overlay.transform.Find("Four").GetComponent<Button>().interactable = false;
        }
        else if (index == 4)
        {
            Overlay.transform.Find("Five").GetComponent<Button>().interactable = false;
        }
        else if (index == 5)
        {
            Overlay.transform.Find("Six").GetComponent<Button>().interactable = false;
        }
        else if (index == 6)
        {
            Overlay.transform.Find("Seven").GetComponent<Button>().interactable = false;
        }
        else if (index == 7)
        {
            Overlay.transform.Find("Eight").GetComponent<Button>().interactable = false;
        }
        else
        {
            Overlay.transform.Find("Nine").GetComponent<Button>().interactable = false;
        }
    }

    public void SetTurret()
    {
        if (checkIfUnlocked(TurretObj))
        {
            SelectedObj = TurretObj;
            SwitchObj();
        }
    }

    public void SetShotgun()
    {
        if (checkIfUnlocked(ShotgunObj))
        {
            SelectedObj = ShotgunObj;
        SwitchObj();
        }
    }

    public void SetSniper()
    {
        if (checkIfUnlocked(SniperObj))
        {
            SelectedObj = SniperObj;
            SwitchObj();
        }
    }

    public void SetSMG()
    {
        if (checkIfUnlocked(SMGObj))
        {
            SelectedObj = SMGObj;
            SwitchObj();
        }
    }

    public void SetBolt()
    {
        if (checkIfUnlocked(BoltObj))
        {
            SelectedObj = BoltObj;
            SwitchObj();
        }
    }

    public void SetWall()
    {
        if (checkIfUnlocked(WallObj))
        {
            SelectedObj = WallObj;
            SwitchObj();
        }
    }

    public void SetCollector()
    {
        if (checkIfUnlocked(CollectorObj))
        {
            SelectedObj = CollectorObj;
            SwitchObj();
        }
    }

    public void SwitchObj()
    {
        DisableActiveInfo();
        Adjustment = 1f;
        Selected.sprite = Resources.Load<Sprite>("Sprites/" + SelectedObj.name);
        //Overlay.transform.Find("Hovering Stats").GetComponent<CanvasGroup>().alpha = 0;
    }

    public bool checkIfUnlocked(GameObject a)
    {
        for (int i = 0; i < unlocked.Count; i++)
        {
            if (a.name == unlocked[i].name)
            {
                return true;
            }
        }
        return false;
    }

    public void DisableActiveInfo()
    {
        Overlay.transform.Find("One").GetComponent<Button>().interactable = true;
        Overlay.transform.Find("Two").GetComponent<Button>().interactable = true;
        Overlay.transform.Find("Three").GetComponent<Button>().interactable = true;
        Overlay.transform.Find("Four").GetComponent<Button>().interactable = true;
        Overlay.transform.Find("Five").GetComponent<Button>().interactable = true;
        Overlay.transform.Find("Six").GetComponent<Button>().interactable = true;
        Overlay.transform.Find("Seven").GetComponent<Button>().interactable = true;
        Overlay.transform.Find("Eight").GetComponent<Button>().interactable = true;
        Overlay.transform.Find("Nine").GetComponent<Button>().interactable = true;
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }

    public float getDistance()
    {
        return distance;
    }

    public void addUnlocked(GameObject a)
    {
        unlocked.Add(a);
    }

}