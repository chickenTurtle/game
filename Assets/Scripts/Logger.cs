using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : Debug
{
    public static void Log<T>(string msg, T type)
    {
        Log("[" + type.GetType().Name + "] " + msg);
    }
}
