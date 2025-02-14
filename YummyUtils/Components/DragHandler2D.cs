using UnityEngine;

namespace Yummy{
public class DragHandler2D : MonoBehaviour
{
    private bool isDragging;
    private Vector2 offset;
    public bool canRotate = false;
    public float rotateAngle = 45f;

    public bool smoothRotate = false;

    public bool isBanCollision = true;
    
    bool _istriggerTmp;

    void OnMouseDown()
    { 
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.SetParent(null);
    }
    void OnMouseUp()
    {
        isDragging = false;
        GetComponent<Collider2D>().isTrigger = _istriggerTmp;
    }

    public void HandleDrag()
    {
        if(!isDragging) return;
        if(isBanCollision)
        {
            var collider = GetComponent<Collider2D>();
            _istriggerTmp = collider.isTrigger;
            collider.isTrigger = true;
        }
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x + offset.x, pos.y + offset.y, 0);
    }
    public void HandleRotate()
    {
        if (!isDragging || !canRotate) return;
        if(smoothRotate && Input.GetKey(KeyCode.R))
        {
            transform.Rotate(0, 0, rotateAngle * Time.deltaTime);
        } 
        else if(Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(0, 0, rotateAngle);
        }
    }
    void Update()
    {
       HandleDrag();
       HandleRotate();
    }
}
}