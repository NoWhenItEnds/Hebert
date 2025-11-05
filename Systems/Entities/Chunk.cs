using System;
using System.Collections.Generic;
using Godot;
using Hebert.Types.AStar;

namespace Hebert.Entities
{
    /// <summary> A unit of world space that keeps track of entities within its bounds. </summary>
    public class Chunk : IEquatable<Chunk>
    {
        /// <summary> The coordinates in chunk space the chunk is positioned. </summary>
        public readonly Vector3I ChunkPosition;

        /// <summary> How many cells the chunk contains. </summary>
        public readonly Int32 ChunkSize;

        /// <summary> A grid of all the cells within the chunk. </summary>
        public readonly Cell[,,] Cells;

        /// <summary> The AStar implementation used for moving between cells within the chunk. </summary>
        public readonly CellAStar3D Graph;


        /// <summary> A unit of world space that keeps track of entities within its bounds. </summary>
        /// <param name="chunkPosition"> The position in chunk space to place the chunk. </param>
        /// <param name="chunkSize"> The number of cells the chunk should represent. </param>
        public Chunk(Vector3I chunkPosition, Int32 chunkSize)
        {
            ChunkPosition = chunkPosition;
            ChunkSize = chunkSize;
            Cells = new Cell[chunkSize, chunkSize, chunkSize];

            // Initialise the cells.
            List<Cell> cells = new List<Cell>();    // A 1D array of the cells for building the graph.
            for (Int32 z = 0; z < chunkSize; z++)
            {
                for (Int32 y = 0; y < chunkSize; y++)
                {
                    for (Int32 x = 0; x < chunkSize; x++)
                    {
                        Vector3I currentPosition = new Vector3I(x, y, z);
                        Cell cell = new Cell(this, currentPosition);
                        Cells[x, y, z] = cell;
                        cells.Add(cell);
                    }
                }
            }

            Graph = new CellAStar3D(Cells);

            GD.Print($"Chunk @ {ChunkPosition} was initialised.");    // TODO - Use logger.
        }


        /// <summary> Get the position of a global position relative to the chunk, or in chunk coordinates. </summary>
        /// <param name="globalPosition"> The global position to convert. </param>
        /// <returns> The converted position in chunk coordinates. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> If the position exceeds the bounds of the chunk. </exception>
        public Vector3I GetLocalPosition(Vector3I globalPosition)
        {
            Vector3I localPosition = globalPosition - (ChunkPosition * ChunkSize);
            if (localPosition < Vector3I.Zero || localPosition >= new Vector3I(ChunkSize, ChunkSize, ChunkSize))
            {
                throw new ArgumentOutOfRangeException($"The given global position, {globalPosition}, when converted to local space, {localPosition}, is beyond the chunk bounds of {Vector3I.Zero} - {ChunkSize}.");
            }

            return localPosition;
        }


        /// <summary> Gets an array of the entities current occupying the given chunk position. </summary>
        /// <param name="localPosition"> The chunk position to get the entities from. </param>
        /// <returns> An array of the entities currently occupying the given position.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> If the position exceeds the bounds of the chunk. </exception>
        public IEntity[] GetEntities(Vector3I localPosition)
        {
            if (localPosition < Vector3I.Zero || localPosition >= new Vector3I(ChunkSize, ChunkSize, ChunkSize))
            {
                throw new ArgumentOutOfRangeException($"The given position, {localPosition}, is beyond the chunk bounds of {Vector3I.Zero} - {ChunkSize}.");
            }

            return Cells[localPosition.X, localPosition.Y, localPosition.Z].Entities.ToArray();
        }


        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(ChunkPosition);


        /// <inheritdoc/>
        public Boolean Equals(Chunk? other) => ChunkPosition == other?.ChunkPosition;
    }
}

