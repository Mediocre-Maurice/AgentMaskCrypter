﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ObfuscationStub.Core.Utils;
using System.Linq;

namespace ObfuscationStub.Core.Protections.AddRandoms
{
	class RandomOutlinedMethods : Randoms
	{
		public static void Execute(ModuleDef module)
		{
			foreach (var type in module.Types)
			{
				foreach (var method in type.Methods.ToArray())
				{
					MethodDef strings = CreateReturnMethodDef(RandomString(), method);
					MethodDef ints = CreateReturnMethodDef(RandomInt(), method);
					type.Methods.Add(strings);
					type.Methods.Add(ints);
				}
			}
		}

		static MethodDef CreateReturnMethodDef(object value, MethodDef source_method)
		{
			CorLibTypeSig corlib = null;

			if (value is int)
				corlib = source_method.Module.CorLibTypes.Int32;
			else if (value is float)
				corlib = source_method.Module.CorLibTypes.Single;
			else if (value is string)
				corlib = source_method.Module.CorLibTypes.String;
			MethodDef newMethod = new MethodDefUser(RandomString(),
					MethodSig.CreateStatic(corlib),
					MethodImplAttributes.IL | MethodImplAttributes.Managed,
					MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
			{
				Body = new CilBody()
			};
			if (value is int)
				newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)value));
			else if (value is float)
				newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_R4, (double)value));
			else if (value is string)
				newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, (string)value));
			newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ret));
			return newMethod;
		}
	}
}