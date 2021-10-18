using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField]
    private float _walkingSpeed = 7.5f;
    [SerializeField]
    private float _runningSpeed = 11.5f;
    [SerializeField]
    private float _jumpSpeed = 8.0f;
    [SerializeField]
    private float _gravity = 9.8f;
    [SerializeField]
    private Camera _playerCamera;
    [SerializeField]
    private float _lookSpeed = 2.0f;
    [SerializeField]
    private float _lookXLimit = 45.0f;
    [SerializeField]
    private bool _canMove = true;
    [SerializeField]
    private bool _canShoot = false;    
    [SerializeField]
    private LayerMask _rayCastIgnore;
    [SerializeField]
    private GameObject _crossHairGameObject;
    [SerializeField]
    private float _timeBeforeRecoilDisabled = 0.3f;

    private CharacterController _characterController;
    private Vector3 _moveDirection = Vector3.zero;
    private float _mouseY = 0;
    private bool _isAimingDownSight;

    private Crosshair _crosshair;
    private WeaponManager _weaponManager;
    private InputManager _inputManager;

    private bool _recoil;

    public bool IsSprinting
    {
        get;
        set;
    }

    public bool IsAimingDownSight
    {
        get
        {
            return _isAimingDownSight;
        }
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _crosshair = _crossHairGameObject.GetComponent<Crosshair>();
        _weaponManager = GetComponent<WeaponManager>();
        _inputManager = GetComponent<InputManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _canShoot = true;
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        IsSprinting = _inputManager.Sprint();

        float curSpeedX = _canMove ? (IsSprinting ? _runningSpeed : _walkingSpeed) * _inputManager.Vertical() : 0;
        float curSpeedY = _canMove ? (IsSprinting ? _runningSpeed : _walkingSpeed) * _inputManager.Horizontal() : 0;

        float movementDirectionY = _moveDirection.y;
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (_inputManager.Jump() && _canMove && _characterController.isGrounded)
        {
            _moveDirection.y = _jumpSpeed;
        }
        else
        {
            _moveDirection.y = movementDirectionY;
        }

        if (_inputManager.ADS())
        {
            _isAimingDownSight = true;           
        }
        else
        {
            _isAimingDownSight = false;
        }

        bool crosshairVisible = (!_isAimingDownSight) && (!IsSprinting);
        _canShoot = !IsSprinting;

        _crossHairGameObject.SetActive(crosshairVisible);

        if (crosshairVisible)
        {
            if ((curSpeedX + curSpeedY) > 0)
            {
                _crosshair.SetScale(CrosshairScale.Walk);
            }
            else
            {
                _crosshair.SetScale(CrosshairScale.Default);
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!_characterController.isGrounded)
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }

        // Move the controller
        _characterController.Move(_moveDirection * Time.deltaTime);

        // Player and Camera rotation
        UpdateLookRotation();

        if (_inputManager.Fire())
        {
            StartCoroutine(Shoot());
        }
    }

    private void UpdateLookRotation()
    {
        if (_canMove)
        {
            _mouseY += -Input.GetAxis("Mouse Y") * _lookSpeed;
            _mouseY = Mathf.Clamp(_mouseY, -_lookXLimit, _lookXLimit);

            if (_recoil)
            {
                _playerCamera.transform.localRotation = Quaternion.Lerp(_playerCamera.transform.localRotation, Quaternion.Euler(_mouseY, 0, 0), Time.deltaTime * _weaponManager.ActiveWeapon.RecoilSpeed);
            }
            else
            {
                _playerCamera.transform.localRotation = Quaternion.Euler(_mouseY, 0, 0);
            }


            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _lookSpeed, 0);
        }
    }

    private IEnumerator Shoot()
    {
        if (_canShoot)
        {
            _weaponManager.ActiveWeapon.PlayShootAnimation();

            yield return new WaitForSeconds(_weaponManager.ActiveWeapon.DelayBeforeRayCast);

            Vector3 shootRayOrigin = _playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;

            if (!_isAimingDownSight)
            {
                float bloom = _weaponManager.ActiveWeapon.HipfireBloom;
                shootRayOrigin += (Random.insideUnitSphere * bloom);
            }

            float range = _weaponManager.ActiveWeapon.Range;

            if (Physics.Raycast(shootRayOrigin, _playerCamera.transform.forward, out hit, range))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
                    enemy.Die();
                }
            }

            ApplyRecoil();

            _crosshair.SetScale(CrosshairScale.Shoot, 1f);
        }
    }

   
    /// <summary>
    /// Gets active weapons recoil amount and applies it to the camera movement
    /// </summary>
    private void ApplyRecoil()
    {
        _mouseY -= _weaponManager.ActiveWeapon.RecoilAmount;
        _recoil = true;
        StartCoroutine(DisableRecoil());
    }

    /// <summary>
    //When we apply the recoil the camera will lerp up to the recoil position, we'll disable this after a moment for smooth camera movement after shooting
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableRecoil()
    {
        yield return new WaitForSeconds(_timeBeforeRecoilDisabled);
        _recoil = false;
    }
}
