using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(Matrix4x4))]
public class MatrixInspector : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        // Add a label for the property
        var label = new Label(property.displayName);
        container.Add(label);

        // Arrange the matrix elements in a four by four grid
        for (int row = 0; row < 4; row++)
        {
            var rowContainer = new VisualElement();
            rowContainer.style.flexDirection = FlexDirection.Row;

            for (int col = 0; col < 4; col++)
            {
                var element = property.FindPropertyRelative($"e{row}{col}");
                var field = new PropertyField(element, "");
                field.style.width = 50;
                rowContainer.Add(field);
            }

            container.Add(rowContainer);
        }

        return container;
    }
}
