// **************************************************************
// **** Created on 10/22/15 by Kevin Means
// **** 1.) This collider makes a seemless circular collider to
// ****     replace the polygon collider on the Cell Membrane and
// ****     Nucleus.
// **************************************************************
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof(EdgeCollider2D))]
public class CircleEdgeCollider2D : MonoBehaviour
{
  public float InnerRadius;
  public float OuterRadius;
  public int NumPoints;

  float CurrentInner = 0.0f;
  float CurrentOuter = 0.0f;
  EdgeCollider2D EdgeCollider;

  //------------------------------------------------------------------------------------------------
  void Start()
  {
    DrawCircle();
  }

  //------------------------------------------------------------------------------------------------
  void Update()
  {
        if(Time.timeScale != 0)
        {
            if (NumPoints != EdgeCollider.pointCount ||
                CurrentInner != InnerRadius ||
                CurrentOuter != OuterRadius)
            {
                DrawCircle();
            }
        }  
  }

  //------------------------------------------------------------------------------------------------
  // Creates the inner and outer circular collider
  void DrawCircle()
  {
    Vector2[] edgePoints = new Vector2[NumPoints + 2];
    EdgeCollider = GetComponent<EdgeCollider2D>();

    for(int loop = 0; loop <= NumPoints / 2; loop++)
    {
      float angle = (Mathf.PI * 2.0f / NumPoints) * loop * 2;
      edgePoints[loop] = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * InnerRadius;
    }

    for(int loop = NumPoints / 2 + 1; loop <= NumPoints + 1; loop++)
    {
      float angle = (Mathf.PI * 2.0f / NumPoints) * loop * 2;
      edgePoints[loop] = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle)) * (OuterRadius);
    }
    EdgeCollider.points = edgePoints;
    CurrentInner = InnerRadius + 1;
    CurrentOuter = OuterRadius + 1;
  }
}

