using MathBeat.Core;
using MathBeat.Game.BeatSync;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The game system of MathBeat
/// </summary>
namespace MathBeat.Game
{
    /// <summary>
    /// Spawns beat objects
    /// </summary>
    /// <remarks>
    /// This script must execute earlier than others
    /// in order to work. Go to Editor > Script Execution Order
    /// and put this script above the others.
    /// </remarks>
    [RequireComponent(typeof(BeatObserver))]
    public class BlockSpawner : MonoBehaviour
    {
        /// <summary>
        /// Speed of the falling object
        /// </summary>
        [Header("Spawn Mechanics")]
        [Range(4, 16)]
        public float Speed = 5f;

        /// <summary>
        /// Beat objects
        /// </summary>
        [Tooltip("Assign beat prefabs to this property.")]
        public Transform NoteBeat, QuestionBeat;
        public float Position;
        public float TriggerPoint = -6.5f;

        public BeatTypeList MapData;

        [SerializeField]
        private int currentBeat = 0;

        private Vector2 fallVector;
        [HideInInspector]
        public float FallTime = 0f;
        private BeatObserver Observer;
        public bool isPlaying = false;

        public ObjectPool NoteRecycler, QuestionRecycler;

        public delegate void OnGameOver();
        public event OnGameOver GameOver;

        void Awake()
        {
            Observer = GetComponent<BeatObserver>();
            Position = transform.position.y;
        }

        void Start()
        {
            fallVector = new Vector2(0, -Speed);
            isPlaying = true;
            //StartCoroutine(Spawn(currentBeat));
        }

        private void FixedUpdate()
        {
            if (isPlaying)
                if (Observer.IsActive)
                    if (currentBeat >= MapData.Length)
                    {
                        // now that it's over the length,
                        // we can end the game.
                        StartCoroutine(ItsOver());
                    }
                    else
                        StartCoroutine(Spawn(currentBeat++));
        }

        IEnumerator Spawn(int currentBeat)
        {
            // start with null
            List<GameObject> generatedBeat = new List<GameObject>();

            if (MapData == null)
                Log.Warn("Nothing in map data!");

            if (MapData[currentBeat] != BeatType.None)
                Debug.LogFormat("{0} is {1}, {2} s after start", currentBeat, MapData[currentBeat], Time.realtimeSinceStartup);
            else yield break;

            switch (MapData[currentBeat])
            {
                case BeatType.Single:
                    BeatProperties prop = new BeatProperties(Random.Range(0, 4));
                    var obj = NoteRecycler.GetObject(prop.position);
                    obj.GetComponent<SpriteRenderer>().color = prop.color;
                    generatedBeat.Add(obj);
                    break;
                case BeatType.Double:
                    float[] xx = new float[2];
                    xx[0] = Random.Range(0, 4);
                    do
                    {
                        xx[1] = Random.Range(0, 4);
                    } while (xx[0] == xx[1]);
                    BeatProperties prop2;
                    foreach (var x3 in xx)
                    {
                        prop2 = new BeatProperties(Random.Range(0, 4));
                        var obj2 = NoteRecycler.GetObject(prop2.position);
                        obj2.GetComponent<SpriteRenderer>().color = prop2.color;
                        generatedBeat.Add(obj2);
                    }
                    break;
                case BeatType.Block:
                    generatedBeat.Add(QuestionRecycler.GetObject());
                    break;
                case BeatType.None:
                default:
                    break;
            }

            Observer.IsActive = false;

            // null checking
            if (generatedBeat.Count > 0)
            {
                // give each generatedBeat a fall speed
                generatedBeat.ForEach(beat => beat.GetComponent<Rigidbody2D>().velocity = fallVector);
                generatedBeat.Clear();
            }

            yield break;
      
        }

        IEnumerator ItsOver()
        {
            isPlaying = false;
            yield return new WaitForSeconds(3 * FallTime);
            GameOver();
        }

        struct BeatProperties
        {
            public float position
            {
                get
                {
                    return _pos;
                }
            }
            public Color color
            {
                get
                {
                    return _col;
                }
            }

            public BeatProperties(int code)
            {
                switch (code)
                {
                    case 0:
                        _pos = -3.18f;
                        _col = new Color(224 / 255f, 76 / 255f, 76 / 255f);
                        break;
                    case 1:
                        _pos = -1.06f;
                        _col = new Color(219 / 255f, 180 / 255f, 26 / 255f);
                        break;
                    case 2:
                        _pos = 1.06f;
                        _col = new Color(76 / 255f, 175 / 255f, 80 / 255f);
                        break;
                    case 3:
                        _pos = 3.18f;
                        _col = new Color(44 / 255f, 143 / 255f, 224 / 255f);
                        break;
                    default:
                        _pos = 0;
                        _col = Color.white;
                        break;
                }
            }

            private float _pos;
            private Color _col;

        }
    }
}
