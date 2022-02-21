using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase
{
    public class FileBase
    {
        string PATH_TO_WWWROOT;
        string EXTENSION = "jpg";
        const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random;

        public FileBase(string pathToWWWroot)
        {
            this.PATH_TO_WWWROOT = pathToWWWroot;
            random = new Random();
        }

        string getRandomString(int len)
        {
            StringBuilder str = new StringBuilder(len);
            for (int i = 0; i < len; i++) str.Append(CHARS[random.Next(CHARS.Length)]);
            return str.ToString();
        }
        
        string getUiqueName(string pathToDirectory)
        {
            pathToDirectory = PATH_TO_WWWROOT + "/" + pathToDirectory;
            string name = getRandomString(20) + "." + EXTENSION;
            while(File.Exists(pathToDirectory + "/" + name))
                name = getRandomString(20) + "." + EXTENSION;
            return name;
        }
        
        public string createPicture(string base64Data) {
            string name = getUiqueName("/photos");
            using (FileStream fstream = new FileStream(PATH_TO_WWWROOT + "/photos/" + name, FileMode.OpenOrCreate))
            {
                byte[] array = Convert.FromBase64String(base64Data);
                fstream.Write(array, 0, array.Length);
            }
            return "/photos/" + name;
        }

        public bool deletePicture(string url) {
            File.Delete(PATH_TO_WWWROOT + url);
            return true;
        }

        public bool updatePicture(string url, string base64Data) {
            using (FileStream fstream = new FileStream(PATH_TO_WWWROOT + url, FileMode.OpenOrCreate))
            {
                byte[] array = Convert.FromBase64String(base64Data);
                fstream.Write(array, 0, array.Length);
            }
            return true;
        }
    }
}
