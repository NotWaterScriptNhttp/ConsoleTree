using System;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleProgressList
{
    internal enum NodeStatus
    {
        Idle,
        Running,

        Success,
        Fail
    }

    internal class Node
    {
        internal struct ToStringData
        {
            public bool showTop;
            public bool childrenAsSections;
        }

        public const string PREFIX = "+- ";
        public static readonly Color IdleColor = new Color(16, 117, 224);
        public static readonly Color RunningColor = new Color(219, 240, 29);
        public static readonly Color SuccessColor = new Color(30, 208, 44);
        public static readonly Color FailColor = new Color(200, 28, 10);

        public delegate void OnNodeStatusUpdate(Node node, NodeStatus status);
        public static event OnNodeStatusUpdate onNodeStatusUpdate;

        private Dictionary<string, Node> ChildNodes = new Dictionary<string, Node>();
        private NodeStatus _status = NodeStatus.Idle;

        internal bool WasParented = false;
        internal bool IsBeingDebugged = false;

        public string Name { get; private set; } = "";
        public Node Parent { get; internal set; } = null;
        public bool IsTop => Parent == null && !WasParented;

        public string Text = "";
        public NodeStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                if (onNodeStatusUpdate != null)
                    onNodeStatusUpdate.Invoke(this, value);
                if (Parent != null)
                    Parent.CheckStatuses();
            }
        }

        internal void CheckStatuses()
        {
            NodeStatus temp = Status;
            bool success = true;
            foreach (Node node in ChildNodes.Values)
            {
                if (success && node.Status != NodeStatus.Success)
                    success = false;

                switch (node.Status)
                {
                    case NodeStatus.Idle:
                        continue;
                    case NodeStatus.Running:
                        temp = NodeStatus.Running;
                        break;
                    case NodeStatus.Fail:
                        temp = NodeStatus.Fail;
                        break;
                }

                if (temp == NodeStatus.Fail)
                    break;
            }

            if (success)
                temp = NodeStatus.Success;

            if (Status == temp)
                return;

            Status = temp;
        }

        public Node(string name) => Name = name;
        public Node(string name, Node[] children)
        {
            Name = name;
            if (children == null)
                throw new ArgumentNullException("children cannot be null");

            foreach (Node n in children)
                AddNode(n);

            CheckStatuses();
        }

        public void AddNode(Node node)
        {
            node.Parent = this;
            node.WasParented = true;
            ChildNodes.Add(node.Name, node);
        }
        public Node GetNode(string node, bool throwEx = true)
        {
            if (!ChildNodes.TryGetValue(node, out Node n))
                if (throwEx)
                    throw new ApplicationException($"Node '{node}' does not exist");
                else return null;

            return n;
        }

        public void RemoveNode(Node n)
        {
            ChildNodes.Remove(n.Name);
            n.Parent = null;
        }
        public void RemoveNode(string n)
        {
            if (!ChildNodes.TryGetValue(n, out Node node))
                throw new ApplicationException($"Node '{node}' does not exist");

            ChildNodes.Remove(n);
            node.Parent = null;
        }

        public void ClearNodes()
        {
            foreach (KeyValuePair<string, Node> kvp in ChildNodes)
                RemoveNode(kvp.Value);
        }

        internal string ToString(string prev, string indent, bool last, ToStringData tsd = default)
        {
            string output = "";
            if (!tsd.showTop)
            {
                TextBlock block = new TextBlock()
                {
                    text = Text + $" [{Status}]\n"
                };
                switch (Status)
                {
                    case NodeStatus.Idle:
                        block.foreground = IdleColor;
                        break;
                    case NodeStatus.Running:
                        block.foreground = RunningColor;
                        break;
                    case NodeStatus.Success:
                        block.foreground = SuccessColor;
                        break;
                    case NodeStatus.Fail:
                        block.foreground = FailColor;
                        break;
                }

                output = indent + PREFIX + ConsoleTextUtils.DoString(block);
                indent += tsd.childrenAsSections ? "   " : (last ? "   " : "|  ");
            }

            if (Status == NodeStatus.Success)
                return output;

            Dictionary<string, Node>.ValueCollection coll = ChildNodes.Values;
            for (int i = 0; i < coll.Count; i++)
                output += coll.ElementAt(i).ToString(output, indent, ChildNodes.Count - 1 == i, IsTop ? new ToStringData { childrenAsSections = tsd.childrenAsSections } : default);

            return output;
        }

        public override string ToString()
        {
            if (ChildNodes == null)
                throw new ApplicationException("ChildNodes cannot be null");

            return ToString("", "", true);
        }
        public string ToString(bool showTopNode, bool ChildrenAsSections = false)
        {
            if (ChildNodes == null)
                throw new ApplicationException("ChildNodes cannot be null");

            ToStringData tsd = default;
            tsd.showTop = !showTopNode;
            tsd.childrenAsSections = ChildrenAsSections;

            return ToString("", "", true, tsd);
        }
    }
}
