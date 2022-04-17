using XNodeEditor;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using State_Machine.Core;

namespace State_Machine.xNode
{
    [CustomEditor(typeof(xNode_EventNode), true)]
    public class xNode_EventInspector : GlobalNodeEditor
    {
        const string componentDropDownLabel = "Component Type";
        const string eventDropDownLabel = "Event";

        const string statesThatExitLabel = "States that exit to this Event";
        const string statesUnconnectedLabel = "Unconnected States";

        xNode_EventNode node => (xNode_EventNode)target;
        StateGraph graph => (StateGraph)node.graph;
        #region Event Type and Event Enum
        Dictionary<Type, Enum[]> typeEventEnumDictionary;

        Type[] componentOptions;
        Type selectedComponentType;
        string[] componentOptionsAsStrings;
        int currentComponentIndex;

        Enum[] eventEnumOptions;
        Enum selectedEventEnum;
        string[] eventEnumsAsStrings;
        int currentEventEnumIndex;

        private void SetupEventTypeAndEnums()
        {
            SetupTypeEventDictionary();
            selectedComponentType = node.componentType.type;
            selectedEventEnum = node.GetEnum();

            //Get all CreatureComponent scripts in project
            UpdateTypeOptions();
            UpdateEventEnumOptions();

            currentComponentIndex = GetIndexOfType(selectedComponentType);
            currentEventEnumIndex = GetIndexOFEnum(selectedEventEnum);

        }
        void UpdateTypeOptions()
        {
            List<Type> types = new List<Type>();
            foreach (Type t in typeEventEnumDictionary.Keys)
                types.Add(t);
            componentOptions = types.ToArray();
            componentOptionsAsStrings = GetOptionsAsStrings(componentOptions);
        }
        void UpdateEventEnumOptions()
        {
            eventEnumOptions = typeEventEnumDictionary[selectedComponentType];
            eventEnumsAsStrings = GetOptionsAsStrings(eventEnumOptions);
        }

        int GetIndexOfType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type", "Cannot get index for null Type");

            for (int i = 0; i < componentOptions.Length; i++)
                if (componentOptions[i] == type)
                    return i;
            throw new InvalidOperationException($"Type of {type} does not exist in componentOptions array");
        }
        int GetIndexOFEnum(Enum eventEnum)
        {
            if (eventEnum == null) throw new ArgumentNullException("enum", "Cannot get index for null Enum");

            for (int i = 0; i < eventEnumOptions.Length; i++)
                if (eventEnumOptions[i].GetType() == eventEnum.GetType())
                    return i;
            foreach (Enum e in eventEnumOptions)
                Debug.Log(e);
            throw new InvalidOperationException($"Enum of {eventEnum} does not exist in eventEnumOptions array with length {eventEnumOptions.Length}");
        }

        string[] GetOptionsAsStrings(object[] array)
        {
            List<string> objsAsString = new List<string>();
            foreach (object obj in array)
            {
                if (obj == null)
                    throw new Exception($"Null option in State options for {node.name}");
                objsAsString.Add(obj.ToString());
            }
            return objsAsString.ToArray();
        }

        void ComponentOptionDropDown()
        {
            if (componentOptions == null || componentOptions.Length == 0) return;

            EditorGUILayout.LabelField(componentDropDownLabel, EditorStyles.boldLabel);
            int newIndex = EditorGUILayout.Popup(currentComponentIndex, componentOptionsAsStrings);

            if (newIndex != currentComponentIndex)
                SelectedComponentIndexChanged(newIndex);
        }
        void SelectedComponentIndexChanged(int newIndex)
        {
            currentComponentIndex = newIndex;
            selectedComponentType = componentOptions[newIndex];
            UpdateEventEnumOptions();
            SelectedEventEnumIndexChanged(0);
        }
        void EventEnumDropDown()
        {
            if (eventEnumOptions == null || eventEnumOptions.Length == 0) return;

            EditorGUILayout.LabelField(eventDropDownLabel, EditorStyles.boldLabel);
            int newIndex = EditorGUILayout.Popup(currentEventEnumIndex, eventEnumsAsStrings);

            if (newIndex != currentEventEnumIndex)
                SelectedEventEnumIndexChanged(newIndex);
        }
        void SelectedEventEnumIndexChanged(int newIndex)
        {
            currentEventEnumIndex = newIndex;
            selectedEventEnum = eventEnumOptions[newIndex];
            node.SetTypeAndEnum(selectedComponentType, selectedEventEnum);
        }

