namespace XamarinShot.Models;

/// <summary>
/// Abstract:
/// Manages user interactions.
/// </summary>
public class InteractionManager
{
        readonly Dictionary<int, IInteraction> interactions = new Dictionary<int, IInteraction> ();

        public void AddInteraction (IInteraction interaction)
        {
                var classIdentifier = interaction.GetType ().GetHashCode ();
                interactions [classIdentifier] = interaction;
        }

        public IInteraction? Interaction (Type interactionClass)
        {
                IInteraction? result = null;

                var classIdentifier = interactionClass.GetHashCode ();
                if (interactions.TryGetValue (classIdentifier, out IInteraction? interaction))
                {
                        result = interaction;
                }

                return result;
        }

        public void RemoveAllInteractions ()
        {
                interactions.Clear ();
        }

        public void UpdateAll (CameraInfo cameraInfo)
        {
                foreach (var interaction in interactions.Values)
                {
                        interaction.Update (cameraInfo);
                }
        }

        public void Handle (GameActionType gameAction, Player player)
        {
                foreach (var interaction in interactions.Values)
                {
                        interaction.Handle (gameAction, player);
                }
        }

        #region Touch Event Routing

        public void HandleTouch (TouchType type, Ray camera)
        {
                foreach (var interaction in interactions.Values)
                {
                        interaction.HandleTouch (type, camera);
                }
        }

        public void DidCollision (SCNNode nodeA, SCNNode nodeB, SCNVector3 position, float impulse)
        {
                foreach (var interaction in interactions.Values)
                {
                        // nodeA and nodeB take turn to be the main node
                        interaction.DidCollision (nodeA, nodeB, position, impulse);
                        interaction.DidCollision (nodeB, nodeA, position, impulse);
                }
        }

        #endregion
}