/*
public partial class Chunk : Node3D
{
    public const int Size = 16;
    public AStar3D Astar { get; } = new AStar3D();
    public Vector3I ChunkPos { get; set; }
    public Dictionary<Vector3I, List<long>> BorderPoints { get; } = new();

    public void BuildAStar()
    {
        Astar.ReserveSpace(Size * Size * Size);

        Vector3I[] directions =
        {
            Vector3I.Left, Vector3I.Right,
            Vector3I.Up, Vector3I.Down,
            Vector3I.Forward, Vector3I.Back
        };
        foreach (var d in directions)
            BorderPoints[d] = new List<long>();

        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                for (int z = 0; z < Size; z++)
                {
                    Vector3I local = new Vector3I(x, y, z);
                    if (IsNavigable(local))
                    {
                        long id = GetPointId(local);
                        Astar.AddPoint(id, new Vector3(x, y, z));

                        if (x == 0) BorderPoints[Vector3I.Left].Add(id);
                        if (x == Size - 1) BorderPoints[Vector3I.Right].Add(id);
                        if (y == 0) BorderPoints[Vector3I.Down].Add(id);
                        if (y == Size - 1) BorderPoints[Vector3I.Up].Add(id);
                        if (z == 0) BorderPoints[Vector3I.Back].Add(id);
                        if (z == Size - 1) BorderPoints[Vector3I.Forward].Add(id);
                    }
                }

        Vector3I[] neighborDirs =
        {
            Vector3I.Left, Vector3I.Right,
            Vector3I.Up, Vector3I.Down,
            Vector3I.Forward, Vector3I.Back
        };
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                for (int z = 0; z < Size; z++)
                {
                    Vector3I local = new Vector3I(x, y, z);
                    if (IsNavigable(local))
                    {
                        long id = GetPointId(local);
                        foreach (var nd in neighborDirs)
                        {
                            Vector3I nlocal = local + nd;
                            if (nlocal.X >= 0 && nlocal.X < Size &&
                                nlocal.Y >= 0 && nlocal.Y < Size &&
                                nlocal.Z >= 0 && nlocal.Z < Size &&
                                IsNavigable(nlocal))
                            {
                                long nid = GetPointId(nlocal);
                                Astar.ConnectPoint(id, nid, true);
                            }
                        }
                    }
                }
    }

    private long GetPointId(Vector3I local) => local.X + (long)local.Y * Size + (long)local.Z * Size * Size;

    protected virtual bool IsNavigable(Vector3I local)
    {
        // Override this in a derived class to check if a position is navigable based on chunk data (e.g., voxel terrain).
        // For demonstration, assume all positions are navigable.
        return true;
    }
}

public partial class WorldChunkManager : Node
{
    private Dictionary<Vector3I, Chunk> chunks = new();

    public Chunk GetChunk(Vector3I pos)
    {
        if (!chunks.TryGetValue(pos, out Chunk chunk))
        {
            chunk = new Chunk { ChunkPos = pos };
            AddChild(chunk);
            chunk.Position = new Vector3(pos.X, pos.Y, pos.Z) * Chunk.Size;
            chunk.BuildAStar();
            chunks[pos] = chunk;
        }
        return chunk;
    }

    private Vector3I WorldToChunk(Vector3 world)
    {
        return new Vector3I(
            (int)MathF.Floor(world.X / Chunk.Size),
            (int)MathF.Floor(world.Y / Chunk.Size),
            (int)MathF.Floor(world.Z / Chunk.Size)
        );
    }

    private Vector3 WorldToLocal(Vector3 world, Vector3I chunkPos)
    {
        return world - new Vector3(chunkPos.X, chunkPos.Y, chunkPos.Z) * Chunk.Size;
    }

    private Vector3 LocalToWorld(Vector3 local, Vector3I chunkPos)
    {
        return local + new Vector3(chunkPos.X, chunkPos.Y, chunkPos.Z) * Chunk.Size;
    }

    public Vector3[] FindPath(Vector3 from, Vector3 to)
    {
        Vector3I startChunkPos = WorldToChunk(from);
        Vector3I endChunkPos = WorldToChunk(to);
        Chunk startChunk = GetChunk(startChunkPos);
        Chunk endChunk = GetChunk(endChunkPos);
        Vector3 localFrom = WorldToLocal(from, startChunkPos);
        Vector3 localTo = WorldToLocal(to, endChunkPos);

        if (startChunkPos == endChunkPos)
        {
            long startId = startChunk.Astar.GetClosestPoint(localFrom);
            long endId = endChunk.Astar.GetClosestPoint(localTo);
            Vector3[] localPath = startChunk.Astar.GetPointPath(startId, endId);
            for (int i = 0; i < localPath.Length; i++)
            {
                localPath[i] = LocalToWorld(localPath[i], startChunkPos);
            }
            return localPath;
        }

        // Create high-level AStar3D for chunk pathfinding (bounding box only)
        AStar3D chunkAstar = new AStar3D();
        Vector3I minPos = Vector3I.Min(startChunkPos, endChunkPos);
        Vector3I maxPos = Vector3I.Max(startChunkPos, endChunkPos);
        Dictionary<Vector3I, long> chunkToId = new();
        long nextChunkId = 0;
        for (int x = minPos.X; x <= maxPos.X; x++)
            for (int y = minPos.Y; y <= maxPos.Y; y++)
                for (int z = minPos.Z; z <= maxPos.Z; z++)
                {
                    Vector3I cp = new Vector3I(x, y, z);
                    long cid = nextChunkId++;
                    chunkToId[cp] = cid;
                    chunkAstar.AddPoint(cid, new Vector3(x, y, z));
                }

        // Connect adjacent chunks in the high-level graph
        Vector3I[] chunkDirs =
        {
            Vector3I.Left, Vector3I.Right,
            Vector3I.Up, Vector3I.Down,
            Vector3I.Forward, Vector3I.Back
        };
        foreach (var kv in chunkToId)
        {
            Vector3I cp = kv.Key;
            long cid = kv.Value;
            foreach (var cd in chunkDirs)
            {
                Vector3I np = cp + cd;
                if (chunkToId.TryGetValue(np, out long nid))
                {
                    chunkAstar.ConnectPoint(cid, nid);
                }
            }
        }

        long startChunkId = chunkToId[startChunkPos];
        long endChunkId = chunkToId[endChunkPos];
        Vector3[] chunkPathPos = chunkAstar.GetPointPath(startChunkId, endChunkId);
        if (chunkPathPos.Length == 0)
            return Array.Empty<Vector3>();

        List<Vector3I> chunkPath = chunkPathPos.Select(p => new Vector3I((int)MathF.Round(p.X), (int)MathF.Round(p.Y), (int)MathF.Round(p.Z))).ToList();

        // Stitch detailed paths across chunks
        List<Vector3> fullPath = new();
        Vector3 currentFromWorld = from;
        Vector3I currentChunkPos = startChunkPos;
        Chunk currentChunk = startChunk;
        long currentStartId = currentChunk.Astar.GetClosestPoint(WorldToLocal(currentFromWorld, currentChunkPos));

        for (int i = 0; i < chunkPath.Count - 1; i++)
        {
            Vector3I nextChunkPos = chunkPath[i + 1];
            Vector3I dir = nextChunkPos - currentChunkPos;
            int axis = dir.X != 0 ? 0 : dir.Y != 0 ? 1 : 2;
            int dirSign = GetComponent(dir, axis);
            Vector3I borderDir = new Vector3I();
            borderDir[axis] = dirSign;

            // Compute border plane position in world space
            float borderWorld = dirSign > 0 ? (currentChunkPos[axis] + 1) * Chunk.Size : currentChunkPos[axis] * Chunk.Size;

            // Compute intersection parameter t
            float den = GetComponent(to, axis) - GetComponent(currentFromWorld, axis);
            float t = 0;
            if (MathF.Abs(den) >= 1e-6f)
            {
                t = (borderWorld - GetComponent(currentFromWorld, axis)) / den;
                t = MathF.Clamp(t, 0, 1);
            }

            // Compute intersection on perpendicular axes
            int perp1 = (axis + 1) % 3;
            int perp2 = (axis + 2) % 3;
            float interPerp1 = GetComponent(currentFromWorld, perp1) + t * (GetComponent(to, perp1) - GetComponent(currentFromWorld, perp1));
            float interPerp2 = GetComponent(currentFromWorld, perp2) + t * (GetComponent(to, perp2) - GetComponent(currentFromWorld, perp2));

            // Convert to local
            float localInterPerp1 = interPerp1 - GetComponent(currentChunkPos, perp1) * Chunk.Size;
            float localInterPerp2 = interPerp2 - GetComponent(currentChunkPos, perp2) * Chunk.Size;

            // Find closest exit point on border
            if (!BorderPoints.TryGetValue(borderDir, out var exitCandidates) || exitCandidates.Count == 0)
                return Array.Empty<Vector3>();

            long exitId = -1;
            float minDist = float.MaxValue;
            foreach (long cand in exitCandidates)
            {
                Vector3 lpos = currentChunk.Astar.GetPointPosition(cand);
                float d1 = GetComponent(lpos, perp1) - localInterPerp1;
                float d2 = GetComponent(lpos, perp2) - localInterPerp2;
                float dist = d1 * d1 + d2 * d2;
                if (dist < minDist)
                {
                    minDist = dist;
                    exitId = cand;
                }
            }
            if (exitId == -1)
                return Array.Empty<Vector3>();

            // Get path to exit and add to full path
            Vector3[] segmentPath = currentChunk.Astar.GetPointPath(currentStartId, exitId);
            foreach (var p in segmentPath)
            {
                fullPath.Add(LocalToWorld(p, currentChunkPos));
            }

            // Find corresponding entry point in next chunk
            Chunk nextChunk = GetChunk(nextChunkPos);
            Vector3I entryBorderDir = new Vector3I();
            entryBorderDir[axis] = -dirSign;
            if (!nextChunk.BorderPoints.TryGetValue(entryBorderDir, out var entryCandidates) || entryCandidates.Count == 0)
                return Array.Empty<Vector3>();

            long entryId = -1;
            minDist = float.MaxValue;
            foreach (long cand in entryCandidates)
            {
                Vector3 lpos = nextChunk.Astar.GetPointPosition(cand);
                float d1 = GetComponent(lpos, perp1) - localInterPerp1;
                float d2 = GetComponent(lpos, perp2) - localInterPerp2;
                float dist = d1 * d1 + d2 * d2;
                if (dist < minDist)
                {
                    minDist = dist;
                    entryId = cand;
                }
            }
            if (entryId == -1)
                return Array.Empty<Vector3>();

            // Update for next iteration
            currentChunkPos = nextChunkPos;
            currentChunk = nextChunk;
            currentStartId = entryId;
            currentFromWorld = LocalToWorld(nextChunk.Astar.GetPointPosition(entryId), currentChunkPos);
        }

        // Final segment to destination
        long finalEndId = currentChunk.Astar.GetClosestPoint(localTo);
        Vector3[] lastPath = currentChunk.Astar.GetPointPath(currentStartId, finalEndId);
        foreach (var p in lastPath)
        {
            fullPath.Add(LocalToWorld(p, currentChunkPos));
        }

        return fullPath.ToArray();
    }

    private int GetComponent(Vector3I v, int i)
    {
        return i switch
        {
            0 => v.X,
            1 => v.Y,
            _ => v.Z
        };
    }

    private float GetComponent(Vector3 v, int i)
    {
        return i switch
        {
            0 => v.X,
            1 => v.Y,
            _ => v.Z
        };
    }
}*/
