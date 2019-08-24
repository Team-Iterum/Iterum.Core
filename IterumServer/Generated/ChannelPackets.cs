using Magistr.Network;
using System.Collections.Generic;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
	using Magistr.Math;
	using Magistr.Log;
#else
	using UnityEngine;
#endif

namespace Packets
{	
    public static class ChannelPackets
    {

		public static bool DebugHeaders = false;
		#region Registry 
		public delegate void PacketParse(uint con, ISerializablePacket _packet);
		public static void Dispatch(uint conn, byte[] data)
        {
            switch (data[0])
            {
                case 1:
                    {
						var packet = default(FirstPacket);
						if(DebugHeaders) Debug.Log("FirstPacket");
						packet.Deserialize(data);
						try 
						{
							ParseFirstPacket?.Invoke(conn, packet);
						}
						catch(System.Exception e) 
						{
							Debug.LogError("[FirstPacket] " + e.ToString());
						}
					}
					break;
                case 2:
                    {
						var packet = default(Login);
						if(DebugHeaders) Debug.Log("Login");
						packet.Deserialize(data);
						try 
						{
							ParseLogin?.Invoke(conn, packet);
						}
						catch(System.Exception e) 
						{
							Debug.LogError("[Login] " + e.ToString());
						}
					}
					break;
                case 11:
                    {
						var packet = default(Direction);
						if(DebugHeaders) Debug.Log("Direction");
						packet.Deserialize(data);
						try 
						{
							ParseDirection?.Invoke(conn, packet);
						}
						catch(System.Exception e) 
						{
							Debug.LogError("[Direction] " + e.ToString());
						}
					}
					break;
                case 12:
                    {
						var packet = default(LookFull);
						if(DebugHeaders) Debug.Log("LookFull");
						packet.Deserialize(data);
						try 
						{
							ParseLookFull?.Invoke(conn, packet);
						}
						catch(System.Exception e) 
						{
							Debug.LogError("[LookFull] " + e.ToString());
						}
					}
					break;

                default:
                    break;
            }
        }
		
		#endregion

		#region Parse
		public delegate void ParseFirstPacketDelegate(uint con, FirstPacket data);
		public static event ParseFirstPacketDelegate ParseFirstPacket;

		public delegate void ParseLoginDelegate(uint con, Login data);
		public static event ParseLoginDelegate ParseLogin;

		public delegate void ParseDirectionDelegate(uint con, Direction data);
		public static event ParseDirectionDelegate ParseDirection;

		public delegate void ParseLookFullDelegate(uint con, LookFull data);
		public static event ParseLookFullDelegate ParseLookFull;


		#endregion
    }  
}
