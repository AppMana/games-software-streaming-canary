using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AppMana.SoftwareStreamingCanary.Editor
{
  public static class CanaryProjectSetup
  {
    public static void CreateScene()
    {
      const string sceneDirectory = "Assets/Scenes";
      const string scenePath = sceneDirectory + "/SoftwareStreamingCanary.unity";
      Directory.CreateDirectory(sceneDirectory);

      var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
      var cameraObject = new GameObject("Main Camera");
      cameraObject.tag = "MainCamera";
      cameraObject.AddComponent<Camera>();
      cameraObject.AddComponent<StreamingCanaryPattern>();

      EditorSceneManager.SaveScene(scene, scenePath);
      EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };
      AssetDatabase.SaveAssets();
      Debug.Log("SoftwareStreamingCanary scene is ready");
    }
  }
}
