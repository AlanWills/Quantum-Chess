using System;
using System.Collections.Generic;
using Celeste.Tools;
using PolyAndCode.UI;
using QC.Campaign.Catalogue;
using QC.Campaign.Objects;
using UnityEngine;

namespace QC.Campaign.UI
{
    public class PickCampaignsUIController : MonoBehaviour, IRecyclableScrollRectDataSource
	{
		#region Properties and Fields

		[SerializeField] private RecyclableScrollRect scrollRect;
		[SerializeField] private CampaignDefinitionCatalogue campaignDefinitionCatalogue;

		[NonSerialized] private readonly List<PickCampaignUIData> pickCampaignCellData = new();

		#endregion

		#region Unity Methods

		private void OnValidate()
		{
			this.TryGetInChildren(ref scrollRect);
		}

		private void Start()
		{
			SetUpUI();
		}

		#endregion

		private void SetUpUI()
		{
			pickCampaignCellData.Clear();

			for (int i = 0, n = campaignDefinitionCatalogue.NumItems; i < n; ++i)
			{
				CampaignDefinition campaign = campaignDefinitionCatalogue.GetItem(i);
				pickCampaignCellData.Add(new PickCampaignUIData(campaign));
			}

			scrollRect.Initialize(this);
		}

		#region IRecyclableScrollRectDataSource

		public int GetItemCount()
		{
			return pickCampaignCellData.Count;
		}

		public void SetCell(ICell cell, int index)
		{
			(cell as PickCampaignUI).Hookup(pickCampaignCellData[index]);
		}

		#endregion
	}
}