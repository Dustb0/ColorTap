using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

#region " Item class "

[System.Serializable()]
public class HighscoreEntry : ISerializable
{
    public int ScorePoints;
    public string PlayerName;

    public HighscoreEntry(int scorePoints, string playerName)
    {
        ScorePoints = scorePoints;
        PlayerName = playerName;
    }

    //Deserialization constructor.
    public HighscoreEntry(SerializationInfo info, StreamingContext ctxt)
    {
        //Get the values from info and assign them to the appropriate properties
        ScorePoints = (int)info.GetValue("ScorePoints", typeof(int));
        PlayerName = info.GetString("PlayerName");
    }

    //Serialization function.
    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
        info.AddValue("ScorePoints", ScorePoints);
        info.AddValue("PlayerName", PlayerName);
    }
}

#endregion

#region " List class "

[System.Serializable()]
public class Highscore : List<HighscoreEntry>
{

    #region " Constants "

    public const int MAX_COUNT = 5;
    public const string HIGHSCORE_FILENAME = "hs3.ml";

    #endregion

    #region " Class Fields "

    private static Highscore s_list;
    private static HighscoreEntry s_preparedEntry;
    private static int s_achievedHighscorePlace;
    private static bool s_lastEntryMadeItIntoList;

    public static HighscoreEntry PreparedEntry { get { return s_preparedEntry; } }

    public static bool LastEntryMadeItIntoList { get { return s_lastEntryMadeItIntoList; } }

    public static int AchievedHighscorePlace { get { return s_achievedHighscorePlace; } }

    #endregion

    #region " Highscore Managing "

    public static Highscore Get()
    {
        if (s_list == null) s_list = Highscore.Load();
        return s_list;
    }

    /// <summary>
    /// Fully reloads the highscore from disk
    /// </summary>
    /// <returns></returns>
    public static void Refresh()
    {
        s_list = Highscore.Load();
    }

    /// <summary>
    /// Creates a new entry that will be inserted into the highscore-list
    /// </summary>
    public static bool AddNewScore(int killedWaffles, string playerName)
    {
        s_preparedEntry = new HighscoreEntry(killedWaffles, playerName);

        // Insert the entry to the list
        s_lastEntryMadeItIntoList = Get().InsertEntry(s_preparedEntry);

        return s_lastEntryMadeItIntoList;
    }

    public static void Reset()
    {
        string path = Path.Combine(Application.persistentDataPath, HIGHSCORE_FILENAME);
        File.Delete(path);
    }

    #endregion

    #region " List Methods "

    /// <summary>
    /// Tries to insert a new highscore entry in the list
    /// </summary>
    /// <returns>FALSE if the entry was not good enough to be inserted in the highscore list</returns>
    public bool InsertEntry(HighscoreEntry newEntry)
    {
        // If there are already some entries, loop through the list and get the correct ranking for the current entry
        for (int i = 0; i < MAX_COUNT; ++i)
        {
            // Check if there's an entry at the current position
            if (i < this.Count)
            {
                // Insert new item at the current position, if the current entry has a lower score
				if (this[i].ScorePoints < newEntry.ScorePoints)
                {
                    this.Insert(i, newEntry);

                    // Check if there are more entries in the list than allowed
                    if (this.Count > MAX_COUNT)
                    {
                        this.RemoveAt(MAX_COUNT); // Remove last entry
                    }

                    s_achievedHighscorePlace = i;
                    return true;
                }
            }
            else
            {
                // There isn't so just insert
                s_achievedHighscorePlace = i;
                this.Insert(i, newEntry);
                return true;
            }
        }

        return false;
    }

    #endregion

    #region " Serialization "

    private static Highscore Load()
    {
        string path = Path.Combine(Application.persistentDataPath, HIGHSCORE_FILENAME);

        // Check if highscore file for deserialization exists
        if (File.Exists(path))
        {
            // Deserialize
            StreamReader reader = new StreamReader(path);
            BinaryFormatter bformatter = new BinaryFormatter();
            Highscore obj;

            obj = (Highscore)bformatter.Deserialize(reader.BaseStream);
            reader.Close();

            return obj;
        }
        else
        {
            // Simply return a new object
            return new Highscore();
        }
    }

    public static void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, HIGHSCORE_FILENAME);

        StreamWriter writer = new StreamWriter(path, false);
        BinaryFormatter bformatter = new BinaryFormatter();

        bformatter.Serialize(writer.BaseStream, s_list);
        writer.Close();
    }

    #endregion

}

#endregion