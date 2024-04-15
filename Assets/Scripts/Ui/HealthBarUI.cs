using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject Player;
    //��ȡԤ����
    public GameObject healthUIPrefab;
    //�õ���ǰ���������HealthBar Point
    public Transform barPoint;
    //Ѫ���Ƿ��ǳ��ÿɼ���
    public bool alwaysVisible;
    //���ӻ�ʱ��
    public float visibleTime;

    //�õ��ɻ�����Ѫ��
    Image healthSlider;
    //������Prefab��ʵ��󣬻����������λ�ã�ʹ����barPoint����һ��
    Transform UIbar;
    //��ȡ�������λ�ã�ʹ��Ѫ��һֱ������Ծ�ͷ
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
                //����healthPrefab����������λ�ø�ֵ��UIbar
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
        //����ٷֱ� ��Ѫ���ٷֱȣ�
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
