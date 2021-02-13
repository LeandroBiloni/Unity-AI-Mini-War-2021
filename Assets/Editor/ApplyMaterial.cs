using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class ApplyMaterial : EditorWindow
{
    Material _mat;
    float _size;
    MeshRenderer[] _array;
    MeshRenderer[] _copy;
    Material[] _matArray;
    Material[] _matCopy;
    bool[] _boolArray;
    float _matsSize;
    bool _multiple;
    int _selectedMaterial;
    bool _showError;


    public static void OpenWindow()
    {
        var window = GetWindow<ApplyMaterial>();
        window.wantsMouseMove = true;
        window.Show();
    }

    private void OnGUI()
    {
        maxSize = new Vector2(1000, 500);
        minSize = new Vector2(1000, 500);
        Apply();
    }

    private void Apply()
    {
        GUILayout.BeginArea(new Rect(4, 5, 450, 500));
        _multiple = EditorGUILayout.Toggle("Multiple Materials", _multiple);
        if (_multiple == false)
        {
            if (_matArray == null)
                _matArray = new Material[1];
            _matArray[0] = (Material)EditorGUILayout.ObjectField("Material", _matArray[0], typeof(Material), false);
            _selectedMaterial = 0;
        }
            
        else
        {
            var matsAux = _matsSize;
            _matsSize = EditorGUILayout.Slider("Objects quantity", (int)_matsSize, 1, 10);

            if (_matsSize != matsAux)
            {
                //Creo array de copia
                if (_matArray != null)
                {
                    _matCopy = new Material[_matArray.Length];
                    //guardo una copia de todos los materiales
                    _matCopy = _matArray;
                }

                _matArray = new Material[(int)_matsSize];
                _boolArray = new bool[(int)_matsSize];

                if (_matCopy != null)
                {
                    if (_matArray.Length >= _matCopy.Length)
                    {
                        for (int i = 0; i < _matCopy.Length; i++)
                        {
                            _matArray[i] = _matCopy[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _matArray.Length; i++)
                        {
                            _matArray[i] = _matCopy[i];
                        }
                    }
                }
            }
            for (int i = 0; i < (int)_matsSize; i++)
            {
                _matArray[i] = (Material)EditorGUILayout.ObjectField("Material " + (i + 1), _matArray[i], typeof(Material), true);
                _boolArray[i] = EditorGUILayout.Toggle("Use this material", _boolArray[i]);
            }

            for (int i = 0; i < _boolArray.Length; i++)
            {
                //Me fijo que material tengo seleccionado con el bool
                if (_boolArray[i] && _selectedMaterial != i)
                {
                    //Guardo el indice del material
                    _selectedMaterial = i;
                    for (int j = 0; j < _boolArray.Length; j++)
                    {
                        //Hago false todos los bool menos el seleccionado
                        if (j != i)
                            _boolArray[j] = false;
                    }
                }
            }
        }
        GUILayout.EndArea();


        GUILayout.BeginArea(new Rect(545, 5, 450, 500));
        var sizeaux = _size;
        _size = EditorGUILayout.Slider("Objects quantity", (int)_size, 1, 10);
        
        if (_size != sizeaux)
        {
            //Creo array de copia
            if(_array != null)
            {
                _copy = new MeshRenderer[_array.Length];
                //Guardo una copia de todos los objetos
                _copy = _array;
            }

            _array = new MeshRenderer[(int)_size];

            //Recupero la copia de los objetos
            if (_copy != null)
            {
                if (_array.Length >= _copy.Length)
                {
                    for (int i = 0; i < _copy.Length; i++)
                    {
                        _array[i] = _copy[i];
                    }
                }
                else
                {
                    for (int i = 0; i < _array.Length; i++)
                    {
                        _array[i] = _copy[i];
                    }
                } 
            }
        }

        for (int i = 0; i < (int)_size; i++)
        {

            _array[i] = (MeshRenderer)EditorGUILayout.ObjectField("Object " + (i+1) , _array[i], typeof(MeshRenderer), true);

            bool button = GUILayout.Button("Apply material", GUILayout.Width(200));

            if (button)
            {
                if (_array[i] != null && _matArray[_selectedMaterial] != null)
                {
                    _array[i].material = _matArray[_selectedMaterial];
                    if (_showError)
                    {
                        _showError = false;
                    }
                }
                else _showError = true;
            }
        }
        if (_showError)
            ShowError();
        else this.Repaint(); //Para borrar los mensajes de error

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(300, maxSize.y - 50, 300, 300));
        
        bool buttonAll = GUILayout.Button("Apply material to all objects");
        if (buttonAll)
        {
            for (int i = 0; i < _array.Length; i++)
            {
                if (_array[i] != null && _matArray[_selectedMaterial] != null)
                {
                    _array[i].material = _matArray[_selectedMaterial];
                }
            }
        }
        GUILayout.EndArea();
    }

    private void ShowError()
    {
        if (_matArray[_selectedMaterial] == null)
        {
            EditorGUILayout.HelpBox("Missing material.", MessageType.Error);
        }
        else
        {
            EditorGUILayout.HelpBox("Missing object.", MessageType.Error);
        }
    }
}
