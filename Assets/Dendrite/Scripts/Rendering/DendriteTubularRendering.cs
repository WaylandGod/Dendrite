﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dendrite
{

    public class DendriteTubularRendering : DendriteRenderingBase
    {

        [SerializeField] protected Material material;

        protected ComputeBuffer drawBuffer;
        protected uint[] 
            drawArgs = new uint[5] { 0, 0, 0, 0, 0 };

        protected Mesh segment;

        #region MonoBehaviour

        protected void OnEnable()
        {
            segment = BuildSegment();
        }

        protected void Update()
        {
            SetupDrawArgumentsBuffers(dendrite.Count);
            Render();
        }

        protected void OnDestroy()
        {
            if (drawBuffer != null) drawBuffer.Dispose();
        }

        #endregion

        protected void SetupDrawArgumentsBuffers(int count)
        {
            if (drawArgs[1] == (uint)count) return;

            drawArgs[0] = segment.GetIndexCount(0);
            drawArgs[1] = (uint)count;

            if (drawBuffer != null) drawBuffer.Dispose();
            drawBuffer = new ComputeBuffer(1, sizeof(uint) * drawArgs.Length, ComputeBufferType.IndirectArguments);
            drawBuffer.SetData(drawArgs);
        }

        protected void Render(float extents = 100f)
        {
            material.SetBuffer("_Nodes", dendrite.NodeBuffer);
            material.SetBuffer("_Edges", dendrite.EdgeBuffer);
            material.SetInt("_EdgesCount", dendrite.EdgesCount);
            material.SetMatrix("_World2Local", transform.worldToLocalMatrix);
            material.SetMatrix("_Local2World", transform.localToWorldMatrix);
            material.SetPass(0);
            Graphics.DrawMeshInstancedIndirect(segment, 0, material, new Bounds(Vector3.zero, Vector3.one * extents), drawBuffer);
        }

        protected Mesh BuildSegment()
        {
            var mesh = new Mesh();
            mesh.hideFlags = HideFlags.DontSave;
            mesh.vertices = new Vector3[2] { Vector3.zero, Vector3.up };
            mesh.uv = new Vector2[2] { new Vector2(0f, 0f), new Vector2(0f, 1f) };
            mesh.SetIndices(new int[2] { 0, 1 }, MeshTopology.Lines, 0);
            return mesh;
        }

    }

}


