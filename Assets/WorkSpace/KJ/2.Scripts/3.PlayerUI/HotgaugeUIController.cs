using UnityEngine;
using UnityEngine.UI;

namespace KJ.UI
{
    public class HotgaugeUIController : MonoBehaviour
    {
        [SerializeField] private Slider hotSlider;

        public void UpdateHotUI(float currentHeat, float maxHeat)
        {
            hotSlider.value = currentHeat / maxHeat;
        }
    }
}
