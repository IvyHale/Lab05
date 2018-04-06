using Lab05.Annotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;
using Lab05.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System;
using System.Windows.Threading;

namespace Lab05.ModelView
{
    class ProcessGridViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TaskManagerProcess> _processes;
        private TaskManagerProcess _selectedProcess;
        private RelayCommand _getModulesListCommand;
        private RelayCommand _getThreadsListCommand;
        private RelayCommand _stopCommand;
        private RelayCommand _openDirectoryCommand;

        internal ProcessGridViewModel()
        {
            RefreshProcesses();

            DispatcherTimer processTimer = new DispatcherTimer();
            processTimer = new DispatcherTimer();
            processTimer.Interval = new TimeSpan(0, 0, 5);
            processTimer.Tick += new EventHandler(this.ProcessTimer);
            processTimer.Start();


            var progress = new Progress<List<TaskManagerProcess>>(processes =>
            {
                Thread.Sleep(100);
                Processes = new ObservableCollection<TaskManagerProcess>(processes);
            });
            Task.Factory.StartNew(() => UpdateProcessListWorker(progress), TaskCreationOptions.LongRunning);

        }

        private void UpdateProcessListWorker(IProgress<List<TaskManagerProcess>> progress)
        {
            Process[] proc = Process.GetProcesses();
            int i = 0;
            while (i<proc.Length)
            {
                List<TaskManagerProcess> processes = _processes.ToList();
                processes.Add(new TaskManagerProcess(proc[i++]));
                progress.Report(processes);
                Thread.Sleep(100);
            }
        }
        

        private void ProcessTimer(object state, EventArgs e)
        {
            //RefreshProcesses();
        }

        public ObservableCollection<TaskManagerProcess> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }
        public TaskManagerProcess SelectedProc
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand GetModulesListCommand
        {
            get { return _getModulesListCommand ?? (_getModulesListCommand = new RelayCommand(GetModulesListImpl, o => _selectedProcess != null)); }
        }
        private async void GetModulesListImpl(object o)
        {
            List<ProcessModule> list = new List<ProcessModule>();
            await Task.Run((() =>
            {
                try
                {
                    ProcessModuleCollection pmc = _selectedProcess.Modules;
                    foreach (ProcessModule pm in pmc)
                    {
                        list.Add(pm);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                Thread.Sleep(500);
            }));
            string res = "";
            foreach (ProcessModule m in list)
            {
                res += m.ModuleName + " " + m.FileName + "\n";
            }
            MessageBox.Show(res);

        }
        public RelayCommand GetThreadsListCommand
        {
            get { return _getThreadsListCommand ?? (_getThreadsListCommand = new RelayCommand(GetThreadsListImpl, o => _selectedProcess != null)); }
        }
        private async void GetThreadsListImpl(object o)
        {
            List<ProcessThread> list = new List<ProcessThread>();
            await Task.Run((() =>
            {
                try
                {
                    ProcessThreadCollection ptc = _selectedProcess.Threads;
                    foreach (ProcessThread pt in ptc)
                    {
                        list.Add(pt);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                Thread.Sleep(500);
            }));
            string res = "";
            foreach (ProcessThread t in list)
            {
                res += t.Id + " " + t.ThreadState + " " + t.StartTime + "\n";
            }
            MessageBox.Show(res);

        }
        public RelayCommand StopCommand
        {
            get { return _stopCommand ?? (_stopCommand = new RelayCommand(StopImpl, o => _selectedProcess != null)); }
        }
        private async void StopImpl(object o)
        {
            List<ProcessThread> list = new List<ProcessThread>();
            await Task.Run((() =>
            {
                try
                {
                    _selectedProcess.StopProcess();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                Thread.Sleep(200);
                RefreshProcesses();
                MessageBox.Show("Process stopped successfully.");
            }));
        }
        public RelayCommand OpenDirectoryCommand
        {
            get { return _openDirectoryCommand ?? (_openDirectoryCommand = new RelayCommand(OpenDirectoryImpl, o => _selectedProcess != null)); }
        }

        private async void OpenDirectoryImpl(object o)
        {
            List<ProcessThread> list = new List<ProcessThread>();
            await Task.Run((() =>
            {
                try
                {
                    string str = _selectedProcess.Path;
                    Process.Start(str.Substring(0, str.Length - _selectedProcess.Name.Length - 4));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                Thread.Sleep(100);
                //MessageBox.Show("Success");
            }));
        }

        private void RefreshProcesses()
        {
                Process[] processes = Process.GetProcesses();
                if (Processes==null || Processes.Count != processes.Length)
                { 
                    List<Process> processesList = processes.ToList<Process>();
                    Processes = new ObservableCollection<TaskManagerProcess>();
                    foreach (Process p in processesList)
                    {
                        Processes.Add(new TaskManagerProcess(p));
                    }
                    OnPropertyChanged();
                }
            if (_selectedProcess != null)
                SelectedProc = _selectedProcess;
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
