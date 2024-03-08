using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.VisionOS;
// #if UNITY_IOS && !UNITY_EDITOR
//     using UnityEngine.XR.ARKit;
// #endif // UNITY_IOS && !UNITY_EDITOR

using Object = UnityEngine.Object;

// namespace UnityEngine.XR.ARFoundation.Samples
// {
public class MeshClassificationFracking : MonoBehaviour
{
    /// <summary>
    /// The number of mesh classifications detected.
    /// </summary>
    const int k_NumClassifications = 20;

    /// <summary>
    /// The mesh manager for the scene.
    /// </summary>
    public ARMeshManager m_MeshManager;

    // new prefabs for visionOS
    public MeshFilter m_NoneMeshPrefab;
    public MeshFilter m_BedMeshPrefab;
    public MeshFilter m_BlindsMeshPrefab;
    public MeshFilter m_CabinetMeshPrefab;
    public MeshFilter m_CeilingMeshPrefab;
    public MeshFilter m_CounterMeshPrefab;
    public MeshFilter m_DoorMeshPrefab;
    public MeshFilter m_DoorFrameMeshPrefab;
    public MeshFilter m_FireplaceMeshPrefab;
    public MeshFilter m_FloorMeshPrefab;
    public MeshFilter m_HomeApplianceMeshPrefab;
    public MeshFilter m_PlantMeshPrefab;
    public MeshFilter m_SeatMeshPrefab;
    public MeshFilter m_StairsMeshPrefab;
    public MeshFilter m_TVMeshPrefab;
    public MeshFilter m_TableMeshPrefab;
    public MeshFilter m_WallMeshPrefab;
    public MeshFilter m_WallDecorationMeshPrefab;
    public MeshFilter m_WhiteboardMeshPrefab;
    public MeshFilter m_WindowMeshPrefab;


    // /// <summary>
    // /// The mesh prefab for the None classification.
    // /// </summary>
    // public MeshFilter m_NoneMeshPrefab;

    // /// <summary>
    // /// The mesh prefab for the Wall classification.
    // /// </summary>
    // public MeshFilter m_WallMeshPrefab;

    // /// <summary>
    // /// The mesh prefab for the Floor classification.
    // /// </summary>
    // public MeshFilter m_FloorMeshPrefab;

    // /// <summary>
    // /// The mesh prefab for the Ceiling classification.
    // /// </summary>
    // public MeshFilter m_CeilingMeshPrefab;

    // /// <summary>
    // /// The mesh prefab for the Table classification.
    // /// </summary>
    // public MeshFilter m_TableMeshPrefab;

    // /// <summary>
    // /// The mesh prefab for the Seat classification.
    // /// </summary>
    // public MeshFilter m_SeatMeshPrefab;

    // /// <summary>
    // /// The mesh prefab for the Window classification.
    // /// </summary>
    // public MeshFilter m_WindowMeshPrefab;

    // /// <summary>
    // /// The mesh prefab for the Door classification.
    // /// </summary>
    // public MeshFilter m_DoorMeshPrefab;

// #if UNITY_IOS && !UNITY_EDITOR

    /// <summary>
    /// A mapping from tracking ID to instantiated mesh filters.
    /// </summary>
    readonly Dictionary<TrackableId, MeshFilter[]> m_MeshFrackingMap = new Dictionary<TrackableId, MeshFilter[]>();

    /// <summary>
    /// The delegate to call to breakup a mesh.
    /// </summary>
    Action<MeshFilter> m_BreakupMeshAction;

    /// <summary>
    /// The delegate to call to update a mesh.
    /// </summary>
    Action<MeshFilter> m_UpdateMeshAction;

    /// <summary>
    /// The delegate to call to remove a mesh.
    /// </summary>
    Action<MeshFilter> m_RemoveMeshAction;

    /// <summary>
    /// An array to store the triangle vertices of the base mesh.
    /// </summary>
    readonly List<int> m_BaseTriangles = new List<int>();

    /// <summary>
    /// An array to store the triangle vertices of the classified mesh.
    /// </summary>
    readonly List<int> m_ClassifiedTriangles = new List<int>();

