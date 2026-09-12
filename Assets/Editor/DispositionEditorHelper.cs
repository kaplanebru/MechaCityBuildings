using System;
using UnityEditor;
using UnityEngine;

public class DispositionEditorHelper
{
    private int _selectedDispositionIndex;
    
    public string ShowDispositions(DispositionDb dispositionDb)
    {
        if (dispositionDb.Names == null || dispositionDb.Names.Length == 0)
        {
            EditorGUILayout.HelpBox("No dispositions to show.", MessageType.Info);
            return null;
        }

        _selectedDispositionIndex =
            Mathf.Clamp(_selectedDispositionIndex, 0, dispositionDb.Names.Length - 1);

        _selectedDispositionIndex = EditorGUILayout.Popup(
            "Arrangement",
            _selectedDispositionIndex,
            dispositionDb.Names,
            GUILayout.MaxWidth(400)
        );

        return dispositionDb.Names[_selectedDispositionIndex];
    }

   
}