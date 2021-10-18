using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    private enum WeaponPositionState
    {
        Up,
        Down,
        PutUp,
        PutDown,
        Aim,
        Sprint
    }

    public UnityAction<Gun> WeaponSwitchEvent;

    [SerializeField]
    [Tooltip("The selectable weapons")]
    private List<Gun> _availableWeaponsList;
    [SerializeField]
    private Transform _weaponParentPosition;
    [SerializeField]
    private Transform _weaponDownPosition;
    [SerializeField]
    private Transform _weaponDefaultPosition;
    [SerializeField]
    private Transform _weaponAimingPosition;
    [SerializeField]
    private Transform _weaponSprintPosition;
    [SerializeField]
    private LayerMask _weaponLayer;
    [SerializeField]
    private Camera _playerCamera;
    [SerializeField]
    private Camera _weaponCamera;
    [SerializeField]
    private PlayerCharacterController _playerController;
    [SerializeField]
    private float _aimAnimationSpeed = 10f;
    [SerializeField]
    private float _sprintAnimationSpeed = 10f;
    [SerializeField]
    private float _weaponPutUpAnimationSpeed = 5f;
    [SerializeField]
    private float _defaultFOV = 60f;

    //Create weapons inventory with 2 slots for weapons
    private Gun[] _weaponSlots = new Gun[2];
    //The currently active weapon
    private int _activeWeaponIndex;
    //The calculated position of the weapon
    private Vector3 _weaponPosition;
    //The switch state of the weapon
    private WeaponPositionState _weaponPositionState;
    //The currently active weapon
    private Gun _activeWeapon;

    private float _timeWeaponSwitchStarted;

    public Gun ActiveWeapon
    {
        get
        {
            return _activeWeapon;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _activeWeaponIndex = -1;
        _weaponPositionState = WeaponPositionState.Down;

        WeaponSwitchEvent += OnWeaponSwitched;

        foreach (Gun weapon in _availableWeaponsList)
        {
            AddWeapon(weapon);
        }

        SwitchWeapon(0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //If the player is holding the sprint button, this method will lerp the position to the sprinting position
        UpdateWeaponSprintState();
        //If the player is holding the aim , this method will lerp the position to the aiming position
        UpdateWeaponAimState();
        //If the player is switching weapons, this method will update the positions
        UpdateWeaponPutUpState();

        UpdateWeaponPosition();
        UpdateCameraFieldOfView();

        _weaponParentPosition.localPosition = _weaponPosition;
    }

    private void SwitchWeapon(int index)
    {
        int newWeaponIndex = -1;
        int closestSlotDistance = _weaponSlots.Length;

        if (index < _weaponSlots.Length && index >= 0 && index != _activeWeaponIndex)
        {
            newWeaponIndex = index;
        }

        //Valid weapon index we switch
        if (newWeaponIndex > -1)
        {
            _timeWeaponSwitchStarted = Time.time;
            _weaponPosition = _weaponDownPosition.localPosition;

            _activeWeaponIndex = newWeaponIndex;
            _activeWeapon = _weaponSlots[_activeWeaponIndex];
            _weaponPositionState = WeaponPositionState.PutUp;
            WeaponSwitchEvent?.Invoke(_activeWeapon);
        }
    }

    private bool AddWeapon(Gun weaponToAdd)
    {
        if (weaponToAdd != null)
        {
            for (int i = 0; i < _weaponSlots.Length; i++)
            {
                if (_weaponSlots[i] == null)
                {
                    Gun weaponInstance = Instantiate(weaponToAdd, _weaponParentPosition);
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;

                    weaponInstance.SetVisibility(false);

                    //Convert to selected weapon layer to an index.
                    int layerIndex = Mathf.RoundToInt(Mathf.Log(_weaponLayer.value, 2));

                    //Loop through each weapon hierarchy of gameobjects and assign them to the weapon layer.
                    foreach (Transform t in weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
                    {
                        t.gameObject.layer = layerIndex;
                    }

                    _weaponSlots[i] = weaponInstance;

                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Will lerp the current position to the default position
    /// </summary>
    private void UpdateWeaponPutUpState()
    {
        if (_weaponPositionState == WeaponPositionState.PutUp)
        {
            if(_weaponPosition == _weaponDefaultPosition.localPosition)
            {
                _weaponPositionState = WeaponPositionState.Up;
            }
        }
    }

    //Will lerp the current weapon position to the ADS position
    private void UpdateWeaponAimState()
    {
        if (_playerController.IsAimingDownSight && _activeWeapon)
        {
            _weaponPositionState = WeaponPositionState.Aim;
        }
        else
        {
            if(_weaponPositionState == WeaponPositionState.Aim)
            {
                _weaponPositionState = WeaponPositionState.PutUp;
            }
        }
    }

    private void UpdateCameraFieldOfView()
    {
        switch (_weaponPositionState)
        {
            case WeaponPositionState.Aim:
                SetFieldOfView(Mathf.Lerp(_playerCamera.fieldOfView, _activeWeapon.ZoomRatio * _defaultFOV, _aimAnimationSpeed * Time.deltaTime));
                break;
            default:
                SetFieldOfView(Mathf.Lerp(_playerCamera.fieldOfView, _defaultFOV, _aimAnimationSpeed * Time.deltaTime));
                break;
        }
    }

    private void UpdateWeaponSprintState()
    {
        if (_playerController.IsSprinting)
        {
            _weaponPositionState = WeaponPositionState.Sprint;
        }
        else if (_weaponPositionState == WeaponPositionState.Sprint)
        {
            _weaponPositionState = WeaponPositionState.PutUp;
        }
    }

    private void UpdateWeaponPosition()
    {
        switch (_weaponPositionState)
        {
            case WeaponPositionState.PutUp:
                _weaponPosition = Vector3.Lerp(_weaponPosition, _weaponDefaultPosition.localPosition, _weaponPutUpAnimationSpeed * Time.deltaTime);
                break;
            case WeaponPositionState.Aim:
                _weaponPosition = Vector3.Lerp(_weaponPosition, _weaponAimingPosition.localPosition, _aimAnimationSpeed * Time.deltaTime);
                break;
            case WeaponPositionState.Sprint:
                _weaponPosition = Vector3.Lerp(_weaponPosition, _weaponSprintPosition.localPosition, _sprintAnimationSpeed * Time.deltaTime);
                break;
            default:
                _weaponPosition = Vector3.Lerp(_weaponPosition, _weaponDefaultPosition.localPosition, _weaponPutUpAnimationSpeed * Time.deltaTime);
                break;
        }
    }

    private void SetFieldOfView(float fov)
    {
        _playerCamera.fieldOfView = fov;
        _weaponCamera.fieldOfView = fov;
    }

    private void OnWeaponSwitched(Gun weapon)
    {
        if (weapon != null)
        {
            weapon.SetVisibility(true);
        }
    }

    public void PlayShootAnimation()
    {
        _activeWeapon.PlayShootAnimation();
    }
}
