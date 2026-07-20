using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VectorStore
{
    private readonly List<DocChunk> _chunks = new List<DocChunk>();

    public void Add(DocChunk chunk) => _chunks.Add(chunk);
    public int Count => _chunks.Count;

    private static float Cosine(float[] a, float[] b)
    {
        float dot = 0f, na = 0f, nb = 0f;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            na  += a[i] * a[i];
            nb  += b[i] * b[i];
        }
        return dot / (Mathf.Sqrt(na) * Mathf.Sqrt(nb) + 1e-8f);
    }

    public List<DocChunk> Search(float[] queryVec, int k = 5)
    {
        return _chunks
            .OrderByDescending(c => Cosine(c.embedding, queryVec))
            .Take(k)
            .ToList();
    }
}