    /// <summary>
    /// On awake, set up the mesh filter delegates.
    /// </summary>
    void Awake()
    {
        m_BreakupMeshAction = new Action<MeshFilter>(BreakupMesh);
        m_UpdateMeshAction = new Action<MeshFilter>(UpdateMesh);
        m_RemoveMeshAction = new Action<MeshFilter>(RemoveMesh);
    }

    /// <summary>
    /// On enable, subscribe to the meshes changed event.
    /// </summary>
    void OnEnable()
    {
        Debug.Assert(m_MeshManager != null, "mesh manager cannot be null");
        m_MeshManager.meshesChanged += OnMeshesChanged;
    }

    /// <summary>
    /// On disable, unsubscribe from the meshes changed event.
    /// </summary>
    void OnDisable()
    {
        Debug.Assert(m_MeshManager != null, "mesh manager cannot be null");
        m_MeshManager.meshesChanged -= OnMeshesChanged;
    }

    /// <summary>
    /// When the meshes change, update the scene meshes.
    /// </summary>
    void OnMeshesChanged(ARMeshesChangedEventArgs args)
    {
        if (args.added != null)
        {
            args.added.ForEach(m_BreakupMeshAction);
        }

        if (args.updated != null)
        {
            args.updated.ForEach(m_UpdateMeshAction);
        }

        if (args.removed != null)
        {
            args.removed.ForEach(m_RemoveMeshAction);
        }
    }

    /// <summary>
    /// Parse the trackable ID from the mesh filter name.
    /// </summary>
    /// <param name="meshFilterName">The mesh filter name containing the trackable ID.</param>
    /// <returns>
    /// The trackable ID parsed from the string.
    /// </returns>
    TrackableId ExtractTrackableId(string meshFilterName)
    {
        string[] nameSplit = meshFilterName.Split(' ');
        return new TrackableId(nameSplit[1]);
    }

    /// <summary>
    /// Given a base mesh, the face classifications for all faces in the mesh, and a single face classification to
    /// extract, extract into a new mesh only the faces that have the selected face classification.
    /// </summary>
    /// <param name="baseMesh">The original base mesh.</param>
    /// <param name="faceClassifications">The array of face classifications for each triangle in the
    /// <paramref name="baseMesh"/></param>
    /// <param name="selectedMeshClassification">A single classification to extract the faces from the
    /// <paramref="baseMesh"/>into the <paramref name="classifiedMesh"/></param>
    /// <param name="classifiedMesh">The output mesh to be updated with the extracted mesh.</param>
    void ExtractClassifiedMesh(Mesh baseMesh, NativeArray<ARMeshClassification> faceClassifications, ARMeshClassification selectedMeshClassification, Mesh classifiedMesh)
    {
        // Count the number of faces matching the selected classification.
        int classifiedFaceCount = 0;
        for (int i = 0; i < faceClassifications.Length; ++i)
        {
            if (faceClassifications[i] == selectedMeshClassification)
            {
                ++classifiedFaceCount;
            }
        }

        // Clear the existing mesh.
        classifiedMesh.Clear();

        // If there were matching face classifications, build a new mesh from the base mesh.
        if (classifiedFaceCount > 0)
        {
            baseMesh.GetTriangles(m_BaseTriangles, 0);
            Debug.Assert(m_BaseTriangles.Count == (faceClassifications.Length * 3),
                        "unexpected mismatch between triangle count and face classification count");

            m_ClassifiedTriangles.Clear();
            m_ClassifiedTriangles.Capacity = classifiedFaceCount * 3;

            for (int i = 0; i < faceClassifications.Length; ++i)
            {
                if (faceClassifications[i] == selectedMeshClassification)
                {
                    int baseTriangleIndex = i * 3;

                    m_ClassifiedTriangles.Add(m_BaseTriangles[baseTriangleIndex + 0]);
                    m_ClassifiedTriangles.Add(m_BaseTriangles[baseTriangleIndex + 1]);
                    m_ClassifiedTriangles.Add(m_BaseTriangles[baseTriangleIndex + 2]);
                }
            }

            classifiedMesh.vertices = baseMesh.vertices;
            classifiedMesh.normals = baseMesh.normals;
            classifiedMesh.SetTriangles(m_ClassifiedTriangles, 0);
        }

    }

