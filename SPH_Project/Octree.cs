using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Mark DiVelbiss
/// The Octree class is for storing data within a 3D spatial structure.
/// </summary>
/// <typeparam name="T">The data type of objects to be stored in the octree.</typeparam>
public class Octree<T> {
    Vector3 position, dimensions;
    public Node Root;

    /// <summary>
    /// Creates a new octree object based on bounds.
    /// </summary>
    /// <param name="position">The position where the octree begins.</param>
    /// <param name="dimensions">The positive x, y and z dimensions of the bounding box going out from the position vector.</param>
    public Octree(Vector3 position, Vector3 dimensions)
    {
        this.position = position;
        this.dimensions = dimensions;
        Reset();
    }

    /// <summary>
    /// Empties the octree by reseting the root node.
    /// This is the only way to remove values from the tree.
    /// </summary>
    public void Reset() { Root = new Node(position, dimensions); }

    /// <summary>
    /// Node structure that is used to build the octree.
    /// </summary>
    public struct Node
    {
        /// <summary>
        /// Creates a new node at position with a bounding box defined by dimenions.
        /// Nodes begin unbranched and empty.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="dimensions">The positive dimensions of the bounding box of the node beginning at the node's position.</param>
        public Node(Vector3 position, Vector3 dimensions)
        {
            this.position = position;
            this.dimensions = dimensions;
            branch = new Node[8];
            branched = false;
            hasObject = false;
            obj = default(T);
        }

        Node[] branch;
        Vector3 position, dimensions;
        T obj;
        bool branched, hasObject;

        /// <summary>
        /// Returns whether or not the current node is storing data
        /// </summary>
        public bool HasObject { get { return hasObject; } }
        /// <summary>
        /// Returns whether or not the current node is branched into more nodes.
        /// </summary>
        public bool Branched { get { return branched; } }
        /// <summary>
        /// Returns the data stored at the current node.
        /// Will return default value for T if node !HasObject
        /// </summary>
        public T Object { get { return obj; } }
        /// <summary>
        /// Allows array-style access of branching nodes.
        /// </summary>
        /// <param name="i">The index for a specific branch of the node, 0-7.</param>
        /// <returns>Returns the branched node specified by i, or this node if this node is not branched or if the index is out of bounds.</returns>
        public Node this[int i] { get { return (branched && i >= 0 && i < 8 ? branch[i] : this); } }

        /// <summary>
        /// Fast bound checking for nodes.
        /// </summary>
        /// <param name="position">The position you're verifying is within bounds.</param>
        /// <param name="range">The acceptable range of the box surrounding the position. Normally zero.</param>
        /// <returns></returns>
        public bool IsInBounds(Vector3 position, float range = 0)
        {
            return (position.x + range >= this.position.x && position.x - range <= this.position.x + this.dimensions.x &&
                position.y + range >= this.position.y && position.y - range <= this.position.y + this.dimensions.y &&
                position.z + range >= this.position.z && position.z - range <= this.position.z + this.dimensions.z);
        }

        /// <summary>
        /// Attempts to add the given object data to this node.
        /// Fails if position is out of bounds.
        /// Succeeds if in bounds and this node is not storing object data.
        /// Otherwise, node attempts to add the object to branching nodes.
        /// </summary>
        /// <param name="obj">Object data to be added to the octree.</param>
        /// <param name="position">Position the object is located in space.</param>
        /// <returns>Whether or not adding the object succeeded, either in this node or in some child node.</returns>
        public bool AddObject(T obj, Vector3 position)
        {
            if (!IsInBounds(position)) return false;
            else if (!hasObject)
            {
                this.obj = obj;
                hasObject = true;
                return true;
            }
            else
            {
                Branch();
                return branch[0].AddObject(obj, position) || branch[1].AddObject(obj, position) ||
                       branch[2].AddObject(obj, position) || branch[3].AddObject(obj, position) ||
                       branch[4].AddObject(obj, position) || branch[5].AddObject(obj, position) ||
                       branch[6].AddObject(obj, position) || branch[7].AddObject(obj, position);
            }
        }

        /// <summary>
        /// Private method for creating the 8 branch nodes for any given node in the octree.
        /// Will do nothing if the node is already branched.
        /// </summary>
        private void Branch()
        {
            if (branched) return;
            branch[0] = new Node(position, dimensions / 2); // LSW
            branch[1] = new Node(new Vector3(position.x + dimensions.x / 2, position.y, position.z), dimensions / 2); // LSE
            branch[2] = new Node(new Vector3(position.x, position.y, position.z + dimensions.z / 2), dimensions / 2); // LNW
            branch[3] = new Node(new Vector3(position.x + dimensions.x / 2, position.y, position.z + dimensions.z / 2), dimensions / 2); // LNE
            branch[4] = new Node(new Vector3(position.x, position.y + dimensions.y / 2, position.z), dimensions / 2); // USW
            branch[5] = new Node(new Vector3(position.x + dimensions.x / 2, position.y + dimensions.y / 2, position.z), dimensions / 2); // USE
            branch[6] = new Node(new Vector3(position.x, position.y + dimensions.y / 2, position.z + dimensions.z / 2), dimensions / 2); // UNW
            branch[7] = new Node(new Vector3(position.x + dimensions.x / 2, position.y + dimensions.y / 2, position.z + dimensions.z / 2), dimensions / 2); // UNE
            branched = true;
        }
    }
}
