using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class BatchDirectionalAnimGenerator : EditorWindow
{
    private DefaultAsset inputFolder;
    private string savePath = "Assets/Animations";
    private int fps = 12;

    [MenuItem("Tools/Batch Directional SpriteSheet Generator")]
    public static void ShowWindow()
    {
        GetWindow<BatchDirectionalAnimGenerator>("Batch Anim Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Directional Animation Generator", EditorStyles.boldLabel);

        inputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Input Folder", inputFolder, typeof(DefaultAsset), false);
        savePath = EditorGUILayout.TextField("Save Path", savePath);
        fps = EditorGUILayout.IntField("FPS", fps);

        if (GUILayout.Button("Generate Animations for All SpriteSheets"))
        {
            GenerateAll();
        }
    }

    void GenerateAll()
    {
        if (inputFolder == null) return;

        string folderPath = AssetDatabase.GetAssetPath(inputFolder);
        string[] files = Directory.GetFiles(folderPath, "*.png", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            Debug.Log("Processing: " + file);
            GenerateAnimationsForFile(file);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void GenerateAnimationsForFile(string path)
    {
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
        var spriteList = sprites.OfType<Sprite>().OrderBy(x => int.Parse(x.name.Split('_').Last())).ToList();
        if (spriteList.Count == 0)
        {
            Debug.LogWarning("No sprites found in " + path);
            return;
        }

        string baseName = Path.GetFileNameWithoutExtension(path);
        string[] parts = baseName.Split('_');
        if (parts.Length < 2)
        {
            Debug.LogError("Expected filename format like Beholder1_Attack_full.png, got " + baseName);
            return;
        }
        string prefix = parts[0] + parts[1];

        int groupSize = spriteList.Count / 4;
        if (groupSize == 0)
        {
            Debug.LogError("Not enough frames in " + baseName + " to split into 4 animations.");
            return;
        }

        string[] directions = { "Down", "Up", "Left", "Right" };

        for (int i = 0; i < 4; i++)
        {
            var frames = spriteList.Skip(i * groupSize).Take(groupSize).ToList();
            string animName = prefix + directions[i];
            CreateAnimationClip(animName, frames);
        }
    }

    void CreateAnimationClip(string animName, List<Sprite> frames)
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = fps;

        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[frames.Count];
        for (int i = 0; i < frames.Count; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / clip.frameRate,
                value = frames[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyFrames);

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string clipPath = Path.Combine(savePath, animName + ".anim");
        AssetDatabase.CreateAsset(clip, clipPath);

        Debug.Log("Created animation: " + animName);
    }
}
