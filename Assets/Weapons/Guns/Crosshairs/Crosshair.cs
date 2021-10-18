using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrosshairScale
{
    Default,
    Walk,
    Run,
    Shoot
}

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Vector3 _crosshairScale = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField]
    private Vector3 _crosshairWalkScale = new Vector3(0.02f, 0.02f, 0.02f);
    [SerializeField]
    private Vector3 _crosshairShootScale = new Vector3(0.02f, 0.02f, 0.02f);
    [SerializeField]
    private Vector3 _crosshairRunScale = new Vector3(0.03f, 0.03f, 0.03f);
    [SerializeField]
    private float _scaleSpeed = 2f;


    private CrosshairScale _currentScale = default;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = _crosshairScale;
        _currentScale = CrosshairScale.Default;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newScale = Vector3.zero;

        switch (_currentScale)
        {
            case CrosshairScale.Default:
                newScale = _crosshairScale;
                break;
            case CrosshairScale.Walk:
                newScale = _crosshairWalkScale;
                break;
            case CrosshairScale.Run:
                newScale = _crosshairRunScale;
                break;
            case CrosshairScale.Shoot:
                newScale = _crosshairShootScale;
                break;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, newScale, _scaleSpeed * Time.deltaTime);
    }

    //Will scale the crosshair up to the selected scale
    public void SetScale(CrosshairScale scale)
    {
        _currentScale = scale;
    }

    /// <summary>
    /// Will wait resetTime seconds before resetting the scale to default
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="resetTime"></param>
    public void SetScale(CrosshairScale scale, float resetTime)
    {
        if (isActiveAndEnabled)
        {
            _currentScale = scale;
            StartCoroutine(ResetCrosshair(resetTime));
        }
    }

    private IEnumerator ResetCrosshair(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        _currentScale = CrosshairScale.Default;
    }
}
