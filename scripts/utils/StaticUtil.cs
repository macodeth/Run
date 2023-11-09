using System;
using System.Diagnostics;
using Enum;
using Godot;

public static class StaticUtil {
    public static void Log (String str) {
        Debug.WriteLine(str);
    }
    public static double EaseOutQuint (double x) {
        return 1 - Math.Pow(1 - x, 5);
    }
}