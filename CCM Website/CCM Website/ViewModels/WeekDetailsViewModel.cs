using CCM_Website.Models;

namespace CCM_Website.ViewModels;

public class WeekDetailsViewModel
{
    public Week? Week { get; set; }
    public IEnumerable<WeekActivities>? ActivitiesList { get; set; }

    public IEnumerable<TaskProgressStatus>? StatusList { get; set; }

    public IEnumerable<TaskLocation>? LocationList { get; set; }

    public IEnumerable<TaskApproach>? ApproachList { get; set; }
}
