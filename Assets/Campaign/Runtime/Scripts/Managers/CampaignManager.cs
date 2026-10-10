using UnityEngine;
using Celeste.Persistence;
using QC.Campaign.Catalogue;
using QC.Campaign.Persistence;
using QC.Campaign.Record;

namespace QC.Campaign.Managers
{
    [AddComponentMenu("QC/Campaign/Campaign Manager")]
    public class CampaignManager : PersistentSceneManager<CampaignManager, CampaignManagerDTO>
    {
		#region Properties and Fields
	
        public const string FILE_NAME = "CampaignManager.dat";
		
        protected override string FileName => FILE_NAME;

        [SerializeField] private CampaignRecord campaignRecord;
        [SerializeField] private CampaignDefinitionCatalogue campaignDefinitionCatalogue;

		#endregion
		
		#region Save/Load

        protected override CampaignManagerDTO Serialize()
        {
            return new CampaignManagerDTO(campaignRecord);
        }

        protected override void Deserialize(CampaignManagerDTO dto)
        {
        }

        protected override void SetDefaultValues()
        {
	        campaignRecord.Initialize();
        }
		
		#endregion
    }
}