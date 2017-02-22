﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    class LoadFunctionNode : InstructionNode
    {
        public MethodDefinition LoadedFunction { get; private set; }
        public LoadFunctionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            if (instruction.Operand is MethodDefinition)
            {
                LoadedFunction = (MethodDefinition)instruction.Operand;
            }
        }

    }
}
