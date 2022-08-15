using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WorldObject : MonoBehaviour
{
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    [Header("Weapon Scriptable")]
    public Weapon weapon;
    [Header("Canvas Refernce")]
    public Canvas inspectionCanvas;
    [Header("Text Object Referneces")]
    public Image weaponIcon;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI itemLevel;
    public TextMeshProUGUI itemRarity;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI handlingText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI rateOfFireText;
    public TextMeshProUGUI reloadSpeedText;
    public TextMeshProUGUI magazineText;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI flavorText;

    [Header("Comparison Arrow References")]
    public Sprite upArrow;
    public Sprite downArrow;
    public Sprite dash;
    public Image damageComparison;
    public Image rangeComparison;
    public Image handlingComparison;
    public Image accuracyComparison;
    public Image rateOfFireComparison;
    public Image reloadComparison;
    public Image magazineSizeComparison;

    private Camera playerCam;

    private Coroutine raiseRoutine;
    private Coroutine lowerRoutine;

    //-------------------------------------------------------------------------
    // Initializations
    //-------------------------------------------------------------------------
    private void Start()
    {
        playerCam = Camera.main;
        inspectionCanvas.worldCamera = playerCam;
    }

    //-------------------------------------------------------------------------
    // Update
    //-------------------------------------------------------------------------
    private void Update()
    {
        inspectionCanvas.transform.position = transform.position + Vector3.up;
        // Billboard Inspection Panel
        inspectionCanvas.transform.rotation = playerCam.transform.rotation;
    }

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------
    private void PopulateInspectionPanel()
    {
        weaponIcon.sprite = weapon.weaponIcon;
        weaponName.text = weapon.name;
        itemLevel.text = weapon.level.ToString();
        itemRarity.text = weapon.rarity.ToString();

        if (weapon.firePattern == FirePattern.Spread)
            damageText.text = weapon.damage.ToString() + " x 8";
        else
            damageText.text = weapon.damage.ToString();

        rangeText.text = weapon.range.ToString() + " m";
        handlingText.text = weapon.handling.ToString();
        accuracyText.text = weapon.accuracy.ToString();
        rateOfFireText.text = weapon.fireRate.ToString() + " rpm";
        reloadSpeedText.text = weapon.reloadDuration.ToString();
        magazineText.text = weapon.magazineSize.ToString();
        itemPrice.text = "$" + weapon.price.ToString();
        flavorText.text = weapon.flavorText;
    }

    private void CompareWeapons(Weapon compareWeapon)
    {
        if (weapon.firePattern == FirePattern.Spread && compareWeapon.firePattern != FirePattern.Spread)
        {
            if (weapon.damage * 8 < compareWeapon.damage)
                damageComparison.sprite = downArrow;
            else if (weapon.damage * 8 == compareWeapon.damage)
                damageComparison.sprite = dash;
            else
                damageComparison.sprite = upArrow;
        }
        else
        {
            if (weapon.damage < compareWeapon.damage)
                damageComparison.sprite = downArrow;
            else if (weapon.damage == compareWeapon.damage)
                damageComparison.sprite = dash;
            else
                damageComparison.sprite = upArrow;
        }


        if (weapon.range < compareWeapon.range)
            rangeComparison.sprite = downArrow;
        else if (weapon.range == compareWeapon.range)
            rangeComparison.sprite = dash;
        else
            rangeComparison.sprite = upArrow;

        if (weapon.handling < compareWeapon.handling)
            handlingComparison.sprite = downArrow;
        else if (weapon.handling == compareWeapon.handling)
            handlingComparison.sprite = dash;
        else
            handlingComparison.sprite = upArrow;

        if (weapon.accuracy < compareWeapon.accuracy)
            accuracyComparison.sprite = downArrow;
        else if (weapon.accuracy == compareWeapon.accuracy)
            accuracyComparison.sprite = dash;
        else
            accuracyComparison.sprite = upArrow;

        if (weapon.fireRate < compareWeapon.fireRate)
            rateOfFireComparison.sprite = downArrow;
        else if (weapon.fireRate == compareWeapon.fireRate)
            rateOfFireComparison.sprite = dash;
        else
            rateOfFireComparison.sprite = upArrow;

        if (weapon.reloadDuration < compareWeapon.reloadDuration)
            reloadComparison.sprite = downArrow;
        else if (weapon.reloadDuration == compareWeapon.reloadDuration)
            reloadComparison.sprite = dash;
        else
            reloadComparison.sprite = upArrow;

        if (weapon.magazineSize < compareWeapon.magazineSize)
            magazineSizeComparison.sprite = downArrow;
        else if (weapon.magazineSize == compareWeapon.magazineSize)
            magazineSizeComparison.sprite = dash;
        else
            magazineSizeComparison.sprite = upArrow;
    }

    private IEnumerator RaisePanel()
    {
        Debug.Log("Raising Panel");
        inspectionCanvas.gameObject.SetActive(true);
        PopulateInspectionPanel();

        float progress = .025f;
        while (inspectionCanvas.transform.localScale.x < 1)
        {
            inspectionCanvas.transform.position = Vector3.Lerp(inspectionCanvas.transform.position, transform.position + Vector3.up, progress);
            inspectionCanvas.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
            progress += .025f;
            yield return 0;
        }

        raiseRoutine = null;
    }

    private IEnumerator LowerPanel()
    {
        float progress = .025f;
        while (inspectionCanvas.transform.localScale.x > 0)
        {
            inspectionCanvas.transform.position = Vector3.Lerp(inspectionCanvas.transform.position, transform.position, progress);
            inspectionCanvas.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress);
            progress += .025f;
            yield return 0;
        }

        inspectionCanvas.gameObject.SetActive(false);

        lowerRoutine = null;
    }

    //-------------------------------------------------------------------------
    // Events
    //-------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (lowerRoutine != null)
            {
                StopCoroutine(lowerRoutine);
                raiseRoutine = StartCoroutine(RaisePanel());
            }
            else
                raiseRoutine = StartCoroutine(RaisePanel());

            CompareWeapons(other.gameObject.GetComponentInChildren<PlayerAttack>().curWeapon);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (raiseRoutine != null)
            {
                StopCoroutine(raiseRoutine);
                lowerRoutine = StartCoroutine(LowerPanel());
            }
            else
                lowerRoutine = StartCoroutine(LowerPanel());
        }
    }
}
