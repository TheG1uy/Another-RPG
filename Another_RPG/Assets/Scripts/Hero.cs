﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
public class Hero : Unit
{
    [SerializeField]
    private Image progressIM;
    [SerializeField]
    private float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    [SerializeField]
    private float epsilon = 0;
    [SerializeField]
    private float jumpforce = 10.0F;
    private bool isGrounded = true;
    [SerializeField]
    private float attackTime = 0.5f;
    [SerializeField]
    private Weapon HeroWeapon;
    bool flag = true;
    Vector3 delta = new Vector3(0.6f, 0.5f);
    private float KOSTYL = 0.1f;
    public GameObject Joystick;
    public float flyTime;
    public float realSpeed; // Скорость после пересчета Time.deltatime
    public float deltaX;
    public float startHealth;
    private void Start()
    {
        startHealth = health_point;
    }
    public override void DamageRecive(float damage)
    {
        health_point -= (int)damage;
        progressIM.fillAmount = (health_point / startHealth);
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGround();

        deltaX = Mathf.Abs(rigidbody.velocity.x * Time.deltaTime);


        if (isGrounded)           //Вот эта штука призвана решить проблему прилипания к стенам
            flyTime = 0;
        else
            flyTime += Time.deltaTime;

        realSpeed = speed * 1.0f / ((flyTime + 1));

        






        if ((Input.GetButton("Jump") || Joystick.GetComponent<joystick>().GetY() > 0) && isGrounded)
        {
            Jump();
        }



     /*   if (!isGrounded && Mathf.Abs(rigidbody.velocity.y) <= epsilon && rigidbody.velocity.y <= 0)
        {  
            return;
        }  Пока закоментил. Она слишком много от управления жрет
    */



        if (Input.GetButton("Horizontal") || Joystick.GetComponent<joystick>().GetX() != 0)
        {
            if (Input.GetAxis("Horizontal") > 0 || Joystick.GetComponent<joystick>().GetX() > 0)
                direction = 1;
            else
                direction = -1;
            Move();
        }




       


        if ((Input.GetButton("Fire1") && (Application.platform != RuntimePlatform.Android) ||  flag && Joystick.GetComponent<joystick>().GetAtack() > 0) && flag)
        {
            StartCoroutine(Attack());
        }
        
    }

    private void Update()
    {

    }



    private IEnumerator Attack()
    {
        
        HeroWeapon.gameObject.SetActive(true);
        delta.x = 0.6f * direction;
        HeroWeapon.transform.position = gameObject.transform.position + delta;
        Vector3 pos = HeroWeapon.transform.position;
        if (KOSTYL == 0.02f)
        {
            pos.y += KOSTYL;
            HeroWeapon.transform.position=pos;
            KOSTYL = -0.02f;
        }
        else
        {
            pos.y += KOSTYL;
            HeroWeapon.transform.position = pos;
            KOSTYL = 0.02f;
        }
        HeroWeapon.getSprite().flipX = direction < 0.0f;
        flag = false;
        yield return new WaitForSeconds(attackTime);
        flag = true;


    }
    protected override void Move()
    {
        rigidbody.velocity = new Vector2(direction * realSpeed, rigidbody.velocity.y);
        //  sprite.flipX = direction < 0.0F;

       
            SpriteRenderer [] sp = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < sp.GetLength(0); i++)
        {
            sp[i].flipX = direction < 0.0F;

        }


        

    }


    private void Jump()
    {
        //rigidbody.velocity = Vector3.zero; 
      /*
       * Ну нельзя так делать. Это останавливает персонажа. Персонаж не должен останавливаться
       * при прыжке
       * 
       */
        rigidbody.AddForce(new Vector2(0, jumpforce), ForceMode2D.Impulse);
        
    }

    public bool CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, groundRadius, whatIsGround);
        return isGrounded = colliders.Length > 0;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Bullet bullet = collider.GetComponent<Bullet>();
        if (bullet)
        {
            DamageRecive(1.0f);
        }
    }
}