using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveManager
{
    void loadData(GameData _data);
    void saveData(ref GameData _data); //Pointer?
}
