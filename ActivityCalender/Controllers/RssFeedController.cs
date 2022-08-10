using ActivityCalender.Data;
using ActivityCalender.Pages;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ActivityCalender.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RssFeedController : ControllerBase
	{
		private ActivityService ActivityService { get; }
		public ILocalStorageService LocalStorageService { get; }
		public IndexVm Vm { get; }

		public RssFeedController(Blazored.LocalStorage.ILocalStorageService localStorageService)
		{
			this.ActivityService = new ActivityService();
			this.LocalStorageService = localStorageService;
            this.Vm = new IndexVm(this.LocalStorageService);
        }

        // GET: api/<RssFeedController>
        [HttpGet(Name = "RSSFeed")]
		public string Get()
		{
            var feed = new SyndicationFeed("Title", "Description", new Uri(HttpContext.Request.Host.ToUriComponent()), "HOKRssFeed1", DateTime.Now);
            
            feed.Copyright = new TextSyndicationContent($"{DateTime.Now.Year} Bantarn");

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = false,
                Indent = true,
            };

            var yearMonthActivities = this.ActivityService.GetActivitiesAsync(this.Vm.ActivityTypes.Where(x => x.IsIncluded).Select(x => x.ActivityType)
                , this.Vm.DaysHorizon
            );

            var items = new List<SyndicationItem>();

            foreach (var item in yearMonthActivities.Result.SelectMany(x => x))
            {
                items.Add(new SyndicationItem(item.Summary, item.Description, item.Uri, item.Id, item.CalendarDateTime));
            }

            feed.Items = items;

            var sw = new StringWriter();

            using (var writer = XmlWriter.Create(sw, settings))
            {
                var rssFormator = new Rss20FeedFormatter(feed, false);
                rssFormator.WriteTo(writer);
                writer.Flush();
            }

            return sw.ToString();
        }
    }
}
