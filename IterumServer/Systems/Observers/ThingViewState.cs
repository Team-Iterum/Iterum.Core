using Magistr.Log;
using Magistr.Math;
using Magistr.Things;
using Packets;
using static Magistr.Services.NetworkAlias;
namespace Magistr.Observers
{
    public partial class Observer
    {
        public class ThingViewState
        {
            public uint viewId;
            public IThing thing;
            public Vector3 LastPosition;
            public Quaternion LastRotation;
            public bool IsSend;
            public int SkipCounter = 0;
            public Observer Owner;

            internal void OnChange(uint connectionId)
            {
                var pos = thing.Position;
                var rot = thing.Rotation;
                var diff = (pos - LastPosition);

                
                // Move Full
                if (!IsSend)
                {
                    Net.Send(connectionId, new MoveFull() { Position = pos, Rotation = rot, viewId = viewId });
                    IsSend = true;
                }

                if (SkipCounter == 0)
                {
                    // MoveShort
                    if (diff.magnitude < 10 && diff.magnitude > 0)
                    {
                        var x = (uint)Mathf.Abs(Mathf.RoundToInt(diff.x * 100));
                        var y = (uint)Mathf.Abs(Mathf.RoundToInt(diff.y * 100));
                        var z = (uint)Mathf.Abs(Mathf.RoundToInt(diff.z * 100));

                        var xsign = Mathf.Sign(diff.x) < 0 ? 1 : 0;
                        var ysign = Mathf.Sign(diff.y) < 0 ? 1 : 0;
                        var zsign = Mathf.Sign(diff.z) < 0 ? 1 : 0;

                        Net.Send(connectionId, new MoveShort()
                        {
                            viewId = (uint)viewId,
                            PosX = x,
                            PosY = y,
                            PosZ = z,
                            PosXSign = (uint)xsign,
                            PosYSign = (uint)ysign,
                            PosZSign = (uint)zsign,
                            Rot = (uint)Clamp0360(rot.eulerAngles.y)
                        });
                    }
                    // MoveNormal
                    if (diff.magnitude >= 10)
                    {
                        Net.Send(connectionId, new MoveNormal() { viewId = (uint)viewId, Position = pos, Rot = (uint)Clamp0360(rot.eulerAngles.y) });
                    }
                }
                if ((pos - Owner.Position).magnitude >= MaxViewDistance/2 && SkipCounter == 0)
                {
                    Debug.Log($"[Observer {Owner.ConnectionId}] Skip viewId={viewId} length={(pos - Owner.Position).magnitude}");
                    SkipCounter = 5;
                }

                if (SkipCounter > 0)
                {
                    SkipCounter -= 1;
                }


                LastRotation = rot;
                LastPosition = pos;
                
            }

            private static float Clamp0360(float eulerAngles)
            {
                float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
                if (result < 0)
                {
                    result += 360f;
                }
                return result;
            }
        }
    }
}
