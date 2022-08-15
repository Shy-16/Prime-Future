using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    public float maxShield = 100;
    public float maxHealth = 100;

    public Image shieldForeground;
    public Image healthForeground;

    private MeshRenderer meshRend;
    private CapsuleCollider capsuleCollider;
    private Canvas healthBarCanvas;
    private Camera HUDCam;
    private CapsuleCollider capCollider;
    private float shield;
    private float health;
    private bool healing = false;
    private float respawnTimer;
    private float shieldRechargeTimer = 0f;
    private float shieldRechargeDelay = 5f;
    private const float damageCatchupStep = .1f;

    [Space]
    public TextMeshProUGUI ttkReadout;
    private bool ttkCounting = false;
    private float ttkTimer = 0f;

    //-------------------------------------------------------------------------
    // Initializations
    //-------------------------------------------------------------------------
    private void Start()
    {
        capCollider = GetComponent<CapsuleCollider>();

        // Never do this
        HUDCam = Camera.main.transform.GetChild(0).GetComponent<Camera>();
        healthBarCanvas = GetComponentInChildren<Canvas>();
        meshRend = GetComponent<MeshRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        healthBarCanvas.worldCamera = HUDCam;
        shield = maxShield;
        health = maxHealth;
    }

    //-------------------------------------------------------------------------
    // Update
    //-------------------------------------------------------------------------
    private void Update()
    {
        healthBarCanvas.transform.rotation = HUDCam.transform.rotation;

        if (respawnTimer > 0)
            respawnTimer -= Time.deltaTime;
        else if (respawnTimer <= 0 && meshRend.enabled == false)
        {
            shield = maxShield;
            health = maxHealth;
            shieldForeground.fillAmount = shield / maxShield;
            healthForeground.fillAmount = health / maxHealth;
            meshRend.enabled = true;
            capCollider.enabled = true;
            healthBarCanvas.enabled = true;
        }

        if (healing)
        {
            if (shield < maxShield)
            {
                shield += damageCatchupStep;
                shieldForeground.fillAmount = shield / maxShield;
            }
            else
                healing = false;
        }

        if (shieldRechargeTimer > 0 && respawnTimer <= 0)
        {
            shieldRechargeTimer -= Time.deltaTime;

            if (shieldRechargeTimer <= 0)
                healing = true;
        }

        if (ttkCounting)
            ttkTimer += Time.deltaTime;
    }

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------
    public void Heal(float amount)
    {
        if (health < maxHealth)
        {
            if (health + amount < maxHealth)
                health += amount;
            else
            {
                float remainder = amount - (maxHealth - health);
                health = maxHealth;
                shield += remainder;
            }
        }
        else
        {
            if (shield + amount <= maxShield)
                shield += amount;
            else
                shield = maxShield;
        }

        healthForeground.fillAmount = health / maxHealth;
    }

    public void TakeDamage(float amount)
    {
        healing = false;
        shieldRechargeTimer = shieldRechargeDelay;

        if (health == maxHealth && shield == maxShield)
            ttkCounting = true;

        if (shield > 0)
        {
            if (shield >= amount)
                shield -= amount;
            else
            {
                float remainder = amount - shield;
                shield = 0;
                health -= remainder;
            }
        }
        else
        {
            if (health > amount)
                health -= amount;
            else
                health = 0;
        }

        shieldForeground.fillAmount = shield / maxShield;
        healthForeground.fillAmount = health / maxHealth;

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        respawnTimer = 3f;
        ttkCounting = false;
        meshRend.enabled = false;
        capsuleCollider.enabled = false;
        //healthBarCanvas.enabled = false;
        ttkReadout.text = "TTK: " + ttkTimer.ToString("F4") + "s" +
                          "\nDPS: " + (maxShield + maxHealth) / ttkTimer + "DPS";
        ttkTimer = 0;
    }
}
