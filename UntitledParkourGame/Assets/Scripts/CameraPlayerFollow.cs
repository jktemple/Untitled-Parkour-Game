using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerFollow : Singleton<CameraPlayerFollow> 
{
    private CinemachineVirtualCamera _camera;
    private void Awake()
    {
        _camera= GetComponent<CinemachineVirtualCamera>();
    }

    public void CameraFollow(Transform target)
    {
        _camera.Follow = target;
    }

    private void Update()
    {
        //Debug.Log("Field of View = " + _camera.m_Lens.FieldOfView);
    }

}

