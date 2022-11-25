using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProgressList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleTextUtils.EnableTrueColor();

            Node owner = new Node("Starter", new Node[]
            {
                new Node("Node2", new Node[]
                {
                    new Node("Node4")
                    {
                        Text = "Finding xorkey",
                        Status = NodeStatus.Running
                    },
                    new Node("Node5")
                    {
                        Text = "Resolving calls to decrypt method"
                    }
                })
                {
                    Text = "String Decryption",
                    IsBeingDebugged = true
                },
                new Node("Node3", new Node[]
                {
                    new Node("Node6")
                    {
                        Text = "Finding obfuscated blocks",
                        Status = NodeStatus.Success
                    },
                    new Node("Node7")
                    {
                        Text = "Solving Arithmetics",
                        Status = NodeStatus.Success
                    },
                    new Node("Node8")
                    {
                        Text = "Solving the flows",
                        Status = NodeStatus.Success
                    }
                })
                {
                    Text = "Control Flow",
                    IsBeingDebugged = true
                },
                new Node("Node9", new Node[]
                {
                    new Node("Node10")
                    {
                        Text = "Reading all VMethods",
                        Status = NodeStatus.Success
                    },
                    new Node("Node11")
                    {
                        Text = "Finding all calls to VM.RunMethod",
                        Status = NodeStatus.Success
                    },
                    new Node("Node12")
                    {
                        Text = "DeVirtualizing Method <bruh>",
                        Status = NodeStatus.Success
                    },
                    new Node("Node13")
                    {
                        Text = "Fixing devirtualized methods",
                        Status = NodeStatus.Fail
                    }
                })
                {
                    Text = "DeVirtualization"
                }
            });
            owner.Text = "Deobfuscation";

            Console.WriteLine(owner.ToString(true, true));

            TextBlock[] blocks = new TextBlock[]
            {
                new TextBlock
                {
                    text = "Text1",
                    foreground = Node.SuccessColor,
                    background = Node.FailColor
                }
            };

            Console.WriteLine(ConsoleTextUtils.DoString(blocks));

            Console.ReadLine();
        }
    }
}
