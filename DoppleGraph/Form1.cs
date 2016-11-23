using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using DoppleTry2;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Northwoods.Go;
using DoppleTry2.BackTrackers;

namespace DoppleGraph
{
    public partial class Form1 : Form
    {
     
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Maybe swtich to graphVIZZZZ
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>

        private void Form1_Load(object sender, EventArgs e)
        {
            AssemblyDefinition myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\Release\Utility.dll");
            //myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\Debug\Utility.dll");

            TypeDefinition type = myLibrary.MainModule.Types[2];

            foreach (var method in type.Methods.Where(x => !x.IsConstructor && x.Name == "DoMerge"))
            //foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            {
                GraphBuilder backTraceManager = new GraphBuilder(method);
                var instructionWrappers = backTraceManager.Run();

                Form2 newForm = new Form2(instructionWrappers);
                newForm.Show();
            }
        }
    }
}
