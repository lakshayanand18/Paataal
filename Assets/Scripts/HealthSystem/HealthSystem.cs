public class HealthSystem
{
    int health;
    int healthMax;
    public HealthSystem(int healthMax)
    {
        this.healthMax = healthMax;
        health = healthMax;
    }

    public bool HasTakenDamage()
    {
        if (health < healthMax)
        {
            return true;
        }
        return false;
    }

    public  float GetHealth()
    {
        return health;
    }


    
    public void Damage(int damageAmount)
    {
        health -= damageAmount;

        if (health < 0) health = 0;
    }

    public float GetHealthPercentage()
    {
        return (float) health / healthMax;
    }

    public void Heal(int healAmount)
    {
        health += healAmount;

        if (health > healthMax) health = healthMax;
    }
}

