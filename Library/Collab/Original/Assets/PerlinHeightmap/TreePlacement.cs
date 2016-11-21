using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreePlacement : MonoBehaviour {
    public Terrain terrain;
    public Transform exclude;

    // пачка параметров
    public int countSpawn = 625;
    [Range(1, 100)]
    public int spawnPerFrame = 25;
    
    [MinMaxSlider(0.1f, 6.0f)]
    public Vector2 scale = new Vector2(3, 5);
    [MinMaxSlider(0.1f, 2.0f)]
    public Vector2 segmentLength = new Vector2(0.6f, 0.9f);
    [MinMaxSlider(0, 40)]
    public Vector2 twisting = new Vector2(1f, 38f);
    [MinMaxSlider(0.01f, 0.25f)]
    public Vector2 branchProbability = new Vector2(0.15f, 0.25f);
    [MinMaxSlider(0.25f, 4)]
    public Vector2 baseRadius = new Vector2(0.4f, 3.5f);
    [MinMaxSlider(0.02f, 0.15f)]
    public Vector2 minimumRadius = new Vector2(0.02f, 0.1f);
    [MinMaxSlider(0.0f, 1)]
    public Vector2 roudness = new Vector2(0.2f, 0.9f);
    public Material[] materials;

    private GameObject tree;
    
    void Start () {
        // выгружаем префаб
        tree = Resources.Load("Tree") as GameObject;

        // и запускам корутин
        StartCoroutine( Generator() );
    }

    // выбираем случаное значение между x и y вектора
    float Roll(Vector2 range) {
        return Random.Range( range.x, range.y );
    }

    // спавним дерево по x и y
    void RollNSpawn(int x, int y) {
        float h = terrain.SampleHeight( new Vector3( x, 0, y ) ); // вытаскивая высоту из карты высот ландшафта
        // копируя префаб
        ProceduralTree placement = (Instantiate( tree, terrain.transform ) as GameObject).GetComponent<ProceduralTree>();
        placement.transform.position = new Vector3( x, h, y );
        // и рандомля пачку параметров
        placement.transform.localScale = new Vector3( Roll(scale), Roll(scale), Roll(scale) );
        placement.SegmentLength = Roll( segmentLength );
        placement.Twisting = Roll( twisting );
        placement.BaseRadius = Roll( baseRadius );
        placement.MinimumRadius = Roll( minimumRadius );
        placement.BranchRoundness = Roll( roudness );
        placement.Renderer.material = materials[Random.Range( 0, 4 )];
        // после чего обновляя меш у дерева
        placement.ForceUpdate();
    }

    IEnumerator Generator () {
        while(countSpawn > 0) {
            for ( int j = 0; j < spawnPerFrame; ) {
                int x = (int)Random.Range(10, 490),
                    y = (int)Random.Range(10, 490);

                // с этим не получилось, плохо у меня с математикой
                bool isInclude = x < exclude.position.x - 4f*exclude.localScale.x;
                isInclude |= x > exclude.position.x + 4f*exclude.localScale.x;
                isInclude &= y < exclude.position.z - 4f*exclude.localScale.z
                          || y > exclude.position.z + 4f*exclude.localScale.z;
                // поэтому будем экскудить по радиусу вокруг "центра"
                isInclude = (x - 250)*(x - 250) + (y - 250)*(y - 250) > 75*75;

                if ( isInclude ) {
                    RollNSpawn( x, y );
                    j += 1;
                }
            }

            countSpawn -= spawnPerFrame;
            yield return new WaitForEndOfFrame(); // ждём следующего кадра чтобы не лагало чересчур
        }
    }
}
