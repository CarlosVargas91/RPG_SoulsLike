using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //to make it visible in inspector
public class Stat
{
    [SerializeField]private int baseValue;
    public List<int> modifiers;

    public int getValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public void setDefaultValue(int _value)
    {
        baseValue = _value;
    }

    public void addModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void removeModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}
