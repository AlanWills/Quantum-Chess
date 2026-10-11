using QC.Campaign.Objects;

namespace QC.Campaign.UI
{
	public class PickCampaignUIData
	{
		#region Properties and Fields
	
		public CampaignDefinition CampaignDefinition { get; }
	
		#endregion

		public PickCampaignUIData(CampaignDefinition campaignDefinition)
		{
			CampaignDefinition = campaignDefinition;
		}
	}
}