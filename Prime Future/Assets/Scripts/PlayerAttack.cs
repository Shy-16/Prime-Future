using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine;
using TMPro;

public class PlayerAttack : MonoBehaviour
{
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    public static PlayerAttack _instance;

    [Header("Virtual Camera")]
    public CinemachineVirtualCamera vCam;

    [Header("Weapon Instance References")]
    public Weapon curWeapon;
    public Weapon inventoryWeapon_1;
    public Weapon inventoryWeapon_2;
    public GameObject damageNumber;
    public Transform shootPoint;
    public AudioClip reloadClip;

    [Header("Current Weapon Values")]
    public int mag;
    public int reserves;

    [Header("Audio Sources")]
    public AudioSource shootSource;
    public AudioSource reloadSource;

    [Header("UI Instance References")]
    public Slider reloadSlider;
    public Image selectedWeaponIcon;
    public Image inventoryWeaponIcon_1;
    public Image inventoryWeaponIcon_2;
    public TextMeshProUGUI selectedWeaponReserves;
    public TextMeshProUGUI selectedWeaponMagazine;
    public TextMeshProUGUI inventoryWeaponReserve_1;
    public TextMeshProUGUI inventoryWeaponReserve_2;

    [Header("Bullet Decal Material")]
    public Material bulletCubeMat;


    private CinemachineBasicMultiChannelPerlin noiseComp;
    private float shakeDecay = 0.275f;

    private float fireTimer = 0;
    private float reloadTimer = 0;
    private float accuracy = 1;
    private GameObject weaponInstance;
    private Camera mainCam;

    //-------------------------------------------------------------------------
    // Initializations
    //-------------------------------------------------------------------------
    private void OnEnable()
    {
        _instance = this;   // This is the laziest thing possible; don't do this
    }

    private void Start()
    {
        noiseComp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        mainCam = Camera.main;
        shootSource.clip = curWeapon.shootSound;
        reloadSource.clip = reloadClip;

        weaponInstance = Instantiate(curWeapon.weaponPrefab, transform.position, transform.rotation, transform);
        weaponInstance.transform.localScale = curWeapon.scale;

        selectedWeaponMagazine.text = curWeapon.magazineSize.ToString();
        selectedWeaponReserves.text = (curWeapon.maxReserves - curWeapon.magazineSize).ToString();

        mag = curWeapon.magazineSize;
        reserves = curWeapon.maxReserves - curWeapon.magazineSize;

        selectedWeaponIcon.sprite = curWeapon.weaponIcon;
        inventoryWeaponIcon_1.sprite = inventoryWeapon_1.weaponIcon;
        inventoryWeaponIcon_2.sprite = inventoryWeapon_2.weaponIcon;
        inventoryWeaponReserve_1.text = inventoryWeapon_1.maxReserves.ToString();
        inventoryWeaponReserve_2.text = inventoryWeapon_2.maxReserves.ToString();
    }

    public float SetAccuracy
    {
        get
        {
            return accuracy;
        }
        set
        {
            accuracy = value;
        }
    }

    //-------------------------------------------------------------------------
    // Update
    //-------------------------------------------------------------------------
    private void FixedUpdate()
    {
        if (accuracy < 1)
        {
            accuracy += .01f + (curWeapon.bloomRecovRate * (accuracy * accuracy * accuracy)) * Time.fixedDeltaTime;

            if (accuracy > 1)
                accuracy = 1;
        }
    }

