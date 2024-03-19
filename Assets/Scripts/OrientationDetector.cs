using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationDetector : MonoBehaviour
{
    [SerializeField]
    private GameObject _alert;

    private void Update()
    {
        _alert.SetActive(Screen.height > Screen.width);
    }
}
