using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class PlayerShooter : MonoBehaviour
{
    private PlayerInputs input;
    private PlayerMovement playerMovement;
    private CameraSetup cameraSet;
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11);    // ������ ���� ������ ���̾� ����ũ

    public Transform aimTarget;
    public float damage;
    public float range = 100f;

    [Header("Animator IK")]
    protected Animator animator;
    public Animator handAnimator;

    public bool ikActive = false;
    [Range(0,1)]
    public float handIKAmount = 1;
    public float elbowIKAmount = 1;
    public Transform weaponPosition = null;    // ���� ��ġ ������
    public Transform targetObj;                // �÷��̾� ����

    [Header("TPS Weapon")]
    public Weapon equipedWeapon;
    public Transform rightHandObj = null;   // ������
    public Transform leftHandObj = null;    // �޼�
    public Transform rightElbowObj = null;   // ������ �׷�
    public Transform leftElbowObj = null;    // �޼� �׷�
    private int weaponSlot;

    Weapon tpsPistol;    // ������ ���� ���� ����
    Weapon tpsRifle;     // ������ ������ ���� ����

    [Header("FPS Weapon")]
    public Transform fpsPosition;
    public Transform fpsPistolObj;
    public Transform fpsRifleObj;
    public Transform fpsWeapon;



    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInputs>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraSet = GetComponent<CameraSetup>();
        animator = GetComponent<Animator>();

        // TPS ���� ��������
        tpsPistol = weaponPosition.GetChild(0).GetComponent<Weapon>();
        tpsRifle = weaponPosition.GetChild(1).GetComponent<Weapon>();
        tpsRifle.gameObject.SetActive(false);    // �̸� ���α�

        equipedWeapon = tpsPistol;                          // �⺻���� �������� ����
        rightHandObj = equipedWeapon.rightHandObj.transform;   // ������ ������ �׷�
        leftHandObj = equipedWeapon.leftHandObj.transform;     // ������ �޼� �׷�
        rightElbowObj = equipedWeapon.rightElbowObj.transform;     // ������ �����Ȳ�ġ
        leftElbowObj = equipedWeapon.leftElbowObj.transform;     // ������ ���Ȳ�ġ
        weaponSlot = 1;                                 // ���� ���� ����

        // FPS ���� ��������
        fpsPosition = transform.GetChild(0).GetChild(0).GetComponent<Transform>();
        fpsPistolObj = fpsPosition.transform.GetChild(0).GetComponent<Transform>();
        fpsRifleObj = fpsPosition.transform.GetChild(1).GetComponent<Transform>();  // �������� �̸� �ҷ��ͼ� ���α�
        fpsRifleObj.gameObject.SetActive(false);

        fpsWeapon = fpsPistolObj;
        handAnimator = fpsWeapon.GetComponent<Animator>();
        playerMovement.fpsAnimator = handAnimator;

        PlayerUIManager.instance.SetAmmo(tpsPistol.ammo);
        PlayerUIManager.instance.SetTotalAmmo(tpsPistol.totalAmmo);
    }

    // Update is called once per frame
    void Update()
    {
        weaponPosition.position = targetObj.position;
        weaponPosition.rotation = targetObj.rotation;

        Shoot();
        Reload();
        ActiveAnimation();
        WeaponSlots();
    }

    // ��� �Է�

    void Shoot()
    {
        RaycastHit hit;
        Vector3 hitPoint = cameraSet.followCam.transform.forward * 1f;
        if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range))
        {
            hitPoint = hit.point;
        }

        if (input.shoot && tpsPistol.ammo > 0)
        {
            if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range, layerMask))
            {
                Debug.DrawRay(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward * 100f, Color.red, 3f);
                GameObject hitObj = hit.transform.gameObject;
                hitPoint = hit.point;
                Damage(hitObj);
            }
            tpsPistol.ammo -= 1;
            PlayerUIManager.instance.SetAmmo(tpsPistol.ammo);
            //handAnimator.SetTrigger("isFire");
            //animator.SetTrigger("isFire");
            input.shoot = false;

        }

        aimTarget.transform.position = hitPoint;
    }



    void Damage(GameObject _hitObj)
    {
        if (_hitObj.layer == 8) // ����
        {
            Debug.Log("������ �¾Ҵ�.");
            //Legacy:
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().healthBody -= 10f;
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().health -= tpsPistol.damage;
        }
        else if (_hitObj.layer == 9) // ��
        {
            //Legacy:
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().healthBody -= 10f;
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().health -= tpsPistol.damage;

        }
        else if (_hitObj.layer == 10) // ������
        {
            Debug.Log("�������� �¾Ҵ�.");
            //Legacy:
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().healthBody -= 10f;
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().health -= tpsPistol.damage;

        }
        else if (_hitObj.layer == 11) // ��弦
        {
            Debug.Log("��弦");
            //Legacy:
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().healthHead -= 10f;
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().health -= tpsPistol.damage * 1.5f;

        }
    }
    // ����
    public void Reload()
    {
        if (input.reload)
        {
            if (0 < tpsPistol.totalAmmo && tpsPistol.ammo != tpsPistol.magazineSize)
            {
                float newAmmo = tpsPistol.ammo;
                tpsPistol.ammo = tpsPistol.ammo + tpsPistol.totalAmmo;
                if (tpsPistol.ammo > tpsPistol.magazineSize)
                {
                    tpsPistol.ammo = tpsPistol.magazineSize;
                }

                tpsPistol.totalAmmo = tpsPistol.totalAmmo - (tpsPistol.magazineSize - newAmmo);
                if (0 > tpsPistol.totalAmmo)
                { tpsPistol.totalAmmo = 0; }

                PlayerUIManager.instance.SetAmmo(tpsPistol.ammo);
                PlayerUIManager.instance.SetTotalAmmo(tpsPistol.totalAmmo);
                handAnimator.SetTrigger("isReload");

            }
            input.reload = false;
        }
    }

    public void WeaponSlots()
    {
        WeaponSlot1();
        WeaponSlot2();
    }

    public void WeaponSlot1()
    {
        if (input.weaponSlot1 && weaponSlot == 2)
        {
            tpsRifle.gameObject.SetActive(false);
            tpsPistol.gameObject.SetActive(true);

            equipedWeapon = tpsPistol;
            rightHandObj = equipedWeapon.rightHandObj.transform;
            leftHandObj = equipedWeapon.leftHandObj.transform;
            weaponSlot = 1;

            //fpsRifleObj.gameObject.SetActive(false);
            //fpsPistolObj.gameObject.SetActive(true);

            //fpsWeapon = fpsPistolObj;
            //handAnimator = fpsWeapon.GetComponent<Animator>();
            //playerMovement.fpsAnimator = handAnimator;

            input.weaponSlot1 = false;
        }
    }

    public void WeaponSlot2()
    {
        if (input.weaponSlot2 && weaponSlot == 1)
        {
            tpsPistol.gameObject.SetActive(false);
            tpsRifle.gameObject.SetActive(true);

            equipedWeapon = tpsRifle;
            rightHandObj = equipedWeapon.rightHandObj.transform;
            leftHandObj = equipedWeapon.leftHandObj.transform;
            weaponSlot = 2;

            //fpsPistolObj.gameObject.SetActive(false);
            //fpsRifleObj.gameObject.SetActive(true);

            //fpsWeapon = fpsRifleObj;
            //handAnimator = fpsWeapon.GetComponent<Animator>();
            //playerMovement.fpsAnimator = handAnimator;

            input.weaponSlot2 = false;
        }
    }

    public void ActiveAnimation()
    {

    }


    // ���� IK �ִϸ��̼� ó��
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // �÷��̾� lookat
                if (targetObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(targetObj.position);
                }
                // ������ �׷�
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                // �޼� �׷�
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
              
                // ���� �Ȳ�ġ
                if (leftElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, elbowIKAmount);
                }
                // ������ �Ȳ�ġ
                if (rightElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, elbowIKAmount);
                }
            }
            // �׷��� �ƹ��͵� ���ٸ� 0
            else
            {
                //animator.SetLookAtWeight(0);

                //animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

                //animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

                //animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                //animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
            }
        }
    }
}