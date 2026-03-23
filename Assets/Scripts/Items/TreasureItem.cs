namespace DefaultNamespace
{
    /// <summary>
    /// Concrete Item subclass for Treasure, Keys, and Maps.
    /// These don't have combat stats — they're collected for progression.
    /// </summary>
    public class TreasureItem : Item
    {
        public TreasureItem(string id, string name, string description,
            int goldValue, Rarity rarity, ItemType type = ItemType.Treasure)
            : base(id, name, description, goldValue, rarity, type)
        {
        }
 
        /// <summary>
        /// No active use effect — ProgressionSystem handles key/map logic separately.
        /// </summary>
        public override void use(Player player) { }
    }
}