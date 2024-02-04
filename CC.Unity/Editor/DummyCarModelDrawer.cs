using CC.Common;
using System.Collections.Generic;
using UnityEngine;

namespace CC.Unity.Editor
{
    internal class DummyCarModelDrawer : MonoBehaviour
    {
        public CarParentType CarType;
        public Color DrawColour = new Color(0.3f, 0.5f, 1.0f);
        public Color ErrorColour = new Color(1.0f, 0.5f, 0.3f);
        public bool DrawIntersections = true;

        public Color DarkColour => DrawColour * 0.5f;

        private void OnDrawGizmos()
        {
            VolumeCollection toDraw;

            switch (CarType)
            {
                case CarParentType.Flatbed:
                case CarParentType.FlatbedMilitary:
                    toDraw = VolumeCollection.Flatbed;
                    break;
                case CarParentType.FlatbedStakes:
                    toDraw = VolumeCollection.FlatbedStakes;
                    break;
                case CarParentType.Autorack:
                    toDraw = VolumeCollection.Autorack;
                    break;
                case CarParentType.TankOil:
                case CarParentType.TankGas:
                case CarParentType.TankChem:
                    return;
                case CarParentType.TankFood:
                    return;
                case CarParentType.Stock:
                    toDraw = VolumeCollection.Stock;
                    break;
                case CarParentType.Boxcar:
                case CarParentType.BoxcarMilitary:
                case CarParentType.Refrigerator:
                    toDraw = VolumeCollection.Boxcar;
                    break;
                case CarParentType.Hopper:
                    toDraw = VolumeCollection.Hopper;
                    break;
                case CarParentType.Gondola:
                    toDraw= VolumeCollection.Gondola;
                    break;
                case CarParentType.Passenger:
                    toDraw = VolumeCollection.Passenger;
                    break;
                case CarParentType.NuclearFlask:
                    toDraw = VolumeCollection.NuclearFlask;
                    break;
                default:
                    return;
            }

            Gizmos.color = DrawColour;

            foreach (var (Position, Size) in toDraw.FullColour)
            {
                if (DrawIntersections)
                {
                    Gizmos.color = DrawColour;

                    if (Physics.CheckBox(Position, Size))
                    {
                        Gizmos.color = ErrorColour;
                    }
                }

                EditorHelper.DrawCubeGizmoOutlined(Position, Size);
            }

            Gizmos.color = DarkColour;

            foreach (var (Position, Size) in toDraw.DarkColour)
            {
                EditorHelper.DrawCubeGizmoOutlined(Position, Size);
            }
        }

        private class VolumeCollection
        {
            public List<(Vector3 Position, Vector3 Size)> FullColour = new List<(Vector3, Vector3)>();
            public List<(Vector3 Position, Vector3 Size)> DarkColour = new List<(Vector3, Vector3)>();