        void SetupTypeEventDictionary()
        {
            Dictionary<Type, Type> dict = EventLoading.GetAllTypesImplementingEnumsAndTheirEnums();

            typeEventEnumDictionary = new Dictionary<Type, Enum[]>();

            foreach (KeyValuePair<Type, Type> pair in dict)
            {
                //Debug.Log($"Adding pair Key: {pair.Key} value: {pair.Value}");
                typeEventEnumDictionary.Add(pair.Key, pair.Value.ToEnumArray());
                //foreach (Enum e in pair.Value.ToEnumArray())
                //    Debug.Log(e);
            }
        }

        #endregion

        void DrawAllStateNodes()
        {
            List<xNode_StateNode> triggeredNodes = new List<xNode_StateNode>();
            List<xNode_StateNode> untriggeredNodes = new List<xNode_StateNode>();
            foreach (xNode_StateNode stateNode in graph.stateNodes)
            {
                bool stateGetsTriggered = node.nodesThisTriggers.Contains(stateNode);
                if (stateGetsTriggered)
                    triggeredNodes.Add(stateNode);
                else
                    untriggeredNodes.Add(stateNode);
            }
            GUILayout.BeginVertical();
            GUILayout.Space(20);
            GUILayout.Label(statesThatExitLabel, ButtonLabelStyle());
            GUILayout.Space(2);
            foreach (xNode_StateNode stateNode in triggeredNodes)
                DrawStateNode(stateNode, true);

            GUILayout.Space(20);
            GUILayout.Label(statesUnconnectedLabel, EditorStyles.boldLabel);
            GUILayout.Space(2);
            foreach (xNode_StateNode stateNode in untriggeredNodes)
                DrawStateNode(stateNode, false);
            GUILayout.EndVertical();
        }


        void DrawStateNode(xNode_StateNode stateNode, bool stateIsAdded)
        {
            xNode_EditorHelpers.StartVerticalNode(stateNode.smNode, stateNode.name);
            //Color guiColor = GUI.color;
            //GUI.color = StateColor(stateIsAdded); 

            //GUILayout.BeginVertical(NodeEditorResources.styles.nodeBody);
            //GUI.color = guiColor;

            //GUILayout.Label(stateNode.contents.displayName, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));

            GUILayout.BeginHorizontal();
            if (stateIsAdded)
                DrawRemoveButton(stateNode);
            else
                DrawAddButton(stateNode);
            DrawShowNodeButton(stateNode);
            GUILayout.EndHorizontal();

            xNode_EditorHelpers.EndVerticleNode();
            //GUILayout.EndVertical();
        }
        void DrawRemoveButton(xNode_StateNode stateNode)
        {
            Color backgroundColor = GUI.backgroundColor;
            Color guiColor = GUI.color;

            GUI.color = Color.white;
            GUI.backgroundColor = RemoveButtonColor();

            if (GUILayout.Button("Remove", ButtonStyle()))
                node.RemoveTriggeredNode(stateNode);

            GUI.color = guiColor;
            GUI.backgroundColor = backgroundColor;
        }
        void DrawAddButton(xNode_StateNode stateNode)
        {
            Color backgroundColor = GUI.backgroundColor;
            Color guiColor = GUI.color;

            GUI.color = Color.white;
            GUI.backgroundColor = AddButtonColor();

            if (GUILayout.Button("Add", ButtonStyle()))
                node.AddTriggeredNode(stateNode);

            GUI.color = guiColor;
            GUI.backgroundColor = backgroundColor;
        }

        void DrawShowNodeButton(xNode_StateNode stateNode)
        {
            if (GUILayout.Button("Hop to node", ButtonStyle()))
                xNode_EditorHelpers.PanToNode(stateNode);
        }

        GUIStyle ButtonStyle()
        {
            GUIStyle style = EditorStyles.toolbarButton;
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = 0;
            return style;
        }
        GUIStyle ButtonLabelStyle()
        {
            GUIStyle style = EditorStyles.boldLabel;
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = 0;
            style.fontSize = 14;
            return style;
        }

        Color RemoveButtonColor() => Color.red;

        Color AddButtonColor() => Color.green;


        public void OnEnable()
        {
            SetupEventTypeAndEnums();
        }
        public override void OnInspectorGUI()
        {
            ComponentOptionDropDown();
            EventEnumDropDown();
            DrawAllStateNodes();
        }
    }
}
