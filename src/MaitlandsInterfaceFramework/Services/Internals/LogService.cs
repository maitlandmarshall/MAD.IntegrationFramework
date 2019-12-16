using MaitlandsInterfaceFramework.Database;
using System;
using System.ComponentModel.DataAnnotations;

namespace MaitlandsInterfaceFramework.Services.Internals
{
    public static class LogService
    {
        public static void WriteToLog(string text)
        {
            Console.WriteLine(text);
        }
    }
}
