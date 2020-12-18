using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydranShooter : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == Keyword.FIRE_PARTICLES)
        {
            other.GetComponent<FireBehaviour>().DecreaseFireOvertime();
        }
    }
}

public class HydranTankProperty
{
    float currentHydroValue;
    float maxValue;

    public HydranTankProperty()
    {
        maxValue = 25f;
        currentHydroValue = maxValue;
    }

    public void UseHydran()
    {
        currentHydroValue -= Time.deltaTime;
    }

    public float PercentageValue()
    {
        return currentHydroValue / maxValue;
    }
}
