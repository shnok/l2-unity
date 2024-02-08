using UnityEngine;
using System.Collections.Generic;

namespace KWS
{
    public class KW_MeshGenerator : MonoBehaviour
    {
        static List<int> triangles = new List<int>();
        static List<Vector3> vertices = new List<Vector3>();
        static List<Color> colors = new List<Color>();
        private static bool IsOutFarDistance;
        private static bool IsOutBoxFarDistance;
        private static float FarDistance;
        private static float BottomDistance;
        private static Vector3 GlobalScale;

        public static Mesh GeneratePlane(float startSizeMeters, int quadsPerStartSize, float maxSizeMeters)
        {
            IsOutFarDistance = false;
            IsOutBoxFarDistance = false;
            FarDistance = maxSizeMeters;
            BottomDistance = maxSizeMeters * 0.5f;
            GlobalScale = Vector3.one;

            vertices.Clear();
            triangles.Clear();
            colors.Clear();

            var offset = CreateStartChunk(startSizeMeters, quadsPerStartSize);

            var newSize = quadsPerStartSize / 2 + 4;
            var count = (int)((quadsPerStartSize / 4f));
            var lastCount = newSize - 2;
            do
            {
                var currentScale = count;
                offset += CreateChunk(lastCount + 2, (startSizeMeters * 0.5f + offset), currentScale, out lastCount);

            } while (offset * 0.5f < maxSizeMeters);

            var mesh = new Mesh();
            //mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            return mesh;
        }

        public static Mesh GenerateFinitePlane(int quadsPerStartSize, Vector3 scale)
        {
            IsOutFarDistance = false;
            IsOutBoxFarDistance = false;
            FarDistance = scale.y;
            BottomDistance = FarDistance;
            GlobalScale = scale;

            vertices.Clear();
            triangles.Clear();
            colors.Clear();

            CreateStartChunk(2, quadsPerStartSize, true);

            var mesh = new Mesh();
            //mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            return mesh;
        }

        private static float CreateStartChunk(float startSizeMeters, int quadsPerStartSize, bool isFiniteMesh = false)
        {
            var halfSize = quadsPerStartSize / 2;
            float quadLength = startSizeMeters / quadsPerStartSize;
            for (int i = 0; i < halfSize; i++)
            {
                AddRing(quadsPerStartSize - i * 2, (startSizeMeters * 0.5f - quadLength * i), false, isFiniteMesh);
            }
            var offset = quadLength * 2;
            AddRing(halfSize + 2, (startSizeMeters * 0.5f + offset), true, isFiniteMesh);
            return offset;
        }

        private static float CreateChunk(int size, float startScale, int count, out int lastCount)
        {
            float scaleOffset = 0;
            for (int i = 0; i < count; i++)
            {
                if (i < count - 1)
                {
                    var newSize = size + 2 * i;
                    scaleOffset += 1f / (size - 2) * startScale * 2;
                    AddRing(newSize, startScale + scaleOffset);
                }
                else
                {
                    int newSize = (size + 2 * i) / 2 + 1;
                    scaleOffset += 1f / (size - 2) * startScale * 4;
                    AddRing(newSize, (startScale + scaleOffset), true);
                }
            }
            lastCount = (size + 2 * (count - 1)) / 2 + 1;
            return scaleOffset;
        }



        private static void AddRing(int size, float scale, bool isTripple = false, bool isFiniteMesh = false)
        {
            if (IsOutFarDistance) return;

            if (IsOutFarDistance == false && scale * 0.5f > FarDistance)
            {
                IsOutFarDistance = true;
            }

            if (IsOutBoxFarDistance == false && scale > (isFiniteMesh ? 1 : FarDistance))
            {
                IsOutBoxFarDistance = true;
                AddBoxUnderwater(size, scale, Color.white);
            }


            int x, y = 0;
            for (x = 0; x < size; x++)
                CreateQuad(size, scale, x, y, Side.Down, isTripple, Color.black);

            x = size - 1;
            for (y = 1; y < size; y++)
                CreateQuad(size, scale, x, y, Side.Right, isTripple, Color.black);

            y = size - 1;
            for (x = size - 2; x >= 0; x--)
                CreateQuad(size, scale, x, y, Side.Up, isTripple, Color.black);

            x = 0;
            for (y = size - 2; y > 0; y--)
                CreateQuad(size, scale, x, y, Side.Left, isTripple, Color.black);
        }

