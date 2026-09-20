using System;
using System.Collections.Generic;
using UnityEngine;
public class EnemyUI : MonoBehaviour
{
    [SerializeField] private RectTransform enemyUI;

    //health
    [SerializeField] private RectTransform canvas;


    [SerializeField] private Camera mainCamera;

    public static Dictionary<Transform, RectTransform> allEnemyUI = new Dictionary<Transform, RectTransform>();

    public static EnemyUI instance;
    private void Awake()
    {
        instance = this;
        allEnemyUI.Clear();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        foreach (var enemy in allEnemyUI)
        {
            if (enemy.Key == null) continue;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(enemy.Key.position);
            if (screenPos.z > 0)
            {
                if (enemy.Value.GetComponentInChildren<HealthBar>(true).IsHealthFull())
                {
                    enemy.Value.gameObject.SetActive(false);
                    // Debug.Log("false");
                }
                else
                {
                    enemy.Value.gameObject.SetActive(true);
                    enemy.Value.position = screenPos;
                    // Debug.Log("true");
                }
            }
            else
            {
                enemy.Value.gameObject.SetActive(false);
            }
        }
    }
    public void Register(Transform enemy, HealthSystem healthSystem)
    {
        RectTransform EnemyBox = Instantiate(enemyUI, canvas);
        HealthBar foundBar = EnemyBox.GetComponentInChildren<HealthBar>();
        foundBar.Setup(healthSystem);
        allEnemyUI[enemy] = EnemyBox;

    }
    public void UnRegister(Transform enemy)
    {
        if (allEnemyUI.ContainsKey(enemy))
        {
            Destroy(allEnemyUI[enemy].gameObject);
            allEnemyUI.Remove(enemy);
        }
    }
}