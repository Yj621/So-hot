using UnityEngine;
using UnityEngine.UI;

namespace KJ.UI
{
    public class StaminaUIController : MonoBehaviour
    {
        [SerializeField] private Slider staminaSlider;

        public void UpdateStaminaUI(float currentStamina, float maxStamina)
        {
            staminaSlider.value = currentStamina / maxStamina;
        }
    }
}
