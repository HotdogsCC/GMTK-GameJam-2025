using UnityEngine;
using static StaticObjectHolder;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
//SRSFB

namespace Saving
{
    public class SaveLoadSystem : MonoBehaviour
    {
        // Streaming assets is a folder that Unity creates that we can use
        // to load.save data in, in the Editor, it is the project folder,
        // in a build, it is in the .exe's build folder.
        private string FilePath => Application.streamingAssetsPath + "/gameData";
        [SerializeField] private bool useBinary = true;
        public GameData gameData = new GameData();

        private void Start()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            if (!File.Exists(FilePath + ".save"))
                Save();
            Load();
        }

        public void Save()
        {
            if (useBinary)
                SaveBinary();
            else
                SaveJson();
        }

        public void Load()
        {
            if (useBinary)
                LoadBinary();
            else
                LoadJson();
        }

        void SaveBinary()
        {
            // this opens the 'river' between the ram and the file
            using(FileStream stream = new FileStream(FilePath + ".save", FileMode.OpenOrCreate))
            {
                // like creating the boat that will carry the data from one point to another
                BinaryFormatter formatter = new BinaryFormatter();
                // transports the data from the ram to the specified file, like freezing water into ice.
                formatter.Serialize(stream, gameData);
            }
        }

        private void SaveJson()
        {
            // Converts the object ot a JSON string that we can read/write to and from a file
            string json = JsonUtility.ToJson(gameData);
            // this will write all the contents of the string (param 2) to the file at the path (param 1),
            // the standard is to use .json as the file extension for json files.
            File.WriteAllText(FilePath + ".json", json);
        }

        void LoadBinary()
        {
            // if if there is no save data, we shouldn't attempt to load it
            if (!File.Exists(FilePath + ".save"))
                return;

            // this opens the 'river' between the ram and the file
            using (FileStream stream = new FileStream(FilePath + ".save", FileMode.Open))
            {
                // like creating the boat that will carry the data from one point to another
                BinaryFormatter formatter = new BinaryFormatter();
                // transports the data from the specified file to the ram, like melting ice into water.
                gameData = formatter.Deserialize(stream) as GameData;
            }
        }

        void LoadJson()
        {
            // This is how we Read the string data from a file
            string json = File.ReadAllText(FilePath + ".json");
            //this is how you convert the json back to a data type. the generic is required for making sure the returned data is hte same as the passed in
            gameData = JsonUtility.FromJson<GameData>(json);
        }

        public void ResetProgress()
        {
            gameData = new GameData();
            Save();
        }

        private void Awake()
        {
            theSaveLoadSystem = this;
        }
    }
}
