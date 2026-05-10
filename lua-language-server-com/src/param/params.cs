using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#nullable enable
namespace src.param
{
    
    internal class Params
    {
        private static readonly Lazy<Params> _instance =
            new Lazy<Params>(() => new Params());
        private IDictionary<string,string?> _params = new Dictionary<string,string?>();
        public static Params instance => _instance.Value;

        private Params()
        {
            if (!_instance.IsValueCreated)
            {
                if (Environment.GetCommandLineArgs().Length > 2)
                {
                    string[] args = Environment.GetCommandLineArgs().Skip(2).ToArray();
                    foreach (string arg in args)
                    {
                        parse(arg);
                    }
                }
            }
        }
        public void parse(string arg){
            if(!arg.StartsWith("--")){
                throw new ArgumentException("Invalid argument format");
            }
            arg.Remove(0, 2);
            string[] keyValue = arg.Split("=");
            _params.Add(keyValue[0],keyValue.Length == 2 ? keyValue[1] : "true");
        }
        public string this[string key]{
            get { return _params[key] ?? "false"; }
            set { _params[key] = value; }
        }
        public int Lenght => _params.Count;
    }
}
