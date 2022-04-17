using System.Collections.Generic;
using UnityEngine;
using System;
using State_Machine.Core;
namespace State_Machine.xNode
{
    [NodeTint("#9e700b")]
    [CreateNodeMenu("Event")]
    public class xNode_EventNode : xNode_BaseNodeOverride
    {
        const string portName = "Event Triggered";

        public SerializableType componentType;
        public SerializableType eventEnumType;

        [SerializeField] public string eventEnumName;

        public List<xNode_StateNode> nodesThisTriggers;
        [SerializeField] List<int> indexOfNodes;

        public override void Setup()
        {
            name = "Event Trigger";
            nodesThisTriggers = new List<xNode_StateNode>();
            indexOfNodes = new List<int>();
            SetDefaultValues();
            AddDynamicOutput(typeof(bool), ConnectionType.Override, TypeConstraint.None, portName);
        }
        void SetDefaultValues()
        {
            Type componentType = EventLoading.GetDefaultEvent(out Enum eventEnum);
            //Debug.Log($"Setting default values, ComponentType {componentType} eventType {eventEnum}");
            SetTypeAndEnum(componentType, eventEnum);
        }


        public void SetTypeAndEnum(Type componentType, Enum eventEnum)
        {
            this.componentType = new SerializableType(componentType);
            eventEnumType = new SerializableType(eventEnum.GetType());
            eventEnumName = eventEnum.ToString();
            name = $"{componentType} - {eventEnum}";
        }
        public Enum GetEnum()
        {
            //Debug.Log($"Getting enum of type {eventEnumType.type} with name {eventEnumName}");
            return Enum.Parse(eventEnumType.type, eventEnumName) as Enum;
        }
        public StateEventContents GetEventContents()
        {
            StateEventContents eventContents =
                new StateEventContents(
                    GetNodeContentsThatListenForEvent(),
                    GetEnum(),
                    GetConnectedNode());
            return eventContents;
        }
        SM_Node GetConnectedNode()
        {
            XNode.NodePort exitPortConnectedPort = GetPort(portName).Connection;
            if (exitPortConnectedPort == null)
                throw new Exception($"Event {eventEnumName} does not have a connection to it's exit");
            return (exitPortConnectedPort.node as xNode_BaseNodeOverride).GetContents();
        }
        List<SM_Node> GetNodeContentsThatListenForEvent()
        {
            List<SM_Node> statesThatListenFor = new List<SM_Node>();
            foreach (xNode_StateNode node in nodesThisTriggers)
                statesThatListenFor.Add(node.smNode);
            return statesThatListenFor;
        }

        public void AddTriggeredNode(xNode_StateNode node)
        {
            Debug.Log($"Adding triggered node {node.name}");
            int index = ((StateGraph)graph).GetIndexOfNode(node);

            indexOfNodes.Add(index);
            nodesThisTriggers.Add(node);
        }
        public void RemoveTriggeredNode(xNode_StateNode node)
        {
            Debug.Log($"Removing triggered node {node.name}");
            int index = ((StateGraph)graph).GetIndexOfNode(node);

            indexOfNodes.Remove(index);
            nodesThisTriggers.Remove(node);
        }

        private void OnValidate()
        {
            for(int i = nodesThisTriggers.Count-1; i >=0; i--)
            {
                if(nodesThisTriggers[i] == null)
                {
                    nodesThisTriggers.RemoveAt(i);
                    indexOfNodes.RemoveAt(i);
                }
            }
        }

        public void Copy()
        {
            //Debug.Log("CopyingStateNode");
            List<xNode_StateNode> newNodesTriggered = new List<xNode_StateNode>();
            foreach(int index in indexOfNodes)
            {
                newNodesTriggered.Add(graph.nodes[index] as xNode_StateNode);
            }
            nodesThisTriggers = newNodesTriggered;
        }

        public override SM_Node GetContents()
        {
            throw new NotImplementedException();
        }
    }
}
