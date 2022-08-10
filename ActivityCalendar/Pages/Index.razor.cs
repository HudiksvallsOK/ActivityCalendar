﻿using ActivityCalendar.Core;
using ActivityCalendar.Data;
using ActivityCalendar.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web.Helpers;

namespace ActivityCalendar.Pages
{
	public partial class Index
	{
		public IndexVm? Vm { get; set; }

		public Index()
		{
		}

		private ILookup<string, Activity>? yearMonthActivities;

		protected override async Task OnInitializedAsync()
		{
			this.Vm = new IndexVm(this.LocalStorageService);
		}

		protected override void OnInitialized()
		{
		}

		protected override Task OnAfterRenderAsync(bool firstRender)
		{
			var task = base.OnAfterRenderAsync(firstRender);

			if(firstRender)
			{
				return this.GetDefaults();
			}

			return task;
		}

		public void SelectAllOrNothing()
		{
			this.Vm?.SelectAllOrNothing();

			this.StateHasChanged();
		}

		public async Task GetDefaults()
		{
			if(this.Vm != null)
			{
				await this.Vm.LoadDefaults();

				await this.Filter();
			}
		}


		private bool filtering;
		public async Task Filter()
		{
			if (this.Vm != null)
			{
				this.filtering = true;

				await Task.Delay(1);

				this.yearMonthActivities = await this.ActivityService.GetActivitiesAsync(this.Vm.ActivityTypes.Where(x => x.IsIncluded).Select(x => x.ActivityType)
					, this.Vm.DaysHorizon
					);

				this.filtering = false;

				this.StateHasChanged();
			}
		}

		public string LoaderVisibiblity => this.filtering
			? "visible"
			: "hidden";
	}
}