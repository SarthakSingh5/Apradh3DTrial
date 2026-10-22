using UnityEngine;

public enum Team
{
    TeamAlpha, // The Player and friendly AI
    TeamBeta, // The Enemy AI
}

public class TeamIdentifier : MonoBehaviour
{
    public Team team;
}