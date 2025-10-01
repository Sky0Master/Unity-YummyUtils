using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 用于创建运行时独立的材质
/// </summary>
public class MaterialHelper : MonoBehaviour
{
    [SerializeField] Material sourceMat;
    [Tooltip("勾选后所有子物体会和该物体用同一个材质")]
    [SerializeField] bool applyToChildren = true;
    List<Renderer> _render = new List<Renderer>();

    [HideInInspector]
    public Material runtimeMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _render = GetComponentsInChildren<Renderer>().ToList();
        _render.Add(GetComponent<Renderer>());
        runtimeMaterial = new Material(sourceMat);
        if (applyToChildren)
        {
            foreach (var r in _render)
            {
                if (r == null) continue;
                r.material = runtimeMaterial;
            }
        }
    }
}
