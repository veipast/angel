using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ResourcesType
{
    UI, Sprite, Roles,
    Roads,
    ElseFolder,
    Materials,
    Floors,
    Wings,
    FX
}
public static class ResourcesManager
{

    private static Dictionary<string, Object> modelObjects = new Dictionary<string, Object>();

    public static T Load<T>(ResourcesType type, string path) where T : Object
    {
        return Load<T>(type.ToString() + "/" + path);
    }
    private static T Load<T>(string path) where T : Object
    {
        Object temp;
        if (!modelObjects.TryGetValue(path, out temp))
        {
            temp = Resources.Load<T>(path);
            modelObjects.Add(path, temp);
        }
        return temp as T;
    }
    public static void RemoveObject()
    {
        foreach (var item in modelObjects.Values)
        {
            Resources.UnloadAsset(item);
        }
        System.GC.Collect();
        modelObjects.Clear();
    }

}
