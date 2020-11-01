using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DoubleLinkedDirectedGraph
{
    /// <summary>
    /// Double Linked Directed Graph allows graph crawling in both directions where the edge cannot contain data
    /// </summary>
    /// <typeparam name="NodeData"></typeparam>
    public class DoubleLinkedDirectedGraph<NodeData, EdgeData>
    {
        #region private

        public static readonly string START_NODE_KEY = "START";
        public static readonly string END_NODE_KEY = "END";
        private Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        private bool graphLocked = false;

        /// <summary>
        /// Gets existing or creates a new node with the key. Enforces nodeKey naming
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <returns></returns>
        internal Node GetNode(string nodeKey)
        {
            string correctedNodeKey = EnforceNodeKeyNamingRule(nodeKey);
            if (nodes.ContainsKey(correctedNodeKey))
            {
                return nodes[correctedNodeKey];
            }
            else
            {
                Node node = new Node(this,correctedNodeKey);
                nodes.Add(node.NodeKey, node);
                return node;
            }
        }

        /// <summary>
        /// Returns the start node of the graph
        /// </summary>
        private Node StartNode
        {
            get
            {
                return GetNode(START_NODE_KEY);
            }
        }

        /// <summary>
        /// Returns the end node of the graph
        /// </summary>
        private Node EndNode
        {
            get
            {
                return GetNode(END_NODE_KEY);
            }
        }

        /// <summary>
        /// Attaching the nodes that are not going to anywhere to the end node. 
        /// This should only be called when the building is finished as it locks the graph for further inserts
        /// </summary>
        private void DeduceImplicitEndNodes()
        {
            if (!graphLocked)
            {
                //Make sure end node has been created
                GetNode(END_NODE_KEY);
                foreach (Node node in nodes.Values)
                {
                    if (node.NodeKey != END_NODE_KEY && node.NextEdges.Count == 0)
                    {
                        node.Insert(END_NODE_KEY, string.Empty);
                    }
                }
                graphLocked = true;
            }
        }

        /// <summary>
        /// Enforces nodeKey naming rules
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <returns></returns>
        private string EnforceNodeKeyNamingRule(string nodeKey)
        {
            if (string.IsNullOrWhiteSpace(nodeKey))
            {
                return Guid.NewGuid().ToString().Replace("-", "");
            }
            else
            {
                return nodeKey.Replace(" ", "").Replace("-", "").Replace("_", "").Trim();
            }
        }

        private Options ActiveOptions { get; set; }

        #endregion
        #region public

        public DoubleLinkedDirectedGraph():this(new Options())
        { 
        }

        public DoubleLinkedDirectedGraph(Options options)
        {
            ActiveOptions = options;
        }

        /// <summary>
        /// Returns all the start nodes
        /// </summary>
        public IEnumerable<Node> Start
        {
            get
            {
                foreach (Edge edge in StartNode.NextEdges.Values)
                {
                    yield return edge.ToNode;
                }
            }
        }

        /// <summary>
        /// Returns all the end nodes
        /// </summary>
        public IEnumerable<Node> End
        {
            get
            {
                if (!graphLocked)
                {
                    DeduceImplicitEndNodes();
                }
                foreach (Edge edge in EndNode.PreviousEdges.Values)
                {
                    yield return edge.FromNode;
                }
            }
        }

        /// <summary>
        /// Inserts node from start and returns the new node
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        public Node InsertFromStart(string nodeKey, NodeData nodeData = default)
        {
            if(graphLocked)
            {
                throw new DoubleLinkedDirectedGraphException("Graph is finished either by call to FinishGraph or implicitly by using End");
            }
            return StartNode.Insert(nodeKey, string.Empty, nodeData);
        }

        /// <summary>
        /// NodeKeys are unique in the graph and edgekey is unique in the scope of the from amnd to nodes.
        /// </summary>
        /// <param name="fromNodeKey"></param>
        /// <param name="newNodeKey"></param>
        /// <param name="edgeDescription"></param>
        /// <param name="nodeData"></param>
        /// <param name="edgeData"></param>
        /// <returns></returns>
        public Node Insert(string fromNodeKey, string newNodeKey, string edgeDescription = "", NodeData nodeData = default, EdgeData edgeData = default)
        {
            if (graphLocked)
            {
                throw new DoubleLinkedDirectedGraphException("Graph is finished either by call to FinishGraph or implicitly by using End");
            }
            Node fromNode = GetNode(fromNodeKey);
            return fromNode.Insert(newNodeKey, edgeDescription, nodeData, edgeData);
        }

        /// <summary>
        /// Finshed the graph and locks it
        /// </summary>
        public void FinishGraph()
        {
            DeduceImplicitEndNodes();
        }

        #endregion

        /// <summary>
        /// Representing the options for DoubleLinkedDirectedGraph
        /// </summary>
        public class Options
        {
            /// <summary>
            /// Moves from NodeKey as globally unique to only enforcing it locally
            /// </summary>
            /// TODO implement
            public bool TreatNodeKeysAsOnlyLocallyUnique { get; set; } = false;
        }

        /// <summary>
        /// Represents the node in the graph
        /// </summary>
        public class Node
        {
            private DoubleLinkedDirectedGraph<NodeData, EdgeData> _parent;

            internal Node(DoubleLinkedDirectedGraph<NodeData, EdgeData> parent, string nodeKey)
            {
                _parent = parent;
                NodeKey = nodeKey;
            }

            /// <summary>
            /// Inserts node into the edge
            /// </summary>
            /// <param name="newNodeKey"></param>
            /// <param name="edgeDescription"></param>
            /// <param name="nodeData"></param>
            /// <param name="edgeData"></param>
            /// <returns></returns>
            public Node Insert(string newNodeKey, string edgeDescription = "", NodeData nodeData = default, EdgeData edgeData = default)
            {
                if (_parent.graphLocked)
                {
                    throw new DoubleLinkedDirectedGraphException("Graph is finished either by call to FinishGraph or implicitly by using End");
                }
                Node newNode = _parent.GetNode(newNodeKey);
                newNode.NodeData = nodeData;
                GetEdge(this, newNode, edgeDescription, edgeData);
                return newNode;
            }

            public DoubleLinkedDirectedGraph<NodeData, EdgeData> InsertEnd()
            {
                Insert(END_NODE_KEY);
                return _parent;
            }

            /// <summary>
            /// Finshed the graph and locks it
            /// </summary>
            public void FinishGraph()
            {
                _parent.FinishGraph();
            }

            /// <summary>
            /// Get existing edge or create a new edge
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <param name="edgeDescription"></param>
            /// <param name="edgeData"></param>
            /// <returns></returns>
            private Edge GetEdge(Node fromNode, Node toNode, string edgeDescription = "", EdgeData edgeData = default)
            {
                string edgeKey = $"{fromNode.NodeKey}->{toNode.NodeKey}";
                if (fromNode.NextEdges.ContainsKey(edgeKey))
                {
                    Edge edge = fromNode.NextEdges[edgeKey];
                    edge.EdgeDescription = edgeDescription;
                    edge.EdgeData = edgeData;
                    return edge;
                }
                else
                {
                    return new Edge(fromNode, toNode, edgeKey, edgeDescription, edgeData);
                }

            }

            /// <summary>
            /// Get previous edges going agains direction
            /// </summary>
            public Dictionary<string, Edge> PreviousEdges { get; } = new Dictionary<string,Edge>();

            /// <summary>
            /// NodeKey of the Node
            /// </summary>
            public string NodeKey { get; }

            /// <summary>
            /// The contained Node
            /// </summary>
            public NodeData NodeData { get; set; }

            /// <summary>
            /// Get the next edges going with direction
            /// </summary>
            public Dictionary<string, Edge> NextEdges { get; } = new Dictionary<string,Edge>();
        }

        /// <summary>
        /// Represents the connections between the nodes in the graph
        /// </summary>
        public class Edge
        {
            internal Edge(Node fromNode, Node toNode, string edgeKey, string edgeDescription = "", EdgeData edgeData = default)
            {
                FromNode = fromNode;
                ToNode = toNode;
                EdgeDescription = edgeDescription;
                EdgeKey = edgeKey;
                EdgeData = edgeData;
                fromNode.NextEdges.Add(edgeKey, this);
                toNode.PreviousEdges.Add(edgeKey, this);
            }

            /// <summary>
            /// Edge starting point
            /// </summary>
            public Node FromNode { get; }

            /// <summary>
            /// Edge end point
            /// </summary>
            public Node ToNode { get; }

            /// <summary>
            /// Edge key is unique inside the from node
            /// </summary>
            public string EdgeKey { get; }
            
            /// <summary>
            /// Edge description
            /// </summary>
            public string EdgeDescription { get; internal set; }

            /// <summary>
            /// EgdeData saved on the edges
            /// </summary>
            public EdgeData EdgeData { get; set; }
        }
    }
}
