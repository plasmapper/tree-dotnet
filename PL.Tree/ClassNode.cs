using System.Collections.Generic;

namespace PL.Tree
{
    /// <summary>
    /// Base class for classes that contain nodes as properties and are parents to these nodes.
    /// </summary>
    public abstract class ClassNode : ContainerNode
    {
        private readonly List<Node> _children = new List<Node>();

        /// <summary>
        /// Creates private list of child nodes and assigns self as their parent.
        /// </summary>
        public ClassNode()
        {
            foreach (var p in GetType().GetProperties())
            {
                if (p.Name != "Value" && p.Name != "Parent" && p.CanRead && p.GetIndexParameters().Length == 0 && p.GetValue(this) is Node node)
                {
                    node.Parent = this;
                    _children.Add(node);
                }
            }
        }
        
        public override IEnumerable<Node> GetChildren() => _children;

        public override object GetValue() => this;
    }
}
