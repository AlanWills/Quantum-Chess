using UnityEngine;
using Celeste.Objects;
using QC.Match.Objects;

namespace QC.Match.Catalogue
{
    [CreateAssetMenu(fileName = nameof(MatchScenarioCatalogue), menuName = "QC/Match/Match Scenario Catalogue")]
    public class MatchScenarioCatalogue : ListScriptableObject<MatchScenario>
    {
    }
}