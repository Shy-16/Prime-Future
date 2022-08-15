using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    [Header("Health Stats")]
    public float maxHealth = 100;
    public float maxShield = 100;

    [Header("HUD References")]
    public Image shieldForeground;
    public Image shieldDamaged;
    public Image healthForeground;
    public Image healthDamaged;

    [Header("Sound Effects")]
    public AudioSource heal;
    public AudioSource shieldSmashSource;
    public AudioSource shieldRegenSource;
    public AudioSource shieldImpactSource;
    public AudioSource healthImpactSource;
    public AudioSource death;

    private float health;
    private float shield;
    private float damageCatchupTimer = 0;
    private float shieldRechargeTimer = 0;
    private const float shieldRechargeDelay = 5;
    private const float damageCatchupDelay = .75f;
    private const float damageCatchupStep = .01f;
    
    //-------------------------------------------------------------------------
    // Initializations
    //-------------------------------------------------------------------------
    private void Start()
    {
        health = maxHealth;
        shield = maxShield;
    }

    //-------------------------------------------------------------------------
    // Update
    //-------------------------------------------------------------------------
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
            Heal(30);

        if (Input.GetKeyDown(KeyCode.Minus))
            TakeDamage(30);

        if (damageCatchupTimer > 0)
            damageCatchupTimer -= Time.deltaTime;
        
        // Damaged shields catching up to current shields
        if (shieldDamaged.fillAmount != shieldForeground.fillAmount && damageCatchupTimer <= 0)
            shieldDamaged.fillAmount = Mathf.Lerp(shieldDamaged.fillAmount, shieldForeground.fillAmount, damageCatchupStep);
        // Damaged health catching up to current health
        if (healthDamaged.fillAmount != healthForeground.fillAmount && damageCatchupTimer <= 0)
            healthDamaged.fillAmount = Mathf.Lerp(healthDamaged.fillAmount, healthForeground.fillAmount, damageCatchupStep);

        // Current shields approaching new shield value
        if (shieldForeground.fillAmount != shield / maxShield)
        {
            // Healing
            if (shieldForeground.fillAmount < shield / maxShield)
                shieldForeground.fillAmount += damageCatchupStep / 10;
            // Hurting
            else
                shieldForeground.fillAmount = Mathf.Lerp(shieldForeground.fillAmount, (shield / maxShield), damageCatchupStep);
        }
        // Current health approaching new health value
        if (healthForeground.fillAmount != health / maxHealth)
            healthForeground.fillAmount = Mathf.Lerp(healthForeground.fillAmount, (health / maxHealth), damageCatchupStep);

        if (shieldRechargeTimer > 0)
        {
            shieldRechargeTimer -= Time.deltaTime;

            if (shieldRechargeTimer <= 0)
                Heal(maxShield - shield, true);
        }
    }

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------
    public void Heal(float amount, bool onlyShields = false)
    {
        if (!onlyShields)
        {
            if (health < maxHealth)
            {
                heal.pitch = Random.Range(.8f, 1.2f);
                heal.Play();

                if (health + amount < maxHealth)
                    health += amount;
                else
                {
                    float remainder = amount - (maxHealth - health);
                    health = maxHealth;

                    if (shield == 0)
                    {
                        shield += remainder;
                        shieldRegenSource.pitch = Random.Range(.8f, 1.2f);
                        shieldRegenSource.Play();
                    }
                }
            }
            else
            {
                if (shield + amount <= maxShield)
                    shield += amount;
                else
                    shield = maxShield;
            }
        }
        else
        {
            shieldRegenSource.pitch = Random.Range(.8f, 1.2f);
            shieldRegenSource.Play();

            if (shield + amount <= maxShield)
                shield += amount;
            else
                shield = maxShield;
        }
    }

    public void TakeDamage(float amount)
    {
        damageCatchupTimer = damageCatchupDelay;
        shieldRechargeTimer = shieldRechargeDelay;

        if (shield > 0)
        {
            if (shield > amount)
            {
                shield -= amount;
                shieldImpactSource.pitch = Random.Range(.8f, 1.2f);
                shieldImpactSource.Play();
            }
            else
            {
                float remainder = amount - shield;
                shield = 0;;
                shieldSmashSource.pitch = Random.Range(.8f, 1.2f);
                shieldSmashSource.Play();
                health -= remainder;
            }
        }
        else
        {
            healthImpactSource.pitch = Random.Range(.8f, 1.2f);
            healthImpactSource.Play();

            if (health > amount)
                health -= amount;
            else
                health = 0;
        }

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        death.pitch = Random.Range(.8f, 1.2f);
        death.Play();
    }

    //-------------------------------------------------------------------------
    // Events
    //-------------------------------------------------------------------------

}
