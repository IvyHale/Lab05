using System.Management;
using System.Diagnostics;
using System;

namespace Lab05.Model
{
    class TaskManagerProcess
    {
        private Process _process;
        internal TaskManagerProcess(Process p)
        {
            _process = p;
        }
        public string Name => _process.ProcessName;
        public int Id => _process.Id;
        public bool IsActive => _process.Responding;
        public float CPU
        {
            get
            {
                try
                {
                    return new PerformanceCounter("Process", "% Processor Time", _process.ProcessName).NextValue();
                }
                catch (Exception e) { }
                return 0;
            }
        }
        public double RAM => _process.WorkingSet64 / 1024 / 1024;
        public ProcessThreadCollection Threads
        {
            get
            {
                return _process.Threads;
            }
        }
        public string User
        {
            get
            {
                //ManagementClass classProcess = new ManagementClass("Win32_Process");
                //ManagementObjectCollection processList = classProcess.GetInstances();
                //foreach (ManagementObject currentProcess in processList)
                //{
                //    string[] argList = { string.Empty };
                //    object objReturn = currentProcess.InvokeMethod("GetOwner", argList);
                //    if (Convert.ToInt32(objReturn) == 0)
                //    {
                //        string userName = argList[0];
                //        if (userName != "SYSTEM" && userName != "LOCAL SERVICE" && userName != "NETWORK SERVICE")
                //            return userName;
                //    }
                //}
                //return "system";

                ObjectQuery query = new ObjectQuery("Select * From Win32_Process where ProcessId='" + _process.Id + "'");
                ManagementObjectSearcher mos = new ManagementObjectSearcher(query);
                ManagementObjectCollection _moc = mos.Get();

                string processOwner = "";

                foreach (ManagementObject mo in _moc)
                {
                    string[] s = new string[2];
                    mo.InvokeMethod("GetOwner", (object[])s);
                    if (s[0] == null) return "system";
                    processOwner = s[0].ToString();
                    break;
                }
                return processOwner;

            }
        }
        public string Path
        {
            get
            {
                try
                {
                    return _process.MainModule.FileName;
                }
                catch (Exception e) { }
                return "";

                //foreach (ManagementObject mo in _moc)
                //{
                //    if (mo["ExecutablePath"] != null)
                //    {
                //        string path = mo["ExecutablePath"].ToString();
                //        return path;
                //    }
                //}

                //return "";
            }
        }
        public DateTime Date
        {
            get
            {
                try
                {
                    return _process.StartTime;
                }
                catch (Exception e) { }
                return DateTime.MinValue;

                //foreach (ManagementObject mo in _moc)
                //{
                //    DateTime d = ManagementDateTimeConverter.ToDateTime(mo["CreationDate"].ToString());
                //    return d;
                //}

                //return DateTime.MinValue;
            }
        }
        public ProcessModuleCollection Modules => _process.Modules;
        public void StopProcess() { _process.Kill(); }
    }
}
