// using System.Collections.Generic;
// using QFramework;
// using UnityEngine;
// using WE.UI;

// /// <summary>
// /// 负责UI面板的加载.每个UI Panel预制体的根节点必须有Canvas组件
// /// </summary>
// public class UIPanelManager : MonoSingleton<UIPanelManager>
// {
//     ResLoader _resloader = ResLoader.Allocate();
//     Transform _uiRoot;
//     Camera _uiCamera;
//     Dictionary<string, GameObject> _panelDict = new Dictionary<string, GameObject>();
//     void Start()
//     {
//         _uiRoot = GameObject.Find("UIRoot").transform;
//         _uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
//     }
//     public GameObject LoadPanel(string panelName, bool Active = true)
//     {
//         var panelPrefab = _resloader.LoadSync<GameObject>(panelName);
//         var panelInstance = Instantiate(panelPrefab, _uiRoot);
//         panelInstance.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
//         panelInstance.GetComponent<Canvas>().worldCamera = _uiCamera;
//         panelInstance.SetActive(Active);
//         _panelDict[panelName] = panelInstance;
//         return panelInstance;
//     }

//     public void DestroyPanel(string name)
//     {
//         if (_panelDict.ContainsKey(name))
//         {
//             Destroy(_panelDict[name]);
//             _panelDict.Remove(name);
//         }
//     }
//     public GameObject GetPanel(string name)
//     {
//         if (_panelDict.ContainsKey(name))
//         {
//             return _panelDict[name];
//         }
//         return null;
//     }

// }
