using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stickalungu : Enemy
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public override void Die()
    {
        _animator.SetTrigger("Die");
        base.Die();
    }
}
