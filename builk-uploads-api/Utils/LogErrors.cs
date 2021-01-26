using System;
using System.IO;

namespace builk_uploads_api.Utils
{
    public class LogErrors
    {
        public void WriteLog(string pClass, string pMethod, string pExceptionMessage, string pNote = null)
        {

            try
            {
                var _path = "C:\\Logs\\ExportApi\\";

                if (!Directory.Exists(_path))
                    Directory.CreateDirectory(_path);

                StreamWriter writer;
                if (!File.Exists($"{_path}{pClass}.txt"))
                    writer = File.CreateText($"{_path}{pClass}.txt");
                else
                    writer = File.AppendText($"{_path}{pClass}.txt");
                writer.WriteLine("");
                writer.WriteLine("**********************************" + DateTime.Now.ToString("dd MMMM yyyy H:mm:ss") + "****************************************");
                writer.WriteLine("MethodName: " + pMethod);
                writer.WriteLine("EXMessage: " + pExceptionMessage);
                writer.WriteLine("Notes: " + pNote);
                writer.WriteLine("-----------------------------------------------" + "END" + "------------------------------------------------");
                writer.WriteLine("");
                writer.Close();



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
