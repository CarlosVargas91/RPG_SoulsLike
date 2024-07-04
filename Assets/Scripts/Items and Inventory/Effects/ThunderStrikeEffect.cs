using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item effect/Thunder strike")] //To create data item menu in unity
public class ThunderStrikeEffect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikerPreFab;
    public override void executeEffect(Transform _enemyPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikerPreFab, _enemyPosition.position, Quaternion.identity);

        Destroy(newThunderStrike, 1);
    }
}
