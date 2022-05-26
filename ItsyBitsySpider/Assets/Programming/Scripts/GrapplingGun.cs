using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GrapplingRope grappleRope;

    [Header("Layers Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private int grappableLayerNumber = 9;


    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistnace = 20;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;
    [Header("Other")]
    [SerializeField] [Range(0, 2.0f)] float forgiveness;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    Controls controls;
    bool r1Pressed = false;
    Vector2 lookDirection = Vector2.up;
    GameObject debugCube;
    int mask;
    int blockMask;
    float gravScale;
    bool movingTarget;
    Transform target;
    Vector3 offset;
    public bool pull;
    private void Awake()
    {
        controls = new Controls();
        controls.Enable();
        controls.Movement.Enable();
        controls.Movement.Grapple.performed += ctx => Fire();
        controls.Movement.Grapple.performed += ctx => r1Pressed = true;
        controls.Movement.Freeze.performed += ctx => SetFreeze(true);
        controls.Movement.Freeze.canceled += ctx => SetFreeze(false);
        controls.Movement.Grapple.canceled += ctx => Release();
        controls.Movement.Grapple.canceled += ctx => r1Pressed = false;
        controls.Movement.xJoy.performed += ctx => SetDirection(ctx.ReadValue<Vector2>());
    }
    public void DisableControls()
    {
        controls.Enable();
        controls.Movement.Enable();
        controls.Movement.Grapple.performed -= ctx => Fire();
        controls.Movement.Grapple.performed -= ctx => r1Pressed = true;
        controls.Movement.Grapple.canceled -= ctx => Release();
        controls.Movement.Grapple.canceled -= ctx => r1Pressed = false;
        controls.Movement.Freeze.performed -= ctx => SetFreeze(true);
        controls.Movement.Freeze.canceled -= ctx => SetFreeze(false);
        controls.Movement.xJoy.performed -= ctx => SetDirection(ctx.ReadValue<Vector2>());
        controls.Movement.Disable();
        controls.Disable();
    }
    void SetFreeze(bool b)
    {
        pull = b;
    }
    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        debugCube = GameObject.Find("DebugCube");
        mask = LayerMask.GetMask("Grapple");
        blockMask = LayerMask.GetMask("Block");
        gravScale = m_rigidbody.gravityScale;
    }
    void UpdateGrapplePoint()
    {
        m_springJoint2D.connectedAnchor = target.position + offset;
        
    }

    public Vector3 GetTargetDynamic()
    {
        if(movingTarget)
        {
            return target.position + offset;
        }
        return grapplePoint;
    }
    private void Update()
    {
        
        if (r1Pressed)
        {
            if (grappleRope.enabled)
            {

                RotateGun(grapplePoint, false);
                debugCube.transform.position = Vector3.one * 1000;
                if (pull)
                {
                    m_springJoint2D.frequency = 2.0f;
                }
                else
                {
                    m_springJoint2D.frequency = launchSpeed;
                }
                if (movingTarget)
                {
                    UpdateGrapplePoint();
                }
            }
            else
            {
                RotateGun(lookDirection, true);
            }

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (launchType == LaunchType.Transform_Launch)
                {
                    Vector2 firePointDistnace = firePoint.position - gunHolder.localPosition;
                    Vector2 targetPos = grapplePoint - firePointDistnace;
                    gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
                    
                }
                
            }
        }
        else
        {
            Vector2 distanceVector = lookDirection;
            RaycastHit2D hit = Physics2D.CircleCast(firePoint.position, forgiveness, distanceVector.normalized, maxDistnace, mask);
            if(!hit)
            {
                debugCube.transform.position = Vector3.one * -1000f;
                return;
            }

            if (hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
            {
                Vector2 hitVector = hit.point - new Vector2(firePoint.position.x, firePoint.position.y);
                RaycastHit2D clearCheck = Physics2D.CircleCast(firePoint.position, 0.1f, hitVector.normalized, (new Vector3(hit.point.x, hit.point.y, 0) - firePoint.position).magnitude, blockMask);
                if (clearCheck | !hit)
                {
                    debugCube.transform.position = Vector3.one * -1000f;
                    return;
                }
                if (Vector2.Distance(hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
                {
                    debugCube.transform.position = new Vector3(hit.point.x, hit.point.y, -2.5f);

                }
            }
            RotateGun(lookDirection, true);
            
        }
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void SetGrapplePoint()
    {
        Vector2 distanceVector = lookDirection;
        RaycastHit2D hit = Physics2D.CircleCast(firePoint.position, forgiveness, distanceVector.normalized, maxDistnace,mask);
        if (!hit)
            return;

        if (hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
        {
            Vector2 hitVector = hit.point - new Vector2(firePoint.position.x, firePoint.position.y);
            RaycastHit2D clearCheck = Physics2D.CircleCast(firePoint.position, 0.1f, hitVector.normalized, (new Vector3(hit.point.x, hit.point.y, 0) - firePoint.position).magnitude, blockMask);
            if (clearCheck | !hit)
                return;
            if (Vector2.Distance(hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
            {
                target = hit.transform;
                grapplePoint = hit.point;
                offset = new Vector3(grapplePoint.x, grapplePoint.y, 0) - target.position;
                grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                grappleRope.enabled = true;
                if (hit.transform.gameObject.CompareTag("Moving"))
                {
                    movingTarget = true;
                }
                else
                {
                    movingTarget = false;
                }

            }
        }
        
    }

    public void SetGrapplePoint(Vector2 overrideVec)
    {
        r1Pressed = true;
        controls.Disable();
        Vector2 distanceVector = overrideVec;
        RaycastHit2D hit = Physics2D.CircleCast(firePoint.position, forgiveness, distanceVector.normalized, maxDistnace, mask);
        Vector2 hitVector = hit.point - new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D clearCheck = Physics2D.CircleCast(firePoint.position, 0.1f, hitVector.normalized, maxDistnace, blockMask);
        if (clearCheck | !hit)
            return;

        if (hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
        {
            if (Vector2.Distance(hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
            {
                target = hit.transform;
                grapplePoint = hit.point;
                offset = new Vector3(grapplePoint.x, grapplePoint.y, 0) - target.position;
                grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                grappleRope.enabled = true;
                if (hit.transform.gameObject.CompareTag("Moving"))
                {
                    movingTarget = true;
                }
                else
                {
                    movingTarget = false;
                }

            }
        }

    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;
        if (!launchToPoint && !autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }
        if (!launchToPoint)
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else
        {
            switch (launchType)
            {
                case LaunchType.Physics_Launch:
                    m_springJoint2D.connectedAnchor = grapplePoint;

                    Vector2 distanceVector = firePoint.position - gunHolder.position;

                    m_springJoint2D.distance = distanceVector.magnitude;
                    m_springJoint2D.frequency = launchSpeed;
                    m_springJoint2D.enabled = true;
                    break;
                case LaunchType.Transform_Launch:
                    m_rigidbody.gravityScale = 0;
                    m_rigidbody.velocity = Vector2.zero;
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistnace);
        }
    }

    void Fire()
    {
        SetGrapplePoint();
    }

    void Release()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        m_rigidbody.gravityScale = gravScale;
    }

    void SetDirection(Vector2 dir)
    {
        lookDirection = dir;
    }

    private void OnDestroy()
    {
        DisableControls();
    }
}