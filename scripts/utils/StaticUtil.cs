using System;
using System.Diagnostics;
using Enum;
using Godot;

public class StaticUtil {
    public static void Log (string str) {
        Debug.WriteLine(str);
    }
    public static double EaseOutQuint (double x) {
        return 1 - Math.Pow(1 - x, 5);
    }
    public static string TimeFormat (int miliseconds) {
        int minutes = miliseconds / 60000;
		miliseconds %= 60000;
        int seconds = miliseconds / 1000;
        miliseconds %= 1000;
		return minutes.ToString().PadZeros(2) + ":" + 
        seconds.ToString().PadZeros(2) + "." +
        miliseconds.ToString().PadZeros(3);
    }
    public static void RemoveAllChildren (Node node) {
        foreach (var child in node.GetChildren()) {
            node.RemoveChild(child);
            child.QueueFree();
        }
    }
    // random in [min, max)
    public static int RandomIntRange (int min, int max) {
        var rnd = new Random(DateTime.Now.Millisecond);
        return rnd.Next(min, max);
    }
}