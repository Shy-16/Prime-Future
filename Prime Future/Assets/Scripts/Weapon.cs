using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Weapon Name")]
    public new string name;

    [Header("Rarity and Level Requirements")]
    public Rarity rarity;
    public int level;

    [Header("Mesh Information")]
    public GameObject weaponPrefab;
    public Sprite weaponIcon;
    public Vector3 scale;
    public Vector3 rotation;

    [Header("Weapon Statistics")]
    public float damage;
    public float range;
    public float handling;
    public float accuracy;
    public float fireRate;
    public float reloadDuration;
    public int magazineSize;
    public int maxReserves;

    [Header("Hidden Statistics")]
    [Range(.001f, .5f)]
    public float bloomRate;
    [Range(.2f, 3f)]
    public float bloomRecovRate;
    [Range(0, 10)]
    public float maxBloom;          // value cannot be 0

    [Header("Weapon Fall Off Statistics")]
    public float fallOffResistance; // the higher the value, the longer it for fall off to start having a noticeable effect
    public float fallOffSharpness;  // the higher the value, the more gradual and shallower the fall off curve

    [Header("Miscilaneous Stats")]
    public int price;
    [TextArea(4, 100)]
    public string flavorText;

    [Header("Fire Pattern")]
    public FirePattern firePattern;

    [Header("Weapon Effects")]
    public Vector3 barrelOffset;
    public GameObject muzzleFlash;
    public GameObject impactEffect;
    public AudioClip shootSound;
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
};

public enum FirePattern
{
    SemiAutomatic,
    FullyAutomatic,
    Burst,
    Spread
};