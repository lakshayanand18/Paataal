using UnityEngine;
public class HealthBar : MonoBehaviour
{
    private HealthSystem healthSystem;
    private RectTransform rectTransform;


    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Setup(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;
    }

    public bool IsHealthFull()
    {
        if (healthSystem == null) return false;

        return healthSystem.GetHealthPercentage() >= 1f;
        
    }

    private void Update()
    {
        if (healthSystem == null) return;

        rectTransform.localScale = new Vector3(healthSystem.GetHealthPercentage(), 1);
    }
}
