namespace DefaultNamespace;


public class HealthBar
{
    //fields for tracking the player's health
    private int _currentHP;
    private int _maxHP;

    //constructor
    public HealthBar(int maxHP)
    {
        this._maxHP = maxHP;
        _currentHP = maxHP;
    }
    
    //takes a specified amount of damage from currentHP
    public void TakeDamage(int amount)
    {
        currentHP = currentHP - amount;
        //makes currentHP 0 if the amount taken away makes HP less than zero
        if (currentHP < 0)
        {
            currentHP = 0;
        }
    }
    
    //adds a specified amount of HP to the currentHP
    public void Heal(int amount)
    {
        currentHP = currentHP + amount;
        //checks if currentHP value is over max and assigns max value if true
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }
    
    //checks if player is dead
    public bool IsDead()
    {
        //returns true if player's HP is less than zero and is dead
        if (currentHP <= 0)
        {
            return true;
        }
        return false;
    }
    
    //gets the player's currentHP percentage based on maxHP
    public double GetPercentage()
    {
        return ((double)currentHP / maxHP) * 100;
    }
    
    //resets healthbar to any max value
    public void Reset(int newMax)
    {
        maxHP = newMax;
        currentHP = newMax;
    }
}