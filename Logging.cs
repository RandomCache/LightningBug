using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Logging
{
    public static string DEFAULTLOG = "LB_Log.txt";

    bool isOpen = false;
    FileStream fStream = null;
    static System.Collections.Concurrent.ConcurrentDictionary<string, Logging> loggingInstances = new ConcurrentDictionary<string, Logging>();

    //-----------------------------------------------------------------------------

    private Logging(string filePath, string fileName)
    {
        string userPath;

        if (filePath != null)
            userPath = string.Copy(filePath);
        else
            userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        if (userPath.Length > 0)
        {
            userPath += "\\" + fileName;
            fStream = File.Open(userPath, FileMode.OpenOrCreate);
            //Now clear the file
            fStream.SetLength(0);
            fStream.Flush();
            isOpen = true;
        }
        else
            fStream = null;
    }

    //-----------------------------------------------------------------------------

    ~Logging()
    {
        isOpen = false;
    }

    //-----------------------------------------------------------------------------

    public static Logging Instance(string fileName, string filePath = null)
    {
        if (!loggingInstances.ContainsKey(fileName))
        {
            if (!loggingInstances.TryAdd(fileName, new Logging(filePath, fileName)))
                return null;
        }

        return loggingInstances[fileName];
    }

    //-----------------------------------------------------------------------------

    public void CloseAll()
    {
        loggingInstances.Clear();
    }

    //-----------------------------------------------------------------------------

    public void Log(string logMessage)
    {
        if (isOpen)
        {
            byte[] array = Encoding.ASCII.GetBytes(logMessage);
            int test = Encoding.ASCII.GetByteCount(logMessage);
            fStream.Write(array, 0, test);
            fStream.Flush();
        }
    }
}
