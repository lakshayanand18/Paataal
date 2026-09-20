using System;
using TMPro;
using UnityEngine;

public class TimeStartWarning : MonoBehaviour
{
    [SerializeField] private float timeForTheWarning = 1f;
    
    public void TimeWarning()
    {
        if (Time.timeSinceLevelLoad > timeForTheWarning)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
