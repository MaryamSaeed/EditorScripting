using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
internal sealed class ObjectsList : ScriptableObject
{
    public List<GameObject> ObjectsToBeReplaced;
}
// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(ObjectsList))]
internal sealed class ObjectsListDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        //var listField = new PropertyField(property,"Objects to be replaced");
        //listField.BindProperty(property);
        //return listField;

        var test = new ListView();
        test.BindProperty(property);
        test.headerTitle = "Objects to be replaced";
        test.reorderMode = ListViewReorderMode.Animated;
        test.showFoldoutHeader = true;
        test.showAddRemoveFooter = true;
        test.showBorder = true;
        return test;
    }
}