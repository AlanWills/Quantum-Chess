using UnityEngine;
using PolyAndCode.UI;
using QC.Campaign.Events;
using QC.Campaign.Objects;
using TMPro;

namespace QC.Campaign.UI
{
    public class PickCampaignUI : MonoBehaviour, ICell
    {
        #region Properties and Fields

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI displayName;

        [Header("Events")] 
        [SerializeField] private CampaignDefinitionEvent campaignSelected;
        
        private CampaignDefinition campaignDefinition;

        #endregion

		public void Hookup(PickCampaignUIData pickCampaignUIData)
		{
			campaignDefinition = pickCampaignUIData.CampaignDefinition;

			displayName.text = campaignDefinition.DisplayName;
		}

		#region Unity Methods

		private void OnDisable()
		{
			campaignDefinition = null;
		}

		#endregion
		
		#region Callbacks

		public void OnCampaignSelected()
		{
			campaignSelected.Invoke(campaignDefinition);
		}
		
		#endregion
	}
}