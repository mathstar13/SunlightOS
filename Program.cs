using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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
            while (true)
            {
                bool dt = Run("//", vars);
                if (!dt)
                {
                    break;
                }
            }
        }
        public static bool Run(string di,Dictionary<string,string> vars)
        {
            var fs = JsonSerializer.Deserialize<Dictionary<string,string>>(File.ReadAllText("data/fs.sl"));
            var data = JsonSerializer.Deserialize<Dictionary<string, string[]>>(File.ReadAllText("data/data.sl"));
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
            else if (fs.ContainsKey($"{di}{cmda[0]}.sle"))
            {
                Dictionary<string, string> vr = new Dictionary<string, string>();
                int cnt = 0;
                foreach(string arg in cmda)
                {
                    vr[$"${cnt}"] = arg;
                    cnt += 1;
                }
                Console.WriteLine(JsonSerializer.Serialize(vr));
                foreach (string ln in fs[$"{di}{cmda[0]}.sle"].Split(';')){
                    string line = ln;
                    foreach(string item in vr.Keys)
                    {
                        line = line.Replace($"${item}", vr[item]);
                    }
                    Console.WriteLine(line);
                    Regex rx = new Regex(@"^[ \n\t+]*write:[ \n\t+]*(?<a>.*)$", RegexOptions.Compiled);
                    if(rx.Matches(line).Count == 1)
                    {
                        Console.WriteLine(rx.Matches(line)[0].Groups[1]);
                    }
                    else
                    {
                        Console.WriteLine($"SunlightError: Invalid SLE command \"{line}\".");
                        return false;
                    }
                }
            }
            //$"{data["path"][0]}{cmda[0]}.sle"//
            else if (fs.ContainsKey($"{data["path"][0]}{cmda[0]}.sle"))
            {
                foreach (string line in fs[$"{data["path"][0]}{cmda[0]}.sle"].Split(';'))
                {
                    Regex rx = new Regex(@"^[ \n\t+]*write:[ \n\t+]*(?<a>.*)$", RegexOptions.Compiled);
                    if (rx.Matches(line).Count == 1)
                    {
                        Console.WriteLine(rx.Matches(line)[0].Groups[1]);
                    }
                    else
                    {
                        Console.WriteLine($"SunlightError: Invalid SLE command \"{line}\".");
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine($"SunlightError: Invalid command \"{cmda[0]}\"");
                return false;
            }
            vars["lcmd"] = command;
            return true;
        }
    }
}
