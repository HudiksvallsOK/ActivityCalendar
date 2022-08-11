using ActivityCalendarCore.Core;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ActivityCalendar.Pages
{
	public class IndexVm : PageModel
	{
		public IndexVm(ILocalStorageService localStorageService)
		{
			this.ActivityTypes = new AvtivityTypeVmFactory().Create();
			this.LocalStorageService = localStorageService;
			this.ActivityOpacityFactor = 40;
		}

		public HashSet<AvtivityTypeVm> ActivityTypes;
		[BindProperty]
		public int DaysHorizon { get; set; }
		public ILocalStorageService LocalStorageService { get; }
		public bool SelectAll { get; set; }

		public int ActivityOpacityFactor { get; set; }
		public string ActivityOpacity => "0." + this.ActivityOpacityFactor.ToString();

		public void SelectAllOrNothing()
		{
			this.SelectAll = !this.SelectAll;

			foreach (var activityType in this.ActivityTypes)
			{
				activityType.IsIncluded = this.SelectAll;
			}
		}

		public async void SetDefaults()
		{
			foreach (var activityType in this.ActivityTypes)
			{
				await this.LocalStorageService.SetItemAsync<bool>(activityType.ActivityType.ToString(), activityType.IsIncluded);
			}
			await this.LocalStorageService.SetItemAsync<int>(nameof(this.DaysHorizon), this.DaysHorizon);
			await this.LocalStorageService.SetItemAsync<int>(nameof(this.ActivityOpacityFactor), this.ActivityOpacityFactor);
			await this.LocalStorageService.SetItemAsync<bool>("ActivityTypesStored", true);
		}

		public async Task LoadDefaults()
		{
			if(this.LocalStorageService == null)
			{
				return;
			}

			var activityTypesStored = await this.LocalStorageService.GetItemAsync<bool>("ActivityTypesStored");
			if (activityTypesStored)
			{
				foreach (var activityType in this.ActivityTypes)
				{
					var valueTask = await this.LocalStorageService.GetItemAsync<bool>(activityType.ActivityType.ToString());

					activityType.IsIncluded = valueTask;
				}

				this.DaysHorizon = await this.LocalStorageService.GetItemAsync<int>(nameof(this.DaysHorizon));
				this.ActivityOpacityFactor = await this.LocalStorageService.GetItemAsync<int>(nameof(this.ActivityOpacityFactor));
			}
		}
	}
}