        static void AddBoxUnderwater(int size, float scale, Color color)
        {
            int x, y = 0;
            for (x = 0; x < size; x++)
                CreateQuadVertical(size, scale, x, y, Side.Down, color);

            x = size - 1;
            for (y = 0; y < size; y++)
                CreateQuadVertical(size, scale, x, y, Side.Right, color);

            y = size - 1;
            for (x = size - 1; x >= 0; x--)
                CreateQuadVertical(size, scale, x, y, Side.Up, color);

            x = 0;
            for (y = size - 1; y >= 0; y--)
                CreateQuadVertical(size, scale, x, y, Side.Left, color);

            x = 0;
            y = 0;
            for (x = 0; x < size; x++)
                CreateQuadBootom(size, scale, x, y, Side.Down, color);

            x = size - 1;
            for (y = 0; y < size; y++)
                CreateQuadBootom(size, scale, x, y, Side.Right, color);

            y = size - 1;
            for (x = size - 1; x >= 0; x--)
                CreateQuadBootom(size, scale, x, y, Side.Up, color);

            x = 0;
            for (y = size - 1; y >= 0; y--)
                CreateQuadBootom(size, scale, x, y, Side.Left, color);
        }

        static void CreateQuad(int size, float scale, int x, int y, Side side, bool isTripple, Color color)
        {
            var offset = (1f / size) * GlobalScale * scale;
            var position = new Vector3((x / (float)size - 0.5f) * GlobalScale.x, 0, (y / (float)size - 0.5f) * GlobalScale.z) * scale;

            var leftBottomIndex = AddPoint(position, color);
            var rightBottomIndex = AddPoint(position + new Vector3(offset.x, 0, 0), color);
            var rightUpIndex = AddPoint(position + new Vector3(0, 0, offset.z), color);
            var leftUpIndex = AddPoint(position + new Vector3(offset.x, 0, offset.z), color);
            if (isTripple)
            {
                if (Mathf.Abs(x - y) == size - 1 || Mathf.Abs(x - y) == 0) side = Side.Fringe;
                AddTripplePoint(side, leftBottomIndex, rightBottomIndex, rightUpIndex,
                    leftUpIndex, position, offset, color);
            }
            else
            {
                AddQuadIndexes(leftBottomIndex, rightBottomIndex, rightUpIndex, leftUpIndex);
            }
        }

        static void CreateQuadVertical(int size, float scale, int x, int y, Side side, Color color)
        {
            var offset = (1f / size) * GlobalScale * scale;
            var position = new Vector3((x / (float)size - 0.5f) * GlobalScale.x, 0, (y / (float)size - 0.5f) * GlobalScale.z) * scale;

            var leftBottom_Height = Vector3.zero;
            var rightBottom_Height = new Vector3(offset.x, 0, 0);
            var rightUp_Height = new Vector3(0, 0, offset.z);
            var leftUp_Height = new Vector3(offset.x, 0, offset.z);
            switch (side)
            {
                case Side.Down:
                    rightUp_Height = new Vector3(0, -FarDistance, 0);
                    leftUp_Height = new Vector3(offset.x, -FarDistance, 0);
                    break;
                case Side.Right:
                    leftBottom_Height = new Vector3(offset.x, -FarDistance, 0);
                    rightUp_Height = new Vector3(offset.x, -FarDistance, offset.z);
                    break;
                case Side.Up:
                    leftBottom_Height = new Vector3(0, -FarDistance, offset.z);
                    rightBottom_Height = new Vector3(offset.x, -FarDistance, offset.z);
                    break;
                case Side.Left:
                    rightBottom_Height = new Vector3(0, -FarDistance, 0);
                    leftUp_Height = new Vector3(0, -FarDistance, offset.z);
                    break;
            }

            var leftBottomIndex = AddPoint(position + leftBottom_Height, color);
            var rightBottomIndex = AddPoint(position + rightBottom_Height, color);
            var rightUpIndex = AddPoint(position + rightUp_Height, color);
            var leftUpIndex = AddPoint(position + leftUp_Height, color);

            AddQuadIndexes(rightBottomIndex, leftBottomIndex, leftUpIndex, rightUpIndex);
        }

