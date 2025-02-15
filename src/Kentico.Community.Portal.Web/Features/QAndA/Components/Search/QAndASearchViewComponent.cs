using Kentico.Community.Portal.Web.Components.ViewComponents.Pagination;
using Kentico.Community.Portal.Web.Infrastructure.Search;
using Microsoft.AspNetCore.Mvc;

namespace Kentico.Community.Portal.Web.Features.QAndA;

public class QAndASearchViewComponent(SearchService searchService) : ViewComponent
{
    private readonly SearchService searchService = searchService;

    public IViewComponentResult Invoke()
    {
        var request = new QAndASearchRequest(HttpContext.Request);

        var searchResult = searchService.SearchQAndA(request);

        var vm = new QAndASearchViewModel();

        var hits = searchResult.Hits.ToList();
        var ids = hits.Select(x => x.ID).ToList();

        var viewModels = hits.Select(QAndAPostViewModel.GetModel).ToList();

        var viewModelsSorted = new List<QAndAPostViewModel>();

        foreach (var post in hits)
        {
            var model = viewModels.Single(x => x.ID == post.ID);
            model.LinkPath = post.Url;

            viewModelsSorted.Add(model);
        }

        vm.Questions = viewModelsSorted;
        vm.Page = request.PageNumber;
        vm.SortBy = request.SortBy;
        vm.Query = request.SearchText;
        vm.TotalPages = searchResult.TotalPages;
        vm.OnlyAcceptedResponses = request.OnlyAcceptedResponses;

        return View("~/Features/QAndA/Components/Search/QAndASearch.cshtml", vm);
    }
}

public class QAndASearchViewModel : IPagedViewModel
{
    public IReadOnlyList<QAndAPostViewModel> Questions { get; set; } = [];

    public string? Query { get; set; } = "";
    [HiddenInput]
    public int Page { get; set; }
    public string SortBy { get; set; } = "";
    public bool OnlyAcceptedResponses { get; set; } = false;
    public int TotalPages { get; set; }

    public Dictionary<string, string?> GetRouteData(int page) =>
        new()
        {
            { "query", Query },
            { "page", page.ToString() },
            { "sortBy", SortBy }
        };
}

public class QAndAPostViewModel
{
    public int ID { get; set; }
    public string Title { get; set; } = "";
    public string LinkPath { get; set; } = "";
    public DateTime DateCreated { get; set; }
    public int ResponseCount { get; set; }
    public DateTime LatestResponseDate { get; set; }
    public bool HasAcceptedResponse { get; set; }
    public QAndAPostAuthorViewModel Author { get; set; } = new();

    public static QAndAPostViewModel GetModel(QAndASearchModel result) => new()
    {
        Title = result.Title,
        DateCreated = result.PublishedDate,
        ResponseCount = result.ResponseCount,
        LatestResponseDate = result.LatestResponseDate,
        HasAcceptedResponse = result.HasAcceptedResponse,
        Author = new()
        {
            FullName = result.AuthorFullName,
            MemberID = result.AuthorMemberID,
            Username = result.AuthorUsername
        },
        LinkPath = result.Url,
        ID = result.ID
    };
}

public class QAndAPostAuthorViewModel
{
    public int MemberID { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
}

