using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float _destroyAfter = 1f;
    [SerializeField]
    private GameObject _deathVFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Die()
    {
        if(_deathVFX != null)
        {
            _deathVFX.SetActive(true);
        }   
        
        //Some general death stuff..
        Destroy(gameObject, _destroyAfter);
    }
}
