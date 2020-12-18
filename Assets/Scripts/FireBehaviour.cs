using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    [SerializeField] GameObject[] WideFireParent;
    [SerializeField] BoxCollider FireCollider;
    [SerializeField] BoxCollider FireDamageCollider;
    const float maxLifePoint = 30;
    float LifePoint;
    bool isReviving;

    private void Start() => LifePoint = maxLifePoint;

    private void Update()
    {
        if (isReviving && LifePoint < maxLifePoint)
        {
            LifePoint += Time.deltaTime * 2.5f;
        }
        else if (isReviving)
        {
            isReviving = false;
            FireDamageCollider.enabled = true;
            FireCollider.enabled = true;
            DisplayFire();
        }
    }

    public void DecreaseFireOvertime()
    {
        if (isReviving) return;

        LifePoint -= 1;
        DisplayFire();

        if (LifePoint <= 0)
        {
            isReviving = true;
            FireDamageCollider.enabled = false;
            FireCollider.enabled = false;
        }
    }

    void DisplayFire()
    {
        foreach (GameObject fireParent in WideFireParent)
        {
            ParticleSystem PS_FIRE_ALPHA = fireParent.transform.Find(Keyword.PS_FIRE_ALPHA).GetComponent<ParticleSystem>();
            ParticleSystem PS_FIRE_ADDITIVE = fireParent.transform.Find(Keyword.PS_FIRE_ADDITIVE).GetComponent<ParticleSystem>();
            ParticleSystem PS_FIRE_GLOW = fireParent.transform.Find(Keyword.PS_FIRE_GLOW).GetComponent<ParticleSystem>();
            ParticleSystem PS_FIRE_SPARK = fireParent.transform.Find(Keyword.PS_FIRE_SPARK).GetComponent<ParticleSystem>();

            var alphaEmision = PS_FIRE_ALPHA.emission;
            alphaEmision.rateOverTime = LifePoint;

            var additiveEmision = PS_FIRE_ADDITIVE.emission;
            additiveEmision.rateOverTime = LifePoint;

            var glowEmision = PS_FIRE_GLOW.emission;
            glowEmision.rateOverTime = LifePoint;

            var sparkEmision = PS_FIRE_SPARK.emission;
            sparkEmision.rateOverTime = LifePoint;
        }
    }
}
