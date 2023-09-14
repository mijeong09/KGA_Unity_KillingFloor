using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    MeshRenderer itemRenderer;
    Material mat;
    PlayerInputs input;
    PlayerShooter shooter;

    public int value;
    public bool isBlink;
    public float duration;

    void Start()
    {
        itemRenderer = GetComponent<MeshRenderer>();
        if(itemRenderer != null)
        {
           mat = itemRenderer.material;
        }
    }

    private void Update()
    {
        ItemBlink();
        
    }
    // ������ �ʷϻ����� �����Ÿ���
    public void ItemBlink()
    {
        if (isBlink && itemRenderer != null)
        {
            float emission = Mathf.PingPong(Time.time, duration);
            Color baseColor = new Color(0f, 0.3f, 0f, 1f);
            Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

            mat.SetColor("_EmissionColor", finalColor);
        }
    }

    private void OnTriggerStay(Collider player)
    {
        // �÷��̾ ��ó�� ������
        if(player.CompareTag("Player"))
        {
            input = player.GetComponent<PlayerInputs>();
            shooter = player.GetComponent<PlayerShooter>(); 
            PlayerUIManager.instance.equipUI.SetActive(true);

            if(input.equip)
            {
                shooter.GetAmmo(value);
                PlayerUIManager.instance.equipUI.SetActive(false);
                input.equip = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit()
    {
        PlayerUIManager.instance.equipUI.SetActive(false);
    }

}