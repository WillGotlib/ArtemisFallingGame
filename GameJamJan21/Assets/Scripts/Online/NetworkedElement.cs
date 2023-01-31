using Google.Protobuf;
using UnityEngine;
using protoBuff;

namespace Online
{
    public interface NetworkedElement
    {
        /// <summary>
        /// Gets the position of the object
        /// </summary>
        /// <returns>Vector2 position of the object</returns>
        public Vector2 GetPosition();

        /// <summary>
        /// Gets if the object should listen to info from the server on where it should be or if it should inform the server of its position
        /// </summary>
        /// <returns>Owner or Listener</returns>
        public ElementType GetControlType();

        /// <summary>
        /// ID is a name that the object can be IDed by so other players can spawn it
        /// </summary>
        /// <returns>a unique</returns>
        public string ID();

        /// <summary>
        /// Gets The entities custom data
        /// </summary>
        /// <returns>a byteString encoding of the entities custom data</returns>
        public string Data();

        /// <summary>
        ///  boolean value for whether to remove the object for other people once you disconnect from the server
        /// </summary>
        /// <returns>keep for others or not bool</returns>
        public bool RemoveOnDisconnect();

        /// <summary>
        /// destroy is called when someone said to remove this entity
        /// </summary>
        public void Destroy();

        /// <summary>
        /// Update function, for listener objects
        /// </summary>
        /// <param name="position"></param>
        /// <param name="data"></param>
        public void HandleUpdate(Vector2 position, string data);
    }

    /// <summary>
    /// Enum for if the user owns the object or just listens for its position.
    ///
    /// projectiles should have this set to Listener by default and have the spawner that registers them and set it to Owner
    /// </summary>
    public enum ElementType
    {
        Owner,
        Listener
    }
}