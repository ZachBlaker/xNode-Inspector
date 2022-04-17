using XNodeEditor;
using UnityEditor;
using System;
using System.Collections.Generic;
using State_Machine.Core;

namespace State_Machine.xNode
{
    [CustomEditor(typeof(xNode_BehaviourNode), true)]
    public class xNode_BehaviourInspector : GlobalNodeEditor
    {
        xNode_BehaviourNode xNode => (xNode_BehaviourNode)target;

        Type[] typeOptions;
        Type selectedType;

        string[] typesAsStrings;
        int currentTypeIndex;

        private void Setup()
        {
            selectedType = GetBehaviourType();
            LoadTypeOptions();
            currentTypeIndex = GetIndexOfType(selectedType);
        }


        void LoadTypeOptions()
        {
            typeOptions = BaseLoading.GetAllSubClassesOfType(GetContentsType());

            List<string> options = new List<string>();
            foreach (Type stateType in typeOptions)
                options.Add(GetOptionNameForType(stateType));

            typesAsStrings = options.ToArray();
        }
        string GetOptionNameForType(Type type)
        {
            return type.Name;
        }
        int GetIndexOfType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type", "Cannot get index for null Type");

            for (int i = 0; i < typeOptions.Length; i++)
                if (typeOptions[i] == type)
                    return i;

            throw new InvalidOperationException($"Type of {type} does not exist in typeOptions");
        }

        void DrawDropDown()
        {
            if (typeOptions == null || typeOptions.Length == 0) return;
            int newIndex = EditorGUILayout.Popup(currentTypeIndex, typesAsStrings);
            DrawNode();
            if (newIndex != currentTypeIndex)
                SelectedIndexChanged(newIndex);
        }
        void DrawNode()
        {
            xNode_EditorHelpers.DrawFullNode(GetSMNode());
        }

        void SelectedIndexChanged(int newIndex)
        {
            currentTypeIndex = newIndex;
            ChangeSelectedType(typeOptions[newIndex]);
        }
        void ChangeSelectedType(Type newStateType)
        {
            selectedType = newStateType;
            SetupSMNodeForType(selectedType);
            UpdateNode();
        }


        #region overrides
        protected void UpdateNode()
        {
            xNode.UpdateNode();
        }
        protected Type GetBehaviourType()
        {
            return xNode.smNode.containedBehaviour.GetType();
        }
        protected void SetupSMNodeForType(Type type)
        {
            xNode.smNode.SetupForType(type);
        }
        protected SM_Node GetSMNode()
        {
            return xNode.smNode;
        }
        protected Type GetContentsType()
        {
            return xNode.GetContents().containedParentType.type;
        }
        #endregion

        public void OnEnable()
        {
            Setup();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            DrawDropDown();
            EditorGUILayout.EndVertical();
        }
    }
}
