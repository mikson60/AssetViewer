using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class AssetImporter : MonoBehaviour {

    enum DataType { Vertex, Normal, UV };

    enum ModelType { Obj };

    public unsafe static Asset ImportModelByText(string modelText)
    {
        int modelAddress = Create(new StringBuilder(modelText));

        Debug.LogFormat("Model Address is {0}", modelAddress.ToString());

        int vertexPointCount = GetModelVertexCount(modelAddress);
        int texCoordCount = GetModelTexCoordCount(modelAddress);
        int normalPointCount = GetModelNormalCount(modelAddress);
        int faceKeyCount = GetModelFaceCount(modelAddress);

        Debug.LogFormat("Vertices ({0}); Texture coordinates ({1}); Normals ({2}); Faces ({3})", vertexPointCount.ToString(), texCoordCount.ToString(), normalPointCount.ToString(), faceKeyCount.ToString());

        // Data
        Vector3[] vertices = GenerateData3D(vertexPointCount, DataType.Vertex);
        Vector3[] normals = GenerateData3D(normalPointCount, DataType.Normal);
        Vector2[] uvs = GenerateData2D(texCoordCount, DataType.UV);
        int[] faceData = GenerateFaceData(faceKeyCount);

        Asset asset = new GameObject("ImportedModel", typeof(Asset), typeof(MeshRenderer), typeof(MeshFilter)).GetComponent<Asset>();

        Mesh mesh = asset.GetComponent<MeshFilter>().mesh;
        MeshRenderer mr = asset.GetComponent<MeshRenderer>();

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = faceData;

        mr.material = Core.Instance.resources.defaultMaterial;

        Debug.Log("Returning asset " + asset);
        return asset;
    }

    static unsafe Vector3[] GenerateData3D(int pointCount, DataType dataType)
    {
        if (pointCount == 0 || pointCount % 3 != 0)
        {
            Debug.LogErrorFormat("No data points found ({0}) or % 3 does not equal 0 ({1}). Returning.", (pointCount == 0), (pointCount % 3 != 0));
            return null;
        }

        Vector3[] generatedData = new Vector3[pointCount / 3];

        float[] pointsArray = new float[pointCount];
        float* pointsPointer;

        using (GenerateItemsWrapper(out pointsPointer, out pointCount, dataType))
        {
            for (int i = 0; i < pointCount; i++)
            {
                pointsArray[i] = pointsPointer[i];
            }
        }

        int counter = 0;
        for (int i = 0; i < pointCount; i = i + 3)
        {
            generatedData[counter] = new Vector3(pointsArray[i], pointsArray[i + 1], pointsArray[i + 2]);
            counter++;
        }

        return generatedData;
    }

    static unsafe Vector2[] GenerateData2D(int pointCount, DataType dataType)
    {
        if (pointCount == 0 || pointCount % 2 != 0)
        {
            Debug.LogErrorFormat("No data points found ({0}) or % 2 does not equal 0 ({1}). Returning.", (pointCount == 0), (pointCount % 2 != 0));
            return null;
        }

        Vector2[] generatedData = new Vector2[pointCount / 2];

        float[] pointsArray = new float[pointCount];
        float* pointsPointer;

        using (GenerateItemsWrapper(out pointsPointer, out pointCount, dataType))
        {
            for (int i = 0; i < pointCount; i++)
            {
                pointsArray[i] = pointsPointer[i];
            }
        }

        int counter = 0;
        for (int i = 0; i < pointCount; i = i + 2)
        {
            generatedData[counter] = new Vector2(pointsArray[i], pointsArray[i + 1]);
            counter++;
        }

        return generatedData;
    }

    static unsafe int[] GenerateFaceData(int faceKeyCount)
    {
        StringData theStringData = new StringData(new IntPtr[faceKeyCount], new int[faceKeyCount]);

        int len = Marshal.SizeOf(typeof(StringData));
        IntPtr pnt = Marshal.AllocHGlobal(len);

        Marshal.StructureToPtr(theStringData, pnt, false); // Crashes here

        GetFaceKeys(pnt);

        Marshal.FreeHGlobal(pnt);

        Dictionary<string, int> faceKeyValueCount = new Dictionary<string, int>();

        int allFacesCount = 0; // the count of all faces

        for (int i = 0; i < theStringData.mylist.Length; i++)
        {
            allFacesCount += theStringData.valueCounts[i];
        }

        int[] allFaces = new int[allFacesCount]; // all faces

        int lastIndex = 0;

        for (int i = 0; i < theStringData.mylist.Length; i++)
        {
            string key = Marshal.PtrToStringAnsi(theStringData.mylist[i]);
            int faceCount = 0;

            ushort[] faces = new ushort[theStringData.valueCounts[i]];
            ushort* facesPointer;

            using (GenerateItemsWrapper(out facesPointer, new StringBuilder(key), out faceCount))
            {
                for (int j = 0; j < faceCount; j++)
                {
                    try
                    {
                        faces[j] = facesPointer[j];
                        allFaces[lastIndex + j] = facesPointer[j];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Debug.LogError("Got Index out of range");
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Debug.LogError("Got Argument out of range");
                    }
                    catch (NullReferenceException e)
                    {
                        Debug.LogError("Got a null ref exception");
                    }
                }
            }
            lastIndex += faceCount;
        }

        return allFaces;
    }

    #region wrapper

    [DllImport("ObjMan 7", EntryPoint = "Create", CallingConvention = CallingConvention.Cdecl)]
    public static extern int Create(StringBuilder str);

    [DllImport("ObjMan 7", EntryPoint = "GetModelVertexCount")]
    public static extern int GetModelVertexCount(int modelPointer);

    [DllImport("ObjMan 7", EntryPoint = "GetModelTexCoordCount")]
    public static extern int GetModelTexCoordCount(int modelPointer);

    [DllImport("ObjMan 7", EntryPoint = "GetModelNormalCount")]
    public static extern int GetModelNormalCount(int modelPointer);

    [DllImport("ObjMan 7", EntryPoint = "GetModelFaceCount")]
    public static extern int GetModelFaceCount(int modelPointer);

    [DllImport("ObjMan 7", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    static unsafe extern bool GetVertices(out ItemsSafeHandle itemsHandle,
        out float* items, out int itemCount);

    [DllImport("ObjMan 7", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    static unsafe extern bool GetNormals(out ItemsSafeHandle itemsHandle,
        out float* items, out int itemCount);

    [DllImport("ObjMan 7", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    static unsafe extern bool GetTextureCoordinates(out ItemsSafeHandle itemsHandle,
        out float* items, out int itemCount);

    [DllImport("ObjMan 7", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    static unsafe extern bool GetFaceValues(out ItemsSafeHandle itemsHandle,
        out ushort* items, StringBuilder str, out int faceCount);

    [DllImport("ObjMan 7")]
    private static extern void GetFaceKeys(IntPtr inArr);

    [DllImport("ObjMan 7", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    static unsafe extern bool ReleaseItems(IntPtr itemsHandle);

    static unsafe ItemsSafeHandle GenerateItemsWrapper(out float* items, out int itemsCount, DataType dataType)
    {
        ItemsSafeHandle itemsHandle;

        switch (dataType)
        {
            case DataType.Vertex:
                if (!GetVertices(out itemsHandle, out items, out itemsCount))
                {
                    throw new InvalidOperationException();
                }
                break;
            case DataType.Normal:
                if (!GetNormals(out itemsHandle, out items, out itemsCount))
                {
                    throw new InvalidOperationException();
                }
                break;
            case DataType.UV:
                if (!GetTextureCoordinates(out itemsHandle, out items, out itemsCount))
                {
                    throw new InvalidOperationException();
                }
                break;
            default:
                throw new InvalidOperationException();
        }
        
        return itemsHandle;
    }

    static unsafe ItemsSafeHandle GenerateItemsWrapper(out ushort* items, StringBuilder key, out int itemsCount)
    {
        ItemsSafeHandle itemsHandle;

        if (!GetFaceValues(out itemsHandle, out items, key, out itemsCount))
        {
            throw new InvalidOperationException();
        }

        return itemsHandle;
    }

    class ItemsSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ItemsSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return true;
            // return ReleaseItems(handle);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct StringData
    {
        public IntPtr[] mylist;
        public int[] valueCounts;

        public StringData(IntPtr[] mylist, int[] valueCounts) : this()
        {
            this.mylist = mylist;
            this.valueCounts = valueCounts;
        }
    };

    #endregion
}
