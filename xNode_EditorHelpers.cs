using UnityEngine;
using UnityEditor;
using State_Machine.Core;
using XNodeEditor;

namespace State_Machine.xNode
{
    public static class xNode_EditorHelpers
    {
        public static GUIStyle InfoStyle()
        {
            GUIStyle style = new GUIStyle(EditorStyles.textArea);
            style.stretchWidth = false;

            style.padding.left = 15;
            style.padding.top = 6;
            style.padding.bottom = 6;
            style.padding.right = 15;

            style.margin.left = 25;
            style.margin.right = 20;
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Normal;
            style.fontSize = 12;
            return style;
        }
        public static GUIStyle HeaderStyle()
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            style.margin.top = 5;
            style.margin.bottom = 5;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            return style;
        }
        public static Color ParseHexToColor(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out Color color);
            return color;
        }

        public static void DrawFullNode(SM_Node contents)
        {
            StartVerticalNode(contents);

            GUIStyle headerStyle = HeaderStyle();
            GUIStyle valueStyle = InfoStyle();

            GUILayout.Label("Description:", headerStyle);
            GUILayout.Label(contents.description, valueStyle);
            GUILayout.Space(5);
            GUILayout.Label("Category:", headerStyle);
            GUILayout.Label(contents.category.ToString(), valueStyle);
            GUILayout.Space(5);
            GUILayout.Label("Required Components:", headerStyle);

            foreach (var comp in contents.requiredComponentTypes)
                GUILayout.Label(comp.ToString(), valueStyle);

            EndVerticleNode();
        }
        public static void StartVerticalNode(SM_Node contents, string header = null)
        {
            if (header == null)
                header = contents.name;
            Color guiColor = GUI.color;
            GUI.color = GetColor(contents);

            GUILayout.BeginVertical(NodeEditorResources.styles.nodeBody);
            GUI.color = guiColor;

            GUILayout.Label(header, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));

        }
        public static void EndVerticleNode()
        {
            GUILayout.EndVertical();
        }

        static Color GetColor(SM_Node contents)
        {
            if (contents is SM_StateNode)
                return ParseHexToColor("#114496");
            if (contents is SM_TransitionNode)
                return ParseHexToColor("#5a1196");
            else
                throw new System.Exception();

        }
        public static void PanToNode(xNode_StateNode node)
        {
            NodeEditorWindow window = EditorWindow.GetWindow(typeof(NodeEditorWindow)) as NodeEditorWindow;
            if (window != null)
            {
                window.PanTo(node);
            }
        }
    }
}
