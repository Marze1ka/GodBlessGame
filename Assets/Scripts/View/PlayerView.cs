using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public Animator anim;
    public Slider hpSlider;
    public Slider magicSlider;
    public Image magicFill; // Для смены цвета магии

    public void Initialize(float maxHealth, float maxMagic)
    {
        if (hpSlider != null) hpSlider.maxValue = maxHealth;
        if (magicSlider != null) magicSlider.maxValue = maxMagic;
    }

    public void SetHealth(float value)
    {
        if (hpSlider != null) hpSlider.value = value;
    }

    public void SetMagic(float value)
    {
        if (magicSlider != null) magicSlider.value = value;
    }

    public void SetMagicColor(Color color)
    {
        if (magicFill != null) magicFill.color = color;
    }

    public void PlayAnimation(string trigger)
    {
        if (anim != null) anim.SetTrigger(trigger);
    }

    public void UpdateMoveAnimation(float speed)
    {
        if (anim != null)
        {
            // 0.1f — это время сглаживания. Если поставить 0, анимация переключится мгновенно.
            anim.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        }
    }

}
