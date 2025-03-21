using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public Transform bar;

    public void SetHealth() {
        IHealth healthProp = transform.GetComponent<IHealth>();
        float maxHp = healthProp.MaxHealth;
        float currentHp = healthProp.CurrentHealth;
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, currentHp / maxHp, 10f*Time.deltaTime);
    }

    private void LateUpdate() {
        bar.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        SetHealth();
    }
}
