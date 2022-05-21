using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuadroDiamante : MonoBehaviour
{
    public int celdas = 2;
    public float tamaño = 5;
    public float altura = 1;

    private Vector3[] _vertices;
    private int _cantidadVertices;

    private Color[] colors;
    public Gradient gradiente;

    public GameObject Esfera;
    void Start()
    {
        GenerarTerreno();
    }

    void GenerarTerreno()
    {
        _cantidadVertices = (celdas+1)*(celdas+1);
        _vertices = new Vector3[_cantidadVertices];
        Vector2[] uvs = new Vector2[_cantidadVertices];
        int[] triangulos = new int[celdas*celdas * 6];

        colors = new Color[_cantidadVertices];
        
        float tamañoCeldas = tamaño / celdas;
        Mesh maya = new Mesh();
        GetComponent<MeshFilter>().mesh = maya;

        int offsetTriangulo = 0;
        
        for (int x = 0; x <= celdas; x++)
        {
            for (int y = 0; y <= celdas; y++)
            {
                _vertices[x*(celdas+1) + y] = new Vector3(-(tamaño * 0.5f)+y*tamañoCeldas,0,(tamaño * 0.5f)-x*tamañoCeldas);
                uvs[x*(celdas+1)+y] = new Vector2((float)x/celdas,(float)y/celdas);

                if (x < celdas && y < celdas)
                {
                    int ArribaIzquierda = x * (celdas + 1) + y;
                    int AbajoIzquierda = (x + 1) * (celdas + 1) + y;
                    
                    triangulos[offsetTriangulo] = ArribaIzquierda;
                    triangulos[offsetTriangulo + 1] = ArribaIzquierda + 1;
                    triangulos[offsetTriangulo + 2] = AbajoIzquierda + 1;

                    triangulos[offsetTriangulo + 3] = ArribaIzquierda;
                    triangulos[offsetTriangulo + 4] = AbajoIzquierda + 1;
                    triangulos[offsetTriangulo + 5] = AbajoIzquierda;

                    offsetTriangulo += 6;

                }
            }
        }

        _vertices[0].y = Random.Range(-altura, altura);
        _vertices[celdas].y = Random.Range(-altura, altura);
        _vertices[_vertices.Length - 1].y = Random.Range(-altura, altura);
        _vertices[_vertices.Length - 1 - celdas].y = Random.Range(-altura, altura);

        int iteraciones = (int) Mathf.Log(celdas, 2);
        int cantidadCuadrados = 1;
        int tamañoCuadrados = celdas;

        for (int i = 0; i < iteraciones; i++)
        {
            int fila = 0;
            
            for (int j = 0; j < cantidadCuadrados; j++)
            {
                int col = 0;
                for (int k = 0; k < cantidadCuadrados; k++)
                {
                    DiamondSquare(fila, col, tamañoCuadrados, altura);
                    col += tamañoCuadrados;
                }

                fila += tamañoCuadrados;
            }
            cantidadCuadrados *= 2;
            tamañoCuadrados /= 2;
            altura *= 0.5f;
        }

        maya.vertices = _vertices;
        maya.uv = uvs;
        maya.triangles = triangulos;
        
        maya.RecalculateBounds();
        maya.RecalculateNormals();
    }
    void DiamondSquare(int fila, int col, int tamaño, float Offset)
    {
        int ArIzquierda = fila * (celdas + 1) + col;
        int AbIzquierda = (fila + tamaño) * (celdas + 1) + col;
        
        int centro = (fila + (tamaño / 2)) * (celdas + 1) + (col + (tamaño / 2));
        
        _vertices[centro].y = (_vertices[ArIzquierda].y + _vertices[ArIzquierda + tamaño].y + 
                              _vertices[AbIzquierda].y + _vertices[AbIzquierda + tamaño].y)/4 + Random.Range(-Offset,Offset);

        _vertices[ArIzquierda + (tamaño / 2)].y = (_vertices[ArIzquierda].y + _vertices[ArIzquierda + tamaño].y + _vertices[centro].y) / 3 + Random.Range(-Offset,Offset);
        _vertices[centro - (tamaño / 2)].y = (_vertices[ArIzquierda].y + _vertices[AbIzquierda].y + _vertices[centro].y)/3 + Random.Range(-Offset,Offset);
        _vertices[centro + (tamaño / 2)].y = (_vertices[ArIzquierda + tamaño].y + _vertices[AbIzquierda + tamaño].y + _vertices[centro].y) / 3 + Random.Range(-Offset,Offset);
        _vertices[AbIzquierda + (tamaño / 2)].y = (_vertices[AbIzquierda].y + _vertices[AbIzquierda + tamaño].y + _vertices[centro].y) / 3 + Random.Range(-Offset,Offset);

    }

    void CalcularNormales(Vector3 punto)
    {
        Mesh mesh = ((MeshFilter)GetComponent ("MeshFilter")).mesh;
        float angulo = 0;
        for (int x = 0; x <= celdas; x++)
        {
            for (int y = 0; y <= celdas; y++)
            {
                if (x < celdas && y < celdas)
                {
                    int ArribaIzquierda = x * (celdas + 1) + y;
                    int AbajoIzquierda = (x + 1) * (celdas + 1) + y;
                    Vector3 P0 = _vertices[ArribaIzquierda];
                    Vector3 P1 = _vertices[ArribaIzquierda + 1];
                    Vector3 P2 = _vertices[AbajoIzquierda + 1];

                    Vector3 U = P1 - P0;
                    Vector3 V = P2 - P0;

                    Vector3 N = Vector3.Cross(U, V).normalized;
            
                    colors[ArribaIzquierda] = gradiente.Evaluate(Mathf.InverseLerp(-180,180,Vector3.Angle(punto,N)));
                    colors[ArribaIzquierda + 1] = gradiente.Evaluate(Mathf.InverseLerp(-180,180,Vector3.Angle(punto,N)));
                    colors[AbajoIzquierda + 1] = gradiente.Evaluate(Mathf.InverseLerp(-180,180,Vector3.Angle(punto,N)));
                    
                    Vector3 P02 = _vertices[ArribaIzquierda];
                    Vector3 P12 = _vertices[AbajoIzquierda + 1];
                    Vector3 P22 = _vertices[AbajoIzquierda];

                    Vector3 U2 = P12 - P02;
                    Vector3 V2 = P22 - P02;

                    Vector3 N2 = Vector3.Cross(U2, V2).normalized;
            
                    colors[ArribaIzquierda] = gradiente.Evaluate(Mathf.InverseLerp(-180,180,Vector3.Angle(punto,N2)));
                    colors[AbajoIzquierda + 1] = gradiente.Evaluate(Mathf.InverseLerp(-180,180,Vector3.Angle(punto,N2)));
                    colors[AbajoIzquierda] = gradiente.Evaluate(Mathf.InverseLerp(-180,180,Vector3.Angle(punto,N2)));
                    angulo = Vector3.Angle(punto, N2);
                }
            }
        }
        mesh.colors = colors;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void Update()
    {
        CalcularNormales(Esfera.transform.forward);
    }
}
