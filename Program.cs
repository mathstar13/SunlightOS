using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Linq;
namespace SunlightOS
{
    class Program
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Booting SunlightOS.");
            //bootstuff//
            Console.Clear();
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars["cdir"] = "//";
            var di = "//";
            while (true)
            {
                var dt = Run(di, vars);
                if (dt["ri"]["dt"] != "true")
                {
                    break;
                }
                di = dt["di"]["dt"];
                vars = dt["vars"];
            }
        }
        public static Dictionary<string, Dictionary<string, string>> Run(string di,Dictionary<string,string> vars)
        {
            var fs = JsonSerializer.Deserialize<Dictionary<string,string>>(File.ReadAllText("data/fs.sl"));
            var data = JsonSerializer.Deserialize<Dictionary<string, string[]>>(File.ReadAllText("data/data.sl"));
            var dirs = JsonSerializer.Deserialize<string[]>(File.ReadAllText("data/dirs.sl"));
            if (!dirs.Contains(di))
            {
                Console.WriteLine("SunlightError: Unknown directory.");
                var dt = new Dictionary<string, Dictionary<string, string>>
                {
                    ["vars"] = vars
                };
                dt["di"] = new Dictionary<string, string>();
                dt["di"]["dt"] = di;
                dt["ri"] = new Dictionary<string, string>();
                dt["ri"]["dt"] = "true";
                return dt;
            }
            Console.Write($"{di}> ");
            string command = Console.ReadLine();
            foreach (string v in vars.Keys)
            {
                command = command.Replace($"${v}", vars[v]);
            }
            string[] cmda = command.Split(" ");
            if (cmda[0] == "readf")
            {
               Console.WriteLine(fs[$"{di}{cmda[1]}"]);
            }
            else if(cmda[0] == "cd")
            {

            }
            else if (fs.ContainsKey($"{di}{cmda[0]}.sle"))
            {
                Dictionary<string, string> vr = new Dictionary<string, string>();
                int cnt = 0;
                foreach(string arg in cmda)
                {
                    vr[Convert.ToString(cnt)] = arg;
                    cnt += 1;
                }
                foreach (string ln in fs[$"{di}{cmda[0]}.sle"].Split(';')){
                    string line = ln;
                    foreach(string item in vr.Keys)
                    {
                        line = line.Replace($"${item}", vr[item]);
                    }
                    Regex rx = new Regex(@"^[ \n\t+]*write:[ \n\t+]*(?<a>.*)$", RegexOptions.Compiled);
                    Regex cvr = new Regex(@"^[ \n\t+]*var:[ \n\t+]*(?<a>[^ \n\t]*)[ \n\t+]*=[ \n\t+]*(?<b>.*)$", RegexOptions.Compiled);
                    if (rx.Matches(line).Count == 1)
                    {
                        Console.WriteLine(rx.Matches(line)[0].Groups[1]);
                    }
                    else if(cvr.Matches(line).Count == 1)
                    {
                        var dt = cvr.Matches(line)[0].Groups;
                        vr[dt[1].ToString()] = dt[2].ToString();
                    }
                    else
                    {
                        Console.WriteLine($"SunlightError: Invalid SLE command \"{line}\".");
                        break;
                    }
                }
            }
            else if (fs.ContainsKey($"{data["path"][0]}{cmda[0]}.sle"))
            {
                Dictionary<string, string> vr = new Dictionary<string, string>();
                int cnt = 0;
                foreach (string arg in cmda)
                {
                    vr[Convert.ToString(cnt)] = arg;
                    cnt += 1;
                }
                foreach (string ln in fs[$"{data["path"][0]}{cmda[0]}.sle"].Split(';'))
                {
                    string line = ln;
                    foreach (string item in vr.Keys)
                    {
                        line = line.Replace($"${item}", vr[item]);
                    }
                    Regex rx = new Regex(@"^[ \n\t+]*write:[ \n\t+]*(?<a>.*)$", RegexOptions.Compiled);
                    Regex cvr = new Regex(@"^[ \n\t+]*var:[ \n\t+]*(?<a>[^ \n\t]*)[ \n\t+]*=[ \n\t+]*(?<b>.*)$", RegexOptions.Compiled);
                    if (rx.Matches(line).Count == 1)
                    {
                        Console.WriteLine(rx.Matches(line)[0].Groups[1]);
                    }
                    else if (cvr.Matches(line).Count == 1)
                    {
                        var dt = cvr.Matches(line)[0].Groups;
                        vr[dt[1].ToString()] = dt[2].ToString();
                    }
                    else
                    {
                        Console.WriteLine($"SunlightError: Invalid SLE command \"{line}\".");
                        break;
                    }
                }
            }
            else if(cmda[0] == "var")
            {
                vars[cmda[1]] = cmda[2];
            }
            else
            {
                Console.WriteLine($"SunlightError: Invalid command \"{cmda[0]}\"");
                //return true//
            }
            vars["lcmd"] = command;
            var dts = new Dictionary<string, Dictionary<string, string>>
            {
                ["vars"] = vars
            };
            dts["di"] = new Dictionary<string, string>();
            dts["di"]["dt"] = di;
            dts["ri"] = new Dictionary<string, string>();
            dts["ri"]["dt"] = "true";
            return dts;
        }
    }
}
