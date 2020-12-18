using UnityEngine;
public class Energy 
{
    float currentEnergy;
    float maxEnergy;
    float maxSpeed;
    float minSpeed;
    float speed;

    public Energy(float maxEnergyValue)
    {
        maxEnergy = maxEnergyValue;
        currentEnergy = maxEnergy;
        maxSpeed = 6f;
        minSpeed = 3.5f;
    }

    public void DecreaseOverTime(bool isWearingMask, bool isNarating)
    {
        float multiplier = 1.5f;
        if (isWearingMask) multiplier -= 0.5f;
        if (isNarating) multiplier -= 0.5f;
        
        currentEnergy -= Time.deltaTime * multiplier;
    }

    public void HitFire()
    {
        currentEnergy -= 25 * Time.deltaTime;
    }

    public float SpeedByEnergyValue(bool isNarating)
    {
        if (isNarating) return minSpeed;
        else return maxSpeed;
    }

    public float SpeedPercentageValue(bool isNarating)
    {
        return (SpeedByEnergyValue(isNarating)) / maxSpeed;
    }
    public float EnergyPercentageValue()
    {
        return currentEnergy / maxEnergy;
    }

    public bool isExhausted()
    {
        return currentEnergy / maxEnergy <= 0.4f;
    }

    public bool isFaulted()
    {
        return currentEnergy / maxEnergy <= 0f;
    }
}
