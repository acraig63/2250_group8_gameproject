using UnityEngine;
using DefaultNamespace;

public class ItemTest : MonoBehaviour
{
    void Start()
    {
        Weapon sword = new Weapon("wpn_001", "Iron Cutlass", "A basic sword",
            25, Rarity.Common, 10, WeaponType.Sword);

        Clothing hat = new Clothing("clo_001", "Pirate Hat", "A tricorn",
            15, Rarity.Uncommon, ClothingSlot.Head, 3);

        TreasureItem key = new TreasureItem("key_001", "Brass Key", "Opens a cell",
            0, Rarity.Rare, ItemType.Key);

        Debug.Log($"Weapon: {sword.Name}, Damage: {sword.calculateDamage()}");
        Debug.Log($"Clothing: {hat.Name}, Defense: {hat.DefenseBonus}");
        Debug.Log($"Treasure: {key.Name}, Type: {key.Type}");
    }
}