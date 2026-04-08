using DefaultNamespace;

[System.Serializable]
public class ItemSaveData
{
    public string Id;
    public string Name;
    public string Description;
    public int GoldValue;
    public Rarity Rarity;
    public ItemType Type;
    public UnityEngine.Sprite WorldSprite;

    // Weapon specific
    public int BaseDamage;
    public WeaponType WeaponType;
    public string SpecialEffect;

    // Clothing specific
    public ClothingSlot ClothingSlot;
    public int DefenseBonus;
}