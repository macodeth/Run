using System;
using System.Diagnostics;
using Enum;
using Godot;

public static class StaticUtil {
    public static void Log (string str) {
        Debug.WriteLine(str);
    }
    public static double EaseOutQuint (double x) {
        return 1 - Math.Pow(1 - x, 5);
    }
    public static string TimeFormat (int seconds) {
        int minutes = seconds / 60;
		seconds %= 60;
		return minutes.ToString().PadZeros(2) + ":" + seconds.ToString().PadZeros(2);
    }
    public static void RemoveAllChildren (Node node) {
        foreach (var child in node.GetChildren()) {
            node.RemoveChild(child);
            child.QueueFree();
        }
    }
}