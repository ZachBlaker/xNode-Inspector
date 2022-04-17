using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
namespace State_Machine.Core
{
    public static class EventLoading
    {
        public static Type[] GetAllCreatureComponentTypes()
        {
            return BaseLoading.GetAllSubClassesOfType(typeof(StateComponent));
        }
        public static EventsAttribute GetEventAttribute(Type creactureComponentType)
        {
            EventsAttribute eventAttribute = creactureComponentType.GetCustomAttribute<EventsAttribute>(false);
            Debug.Log(eventAttribute);
            //if (stateAttribute == null)
            //    throw new Exception($"MethodInfo {methodInfo.Name} does not have StateAttribute");
            return eventAttribute;
        }
        public static Dictionary<Type, Type> GetAllTypesImplementingEnumsAndTheirEnums()
        {
            Debug.Log($"Getting event enum dictionary");
            Dictionary<Type, Type> ComponentEventEnumDictionary = new Dictionary<Type, Type>();
            foreach (Type componentType in GetAllCreatureComponentTypes())
            {
                Debug.Log($"Checking type {componentType}");
                Type[] nestedTypes = componentType.GetNestedTypes();
                foreach (Type nestedType in nestedTypes)
                {
                    if (nestedType.IsEnum && nestedType.GetCustomAttribute<EventsAttribute>() != null)
                    {
                        Debug.Log($"Component Type : {componentType} implements eventEnum {nestedType}");
                        ComponentEventEnumDictionary.Add(componentType, nestedType);
                    }
                }
            }
            return ComponentEventEnumDictionary;
        }
        public static Type GetDefaultEvent(out Enum eventEnumType)
        {
            Dictionary<Type, Type> dic = GetAllTypesImplementingEnumsAndTheirEnums();
            Type componentType = dic.Keys.First();
            eventEnumType = dic.Values.First().ToEnumArray()[0];
            return componentType;
        }
    }
}
