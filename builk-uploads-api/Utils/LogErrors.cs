using System;
using System.Diagnostics;
using System.IO;

namespace builk_uploads_api.Utils
{
    public class LogErrors
    {
        public void WriteLog(string exMessage, string exStackTrace, string pNote = null)
        {

            try
            {
                // var _path = "C:\\Logs\\ExportApi\\";
                var sFrame = (new StackFrame(1, true)).GetMethod();
                string _path = @"C:\Logs\BuilkUploadsApi\" + DateTime.Now.Year + @"\Month_" + DateTime.Now.Month + @"\Day_" + DateTime.Now.Day;

                //if (!Directory.Exists(_path))
                //    Directory.CreateDirectory(_path);

                //StreamWriter writer;
                //if (!File.Exists($"{_path}{pClass}.txt"))
                //    writer = File.CreateText($"{_path}{pClass}.txt");
                //else
                //    writer = File.AppendText($"{_path}{pClass}.txt");
                //writer.WriteLine("");
                //writer.WriteLine("**********************************" + DateTime.Now.ToString("dd MMMM yyyy H:mm:ss") + "****************************************");
                //writer.WriteLine("MethodName: " + pMethod);
                //writer.WriteLine("EXMessage: " + pExceptionMessage);
                //writer.WriteLine("Notes: " + pNote);
                //writer.WriteLine("-----------------------------------------------" + "END" + "------------------------------------------------");
                //writer.WriteLine("");
                //writer.Close();

                if (!Directory.Exists(_path))
                    Directory.CreateDirectory(_path);

                string _tmpMethodName = (sFrame != null) ? sFrame.Name : " ... ";
                string _tmpClassName = (sFrame != null && sFrame.ReflectedType != null) ? sFrame.ReflectedType.Name : "GenericLog";

                StreamWriter writer = File.AppendText(_path + @"\ErrorLog_" + _tmpClassName + ".txt");
                writer.WriteLine("********************************** " + DateTime.Now + " ****************************************");
                writer.WriteLine("Class : " + _tmpClassName);
                writer.WriteLine("MethodName: " + _tmpMethodName);
                writer.WriteLine("EXMessage: " + exMessage);
                writer.WriteLine("EXStackTrace: " + exStackTrace);
                writer.WriteLine("Notes: " + pNote);
                writer.WriteLine("**************************************************************************************************");
                writer.Close();



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
