using UnityEngine;

namespace Yummy
{
    /// <summary>
    /// for RTS 支持鼠标拖拽移动，滚轮缩放，边界限制
    /// </summary>
    public class RTSCameraController2D : MonoBehaviour
    {
        [Header("Zoom Settings")]
        public float zoomSpeed = 2f;
        public float zoomLerpSpeed = 10f;
        public float minSize = 4f;
        public float maxSize = 7f;

        public enum MouseDragButton
        {
            LeftClick = 0,
            RightClick = 1,
            MiddleClick = 2 // 也可以根据需要添加中键
        }

        [Header("Drag Move Settings")]
        public MouseDragButton dragMouseButton = MouseDragButton.MiddleClick; // 默认右键拖拽
        public float dragMoveSpeed = 0.05f; // 调整这个值来控制拖拽移动的速度
        public float dragLerpSpeed = 10f; // 保持平滑移动的插值速度

        [Header("Bounds")]
        public Transform bottomLeftBound;
        public Transform topRightBound;

        private Camera cam;
        private float targetSize;
        private Vector3 targetPosition;

        private Vector3 lastMousePosition; // 用于记录鼠标拖拽的起始位置

        void Start()
        {
            cam = Camera.main;
            targetSize = cam.orthographicSize;
            targetPosition = transform.position;
        }

        void Update()
        {
            HandleZoom();
            HandleMouseDrag(); // 调用统一的拖拽方法
        }

        void LateUpdate()
        {
            // 平滑缩放
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomLerpSpeed);

            // 限制移动范围
            targetPosition = ClampPosition(targetPosition, cam.orthographicSize);
            // 使用 dragLerpSpeed 来平滑移动
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * dragLerpSpeed);
        }

        void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                float desiredSize = targetSize - scroll * zoomSpeed;
                desiredSize = Mathf.Clamp(desiredSize, minSize, maxSize);

                // 检查缩放后是否仍在边界内
                // 这里传递desiredSize是为了在缩放前预检查，防止缩放到无法覆盖边界的尺寸
                if (!IsSizeWithinBounds(desiredSize))
                    return;

                // 缩放前鼠标指向的世界位置
                Vector3 mouseWorldBefore = cam.ScreenToWorldPoint(Input.mousePosition);

                // 更新目标缩放大小
                targetSize = desiredSize;

                // 缩放后鼠标指向的世界位置
                // 注意：这里需要再次调用 cam.ScreenToWorldPoint，因为 targetSize 已经更新了
                Vector3 mouseWorldAfter = cam.ScreenToWorldPoint(Input.mousePosition);

                // 缩放时相机要调整位置以保持鼠标点不动
                Vector3 offset = mouseWorldBefore - mouseWorldAfter;
                targetPosition += new Vector3(offset.x, offset.y, 0);
            }
        }

        /// <summary>
        /// 处理按住鼠标拖拽移动相机（根据设定的按键）
        /// </summary>
        void HandleMouseDrag()
        {
            int buttonIndex = (int)dragMouseButton; // 将枚举转换为对应的鼠标按键索引

            // 检测鼠标按键是否按下
            if (Input.GetMouseButtonDown(buttonIndex))
            {
                lastMousePosition = Input.mousePosition; // 记录鼠标按下时的位置
            }

            // 检测鼠标按键是否按住
            if (Input.GetMouseButton(buttonIndex))
            {
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 deltaMouse = currentMousePosition - lastMousePosition;

                // 计算移动量，方向与鼠标拖拽方向相反
                // dragMoveSpeed * cam.orthographicSize / Screen.height 确保在不同缩放级别下移动速度一致
                Vector3 move = new Vector3(-deltaMouse.x, -deltaMouse.y, 0) * dragMoveSpeed * cam.orthographicSize / Screen.height;

                targetPosition += move; // 更新目标位置
                lastMousePosition = currentMousePosition; // 更新上一次鼠标位置
            }
        }

        /// <summary>
        /// 限制相机位置在边界内
        /// </summary>
        /// <param name="pos">待限制的位置</param>
        /// <param name="orthoSize">当前相机正交大小</param>
        /// <returns>限制后的位置</returns>
        Vector3 ClampPosition(Vector3 pos, float orthoSize)
        {
            float aspect = (float)Screen.width / Screen.height;
            float camHalfHeight = orthoSize;
            float camHalfWidth = orthoSize * aspect;

            // 计算相机当前视口的边界
            // 相机中心点减去一半的视口宽度/高度
            float minX = bottomLeftBound.position.x + camHalfWidth;
            float maxX = topRightBound.position.x - camHalfWidth;
            float minY = bottomLeftBound.position.y + camHalfHeight;
            float maxY = topRightBound.position.y - camHalfHeight;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            return pos;
        }

        /// <summary>
        /// 检查相机在给定缩放大小下是否仍在边界内
        /// </summary>
        /// <param name="testSize">要测试的相机正交大小</param>
        /// <returns>是否在边界内</returns>
        bool IsSizeWithinBounds(float testSize)
        {
            // 注意：这里需要考虑当前targetPosition，而不是transform.position，因为它是目标位置
            Vector3 currentTargetPosition = targetPosition;

            float aspect = (float)Screen.width / Screen.height;
            float camHalfHeight = testSize;
            float camHalfWidth = testSize * aspect;

            // 计算当前视口边界
            float left = currentTargetPosition.x - camHalfWidth;
            float right = currentTargetPosition.x + camHalfWidth;
            float bottom = currentTargetPosition.y - camHalfHeight;
            float top = currentTargetPosition.y + camHalfHeight;

            // 检查视口边界是否完全在设置的边界内
            return (
                left >= bottomLeftBound.position.x &&
                right <= topRightBound.position.x &&
                bottom >= bottomLeftBound.position.y &&
                top <= topRightBound.position.y
            );
        }

        /// <summary>
        /// 在Scene视图中绘制边界Gizmos
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (bottomLeftBound && topRightBound)
            {
                Gizmos.color = Color.green;
                Vector3 center = (bottomLeftBound.position + topRightBound.position) / 2f;
                Vector3 size = new Vector3(
                    Mathf.Abs(topRightBound.position.x - bottomLeftBound.position.x),
                    Mathf.Abs(topRightBound.position.y - bottomLeftBound.position.y),
                    0f
                );
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}