using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GrabbingToSql
{
    public class LoadConfig
    {
        public Dictionary<string, string> LoadFields()
        {
            Dictionary<string, string> tempDic = new Dictionary<string, string>();

            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\f2.ini";
            IniFile file = new IniFile(@"C:\Users\MTX\Documents\fields.ini");

            string val = file.IniReadValue("Settings", "FieldCount");
            int fieldCount = int.Parse(val);

            if (fieldCount <= 0)
                return tempDic;

            for (int i = 0; i < fieldCount; i++)
            {
                tempDic.Add(file.IniReadValue("FieldNames", i.ToString()), file.IniReadValue("SQLFieldNames", i.ToString()));
            }

            return tempDic;
        }
    }

    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileStringW(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileStringW(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
    }
}