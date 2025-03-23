using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    void TakeDamage(float amount);
    public float MaxHealth { get; set; }

    public float CurrentHealth { get; set; }
}
