using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class CameraConstraint
{
    public string Name;
    public float mouseSensitivity, controllerSensitivity;
    public float Xmin, Xmax;
    public float Ymin, Ymax;
}

public class CameraController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float cameraZoom;
    public bool CanMove;
    public GameObject Rotator;
    [SerializeField] private List<CameraConstraint> constraints = new List<CameraConstraint>();

    [HideInInspector] public CinemachineVirtualCamera MainCam;
    [SerializeField] private CinemachineImpulseSource recoilSource, hurtSource, rumbleSource;
    private float xRotation, yRotation;
    private float baseFOV;

    private int currentConstraint;

    public float CurrentFOV
    {
        get { return cameraZoom; }
    }

    public float BaseFOV
    {
        get { return baseFOV; }
    }


    private void Awake()
    {
        MainCam = GetComponentInChildren<CinemachineVirtualCamera>();
        Cursor.lockState = CursorLockMode.Locked;
        SetConstraint("Base");
    }
    public void Init()
    {
        MainCam = GetComponentInChildren<CinemachineVirtualCamera>();
        Cursor.lockState = CursorLockMode.Locked;
        SetConstraint("Base");

        baseFOV = MainCam.m_Lens.FieldOfView;
        cameraZoom = baseFOV;
    }


    public void UpdateCamera()
    {
        if (CanMove)
            CamMovement();

        MainCam.m_Lens.FieldOfView = Mathf.Lerp(MainCam.m_Lens.FieldOfView, cameraZoom, 6 * Time.deltaTime);
    }

    private void CamMovement()
    {
        CameraConstraint constraint = constraints[currentConstraint];

        float mouseX = Input.GetAxis("Mouse X") * constraint.mouseSensitivity * 0.01f;
        float mouseY = Input.GetAxis("Mouse Y") * constraint.mouseSensitivity * 0.01f;


        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, constraint.Xmin, constraint.Xmax);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (Rotator != null)
            Rotator.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public void Recoil()
    {
        recoilSource.GenerateImpulse();
    }

    public void HurtShake()
    {
        hurtSource.GenerateImpulse();
    }

    public void Rumble()
    {
        rumbleSource.GenerateImpulse();
    }

    public void SetFOV(float fov)
    {
        cameraZoom = fov;
    }

    public void ResetFOV()
    {
        cameraZoom = baseFOV;
    }

    public void SetConstraint(string name)
    {
        foreach (var cam in constraints)
        {
            if (cam.Name == name)
            {
                currentConstraint = constraints.IndexOf(cam);
                break;
            }
        }
    }
}