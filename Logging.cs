using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

// In order to not conflict with other running apps such as MongooseService and MongooseIG both logging this requires a fileName

class Logging
{
    public static string DEFAULTLOG = "LB_Log.txt";

    bool isOpen = false;
    FileStream fStream = null;
    static System.Collections.Concurrent.ConcurrentDictionary<string, Logging> loggingInstances = new ConcurrentDictionary<string, Logging>();
    /*
    Logging(string fileName)
    {
        //WCHAR filePath[MAX_PATH];
        string filePath;

        //HRESULT hr = SHGetFolderPath(NULL, CSIDL_PERSONAL, NULL, SHGFP_TYPE_CURRENT, filePath);
        String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (path != null)
        {
            string outFile;
            outFile = fileName + "\\";
            path += fileName;
            fStream = File.Open(path, FileMode.OpenOrCreate);
            isOpen = true;
        }
    }
*/
    //-----------------------------------------------------------------------------

    private Logging(string filePath, string fileName)
    {
        bool isError = false;
        //WCHAR userPath[MAX_PATH];
        string userPath;

        if (filePath != null)
        {
            userPath = string.Copy(filePath);
            //if (wcscpy_s (userPath, MAX_PATH, filePath.c_str ()))
            //isError = true;
        }
        else
        {
            userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            /*HRESULT hr =
                SHGetFolderPath (
                    NULL, CSIDL_PERSONAL, NULL, SHGFP_TYPE_CURRENT, userPath);

            if (!SUCCEEDED(hr)) { isError = true; }
            */
        }

        if (userPath.Length > 0)
        {

            //MongooseString outFile; outFile.format (_T("\\%s"), fileName.c_str ());
            userPath += "\\" + fileName;
            fStream = File.Open(userPath, FileMode.OpenOrCreate);
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
        //fStream.Flush();
        //fStream.Close();
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
        //if (loggingInstances[fileName] == null)
          //  loggingInstances[fileName] = new Logging(filePath, fileName);

        return loggingInstances[fileName];
    }

    //-----------------------------------------------------------------------------
    /*
    public static Logging Instance(string filePath, string fileName)
    {
        if (loggingInstances[fileName] == null)
            loggingInstances[fileName] = new Logging(filePath, fileName);

        return loggingInstances[fileName];
    }*/

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
            //int test = System.Text.ASCIIEncoding.Unicode.GetByteCount(logMessage);
            int test = Encoding.ASCII.GetByteCount(logMessage);
            fStream.Write(array, 0, test);
            fStream.Flush();
        }
    }
}
