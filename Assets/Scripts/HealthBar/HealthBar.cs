using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // UI Image that represents the fill of the health bar
    public Image fillImage;
    // The transform of the actual bar object (used for rotating toward the camera)
    public Transform bar;
    private IDamageable _healthProp;
    private Camera _camera;

    private void Awake()
    {
        _healthProp = transform.GetComponent<IDamageable>();
        _camera = Camera.main;
    }

    // Updates the health bar fill based on current health values. Uses Lerp for smooth visual transitions.
    public void SetHealth() {
        float maxHp = _healthProp.MaxHealth;
        float currentHp = _healthProp.CurrentHealth;
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, currentHp / maxHp, 10f*Time.deltaTime);
    }

    // Called after all Update functions have been called. Ensures health bar always faces the camera and updates health fill.
    private void LateUpdate() {
        bar.rotation = Quaternion.LookRotation(_camera.transform.forward);
        SetHealth();
    }
}