    /// <summary>
    /// Break up a single mesh with multiple face classifications into submeshes, each with an unique and uniform mesh
    /// classification.
    /// </summary>
    /// <param name="meshFilter">The mesh filter for the base mesh with multiple face classifications.</param>
    void BreakupMesh(MeshFilter meshFilter)
    {
        XRMeshSubsystem meshSubsystem = m_MeshManager.subsystem as XRMeshSubsystem;
        if (meshSubsystem == null)
        {
            return;
        }

        var meshId = ExtractTrackableId(meshFilter.name);
        var faceClassifications = meshSubsystem.GetFaceClassifications(meshId, Allocator.Persistent);

        if (!faceClassifications.IsCreated)
        {
            return;
        }

        using (faceClassifications)
        {
            if (faceClassifications.Length <= 0)
            {
                return;
            }

            var parent = meshFilter.transform.parent;

            MeshFilter[] meshFilters = new MeshFilter[k_NumClassifications];

            // meshFilters[(int)ARMeshClassification.None] = (m_NoneMeshPrefab == null) ? null : Instantiate(m_NoneMeshPrefab, parent);
            // meshFilters[(int)ARMeshClassification.Wall] = (m_WallMeshPrefab == null) ? null : Instantiate(m_WallMeshPrefab, parent);
            // meshFilters[(int)ARMeshClassification.Floor] = (m_FloorMeshPrefab == null) ? null : Instantiate(m_FloorMeshPrefab, parent);
            // meshFilters[(int)ARMeshClassification.Ceiling] = (m_CeilingMeshPrefab == null) ? null : Instantiate(m_CeilingMeshPrefab, parent);
            // meshFilters[(int)ARMeshClassification.Table] = (m_TableMeshPrefab == null) ? null : Instantiate(m_TableMeshPrefab, parent);
            // meshFilters[(int)ARMeshClassification.Seat] = (m_SeatMeshPrefab == null) ? null : Instantiate(m_SeatMeshPrefab, parent);
            // meshFilters[(int)ARMeshClassification.Window] = (m_WindowMeshPrefab == null) ? null : Instantiate(m_WindowMeshPrefab, parent);
            // meshFilters[(int)ARMeshClassification.Door] = (m_DoorMeshPrefab == null) ? null : Instantiate(m_DoorMeshPrefab, parent);

            // https://docs.unity3d.com/Packages/com.unity.xr.visionos@1.1/api/UnityEngine.XR.VisionOS.ARMeshClassification.html
                // 1. None
                // 2. Bed	
                // 3. Blinds	
                // 4. Cabinet	
                // 5. Ceiling	
                // 6. Counter	
                // 7. Door	
                // 8. DoorFrame	
                // 9. Fireplace	
                // 10. Floor	
                // 11. HomeAppliance	
                // 12. Plant	
                // 13. Seat	
                // 14. Stairs	
                // 15. TV	
                // 16. Table	
                // 17. Wall	
                // 18. WallDecoration	
                // 19. Whiteboard	
                // 20. Window	
            meshFilters[(int)ARMeshClassification.None] = (m_NoneMeshPrefab == null) ? null : Instantiate(m_NoneMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Bed] = (m_BedMeshPrefab == null) ? null : Instantiate(m_BedMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Blinds] = (m_BlindsMeshPrefab == null) ? null : Instantiate(m_BlindsMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Cabinet] = (m_CabinetMeshPrefab == null) ? null : Instantiate(m_CabinetMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Ceiling] = (m_CeilingMeshPrefab == null) ? null : Instantiate(m_CeilingMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Counter] = (m_CounterMeshPrefab == null) ? null : Instantiate(m_CounterMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Door] = (m_DoorMeshPrefab == null) ? null : Instantiate(m_DoorMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.DoorFrame] = (m_DoorFrameMeshPrefab == null) ? null : Instantiate(m_DoorFrameMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Fireplace] = (m_FireplaceMeshPrefab == null) ? null : Instantiate(m_FireplaceMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Floor] = (m_FloorMeshPrefab == null) ? null : Instantiate(m_FloorMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.HomeAppliance] = (m_HomeApplianceMeshPrefab == null) ? null : Instantiate(m_HomeApplianceMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Plant] = (m_PlantMeshPrefab == null) ? null : Instantiate(m_PlantMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Seat] = (m_SeatMeshPrefab == null) ? null : Instantiate(m_SeatMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Stairs] = (m_StairsMeshPrefab == null) ? null : Instantiate(m_StairsMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.TV] = (m_TVMeshPrefab == null) ? null : Instantiate(m_TVMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Table] = (m_TableMeshPrefab == null) ? null : Instantiate(m_TableMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Wall] = (m_WallMeshPrefab == null) ? null : Instantiate(m_WallMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.WallDecoration] = (m_WallDecorationMeshPrefab == null) ? null : Instantiate(m_WallDecorationMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Whiteboard] = (m_WhiteboardMeshPrefab == null) ? null : Instantiate(m_WhiteboardMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Window] = (m_WindowMeshPrefab == null) ? null : Instantiate(m_WindowMeshPrefab, parent);


            m_MeshFrackingMap[meshId] = meshFilters;

            var baseMesh = meshFilter.sharedMesh;
            for (int i = 0; i < k_NumClassifications; ++i)
            {
                var classifiedMeshFilter = meshFilters[i];
                if (classifiedMeshFilter != null)
                {
                    var classifiedMesh = classifiedMeshFilter.mesh;
                    ExtractClassifiedMesh(baseMesh, faceClassifications, (ARMeshClassification)i, classifiedMesh);
                    meshFilters[i].mesh = classifiedMesh;
                }
            }
        }
    }

