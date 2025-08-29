using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class CustomOverrideFiller : EditorWindow
{
    private AnimatorOverrideController overrideController;
    private DefaultAsset inputFolder;

    [MenuItem("Tools/Custom Animator Override Filler")]
    public static void ShowWindow()
    {
        GetWindow<CustomOverrideFiller>("Custom Override Filler");
    }

    void OnGUI()
    {
        GUILayout.Label("Custom Animator Override Filler", EditorStyles.boldLabel);

        overrideController = (AnimatorOverrideController)EditorGUILayout.ObjectField("Override Controller", overrideController, typeof(AnimatorOverrideController), false);
        inputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Animation Folder", inputFolder, typeof(DefaultAsset), false);

        if (GUILayout.Button("Fill Overrides"))
        {
            FillOverrides();
        }
    }

    void FillOverrides()
    {
        if (overrideController == null || inputFolder == null)
        {
            Debug.LogError("Missing Override Controller or Folder!");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(inputFolder);
        string folderName = Path.GetFileName(folderPath);
        string[] animPaths = Directory.GetFiles(folderPath, "*.anim", SearchOption.AllDirectories);

        var newClips = animPaths
            .Select(p => AssetDatabase.LoadAssetAtPath<AnimationClip>(p))
            .Where(c => c != null)
            .ToList();

        if (newClips.Count == 0)
        {
            Debug.LogWarning("No animation clips found in " + folderPath);
            return;
        }

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);

        for (int i = 0; i < overrides.Count; i++)
        {
            AnimationClip origClip = overrides[i].Key;
            if (origClip == null) continue;

            string origName = origClip.name;
            AnimationClip matched = FindMatch(origName, newClips, folderName);

            if (matched != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(origClip, matched);
                Debug.Log($"Mapped {origName} -> {matched.name}");
            }
            else
            {
                Debug.LogWarning($"No match found for {origName}, {folderName}");
            }
        }

        overrideController.ApplyOverrides(overrides);
        EditorUtility.SetDirty(overrideController);
        AssetDatabase.SaveAssets();

        Debug.Log("Overrides applied successfully!");
    }

    AnimationClip FindMatch(string origName, List<AnimationClip> candidates, string folderName)
    {
        var match = candidates.FirstOrDefault(c => origName.Contains(c.name.Substring(folderName.Length)));
        return match;
    }
}
