using System;
using System.Linq;
using System.Text;

namespace Spareio.Installer.AppCore
{
    internal class CommandLineParameterAttribute : Attribute
    {

    }
    internal class RunSpareio : ExecuteCommandStep, IInstallationStep
    {
        [CommandLineParameter]
        public bool silent { set; get; }
        [CommandLineParameter]
        public bool install { set; get; }
        [CommandLineParameter]
        public bool preprod { set; get; }
        [CommandLineParameter]
        public bool update { set; get; }
        [CommandLineParameter]
        public bool afterupdate { set; get; }
        [CommandLineParameter]
        public bool afterinstall { set; get; }
        //[CommandLineParameter]
        //public string geo { set; get; }
        //[CommandLineParameter]
        //public string xToken { set; get; }

        private ExecuteCommandStep _executer;

        internal RunSpareio(string fileName, params string[] args)
            : base(fileName, args)
        {
        }

        private ExecuteCommandStep Executer
        {
            get
            {
                if (_executer == null)
                {
                    // scan properties and create 
                    var str = new StringBuilder();

                    var props = from prop in GetType().GetProperties()
                        where prop.GetCustomAttributes(typeof(CommandLineParameterAttribute), true).Any()
                        select prop;

                    foreach (var prop in props)
                    {
                        if (prop.Name == "geo")
                        {
                            try
                            {
                                var value = (string)prop.GetValue(this, null);
                                if (value != null)
                                    str.AppendFormat("--{0}={1} ", prop.Name, value.ToString());
                            }
                            catch
                            {

                            }
                        }
                        else if (prop.Name == "xToken")
                        {
                            try
                            {
                                var value = (string)prop.GetValue(this, null);
                                if (value != null)
                                    str.AppendFormat("--{0}={1} ", prop.Name, value.ToString());
                            }
                            catch
                            {

                            }

                        }
                        else if ((bool)prop.GetValue(this, null))
                            str.AppendFormat("--{0} ", prop.Name);
                    }

                    // add new args to the existing parameters
                    var newArgs = new string[_args.Length + 1];
                    Array.Copy(_args, newArgs, _args.Length);
                    newArgs[_args.Length] = str.ToString();

                    _executer = new ExecuteCommandStep(_fileName, newArgs)
                    {
                        WaitForExit = this.WaitForExit,
                        AsAdmin = this.AsAdmin,
                        AsUser = this.AsUser,
                        HideWindow = this.HideWindow,
                        InitMessage = this.InitMessage,
                        WorkingFolder = this.WorkingFolder
                    };

                }
                return _executer;
            }
        }

        public new void Report()
        {
            Executer.Report();
        }

        public new void Perform()
        {
            Executer.Perform();
        }

        public new void Init()
        {
            Executer.Init();
        }
    }
}
