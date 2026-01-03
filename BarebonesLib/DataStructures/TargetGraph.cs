using Barebones.Asset;
using Barebones.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.DataStructures
{

    /// <summary>
    /// A graph for navigating targetable objects.
    /// </summary>
    /// <remarks>
    /// Add or remove all your nodes, then call <see cref="CalculateNeighbours"/>. 
    /// </remarks>
    /// <typeparam name="T">The type of object, must implement <see cref="ITargetable"/>.</typeparam>
    public class TargetGraph<T> where T : notnull, ITargetable
    {
        internal class TargetGraphSorter : IComparer<NodeValues>
        {
            float _targetAngle = 0f;

            public TargetGraphSorter(float targetAngle)
            {
                _targetAngle = targetAngle;
            }

            public void SetAngle(float targetAngle)
            {
                _targetAngle = targetAngle;
            }

            public int Compare(NodeValues x, NodeValues y)
            {
                int compare = x.Distance.CompareTo(y.Distance);
                if (compare == 0)
                {
                    double v1 = Math.Abs(_targetAngle - Math.Abs(x.Angle));
                    double v2 = Math.Abs(_targetAngle - Math.Abs(y.Angle));
                    if (v1 < v2)
                        return -1;
                    else if (v1 > v2)
                        return 1;
                }
                return compare;
            }
        }

        internal class Node
        {
            private T _point;

            private List<NodeValues> _upBucket;
            private List<NodeValues> _rightBucket;
            private List<NodeValues> _downBucket;
            private List<NodeValues> _leftBucket;

            public Vector2 Position
            {
                get { return _point.Position; }
            }
            public T Point
            {
                get { return _point; }
            }

            public List<NodeValues> Up
            {
                get { return _upBucket; }
            }

            public List<NodeValues> Right
            {
                get { return _rightBucket; }
            }

            public List<NodeValues> Left
            {
                get { return _leftBucket; }
            }

            public List<NodeValues> Down
            { 
                get { return _downBucket; } 
            }

            public Node(T point)
            {
                _point = point;
                _upBucket = new List<NodeValues>();
                _rightBucket = new List<NodeValues>();
                _downBucket = new List<NodeValues>();
                _leftBucket = new List<NodeValues>();
            }



            public float Distance(Node other)
            {
                return Vector2.Distance(Position, other.Position);
            }

            public float Angle(Node other)
            {
                double angle = Math.Atan2((double)(Position.Y - other.Position.Y), (double)(Position.X - other.Position.X));
                angle = double.RadiansToDegrees(angle);
                return (float)angle;
            }

            public Node? GetClosestNeighbour()
            {
                NodeValues? nearest = null;
                foreach (NodeValues node in _upBucket)
                {
                    if (node.StoredNode.Point.IsTargetable)
                    {
                        if (nearest.HasValue == false)
                            nearest = node;
                        else if (nearest.Value.Distance > node.Distance)
                            nearest = node;
                    }
                }
                
                foreach (NodeValues node in _rightBucket)
                {
                    if (node.StoredNode.Point.IsTargetable)
                    {
                        if (nearest.HasValue == false)
                            nearest = node;
                        else if (nearest.Value.Distance > node.Distance)
                            nearest = node;
                    }
                }
                
                foreach(NodeValues node in _downBucket)
                {
                    if (node.StoredNode.Point.IsTargetable)
                    {
                        if (nearest.HasValue == false)
                            nearest = node;
                        else if (nearest.Value.Distance > node.Distance)
                            nearest = node;
                    }
                }

                foreach(NodeValues node in _leftBucket)
                {
                    if (node.StoredNode.Point.IsTargetable)
                    {
                        if (nearest.HasValue == false)
                            nearest = node;
                        else if (nearest.Value.Distance > node.Distance)
                            nearest = node;
                    }
                }

                if (nearest.HasValue == false)
                    return null;
                else
                    return nearest.Value.StoredNode;
            }

            public void SortBuckets()
            {
                TargetGraphSorter sorter = new TargetGraphSorter(90);
                _upBucket.Sort(sorter);
                _downBucket.Sort(sorter);
                sorter.SetAngle(0);
                _rightBucket.Sort(sorter);
                sorter.SetAngle(180);
                _leftBucket.Sort(sorter);
            }

        }
        internal struct NodeValues
        {
            private Node _node;
            private float _dist;
            private float _angle;

            public float Distance
            {
                get { return _dist; }
            }

            public float Angle
            {
                get { return _angle; }
            }

            public Node StoredNode
            {
                get { return _node; }
            }

            public NodeValues(Node node, float dist, float angle)
            {
                _node = node;
                _dist = dist;
                _angle = angle;
            }
        }


        private Node _selectedNode;
        private List<Node> _nodes;
        private Mutex _mut;
        
        

        private bool _autoCalculate = false;

        private string? _selectionChangeSound = null;

        /// <summary>
        /// Whether or not this graph should automatically call <see cref="CalculateNeighbours"/> whenever a node is added or removed.
        /// </summary>
        /// <remarks>
        /// If you're going to turn this on, I recommend only doing it after your initial set up of the graph.
        /// </remarks>
        public bool AutoCalculate
        {
            get { return _autoCalculate; }
            set { _autoCalculate = value; }
        }

        /// <summary>
        /// Determines if the target graph is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _nodes.Count == 0; }
        }

        /// <summary>
        /// The currently selected <typeparamref name="T"/>.
        /// </summary>
        public T Selected
        {
            get 
            {
                if (_selectedNode != null)
                    return _selectedNode.Point;
                else
                {
                    if (_nodes?.Count > 0)
                    {
                        _selectedNode = _nodes[0];
                        return _selectedNode.Point;
                    }
                }
                return _selectedNode.Point;
            }
        }

        /// <summary>
        /// Initalize an empty TargetGraph.
        /// </summary>
        /// <remarks>
        /// Populate it with <typeparamref name="T"/>, then call <see cref="CalculateNeighbours"/>.
        /// </remarks>
        public TargetGraph()
        {
            _nodes = new List<Node>();
            _mut = new Mutex();
        }

        /// <summary>
        /// Initalize an empty TargetGraph, and set <see cref="AutoCalculate"/>.
        /// </summary>
        /// <remarks>
        /// Populate it with <typeparamref name="T"/>, then call <see cref="CalculateNeighbours"/>.
        /// </remarks>
        /// <param name="autoCalculate">Should we auto calculate neighbours when the contents of the graph are changed?</param>
        public TargetGraph(bool autoCalculate) : this()
        {
            _autoCalculate = autoCalculate;
        }

        /// <summary>
        /// Set a sound to be played when the selection is changed.
        /// </summary>
        /// <param name="scriptPath">The path to the SoundScript.</param>
        public void SetSelectionSound(string scriptPath)
        {
            _selectionChangeSound = scriptPath;
        }

        /// <summary>
        /// Attempts to change the selected node to the provided <typeparamref name="T"/>.
        /// </summary>
        /// <param name="obj">The <typeparamref name="T"/> to select.</param>
        /// <returns>True if the <typeparamref name="T"/> was selected succesfully, False otherwise.</returns>
        public bool SelectNode(T obj)
        {
            _mut.WaitOne();
            foreach (Node node in _nodes)
            {
                if (node.Point.Equals(obj))
                {
                    _selectedNode = node;
                    return true;
                }
            }
            _mut.ReleaseMutex();
            return false;
        }

        internal Node? FindNode(T obj)
        {
            _mut.WaitOne();
            foreach (Node node in _nodes)
            {
                if (node.Point.Equals(obj))
                {
                    return node;
                }
            }
            _mut.ReleaseMutex();
            return null;
        }

        /// <summary>
        /// Add the <typeparamref name="T"/> provided as a new node in the graph.
        /// </summary>
        /// <param name="obj">The <typeparamref name="T"/> to add.</param>
        public void AddNode(T obj)
        {
            if (FindNode(obj) == null)
            {
                _mut.WaitOne();
                _nodes.Add(new Node(obj));
                _mut.ReleaseMutex();
                if (_autoCalculate)
                    CalculateNeighbours();
            } 
            else
            {
                Verbose.WriteErrorMinor($"Attempted to add duplicate {typeof(T).Name} to graph!");
            }
        }

        /// <summary>
        /// Removes the <typeparamref name="T"/> provided from the graph, if it exists.
        /// </summary>
        /// <param name="obj">The <typeparamref name="T"/> to remove.</param>
        public void RemoveNode(T obj)
        {
            Node? n = FindNode(obj);
            if (n == null)
                return;
            _mut.WaitOne();
            _nodes.Remove(n);
            if (n == _selectedNode)
            {
                n = n.GetClosestNeighbour();
                if (n != null)
                {
                    _selectedNode = n;
                }
                else if (_nodes?.Count > 0)
                {
                    foreach (Node node in _nodes)
                    {
                        if (node.Point.IsTargetable)
                            _selectedNode = node;
                        break;
                    }
                }
                else
                {
                    Verbose.WriteLogMinor($"TargetGraph is empty!");
                }
            }
            _mut.ReleaseMutex();
            if (_autoCalculate)
                CalculateNeighbours();
        }

        /// <summary>
        /// Calculates the neighbours for all nodes in the graph.
        /// </summary>
        /// <remarks>
        /// Call this manually after bulk adding/removing nodes.
        /// Or set AutoCalculate to true.
        /// </remarks>
        public void CalculateNeighbours()
        {
            _mut.WaitOne();
            foreach (Node node in _nodes)
            {
                node.Up.Clear();
                node.Down.Clear();
                node.Left.Clear();
                node.Right.Clear();
                List<NodeValues> nodeValues = new List<NodeValues>();
                foreach (Node other in _nodes)
                {
                    if (node != other)
                    {
                        nodeValues.Add(new NodeValues(other, node.Distance(other), node.Angle(other)));
                    }
                }
                foreach (NodeValues value in nodeValues)
                {
                    if (value.Angle <= 135 && value.Angle > 45)
                        node.Up.Add(value);
                    else if (value.Angle <= 45 && value.Angle > -45)
                        node.Right.Add(value);
                    else if (value.Angle <= -45 && value.Angle > -135)
                        node.Down.Add(value);
                    else if (value.Angle >= -135 || value.Angle > 135)
                        node.Left.Add(value);
                }
                if (node.Up.Count == 0)
                {
                    foreach (NodeValues value in nodeValues)
                    {
                        if (value.Angle <= 160 && value.Angle >= 20)
                            node.Up.Add(value);
                    }
                }
                if (node.Right.Count == 0)
                {
                    foreach (NodeValues value in nodeValues)
                    {
                        if (value.Angle <= 70 && value.Angle >= -70)
                            node.Right.Add(value);
                    }
                }
                if (node.Down.Count == 0)
                {
                    foreach (NodeValues value in nodeValues)
                    {
                        if (value.Angle <= -45 && value.Angle >= -135)
                            node.Down.Add(value);
                    }
                }
                if (node.Left.Count == 0)
                {
                    foreach (NodeValues value in nodeValues)
                    {
                        if (value.Angle <= -110 || value.Angle >= 110)
                            node.Left.Add(value);
                    }
                }
                node.SortBuckets();
            }  
            _mut.ReleaseMutex();
        }

        internal void PlaySound()
        {
            if (_selectionChangeSound != null)
            {
                Sound.PlaySound(_selectionChangeSound);
            }
        }

        /// <summary>
        /// Attempt to move to the first node above the currently selected node.
        /// </summary>
        public void SelectUp()
        {
            if (_selectedNode == null)
            {
                if (_nodes?.Count > 0)
                {
                    foreach (Node node in _nodes)
                    {
                        if (node.Point.IsTargetable)
                            _selectedNode = node;
                        break;
                    }
                    PlaySound();
                    return;
                }
            }
            if (_selectedNode != null)
            {
                foreach (NodeValues value in _selectedNode.Up)
                {
                    if (value.StoredNode.Point.IsTargetable)
                    {
                        _selectedNode = value.StoredNode;
                        PlaySound();
                        return;
                    }
                }
                if (Engine.TargetGraphWrapSelection)
                {
                    for (int i = _selectedNode.Down.Count - 1; i >= 0; i--)
                    {
                        if (_selectedNode.Down[i].StoredNode.Point.IsTargetable)
                        {
                            _selectedNode = _selectedNode.Down[i].StoredNode;
                            PlaySound();
                            return;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Attempt to move to the first node to the right of the currently selected node.
        /// </summary>
        public void SelectRight()
        {
            if (_selectedNode == null)
            {
                if (_nodes?.Count > 0)
                {
                    foreach (Node node in _nodes)
                    {
                        if (node.Point.IsTargetable)
                            _selectedNode = node;
                        break;
                    }
                    PlaySound();
                    return;
                }
            }
            if (_selectedNode != null)
            {
                foreach (NodeValues value in _selectedNode.Right)
                {
                    if (value.StoredNode.Point.IsTargetable)
                    {
                        _selectedNode = value.StoredNode;
                        PlaySound();
                        return;
                    }
                }
                if (Engine.TargetGraphWrapSelection)
                {
                    for (int i = _selectedNode.Left.Count - 1; i >= 0; i--)
                    {
                        if (_selectedNode.Left[i].StoredNode.Point.IsTargetable)
                        {
                            _selectedNode = _selectedNode.Left[i].StoredNode;
                            PlaySound();
                            return;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Attempt to move to the first node below the currently selected node.
        /// </summary>
        public void SelectDown()
        {
            if (_selectedNode == null)
            {
                if (_nodes?.Count > 0)
                {
                    foreach (Node node in _nodes)
                    {
                        if (node.Point.IsTargetable)
                            _selectedNode = node;
                        break;
                    }
                    PlaySound();
                    return;
                }
            }
            if (_selectedNode != null)
            {
                foreach (NodeValues value in _selectedNode.Down)
                {
                    if (value.StoredNode.Point.IsTargetable)
                    {
                        _selectedNode = value.StoredNode;
                        PlaySound();
                        return;
                    }
                }
                if (Engine.TargetGraphWrapSelection)
                {
                    for (int i = _selectedNode.Up.Count - 1; i >= 0; i--)
                    {
                        if (_selectedNode.Up[i].StoredNode.Point.IsTargetable)
                        {
                            _selectedNode = _selectedNode.Up[i].StoredNode;
                            PlaySound();
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to move to the first node to the left of the currently selected node.
        /// </summary>
        public void SelectLeft()
        {
            if (_selectedNode == null)
            {
                if (_nodes?.Count > 0)
                {
                    foreach (Node node in _nodes)
                    {
                        if (node.Point.IsTargetable)
                            _selectedNode = node;
                        break;
                    }
                    PlaySound();
                    return;
                }
            }
            if (_selectedNode != null)
            {
                foreach (NodeValues value in _selectedNode.Left)
                {
                    if (value.StoredNode.Point.IsTargetable)
                    {
                        _selectedNode = value.StoredNode;
                        PlaySound();
                        return;
                    }
                }
                if (Engine.TargetGraphWrapSelection)
                {
                    for (int i = _selectedNode.Right.Count - 1; i >= 0; i--)
                    {
                        if (_selectedNode.Right[i].StoredNode.Point.IsTargetable)
                        {
                            _selectedNode = _selectedNode.Right[i].StoredNode;
                            PlaySound();
                            return;
                        }
                    }
                }
            }
        }
    }
}
