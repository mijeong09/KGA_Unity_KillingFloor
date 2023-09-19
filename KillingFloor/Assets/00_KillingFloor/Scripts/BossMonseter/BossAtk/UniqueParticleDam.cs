using Photon.Pun.Demo.SlotRacer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueParticleDam : MonoBehaviour
{
    private bool atkChk = false;
    private float damage = 5f;
    private float coolTime = 0.05f;
    private void OnParticleCollision(GameObject other)
    {
        if (atkChk == false)
        {
            if(transform.name.Equals("Effect_38_SmokeField_2"))
            {
                coolTime = 0;
                damage = 50;      
            }
            else
            {
                coolTime = 0.05f;
                damage = 5;
            }
            // �������κ��� LivingEntity Ÿ���� �������� �õ�
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            if (attackTarget != null)
            {                // ������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
                Vector3 hitPoint = other.transform.position;
                Vector3 hitNormal = transform.position - other.transform.position;

                // ���� ����
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
            Invoke("DamTime", coolTime);
            atkChk = true;
        }
    }

    private void DamTime()
    {
        atkChk = false;
    }
}