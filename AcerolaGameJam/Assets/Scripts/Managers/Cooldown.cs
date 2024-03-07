using UnityEngine;

[System.Serializable]
public class Cooldown
{
    float cooldownTime;
    float _nextFireTime;

    public bool IsCoolingDown => Time.time < _nextFireTime;
    public void SetCooldown(float newCooldown) => cooldownTime = newCooldown;
    public void StartCooldown() => _nextFireTime = Time.time + cooldownTime;
}