    private void Update()
    {
        //----------WEAPON HANDLING----------
        // Firing Timer
        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;

        // Reload Timer
        if (reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
            reloadSlider.value = 1 - (reloadTimer / curWeapon.reloadDuration);
        }
        else if (reloadTimer <= 0 && reloadSlider.gameObject.activeSelf)
            reloadSlider.gameObject.SetActive(false);

        // Screen Shake Decay
        if (noiseComp.m_AmplitudeGain > 0)
        {
            float decay = shakeDecay;

            if (noiseComp.m_AmplitudeGain - decay < 0)
                decay = noiseComp.m_AmplitudeGain;

            noiseComp.m_AmplitudeGain -= decay;
        }


        //----------INPUT HANDLING----------
        // Weapon Switching Via Scroll Wheel
        if (Input.mouseScrollDelta.y > 0)
        {
            Weapon tempWeapon = curWeapon;
            curWeapon = inventoryWeapon_1;
            inventoryWeapon_1 = inventoryWeapon_2;
            inventoryWeapon_2 = tempWeapon;

            shootSource.clip = curWeapon.shootSound;

            selectedWeaponMagazine.text = curWeapon.magazineSize.ToString();
            selectedWeaponReserves.text = (curWeapon.maxReserves - curWeapon.magazineSize).ToString();

            mag = curWeapon.magazineSize;
            reserves = curWeapon.maxReserves - curWeapon.magazineSize;

            selectedWeaponIcon.sprite = curWeapon.weaponIcon;

            reloadSlider.gameObject.SetActive(true);
            reloadTimer = curWeapon.reloadDuration;
            accuracy = 1;

            //---
            inventoryWeaponReserve_1.text = inventoryWeapon_1.maxReserves.ToString();
            inventoryWeaponIcon_1.sprite = inventoryWeapon_1.weaponIcon;

            //---
            inventoryWeaponReserve_2.text = inventoryWeapon_2.maxReserves.ToString();
            inventoryWeaponIcon_2.sprite = inventoryWeapon_2.weaponIcon;

            //---
            GameObject oldInstance = weaponInstance;
            weaponInstance = Instantiate(curWeapon.weaponPrefab, transform.position, transform.rotation, transform);
            weaponInstance.transform.localScale = curWeapon.scale;
            Destroy(oldInstance);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            Weapon tempWeapon = curWeapon;
            curWeapon = inventoryWeapon_2;
            inventoryWeapon_2 = inventoryWeapon_1;
            inventoryWeapon_1 = tempWeapon;

            shootSource.clip = curWeapon.shootSound;

            selectedWeaponMagazine.text = curWeapon.magazineSize.ToString();
            selectedWeaponReserves.text = (curWeapon.maxReserves - curWeapon.magazineSize).ToString();

            mag = curWeapon.magazineSize;
            reserves = curWeapon.maxReserves - curWeapon.magazineSize;

            selectedWeaponIcon.sprite = curWeapon.weaponIcon;

            reloadSlider.gameObject.SetActive(true);
            reloadTimer = curWeapon.reloadDuration;
            accuracy = 1;

            //---
            inventoryWeaponReserve_1.text = inventoryWeapon_1.maxReserves.ToString();
            inventoryWeaponIcon_1.sprite = inventoryWeapon_1.weaponIcon;

            //---
            inventoryWeaponReserve_2.text = inventoryWeapon_2.maxReserves.ToString();
            inventoryWeaponIcon_2.sprite = inventoryWeapon_2.weaponIcon;
            //---
            GameObject oldInstance = weaponInstance;
            weaponInstance = Instantiate(curWeapon.weaponPrefab, transform.position, transform.rotation, transform);
            weaponInstance.transform.localScale = curWeapon.scale;
            Destroy(oldInstance);
        }

        // Reloading
        if (Input.GetButtonDown("Reload"))
        {
            if (reserves - (curWeapon.magazineSize - mag) > 0)
            {
                reloadSlider.gameObject.SetActive(true);
                reloadTimer = curWeapon.reloadDuration;
                reserves -= curWeapon.magazineSize - mag;
                mag = curWeapon.magazineSize;
                reloadSource.pitch = Random.Range(.8f, 1.2f);
                reloadSource.Play();
            }
            else
            {
                reloadSlider.gameObject.SetActive(true);
                reloadTimer = curWeapon.reloadDuration;
                mag += reserves;
                reserves = 0;
                reloadSource.pitch = Random.Range(.8f, 1.2f);
                reloadSource.Play();
            }

            selectedWeaponReserves.text = reserves.ToString();
        }

        // Weapon Firing
        switch (curWeapon.firePattern)
        {
            case FirePattern.SemiAutomatic:
                {
                    if (Input.GetButtonDown("Fire1") && fireTimer <= 0 && mag > 0 && reloadTimer <= 0)
                        Shoot();
                    break;
                }
            case FirePattern.FullyAutomatic:
                {
                    if (Input.GetButton("Fire1") && fireTimer <= 0 && mag > 0 && reloadTimer <= 0)
                        Shoot();
                    break;
                }
            case FirePattern.Burst:
                {
                    if (Input.GetButtonDown("Fire1") && fireTimer <= 0 && mag > 0 && reloadTimer <= 0)
                    {
                        StartCoroutine(BurstCoroutine());
                    }
                    break;
                }
            case FirePattern.Spread:
                {
                    if (Input.GetButtonDown("Fire1") && fireTimer <= 0 && mag > 0 && reloadTimer <= 0)
                        Shoot(true);
                    break;
                }
        }


        //----------UI UPDATE----------
        selectedWeaponMagazine.text = mag.ToString();
    }

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------
    private void Shoot(bool isSpread = false)
    {
        if (isSpread)
            noiseComp.m_AmplitudeGain = Mathf.Clamp(2 * 8 * (curWeapon.damage / 50), 1, 5); // 8 is the number of bullets the spread gun fires
        else
            noiseComp.m_AmplitudeGain = Mathf.Clamp(6 * (curWeapon.damage / 50), 1.5f, 5); // the 2 above and 6 here are just scalars to maket his feel bigger

        Vector3 camForward = mainCam.transform.forward;
        float randomPitchPercentage = .12f;

        fireTimer = 1 / (curWeapon.fireRate / 60);

        shootPoint.localPosition = curWeapon.barrelOffset;
        GameObject muzzleFlash = Instantiate(curWeapon.muzzleFlash,
                                             shootPoint.position,
                                             Quaternion.LookRotation(-weaponInstance.transform.forward, weaponInstance.transform.up),
                                             weaponInstance.transform);
        Destroy(muzzleFlash, 1f);

        shootSource.pitch = 1 + Random.Range(-randomPitchPercentage, randomPitchPercentage);
        shootSource.Play();

        RaycastHit hit;

        if (isSpread)
        {
            int bulletCount = 8;

            for (int i = 0; i < bulletCount; ++i)
            {
                float radius = Random.Range(-curWeapon.maxBloom, curWeapon.maxBloom);
                float theta = Random.Range(0f, 180f);

                Vector3 horizontalRotation = Quaternion.AngleAxis(radius, Vector3.up) * Vector3.forward;
                Vector3 combinedRotation = Quaternion.AngleAxis(theta, Vector3.forward) * horizontalRotation;
                Vector3 rayDir = mainCam.transform.TransformDirection(combinedRotation);

                if (Physics.Raycast(mainCam.transform.position, rayDir, out hit, curWeapon.range * 10))
                {
                    HitScan(hit);
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = hit.point;
                    cube.transform.localScale = Vector3.one * .05f;
                    cube.transform.rotation = mainCam.transform.rotation;
                    cube.GetComponent<MeshRenderer>().material = bulletCubeMat;
                    Destroy(cube.GetComponent<BoxCollider>());
                    Destroy(cube, 10f);
                }
            }

            --mag;
        }
        else
        {
            float radius = Random.Range(-curWeapon.maxBloom, curWeapon.maxBloom);
            float theta = Random.Range(0f, 180f);

            Vector3 horizontalRotation = Quaternion.AngleAxis(radius * (1 - accuracy), Vector3.up) * Vector3.forward;
            Vector3 combinedRotation = Quaternion.AngleAxis(theta * (1 - accuracy), Vector3.forward) * horizontalRotation;
            Vector3 rayDir = mainCam.transform.TransformDirection(combinedRotation);

            if (Physics.Raycast(mainCam.transform.position, rayDir, out hit, curWeapon.range * 10))
            {
                HitScan(hit);
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = hit.point;
                cube.transform.localScale = Vector3.one * .05f;
                cube.transform.rotation = mainCam.transform.rotation;
                cube.GetComponent<MeshRenderer>().material = bulletCubeMat;
                Destroy(cube.GetComponent<BoxCollider>());
                Destroy(cube, 10f);
            }

            --mag;
        }

        if (accuracy > 0)
        {
            accuracy -= curWeapon.bloomRate;

            if (accuracy < 0)
                accuracy = 0;
        }
    }

