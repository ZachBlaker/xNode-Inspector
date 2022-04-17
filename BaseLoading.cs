using System.Collections.Generic;
using System;
using System.Reflection;

public static class BaseLoading 
{
    public static Type[] GetAllSubClassesOfType(Type parentClassType)
    {
        Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
        List<Type> result = new List<Type>();
        foreach (Type type in allTypes)
        {
            if (type.IsSubclassOf(parentClassType))
                result.Add(type);
        }
        return result.ToArray();
    }
}
