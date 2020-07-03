using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultEquipment", menuName = "Data/Equipment")]
public class ScriptableEquipement : ScriptableObject
{
   //string ref require direct mapping to unity's game foundation system
   
   public string mainWeaponRef;
}
