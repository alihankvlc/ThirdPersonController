using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Interface;
using DG.Tweening;
using UnityEngine;

public sealed class HazardousObject : MonoBehaviour, IDamager
{
    [SerializeField] private float _damageAmount;

    public float DamageAmount => _damageAmount;

    public void DealDamage(IDamageable damageable)
    {
        damageable?.TakeDamage(_damageAmount);
    }
}