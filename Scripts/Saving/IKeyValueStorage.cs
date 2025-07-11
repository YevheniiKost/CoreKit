namespace YeKostenko.CoreKit.Scripts.Saving
{
    public interface IKeyValueStorage
    {
        void SetString(string key, string value);
        string GetString(string key, string defaultValue = "");
        void SetInt(string key, int value);
        int GetInt(string key, int defaultValue = 0);
        void SetFloat(string key, float value);
        float GetFloat(string key, float defaultValue = 0f);
        void SetBool(string key, bool value);
        bool GetBool(string key, bool defaultValue = false);
        void DeleteKey(string key);
        bool HasKey(string key);
    }
}