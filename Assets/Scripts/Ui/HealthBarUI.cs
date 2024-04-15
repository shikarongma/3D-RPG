using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject Player;
    //获取预制体
    public GameObject healthUIPrefab;
    //得到当前挂载物体的HealthBar Point
    public Transform barPoint;
    //血条是否是长久可见的
    public bool alwaysVisible;
    //可视化时间
    public float visibleTime;

    //得到可滑动的血条
    Image healthSlider;
    //当生成Prefab的实体后，获得她的坐标位置，使其与barPoint保持一致
    Transform UIbar;
    //获取摄像机的位置，使得血条一直正向面对镜头
    Transform cameraPosition;

    CharacterStates currentStats;

    void Awake()
    {
        currentStats = GetComponent<CharacterStates>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    void OnEnable()
    {
        cameraPosition = Player.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                //生成healthPrefab并将其坐标位置赋值给UIbar
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);

        UIbar.gameObject.SetActive(true);
        //计算百分比 （血条百分比）
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cameraPosition.forward;
        }
    }
}
