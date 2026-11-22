using UnityEngine;
using FrostGame.Health;
using UnityEngine.UI;
using System;

namespace FrostGame.UI
{ 
    public class HealthShowUI : MonoBehaviour
    {
        [SerializeField] Health.Health health;
        [SerializeField] Image healthImage;

        private void Start()
        {
            health.OnChanged += HealthChanged;
        }

        private void HealthChanged(float arg1, float arg2)
        {
            healthImage.fillAmount = arg1 / arg2;
        }
    }
}