        static void CreateQuadBootom(int size, float scale, int x, int y, Side side, Color color)
        {
            var offset = (1f / size) * GlobalScale * scale;
            var position = new Vector3((x / (float)size - 0.5f) * GlobalScale.x, 0, (y / (float)size - 0.5f) * GlobalScale.z) * scale;

            var leftBottom_Height = position + new Vector3(0, -BottomDistance, 0);
            var rightBottom_Height = position + new Vector3(offset.x, -BottomDistance, 0);
            var rightUp_Height = position + new Vector3(0, -BottomDistance, offset.z);
            var leftUp_Height = position + new Vector3(offset.x, -BottomDistance, offset.z);
            if (side == Side.Down)
            {
                rightUp_Height = new Vector3(position.x, -BottomDistance, -position.z);
                leftUp_Height = new Vector3(position.x + offset.x, -BottomDistance, -position.z);
            }

            var leftBottomIndex = AddPoint(leftBottom_Height, color);
            var rightBottomIndex = AddPoint(rightBottom_Height, color);
            var rightUpIndex = AddPoint(rightUp_Height, color);
            var leftUpIndex = AddPoint(leftUp_Height, color);

            AddQuadIndexes(rightBottomIndex, leftBottomIndex, leftUpIndex, rightUpIndex);
        }

        static void AddTripplePoint(Side side, int leftBottomIndex, int rightBottomIndex, int rightUpIndex, int leftUpIndex, Vector3 position, Vector3 offset, Color color)
        {
            int middleIndex;
            if (side == Side.Fringe)
            {
                AddQuadIndexes(leftBottomIndex, rightBottomIndex, rightUpIndex, leftUpIndex);
                return;
            }

            if (side == Side.Down)
            {
                middleIndex = AddPoint(position + new Vector3(offset.x / 2f, 0, offset.z), color);
                AddTripleIndexesDown(leftBottomIndex, rightBottomIndex, rightUpIndex, leftUpIndex, middleIndex);
            }
            if (side == Side.Right)
            {
                middleIndex = AddPoint(position + new Vector3(0, 0, offset.z / 2), color);
                AddTripleIndexesRight(leftBottomIndex, rightBottomIndex, rightUpIndex, leftUpIndex, middleIndex);
            }
            if (side == Side.Up)
            {
                middleIndex = AddPoint(position + new Vector3(offset.x / 2, 0, 0), color);
                AddTripleIndexesUp(leftBottomIndex, rightBottomIndex, rightUpIndex, leftUpIndex, middleIndex);
            }
            if (side == Side.Left)
            {
                middleIndex = AddPoint(position + new Vector3(offset.x, 0, offset.z / 2), color);
                AddTripleIndexesLeft(leftBottomIndex, rightBottomIndex, rightUpIndex, leftUpIndex, middleIndex);
            }

        }


        static void AddQuadIndexes(int index1, int index2, int index3, int index4)
        {
            triangles.Add(index1); triangles.Add(index3); triangles.Add(index2);
            triangles.Add(index2); triangles.Add(index3); triangles.Add(index4);
        }



        #region TripleIndexes

        static void AddTripleIndexesDown(int index1, int index2, int index3, int index4, int index5)
        {
            triangles.Add(index3); triangles.Add(index5); triangles.Add(index1);
            triangles.Add(index1); triangles.Add(index5); triangles.Add(index2);
            triangles.Add(index5); triangles.Add(index4); triangles.Add(index2);
        }

        static void AddTripleIndexesRight(int index1, int index2, int index3, int index4, int index5)
        {
            triangles.Add(index3); triangles.Add(index4); triangles.Add(index5);
            triangles.Add(index1); triangles.Add(index5); triangles.Add(index2);
            triangles.Add(index5); triangles.Add(index4); triangles.Add(index2);
        }

        static void AddTripleIndexesUp(int index1, int index2, int index3, int index4, int index5)
        {
            triangles.Add(index3); triangles.Add(index4); triangles.Add(index5);
            triangles.Add(index3); triangles.Add(index5); triangles.Add(index1);
            triangles.Add(index5); triangles.Add(index4); triangles.Add(index2);
        }

        static void AddTripleIndexesLeft(int index1, int index2, int index3, int index4, int index5)
        {
            triangles.Add(index3); triangles.Add(index4); triangles.Add(index5);
            triangles.Add(index1); triangles.Add(index5); triangles.Add(index2);
            triangles.Add(index3); triangles.Add(index5); triangles.Add(index1);
        }
        #endregion

        enum Side
        {
            Down,
            Right,
            Up,
            Left,
            Fringe
        }


        static int AddPoint(Vector3 position, Color color)
        {
            vertices.Add(position);
            if (color != null) colors.Add(color);
            return vertices.Count - 1;
        }

    }
}
