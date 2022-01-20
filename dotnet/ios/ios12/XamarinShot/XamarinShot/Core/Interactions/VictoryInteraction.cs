namespace XamarinShot.Models;

public class VictoryInteraction : IInteraction
{
        const double TimeUntilPhysicsReleased = 10d;

        const double FadeTime = 0.5d;

        readonly NSLock @lock = new NSLock (); // need thread protection because main thread is the one that called didWin()

        readonly SCNNode victoryNode;

        Team teamWon = Team.None;

        double activationStartTime;

        public VictoryInteraction (IInteractionDelegate @delegate)
        {
                Delegate = @delegate;
                victoryNode = SCNNodeExtensions.LoadSCNAsset ("victory");
        }

        public IInteractionDelegate Delegate { get; private set; }

        public bool DisplayedVictory { get; private set; }

        public bool GameDone { get; private set; }

        public void ActivateVictory ()
        {
                if (!DisplayedVictory)
                {
                        @lock.Lock ();

                        if (Delegate is null)
                        {
                                throw new Exception ("No Delegate");
                        }

                        Delegate.AddNodeToLevel (victoryNode);
                        victoryNode.WorldPosition = new SCNVector3 (0f, 15f, 0f);

                        var eulerAnglesY = teamWon == Team.TeamA ? (float)Math.PI : 0f; // Rotate Victory to face in the right direction
                        victoryNode.EulerAngles = new SCNVector3 (victoryNode.EulerAngles.X, eulerAnglesY, victoryNode.EulerAngles.Z);
                        foreach (var child in victoryNode.ChildNodes)
                        {
                                child.PhysicsBody?.ResetTransform ();
                        }

                        DisplayedVictory = true;
                        activationStartTime = GameTime.Time;

                        // Set color to that of winning team
                        victoryNode.SetPaintColors (teamWon);

                        Delegate.PlayWinSound ();

                        @lock.Unlock ();
                }
        }

        public void Update (CameraInfo cameraInfo)
        {
                if (DisplayedVictory)
                {
                        // Enlarge victory text before falling down
                        victoryNode.Opacity = (float)(DigitExtensions.Clamp ((GameTime.Time - activationStartTime) / FadeTime, 0d, 1d));

                        if (GameTime.Time - activationStartTime > TimeUntilPhysicsReleased)
                        {
                                foreach (var child in victoryNode.ChildNodes)
                                {
                                        if (child.PhysicsBody is not null)
                                        {
                                                child.PhysicsBody.VelocityFactor = SCNVector3.One;
                                                child.PhysicsBody.AngularVelocityFactor = SCNVector3.One;
                                        }
                                }
                        }
                } else {
                        // Update win condition
                        if (DidWin () && teamWon != Team.None)
                        {
                                ActivateVictory ();
                        }
                }
        }

        public void HandleTouch (TouchType type, Ray camera) { }

        private bool DidWin ()
        {
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                var catapults = Delegate.Catapults;

                var teamToCatapultCount = new int [] { 0, 0, 0 };
                foreach (var catapult in catapults.Where (catapult => !catapult.Disabled))
                {
                        teamToCatapultCount [(int)catapult.Team] += 1;
                }

                var gameDone = true;
                if (teamToCatapultCount [1] == 0 && teamToCatapultCount [2] == 0)
                {
                        teamWon = Team.None;
                } else if (teamToCatapultCount [1] == 0) {
                        teamWon = Team.TeamB;
                } else if (teamToCatapultCount [2] == 0) {
                        teamWon = Team.TeamA;
                } else {
                        gameDone = false;
                }

                return gameDone;
        }

        public void DidCollision (SCNNode node, SCNNode otherNode, SCNVector3 position, float impulse) { }

        public void Handle (GameActionType gameAction, Player player) { }
}