    /// <summary>
    /// Update the submeshes corresponding to the single mesh with multiple face classifications into submeshes.
    /// </summary>
    /// <param name="meshFilter">The mesh filter for the base mesh with multiple face classifications.</param>
    void UpdateMesh(MeshFilter meshFilter)
    {
        XRMeshSubsystem meshSubsystem = m_MeshManager.subsystem as XRMeshSubsystem;
        if (meshSubsystem == null)
        {
            return;
        }

        var meshId = ExtractTrackableId(meshFilter.name);
        var faceClassifications = meshSubsystem.GetFaceClassifications(meshId, Allocator.Persistent);

        if (!faceClassifications.IsCreated)
        {
            return;
        }

        using (faceClassifications)
        {
            if (faceClassifications.Length <= 0)
            {
                return;
            }

            var meshFilters = m_MeshFrackingMap[meshId];

            var baseMesh = meshFilter.sharedMesh;
            for (int i = 0; i < k_NumClassifications; ++i)
            {
                var classifiedMeshFilter = meshFilters[i];
                if (classifiedMeshFilter != null)
                {
                    var classifiedMesh = classifiedMeshFilter.mesh;
                    ExtractClassifiedMesh(baseMesh, faceClassifications, (ARMeshClassification)i, classifiedMesh);
                    meshFilters[i].mesh = classifiedMesh;
                }
            }
        }
    }

    /// <summary>
    /// Remove the submeshes corresponding to the single mesh.
    /// </summary>
    /// <param name="meshFilter">The mesh filter for the base mesh with multiple face classifications.</param>
    void RemoveMesh(MeshFilter meshFilter)
    {
        var meshId = ExtractTrackableId(meshFilter.name);
        var meshFilters = m_MeshFrackingMap[meshId];
        for (int i = 0; i < k_NumClassifications; ++i)
        {
            var classifiedMeshFilter = meshFilters[i];
            if (classifiedMeshFilter != null)
            {
                Object.Destroy(classifiedMeshFilter);
            }
        }

        m_MeshFrackingMap.Remove(meshId);
    }
// #endif // UNITY_IOS && !UNITY_EDITOR
}
// }