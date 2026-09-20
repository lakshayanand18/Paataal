using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TimeLine : MonoBehaviour
{
   [SerializeField]private float timeTillWin;
   private TMP_Text time;
   private int myTime;

   private void Start()
   {
       time = GetComponent<TMP_Text>();
   }

   public void TimeDisplay()
   { 
       if (Time.timeSinceLevelLoad  <= timeTillWin)
       {
           myTime = (int)(timeTillWin - Time.timeSinceLevelLoad);
           time.SetText(TimeSpan.FromSeconds(myTime).ToString("mm':'ss"));
       }
   }
   
   public float TimeShown 
   {
       get => myTime;
   }
}
