using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<NoticeData> OnShowNotice;

    public static void ShowNotice(string message)
    {
        OnShowNotice?.Invoke(new NoticeData(message));
    }

    public static void ShowNotice(NoticeData data)
    {
        OnShowNotice?.Invoke(data);
    }
}

[Serializable]
public struct NoticeData
{
    public string message;
    public Color color;
    public float duration;
    public float scale;
    public AudioClip sound;

    public NoticeData(string message, Color? color = null, float duration = 1.5f, float scale = 1.2f, AudioClip sound = null)
    {
        this.message = message;
        this.color = color ?? Color.black; // mặc định là trắng
        this.duration = duration;
        this.scale = scale;
        this.sound = sound;
    }
}
