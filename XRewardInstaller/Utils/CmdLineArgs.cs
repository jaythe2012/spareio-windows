using System;
using System.Collections.Generic;
using System.Linq;

namespace Spareio.Installer.Utils
{
    internal class CmdLineArgs
    {
        private Dictionary<string, string> _args = new Dictionary<string, string>();

        internal CmdLineArgs(params string[] args)
        {
            // parse args in the format --<argname>=<argvalue> or /<argname>
            foreach (var arg in args)
            {
                var grp = arg.Split('=');
                if (grp.Count() > 1)
                {
                    var key = grp[0].ToLower().TrimStart(new char[] { '-', '/' });
                    var val = grp[1];
                    if (_args.ContainsKey(key))
                    {
                        // override with latest
                        _args[key] = val;
                    }
                    else
                    {
                        // add new
                        _args.Add(key, val);
                    }
                }
                else
                {
                    var key = arg.ToLower().TrimStart(new char[] { '-', '/' });
                    if (!_args.ContainsKey(key))
                    {
                        _args.Add(key, String.Empty);
                    }
                }
            }
        }

        internal bool CheckArg(string name)
        {
            bool b = _args.ContainsKey(name.ToLower());
            return _args.ContainsKey(name.ToLower());
        }

        internal string GetArgValue(string name)
        {
            if (_args.ContainsKey(name.ToLower()))
                return _args[name.ToLower()];

            return null;
        }

        internal int GetIntArgValue(string name)
        {
            if (_args.ContainsKey(name.ToLower()))
            {
                int result = 0;

                if (Int32.TryParse(_args[name.ToLower()], out result))
                    return result;
            }

            return 0;
        }
    }
}
