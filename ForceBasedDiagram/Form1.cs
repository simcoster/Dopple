using DoppleGraph;
using DoppleTry2;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForceBasedDiagram
{
    public partial class Form1 : Form
    {
        public Diagram mDiagram = new Diagram();
        private Random mRandom;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mRandom = new Random();


        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // draw with anti-aliasing and a 12 pixel border
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            mDiagram.Draw(e.Graphics, Rectangle.FromLTRB(12, 12, ClientSize.Width - 12, ClientSize.Height - 12));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // redraw on resize
            Invalidate();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            AssemblyDefinition myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\Release\Utility.dll");

            CodeColorHanlder colorCode = new CodeColorHanlder();
            TypeDefinition type = myLibrary.MainModule.Types[2];

            foreach (var method in type.Methods)
            {
                Form1 newForm = new Form1();
                newForm.btnGenerate.Hide();
                GraphBuilder backTraceManager = new GraphBuilder(method);
                var instructionWrappers = backTraceManager.Run();

                List<GoNodeWrapper> nodeWrappers = new List<GoNodeWrapper>();

                newForm.Text = method.Name;
                //foreach (var instWrapper in instructionWrappers.Where(x => x.ForwardDataFlowRelated.Count > 0 || x.BackDataFlowRelated.Count > 0))
                foreach (var instWrapper in instructionWrappers)
                {
                    nodeWrappers.Add(new GoNodeWrapper(
                        new SpotNode(colorCode.GetColor(instWrapper.Instruction.OpCode.Code)),
                        instWrapper));
                }

                foreach (var nodeWrapper in nodeWrappers)
                {
                    newForm.mDiagram.AddNode(nodeWrapper.SpotNode);
                }

                foreach (var nodeWrapper in nodeWrappers)
                {
                    foreach (var forwardNode in nodeWrapper.InstWrapper.DataFlowForwardRelated)
                    {
                        nodeWrapper.SpotNode.AddChild(nodeWrappers.First(x => x.InstWrapper == forwardNode).SpotNode);
                    }
                }

                // mDiagram.Clear();

                // create a basic, random diagram that is between 2 and 4 levels deep 
                // and has between 1 and 10 leaf nodes per branch
                //Node node = new SpotNode(Color.Black);
                //mDiagram.AddNode(node);

                //for (int i = 0; i < mRandom.Next(1, 10); i++)
                //{
                //    Node child = new SpotNode(Color.Navy);
                //    node.AddChild(child);

                //    for (int j = 0; j < mRandom.Next(0, 10); j++)
                //    {
                //        Node grandchild = new SpotNode(Color.Blue);
                //        child.AddChild(grandchild);

                //        for (int k = 0; k < mRandom.Next(0, 10); k++)
                //        {
                //            Node descendant = new SpotNode(Color.CornflowerBlue);
                //            grandchild.AddChild(descendant);
                //        }
                //    }
                //}

                // run the force-directed algorithm (async)
                Cursor = Cursors.WaitCursor;
                btnGenerate.Enabled = false;
                Thread bg = new Thread(newForm.mDiagram.Arrange);
                bg.IsBackground = true;
                bg.Start();

                Graphics g = CreateGraphics();

#if ANIMATE
			while (bg.IsAlive) {
				Invalidate();
				Application.DoEvents();
				Thread.Sleep(20);
			}
#else
                bg.Join();
#endif

                btnGenerate.Enabled = true;
                Cursor = Cursors.Default;

                Invalidate();
                newForm.Show();
            }
        }
    }
}
