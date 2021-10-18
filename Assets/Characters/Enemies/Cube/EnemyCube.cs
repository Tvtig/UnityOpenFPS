using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCube : Enemy
{
    [SerializeField]
    private GameObject _model;

    public override void Die()
    {
        base.Die();

        _model.SetActive(false);
    }

}
