using System;
using dnlib.DotNet;
using ObfuscationStub.Core.Protections;
using ObfuscationStub.Core.Protections.AddRandoms;

namespace ObfuscationStub
{
    public class Obfuscator
{
    public static string Save(byte[] Byte,String Path)
    {
            try
            {
                var module = ModuleDefMD.Load(Byte);
                Execute(module);
                module.Write(Path);
                return "Obfuscated!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
    }
    static void Execute(ModuleDefMD module)
    {
        Renamer.Execute(module);
        RandomOutlinedMethods.Execute(module);
    }
}
}