    private void HitScan(RaycastHit hit)
    {
        float launchForce = 250;
        //Debug.Log(hit.transform.name);

        Enemy enemy = hit.transform.GetComponent<Enemy>();
        if (enemy != null)
        {
            float scaledDamage = ScaleDamage(curWeapon, hit.distance);

            enemy.TakeDamage(scaledDamage);
            GameObject damageNumb = Instantiate(damageNumber, hit.point, Quaternion.identity);
            scaledDamage = (int)scaledDamage;
            damageNumb.GetComponentInChildren<TextMeshProUGUI>().text = scaledDamage.ToString();
            Vector2 launchDir = Random.insideUnitCircle.normalized;
            damageNumb.GetComponent<Rigidbody>().AddForce((new Vector3(launchDir.x, 1, launchDir.y) + hit.normal) * launchForce);
            Destroy(damageNumb, 1f);
        }

        GameObject impact = Instantiate(curWeapon.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 1f);
    }

    // This function handles the damage scaling of weapons over distance
    // I created a neat damage fall off calculator here: https://www.desmos.com/calculator/nbkuf3qu0h
    // D = weapon damage, r = resistance to fall off, s = sharpness of fall off
    private float ScaleDamage(Weapon weapon, float distance)
    {
        float scaledDamage = weapon.damage;
        if (distance > weapon.range)
        {
            // weapon range is the range at which damage fall off begins to occur
            float fallOffResist = 1 - weapon.fallOffResistance;
            float fallOffSharp = weapon.fallOffSharpness;
            float fallOffExpo = distance / fallOffSharp;

            // value clamped between 0 and flat.max
            float damageReduction = Mathf.Clamp(fallOffResist * (-1 + Mathf.Pow(2, fallOffExpo)), 0, weapon.damage);
            scaledDamage -= damageReduction;
        }

        //Debug.Log("Damage to target: " + scaledDamage);
        return scaledDamage;
    }

    private IEnumerator BurstCoroutine()
    {
        int bulletsFired = 0;
        float burstDelay = 0;

        while(bulletsFired < 3)
        {
            if (burstDelay <= 0)
            {
                Shoot();
                ++bulletsFired;
                burstDelay = .1f;
            }
            else
            {
                burstDelay -= Time.deltaTime;
            }

            yield return 0;
        }
    }
}
