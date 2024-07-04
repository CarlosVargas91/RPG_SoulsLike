using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask,
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")] //To create data item menu in unity
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique effect")]
    public float itemCoolDown;
    public ItemEffect[] itemEffects;

    [Header("Major stats")]
    public int strength;
    public int agility;
    public int intelligence;
    public int vitality;

    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critPower; 

    [Header("Defensive stats")]
    public int maxHealth;
    public int armor;
    public int evasion;
    public int magicResistance;

    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightingDamage;

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterial;

    private int descriptionLength;
    public void effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.executeEffect(_enemyPosition);
        }
    }
    public void addModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.addModifier(strength);
        playerStats.agility.addModifier(agility);
        playerStats.intelligence.addModifier(intelligence);
        playerStats.vitality.addModifier(vitality);

        playerStats.damage.addModifier(damage);
        playerStats.critChance.addModifier(critChance);
        playerStats.critPower.addModifier(critPower);

        playerStats.maxHealth.addModifier(maxHealth);
        playerStats.armor.addModifier(armor);
        playerStats.evasion.addModifier(evasion);
        playerStats.magicResistance.addModifier(magicResistance);

        playerStats.fireDamage.addModifier(fireDamage);
        playerStats.iceDamage.addModifier(iceDamage);
        playerStats.lightingDamage.addModifier(lightingDamage);
    }

    public void removeModifiers() 
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.removeModifier(strength);
        playerStats.agility.removeModifier(agility);
        playerStats.intelligence.removeModifier(intelligence);
        playerStats.vitality.removeModifier(vitality);

        playerStats.damage.removeModifier(damage);
        playerStats.critChance.removeModifier(critChance);
        playerStats.critPower.removeModifier(critPower);

        playerStats.maxHealth.removeModifier(maxHealth);
        playerStats.armor.removeModifier(armor);
        playerStats.evasion.removeModifier(evasion);
        playerStats.magicResistance.removeModifier(magicResistance);

        playerStats.fireDamage.removeModifier(fireDamage);
        playerStats.iceDamage.removeModifier(iceDamage);
        playerStats.lightingDamage.removeModifier(lightingDamage);
    }

    public override string getDescription()
    {
        sb.Length = 0;

        descriptionLength = 0;

        addItemDescription(strength, "Strentgh");
        addItemDescription(agility, "Agility");
        addItemDescription(intelligence, "Intelligence");
        addItemDescription(vitality, "Vitality");

        addItemDescription(damage, "Damage");
        addItemDescription(critChance, "Crit. Chance");
        addItemDescription(critPower, "Crit. Power");

        addItemDescription(maxHealth, "Health");
        addItemDescription(evasion, "Evasion");
        addItemDescription(armor, "Armor");
        addItemDescription(magicResistance, "Magic Resist.");

        addItemDescription(fireDamage, "Fire Damage");
        addItemDescription(iceDamage, "Ice Damage");
        addItemDescription(lightingDamage, "Lighting Damage");

        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i].effectDescription.Length > 0)
            {
                sb.AppendLine();
                sb.Append("Unique: " + itemEffects[i].effectDescription);
                descriptionLength++;
            }
        }

        if (descriptionLength < 5)
        {
            for (int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append(" ");
            }
        }

        return sb.ToString();
    }

    private void addItemDescription(int _value, string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();
            if (_value > 0)
                sb.Append("+ " + _value + " " + _name);

            descriptionLength++;
        }
    }
}
