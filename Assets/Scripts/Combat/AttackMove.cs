namespace DefaultNamespace
{
    public class AttackMove
    {
        // Fields
        private string _moveName;
        private double _damageMultiplier;
        private int _goldCost;
        private int _minXPRequired;
        private string _synergyTag;
        private CodingQuestion _question;
    
        // Properties
        public string MoveName {
            get {
                return _moveName;
            }
            set {
                _moveName = value;
            }
        }
    
        public double DamageMultiplier {
            get {return _damageMultiplier;}
            set {
                _damageMultiplier = value;
            }
        }
        public int GoldCost {
            get {return _goldCost;}
            set {
                if (value >= 0) _goldCost = value;
                else _goldCost = 0;
            }
        }
        public int MinXPRequired {
            get {return _minXPRequired;}
            set {
                if (value > 0) _minXPRequired = value;
                else _minXPRequired = 0;
            }
        }
        public string SynergyTag {
            get {return _synergyTag;}
            set {
                _synergyTag = value;
            }
        }
    
        public CodingQuestion  Question {
            get {return _question;}
            set {
                _question = value;
            }
        }

        /**
         * Calculate Damage method: Calculates the overall amount of damage dealt by the attack
         * @param: int baseWeaponDmg: Base amount of damage the weapon deals
         * @return: int value representing overall amount of damage that will be dealt
         */
        public int calculateDamage(int baseWeaponDmg)
        {
            return (int)(DamageMultiplier * baseWeaponDmg);
        }

        /**
         * Is Unlocked method: Checks if the player has unlocked this attack move
         * @param: Player player: instance of player class which contains list of unlocked attack moves
         * @return: boolean true/false depending if move has been unlocked
         */
        public bool isUnlocked(Player player)
        {
            return player.getAttackMoves().Any(x => x.MoveName == MoveName);
        }
    
    
    }
}