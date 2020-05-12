using System.Collections.Generic;
using SqlCipher4Unity3D;
using UnityEngine;
using System.Linq;
using System.IO;
#if !UNITY_EDITOR
#endif

public class DataService
{
    public readonly SQLiteConnection _connection;

    public DataService(string DatabaseName)
    {
#if UNITY_EDITOR
        string dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
            // check if file exists in Application.persistentDataPath
            string filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

            if (!File.Exists(filepath))
            {
                Debug.Log("Database not in Persistent path");
                // if it doesn't ->
                // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
                WWW loadDb =
     new WWW ("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName); // this is the path to your StreamingAssets in android
                while (!loadDb.isDone) { } // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
                // then save to Application.persistentDataPath
                File.WriteAllBytes (filepath, loadDb.bytes);
#elif UNITY_IOS
                string loadDb =
     Application.dataPath + "/Raw/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);
#elif UNITY_WP8
                string loadDb =
     Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);
    
#elif UNITY_WINRT
                string loadDb =
     Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy (loadDb, filepath);
#elif UNITY_STANDALONE_OSX
                string loadDb =
     Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#else
                string loadDb =
     Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#endif

                Debug.Log("Database written");
            }
            
            var dbPath = filepath;

#endif

        if (!File.Exists(dbPath))
        {
            _connection = new SQLiteConnection(dbPath, "puzzle2048!");
            CreateDB();
        }
        else
            _connection = new SQLiteConnection(dbPath, "puzzle2048!");

        Debug.Log("Final PATH: " + dbPath);
    }

    public void CreateDB()
    {
        _connection.DropTable<DB>();
        _connection.CreateTable<DB>();

        _connection.InsertAll(new[]
        {
                new DB
                {
                    ID = 1,
                    score = 0,
                    sound = 1
                }
            });
    }

    public IEnumerable<DB> GetDB()
    {
        return _connection.Table<DB>().ToList();
    }

}