            public static VolumeCollection Flatbed = new VolumeCollection
            {
                FullColour = new List<(Vector3, Vector3)>()
                {
                    (new Vector3(0, 1.18f, 0), new Vector3(2.88f, 0.040625f, 14.9f))
                },
                DarkColour = new List<(Vector3, Vector3)>()
                {
                    (new Vector3(0, 0.843f, 0), new Vector3(2.8f, 0.71f, 16.935f))
                }
            };
            public static VolumeCollection FlatbedStakes = new VolumeCollection
            {
                FullColour = new List<(Vector3, Vector3)>
                {
                    (new Vector3(-1.4656f, 2.295f, -6.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(-1.4656f, 2.295f, -5.2f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(-1.4656f, 2.295f, -3.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(-1.4656f, 2.295f, -2.2f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(-1.4656f, 2.295f, -0.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(-1.4656f, 2.295f, 0.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(-1.4656f, 2.295f, 2.2f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(-1.4656f, 2.295f, 3.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(-1.4656f, 2.295f, 5.2f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(-1.4656f, 2.295f, 6.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, -6.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, -5.2f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, -3.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, -2.2f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, -0.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, 0.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, 2.2f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, 3.8f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, 5.2f), new Vector3(0.13f, 2.61f, 0.16f)),
                    (new Vector3(1.4656f, 2.295f, 6.8f), new Vector3(0.13f, 2.61f, 0.16f))
                }
            } + Flatbed;
            public static VolumeCollection Autorack = new VolumeCollection
            {
                FullColour = new List<(Vector3, Vector3)>
                {
                    (new Vector3(0, 2.797f, 0), new Vector3(2.9f, 0.07f, 17.035f)),
                    (new Vector3(0, 1.05f, 0), new Vector3(2.9f, 0.2f, 17.035f)),
                    (new Vector3(-1.419f, 2.427f, 0), new Vector3(0.12375f, 2.42f, 17.035f)),
                    (new Vector3(1.419f, 2.427f, 0), new Vector3(0.12375f, 2.42f, 17.035f))
                }
            };
            public static VolumeCollection Stock = new VolumeCollection
            {
                DarkColour = new List<(Vector3, Vector3)>
                {
                    (new Vector3(0, 2.26f, 0), new Vector3(2.61f, 2.75f, 13.55f))
                }
            };
            public static VolumeCollection Boxcar = new VolumeCollection
            {
                DarkColour = new List<(Vector3, Vector3)>
                {
                    (new Vector3(0, 2.25f, 0), new Vector3(2.65f, 2.74f, 12.85071f)),
                    (new Vector3(0, 3.898f, 0), new Vector3(0.60245f, 0.11122f, 13.4f))
                }
            };
            public static VolumeCollection Hopper = new VolumeCollection
            {
                FullColour = new List<(Vector3, Vector3)>()
                {
                    (new Vector3(-1.282f, 2.515f, 0), new Vector3(0.24f, 2.75f, 14.79f)),
                    (new Vector3(1.282f, 2.515f, 0), new Vector3(0.24f, 2.75f, 14.79f)),
                    (new Vector3(0, 3.49f, -8.438f), new Vector3(2.4f, 0.8f, 0.15f)),
                    (new Vector3(0, 3.49f, 8.438f), new Vector3(2.4f, 0.8f, 0.15f)),
                    (new Vector3(0, 3.09f, 0f), new Vector3(2.4f, 0.1f, 16.876f))
                },
                DarkColour = new List<(Vector3, Vector3)>()
                {
                    (new Vector3(0, 1.05f, 0), new Vector3(2.8f, 0.2f, 17.115f)),
                    (new Vector3(0, 2.118f, 0f), new Vector3(2.4f, 0.1f, 14.79f))
                }
            };
            public static VolumeCollection Gondola = new VolumeCollection
            {
                FullColour = new List<(Vector3, Vector3)>
                {
                    (new Vector3(0, 1.224f, 0), new Vector3(2.63f, 0.1f, 12.02f)),
                    (new Vector3(-1.37f, 1.823f, 0), new Vector3(0.12f, 1.75f, 12.21f)),
                    (new Vector3(1.37f, 1.823f, 0), new Vector3(0.12f, 1.75f, 12.21f)),
                    (new Vector3(0, 1.823f, -6.047f), new Vector3(2.74725f, 1.75f, 0.11f)),
                    (new Vector3(0, 1.823f, 6.047f), new Vector3(2.74725f, 1.75f, 0.11f))
                }
            };
            public static VolumeCollection Passenger = new VolumeCollection
            {
                FullColour = new List<(Vector3, Vector3)>
                {
                    (new Vector3(0, 3.725f, 0), new Vector3(1.4f, 0.2f, 23.26f)),
                    (new Vector3(0, 1.086f, 0), new Vector3(2.85f, 0.23f, 23.59f)),
                    (new Vector3(-1.18f, 2.176f, 0), new Vector3(0.215f, 2.375f, 23.59f)),
                    (new Vector3(1.18f, 2.176f, 0), new Vector3(0.215f, 2.375f, 23.59f)),
                }
            };
            public static VolumeCollection NuclearFlask = new VolumeCollection
            {
                FullColour = new List<(Vector3, Vector3)>
                {
                    (new Vector3(0, 2.129f, -4.95f), new Vector3(2.03f, 1f, 0.04f)),
                    (new Vector3(0, 2.129f, 4.95f), new Vector3(2.03f, 1f, 0.04f)),
                    (new Vector3(-1.339f, 2.002f, 0), new Vector3(0.12f, 1.34f, 9.9f)),
                    (new Vector3(1.339f, 2.002f, 0), new Vector3(0.12f, 1.34f, 9.9f)),
                    (new Vector3(0f, 0.896f, 0), new Vector3(2.6f, 0.2f, 2.45f)),
                    (new Vector3(0f, 1.05f, -3.37f), new Vector3(2.6f, 0.2f, 3.28f)),
                    (new Vector3(0f, 1.05f, 3.37f), new Vector3(2.6f, 0.2f, 3.28f)),
                    (new Vector3(0f, 1.173f, -1.656f), new Vector3(2.2f, 0.2f, 0.13f)),
                    (new Vector3(0f, 1.173f, 1.656f), new Vector3(2.2f, 0.2f, 0.13f)),
                }
            };

            public static VolumeCollection operator +(VolumeCollection left, VolumeCollection right)
            {
                var result = new VolumeCollection()
                {
                    FullColour = new List<(Vector3, Vector3)>(left.FullColour),
                    DarkColour = new List<(Vector3, Vector3)>(left.DarkColour)
                };

                result.FullColour.AddRange(right.FullColour);
                result.DarkColour.AddRange(right.DarkColour);

                return result;
            }
        }
    }
}
