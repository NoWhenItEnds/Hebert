using Godot;
using Hebert.Entities;
using System;
using System.Collections.Generic;

namespace Hebert.Types.AStar
{
    /// <summary> A AStar implementation for traversing cells in a chunk. </summary>
    public partial class CellAStar3D : AStar3D
    {
        /// <summary> A grid of all the cells within the chunk. </summary>
        private readonly Cell[,,] CELLS;


        /// <summary> A AStar implementation for traversing cells in a chunk. </summary>
        /// <param name="cells"> A grid of all the cells within the chunk. </param>
        public CellAStar3D(Cell[,,] cells)
        {
            NeighborFilterEnabled = true;
            CELLS = cells;

            // Add points.
            for (Int32 z = 0; z < cells.GetLength(2); z++)
            {
                for (Int32 y = 0; y < cells.GetLength(1); y++)
                {
                    for (Int32 x = 0; x < cells.GetLength(0); x++)
                    {
                        // Add point.
                        Vector3I position = new Vector3I(x, y, z);
                        Int64 idx = GetPointId(position);
                        AddPoint(idx, position);
                    }
                }
            }


            // Add connections.
            Vector3I[] neighbours = GenerateNeighbours();
            for (Int32 z = 0; z < cells.GetLength(2) - 1; z++)
            {
                for (Int32 y = 0; y < cells.GetLength(1) - 1; y++)
                {
                    for (Int32 x = 0; x < cells.GetLength(0) - 1; x++)
                    {
                        Vector3I position = new Vector3I(x, y, z);
                        Int64 idx = GetPointId(new Vector3I(x, y, z));
                        foreach (Vector3I neighbour in neighbours)
                        {
                            Vector3I neighbourPosition = position + neighbour;
                            Int64 neighbourIdx = GetPointId(neighbourPosition);
                            ConnectPoints(idx, neighbourIdx, true);
                        }
                    }
                }
            }
        }


        /// <inheritdoc/>
        public override Single _ComputeCost(Int64 fromId, Int64 toId)
        {
            Vector3 fromPoint = GetPointPosition(fromId);
            Cell fromCell = CELLS[(Int32)fromPoint.X, (Int32)fromPoint.Y, (Int32)fromPoint.Z];
            Vector3 toPoint = GetPointPosition(toId);
            Cell toCell = CELLS[(Int32)toPoint.X, (Int32)toPoint.Y, (Int32)toPoint.Z];
            // TODO - Calculate cost.
            return base._ComputeCost(fromId, toId);
        }


        /// <inheritdoc/>
        public override Single _EstimateCost(Int64 fromId, Int64 endId)
        {
            Vector3 fromPoint = GetPointPosition(fromId);
            Cell fromCell = CELLS[(Int32)fromPoint.X, (Int32)fromPoint.Y, (Int32)fromPoint.Z];
            Vector3 endPoint = GetPointPosition(endId);
            Cell endCell = CELLS[(Int32)endPoint.X, (Int32)endPoint.Y, (Int32)endPoint.Z];
            // TODO - Calculate cost.
            return base._EstimateCost(fromId, endId);
        }


        /// <inheritdoc/>
        public override Boolean _FilterNeighbor(Int64 fromId, Int64 neighborId)
        {
            Vector3 neighbourPoint = GetPointPosition(neighborId);
            Cell neighbourCell = CELLS[(Int32)neighbourPoint.X, (Int32)neighbourPoint.Y, (Int32)neighbourPoint.Z];
            return neighbourCell.BlocksMovement;
        }



        /// <summary> Generate an array of relative neighbour positions. </summary>
        private Vector3I[] GenerateNeighbours()
        {
            List<Vector3I> positions = new List<Vector3I>();
            for (Int32 z = 0; z <= 1; z++)
            {
                for (Int32 y = 0; y <= 1; y++)
                {
                    for (Int32 x = 0; x <= 1; x++)
                    {
                        if (!(x == 0 && y == 0 && z == 0))   // All positions but the centre.
                        {
                            positions.Add(new Vector3I(x, y, z));
                        }
                    }
                }
            }
            return positions.ToArray();
        }


        /// <summary> Get the id of the cell at a position. </summary>
        /// <param name="position"> The position to get an id for. </param>
        /// <returns> An id representing the position in the graph. </returns>
        private Int64 GetPointId(Vector3I position) => position.X + position.Y * CELLS.GetLength(0) + position.Z * CELLS.GetLength(0) * CELLS.GetLength(1);
    }
}
