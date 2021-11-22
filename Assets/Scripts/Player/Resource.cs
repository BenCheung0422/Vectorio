using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Michsky.UI.ModernUIPack;

public class Resource : MonoBehaviour
{
    // Active instance
    public static Resource active;

    // Currency types
    public enum CurrencyType
    {
        Gold,
        Essence,
        Iridium,
        Power,
        Heat
    }

    // Resource currency class
    [System.Serializable]
    public class Currency
    {
        // Currency variables
        public CurrencyType type;
        public int amount;
        public int storage;
        public string format;
        public bool allowOverflow;
        public TextMeshProUGUI resourceUI;
        public TextMeshProUGUI storageUI;
        public bool useResourceBar;
        public ProgressBar resourceBar;
    }

    // Dictionary of all currencies
    public Dictionary<CurrencyType, Currency> currencies;
    public Currency[] currencyElements;

    // List of all collectors and storages
    public List<DefaultStorage> storages;

    // Get active instance
    public void Awake()
    {
        // Get active instance
        active = this;

        // Setup currencies
        currencies = new Dictionary<CurrencyType, Currency>();
        foreach (Currency currency in currencyElements)
            currencies.Add(currency.type, currency);

        // Setup events
        Events.active.onStoragePlaced += AddStorageObj;
    }
    
    // Add collector or storage
    public void AddStorageObj(DefaultStorage storage) { storages.Add(storage); }

    // Update storages 
    public void UpdateStorages(CurrencyType type, int amount, bool add)
    {
        // Setup local loop variables
        int amountToAdd = amount;
        if (!add) amountToAdd = -amountToAdd;

        // Updates all storages
        for(int i = 0; i < storages.Count; i++)
        {
            if (storages[i] != null && storages[i].type == type)
            {
                if (add) amountToAdd = storages[i].AddResources(amountToAdd);
                else
                {
                    amountToAdd += storages[i].TakeResource();
                    if (amountToAdd > 0) storages[i].AddResources(amountToAdd);
                }
            }
            else
            {
                storages.RemoveAt(i);
                i--;
            }
        }
    }

    // Get currency class
    public Currency GetCurrency(CurrencyType type)
    {
        if (currencies.ContainsKey(type))
            return currencies[type];
        else return null;
    }

    // Add a resource
    public void Add(CurrencyType type, int amount, bool updateStorages = true)
    {
        // Update storages
        if (updateStorages) UpdateStorages(type, amount, true);

        // Calculate amount
        if (type != CurrencyType.Heat && type != CurrencyType.Power)
        {
            currencies[type].amount += amount;
            if (currencies[type].amount >= currencies[type].storage)
                currencies[type].amount = currencies[type].storage;
        }
        else currencies[type].amount += amount;

        // Display to UI
        UpdateUI(type);

        // Update unlockables
        Buildables.UpdateResourceUnlockables(type, amount);

        // Update variant if heat passed
        if (type == CurrencyType.Heat) EnemyHandler.active.UpdateVariant();
    }

    // Remove a resource
    public void Remove(CurrencyType type, int amount, bool updateStorages = true)
    {
        // Update storages
        if (updateStorages) UpdateStorages(type, amount, false);

        // Calculate amount
        currencies[type].amount -= amount;
        if (currencies[type].amount <= 0)
            currencies[type].amount = 0;

        // Display to UI
        UpdateUI(type);

        if (type == CurrencyType.Heat) EnemyHandler.active.UpdateVariant();
    }

    // Add storage
    public void AddStorage(CurrencyType type, int amount)
    {
        currencies[type].storage += amount;

        // Display to UI
        UpdateUI(type, true);
    }

    // Set storage
    public void SetStorage(CurrencyType type, int amount)
    {
        currencies[type].storage = amount;

        // Display to UI
        UpdateUI(type, true);
    }

    // Remove storage
    public void RemoveStorage(CurrencyType type, int amount)
    {
        // Revert the storage
        currencies[type].storage -= amount;
        if (currencies[type].storage <= 0)
            currencies[type].storage = 0;

        // Display to UI
        UpdateUI(type, true);
    }

    // Update UI
    public void UpdateUI(CurrencyType type, bool updateStorage = false)
    {
        // Display to UI
        if (currencies[type].resourceUI != null)
            currencies[type].resourceUI.text = FormatNumber(currencies[type].amount);

        if (updateStorage && currencies[type].storageUI != null)
            currencies[type].storageUI.text = FormatNumber(currencies[type].storage);

        if (currencies[type].useResourceBar && currencies[type].resourceBar != null)
        {
            currencies[type].resourceBar.currentPercent = (float)currencies[type].amount / (float)currencies[type].storage * 100;
            currencies[type].resourceBar.UpdateUI();
        }
    }

