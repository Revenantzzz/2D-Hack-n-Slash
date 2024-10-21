using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : Attack
{
    [SerializeField] float _speed = 5f;

    public Vector2 Dir = Vector2.left;

    private void FixedUpdate()
    {
        Fly(Dir);
    }
    private void Fly(Vector2 dir)
    {
        Vector3 realdir = new Vector3(dir.x, 0, 0);
        transform.position = transform.position + _speed * realdir * Time.deltaTime ;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        this.transform.position = this.transform.parent.position;
    }
}
