using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class MaterialCreator : EditorWindow
{
    Color _materialColor;
    string _materialName;
    string _saveFolder;
    ApplyMaterial _applyWindow;
    string _error;
    bool _button;
    bool _showError;
    [MenuItem("Custom/Material Creator")]
    public static void OpenWindow()
    {
        var window = GetWindow<MaterialCreator>();
        window.wantsMouseMove = true;
        window.Show();
    }

    private void OnGUI()
    {
        maxSize = new Vector2(350, 150);
        minSize = new Vector2(350, 150);
        CreateMaterial();
        if (_showError)
            ShowError();
        OpenApply();
    }

    private void OpenApply()
    {
        bool button = GUILayout.Button("Apply to objects");
        if (button)
        {
            _applyWindow = GetWindow<ApplyMaterial>();
            _applyWindow.wantsMouseMove = true;
            _applyWindow.Show();
        }
    }

    private void CreateMaterial()
    {
        _materialName = EditorGUILayout.TextField("Material Name", _materialName);
        _materialColor = EditorGUILayout.ColorField("Material Color", _materialColor);
        _button = GUILayout.Button("Create");
        if (_button)
        {
            if (_materialName == "")
            {
                _showError = true;
            }
            else
            {
                _showError = false;
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = _materialColor;
                string path = "Assets/" + _materialName + ".mat";
                AssetDatabase.CreateAsset(mat, AssetDatabase.GenerateUniqueAssetPath(path));
            }
        }
    }

    private void ShowError()
    {
        EditorGUILayout.HelpBox("Name can't be empty.", MessageType.Error);
    }
}
