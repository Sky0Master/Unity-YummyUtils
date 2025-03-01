using UnityEngine;

namespace Yummy{
public class FlipWithMovement : MonoBehaviour
{
   public bool autoFlipX = true;
    public bool isSetAnimator = true;
    public string verticalMoveKey = "Vertical";
    public string horizontalMoveKey = "Horizontal";
    public string isMovingKey = "IsMoving";
    public float sens = 0.01f;
    private Vector3 _lastPos;
    private float _moveOffsetX;
    private float _moveOffsetY;
    private bool _isMoving;
    void Start()
    {
        _lastPos = transform.position;
    }
    void Flip()
    {
        var scalex = Mathf.Abs(transform.localScale.x);
        if(_moveOffsetX > 0)
        {
            transform.localScale = new Vector3(scalex ,transform.localScale.y,transform.localScale.z);
        }
        else if (_moveOffsetX < 0) 
        {
            transform.localScale = new Vector3(-scalex,transform.localScale.y,transform.localScale.z);
        }
    }
    void UpdateAnimator()
    {
        var animator = GetComponent<Animator>();
        if(animator == null)
        {
            return;
        }
        if(Mathf.Abs(_moveOffsetX) > sens)
        {
            animator.SetFloat(horizontalMoveKey,Mathf.Sign(_moveOffsetX));
        }
        else {
            animator.SetFloat(horizontalMoveKey,0);
        }

        if(Mathf.Abs(_moveOffsetY) > sens)
        {
            animator.SetFloat(verticalMoveKey,Mathf.Sign(_moveOffsetY));
        }
        else{
            animator.SetFloat(verticalMoveKey,0);
        }

        animator.SetBool(isMovingKey , _isMoving);

    }
    void FixedUpdate()
    {
        _moveOffsetX = transform.position.x - _lastPos.x;
        _moveOffsetY = transform.position.y - _lastPos.y;
        _lastPos = transform.position;
    }
    void Update()
    {
        if(new Vector2(_moveOffsetX,_moveOffsetY).magnitude > sens )
        {
            _isMoving = true;
        }
        else{
            _isMoving = false;
        }

        if(autoFlipX)
        {
            Flip();
        }
        if(isSetAnimator)
        {
            UpdateAnimator();
        }
        
    }
}
}
