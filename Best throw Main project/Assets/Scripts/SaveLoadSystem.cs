using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
public class SaveLoadSystem
{
    static string PASS = Application.persistentDataPath + "/file.ini";

    #region boolean

    public void SaveBool(string key, bool b)
    {
       

        BoolData data = new BoolData(key, b);

        Save(data, PASS,
        () =>
        {

        },
        (Exception onError) =>
        {

        });
    }

    public bool LoadBool(string key)
    {
        BoolData data = new BoolData();
        data.key = key;

        Load((BoolData boolData) =>
        {
            data = boolData;
        },
        PASS,
        () =>
        {

        },
        (Exception onError) =>
        {
            data.value = false;
        }
        );

        return data.value;
    }

    #endregion

    #region Int
    public void SaveInt(string key, int value)
    {
        IntData data = new IntData(key, value);

        Save(data, PASS,
        () =>
        {

        },
        (Exception onError) =>
        {

        });
    }

    public int LoadInt(string key)
    {
        IntData data = new IntData();
        data.key = key;

        Load((IntData intData) =>
        {
            data = intData;
        },
        PASS,
        () =>
        {

        },
        (Exception onError) =>
        {
            data.value = -1;
        }
        );

        return data.value;
    }


    #endregion


    private void Save<T>(T data, string path, Action OnSuccess, Action<Exception> OnError)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        try
        {
            binary.Serialize(stream, data);
            OnSuccess();
        }
        catch (Exception error)
        {
            OnError(error);
        }
        finally
        {
            stream.Dispose();
        }
    }

    private void Load<T>(Action<T> LoadData, string path, Action OnSuccess, Action<Exception> OnError)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = null;

        try
        {
            stream = File.OpenRead(path);
            LoadData((T)binary.Deserialize(stream));
            OnSuccess();
        }
        catch (Exception error)
        {
            OnError(error);
        }
        finally
        {
            if (stream != null)
                stream.Dispose();
        }

    }



}

[Serializable]
struct BoolData
{
    public string key;
    public bool value;

    public BoolData(string key, bool value)
    {
        this.key = key;
        this.value = value;
    }
}

[Serializable]
struct IntData
{
    public string key;
    public int value;

    public IntData(string key, int value)
    {
        this.key = key;
        this.value = value;
    }
}
