using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SceneViewClipFix
{
    static SceneViewClipFix()
    {
        SceneView.duringSceneGui += view =>
        {
            view.camera.nearClipPlane = 0.01f;
            view.camera.farClipPlane = 10000f;
        };
    }
}
