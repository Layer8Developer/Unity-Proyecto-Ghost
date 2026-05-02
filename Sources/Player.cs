using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Joystick JoyStickMove;
    public float moveSpeed;

    public Transform WeaponManager;
    public float FireDistance;

    //=======CCAMERRA DETECTION======
    public float DetectionRange;
    private float timerRange = 0f;
    bool foundEnemy = false;

    public float deactivateDelay = 2f; 
    public Transform Graphycstransform;
    private GameObject AIPos;
    public Animator Anims;

    //  Nueva variable
    private Vector3 lastMoveDirection = Vector3.forward;

    //============FOOTSTEPS====================
    private bool enableFootSteps;
    private AudioSource sourceAudio;
    public AudioClip[] FootStepsSounds;
    public float rate = 1f; // sonidos por segundo
    float timer;

    void Start()
    {
        JoyStickMove = GameObject.Find("JoystickHUD").GetComponent<Joystick>();
        sourceAudio = GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();
    }
    void Update()
    {
        Movement();
        FootSteps();
    }

    void Movement()
    {
        float MoveX = JoyStickMove.Horizontal + Input.GetAxis("Horizontal");
        float MoveY = JoyStickMove.Vertical + Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(MoveX, 0, MoveY);

        // Si hay movimiento, actualizamos la dirección
        if (movement.magnitude > 0.1f)
        {
            lastMoveDirection = movement.normalized;
            transform.Translate(movement.normalized * moveSpeed * Time.deltaTime, Space.World);
            enableFootSteps = true;
            Anims.SetBool("IsWalk", true);
        }
        else
        {
            enableFootSteps = false;
            Anims.SetBool("IsWalk", false);
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foundEnemy = false;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance && distance <= DetectionRange)
            {
                closestDistance = distance;
                closestEnemy = enemy;
                foundEnemy = true;
            }
        }

        //  SI ENCONTRÓ ENEMIGO
        if (foundEnemy)
        {
            CameraYBoost.aimingEnemy = true;
            WeaponManager.LookAt(closestEnemy.transform.position);
            AIPos = closestEnemy;

            timerRange = 0f; //  reinicia el timer
        }
        else
        {
            //  empieza a contar
            timerRange += Time.deltaTime;

            if (timerRange >= deactivateDelay)
            {
                CameraYBoost.aimingEnemy = false;
            }
        }


        RaycastHit Hit;

        // Dibuja el rayo en la escena (color rojo)
        Debug.DrawRay(Graphycstransform.position, Graphycstransform.forward * FireDistance, Color.red);

        if (Physics.Raycast(Graphycstransform.position, Graphycstransform.forward, out Hit, FireDistance))
        {
            // Dibuja hasta donde golpea (color verde)
            Debug.DrawLine(Graphycstransform.position, Hit.point, Color.green);

            if (Hit.collider.CompareTag("Enemy"))
            {
                Anims.SetBool("ShootIdle", true);
                WeaponScript.CanFire = (WeaponScript.AmmoInMag > 0);
            }
            else
            {
                WeaponScript.CanFire = false;
                Anims.SetBool("ShootIdle", false);
            }
        }
        else
        {

        }
      
        if (AIPos != null)
        {
            if (closestDistance < FireDistance)
            {
                Vector3 directionToEnemy = AIPos.transform.position - Graphycstransform.position;
                directionToEnemy.y = 0f;
                Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                Graphycstransform.rotation = Quaternion.Slerp(Graphycstransform.rotation, targetRotation, 10 * Time.deltaTime);
            }
            else
            {
                WeaponScript.CanFire = false;
                //  Mantiene última dirección al no haber enemigos cerca
                Graphycstransform.forward = Vector3.Slerp(Graphycstransform.forward, lastMoveDirection, 10 * Time.deltaTime);
            }
        }
        else
        {
            WeaponScript.CanFire = false;
            //  Mantiene la última dirección al no moverse
            Graphycstransform.forward = Vector3.Slerp(Graphycstransform.forward, lastMoveDirection, 10 * Time.deltaTime);
        }
    }

    //=====================FOOTSTEPS============================
    void FootSteps()
    {
        if (enableFootSteps)
        {
            timer += Time.deltaTime;

            float delay = 1f / rate; 

            if (timer >= delay)
            {
                PlaySound();
                timer = 0f;
            }
        }
    }

    void PlaySound()
    {
        if (FootStepsSounds.Length == 0) return;

        sourceAudio.PlayOneShot(FootStepsSounds[Random.Range(0, FootStepsSounds.Length)],0.4f);
    }
}