    // Check resources
    public bool CheckResources(Buildable buildable)
    {
        // Check if resource should be used
        foreach (Cost resource in buildable.resources)
        {
            if (!Gamemode.active.useResources) continue;

            if (!resource.storage)
            {
                int amount = GetAmount(resource.resource);
                if (resource.add)
                {
                    Currency currency = GetCurrency(resource.resource);
                    if (currency.allowOverflow && amount > GetStorage(resource.resource)) return false;
                    else if (!currency.allowOverflow && amount + resource.amount > GetStorage(resource.resource)) return false;
                }
                else if (amount < resource.amount) return false;
            }
        }
        return true;
    }


    // Remove a resource based on building
    public void ApplyResources(Buildable buildable, bool refund = false)
    {
        // Update resource values promptly
        foreach (Cost resource in buildable.resources)
        {
            if (!Gamemode.active.useResources) continue;

            if (Gamemode.loadGame && resource.resource != CurrencyType.Heat && resource.resource != CurrencyType.Power) continue;

            if (refund)
            {
                if (resource.storage)
                {
                    if (resource.add) active.RemoveStorage(resource.resource, resource.amount);
                    else active.AddStorage(resource.resource, resource.amount);
                }
                else
                {
                    if (resource.add) active.Remove(resource.resource, resource.amount);
                    else active.Add(resource.resource, resource.amount);
                }
            }
            else
            {
                if (resource.storage)
                {
                    if (resource.add) active.AddStorage(resource.resource, resource.amount);
                    else active.RemoveStorage(resource.resource, resource.amount);
                }
                else
                {
                    if (resource.add) active.Add(resource.resource, resource.amount);
                    else active.Remove(resource.resource, resource.amount);
                }
            }
        }
    }


    // Remove a resource based on building
    public void RevertResources(Buildable buildable)
    {
        // Update resource values promptly
        foreach (Cost resource in buildable.resources)
        {
            if (!Gamemode.active.useResources) continue;

            if (resource.storage)
            {
                if (resource.add) active.RemoveStorage(resource.resource, resource.amount);
                else active.AddStorage(resource.resource, resource.amount);
            }
            else
            {
                if (resource.add) active.Remove(resource.resource, resource.amount);
                else active.Add(resource.resource, resource.amount);
            }
        }
    }

    // Check a resource amount
    public int GetAmount(CurrencyType type)
    {
        if (currencies.ContainsKey(type))
            return currencies[type].amount;
        else return 0;
    }

    // Check a resource storage
    public int GetStorage(CurrencyType type)
    {
        if (currencies.ContainsKey(type))
            return currencies[type].storage;
        else return 0;
    }

    // Get a resource name
    public string GetName(CurrencyType type)
    {
        switch(type)
        {
            case CurrencyType.Gold:
                return "Gold";
            case CurrencyType.Essence:
                return "Essence";
            case CurrencyType.Iridium:
                return "Iridium";
            case CurrencyType.Power:
                return "Power";
            case CurrencyType.Heat:
                return "Heat";
            default:
                return "Unknown";
        }
    }

    // Get a resource sprite
    public Sprite GetSprite(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.Gold:
                return Sprites.GetSprite("Gold");
            case CurrencyType.Essence:
                return Sprites.GetSprite("Essence");
            case CurrencyType.Iridium:
                return Sprites.GetSprite("Iridium");
            case CurrencyType.Power:
                return Sprites.GetSprite("Power");
            case CurrencyType.Heat:
                return Sprites.GetSprite("Heat");
            default:
                return Sprites.GetSprite("Transparent");
        }
    }

    // Get heat or power
    public int GetHeat() { return currencies[CurrencyType.Heat].amount; }
    public int GetPower() { return currencies[CurrencyType.Power].amount; }

    // Number formatter
    static string FormatNumber(int number)
    {
        if (number < 100000)
            return number.ToString();

        if (number < 1000000)
            return String.Format("{0:#,.}K", number - 500);

        if (number < 10000000)
            return String.Format("{0:#,,.##}M", number - 5000);

        if (number < 100000000)
            return String.Format("{0:#,,.#}M", number - 50000);

        if (number < 1000000000)
            return String.Format("{0:#,,.}M", number - 500000);

        return String.Format("{0:#,,,.##}B", number - 5000000);
    }
}
