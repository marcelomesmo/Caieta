using System;

namespace Caieta
{
    public static class Debug
    {

        #region Debug

        public static void Log(params object[] obj)
        {
            foreach (var o in obj)
            {
                if (o == null)
                    System.Diagnostics.Debug.WriteLine("null");
                else
                    System.Diagnostics.Debug.WriteLine(o.ToString());
            }
        }

        public static void LogBreak()
        {
            System.Diagnostics.Debug.WriteLine("-------------------------------------------");
        }

        public static void LogLine()
        {
            System.Diagnostics.Debug.WriteLine("");
        }

        public static void ErrorLog(params object[] obj)
        {

            System.Diagnostics.Debug.Write("### ERROR ### ");

            foreach (var o in obj)
            {
                if (o == null)
                    System.Diagnostics.Debug.WriteLine("null");
                else
                    System.Diagnostics.Debug.WriteLine(o.ToString());
            }
        }

        public static void WarningLog(params object[] obj)
        {

            System.Diagnostics.Debug.Write("* Warning * ");

            foreach (var o in obj)
            {
                if (o == null)
                    System.Diagnostics.Debug.WriteLine("null");
                else
                    System.Diagnostics.Debug.WriteLine(o.ToString());
            }
        }

        /*public static void TimeLog(object obj)
        {
            System.Diagnostics.Debug.WriteLine(//Engine.Scene.RawTimeActive or Engine.Instance.TargetElapsedTime + " : " + obj);
        }*/

        // Notes: Mudar nome para LogObject ?
        public static void LogObject(Object obj)
        {
            System.Diagnostics.Debug.Write(obj.GetType().Name + " { ");
            foreach (var v in obj.GetType().GetFields())
                System.Diagnostics.Debug.Write(v.Name + ": " + v.GetValue(obj) + ", ");
            System.Diagnostics.Debug.WriteLine(" }");
        }

        #endregion

        #region FileLog

        /*public const string Filename = "error_log.txt";
        public const string Marker = "==========================================";

        public static void Write(Exception e)
        {
            Write(e.ToString());
        }

        public static void Write(string str)
        {
            StringBuilder s = new StringBuilder();

            //Get the previous contents
            string content = "";
            if (File.Exists(Filename))
            {
                TextReader tr = new StreamReader(Filename);
                content = tr.ReadToEnd();
                tr.Close();

                if (!content.Contains(Marker))
                    content = "";
            }

            //Header
            if (Engine.Instance != null)
                s.Append(Engine.Instance.Title);
            else
                s.Append("Monocle Engine");
            s.AppendLine(" Error Log");
            s.AppendLine(Marker);
            s.AppendLine();

            //Version Number
            if (Engine.Instance.Version != null)
            {
                s.Append("Ver ");
                s.AppendLine(Engine.Instance.Version.ToString());
            }

            //Datetime
            s.AppendLine(DateTime.Now.ToString());

            //String
            s.AppendLine(str);

            //If the file wasn't empty, preserve the old errors
            if (content != "")
            {
                int at = content.IndexOf(Marker) + Marker.Length;
                string after = content.Substring(at);
                s.AppendLine(after);
            }

            TextWriter tw = new StreamWriter(Filename, false);
            tw.Write(s.ToString());
            tw.Close();
        }

        public static void Open()
        {
            if (File.Exists(Filename))
                System.Diagnostics.Process.Start(Filename);
        }*/

        #endregion
    }
}
