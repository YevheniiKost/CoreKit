using System;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public interface ISaveSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
        object Deserialize(string json, Type type);
    }
}