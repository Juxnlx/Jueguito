
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class PlayModeStartScene
{
    static PlayModeStartScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
        }
    }
}