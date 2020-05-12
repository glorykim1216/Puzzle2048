using System.Collections.Generic;
using SQLite.Attribute;

public class DB
{
    [PrimaryKey] public int ID { get; set; }
    public int score { get; set; }
    public int sound { get; set; }

    public override string ToString()
    {
        return string.Format("[DatabaseTable: ID={0}, Score={1}, Sound={2}]", this.ID, this.score, this.sound);
    }
}

public class DBManager : MonoSingleton<DBManager>
{
    public DB ItemList;  // db에 있는 아이템을 저장할 리스트

    string m_NameDB = "MyDB1.db";  // db 파일 이름

    DataService ds;

    public void CreateTable()
    {
        ds = new DataService(m_NameDB);

        ds.CreateDB();
    }
    public bool Load()
    {
        ds = new DataService(m_NameDB);

        IEnumerable<DB> DBs = ds.GetDB();

        if (DBs != null)
        {
            foreach (DB DB in DBs)
                ItemList = DB;
        }
        return true;
    }

    public void UpdateItemTable_Score(int _score)
    {
        IEnumerable<DB> DBs = ds.GetDB();

        if (DBs != null)
        {
            foreach (DB DB in DBs)
            {
                DB.score = _score;

                ItemList = DB;
            }
            ds._connection.UpdateAll(DBs);
        }
    }
    public void UpdateItemTable_Sound(int _sound)
    {
        IEnumerable<DB> DBs = ds.GetDB();

        if (DBs != null)
        {
            foreach (DB DB in DBs)
            {
                DB.sound = _sound;

                ItemList = DB;
            }
            ds._connection.UpdateAll(DBs);
        }
    }
}
