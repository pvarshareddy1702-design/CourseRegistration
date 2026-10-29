
namespace CourseRegistration.Core.Services;

public enum EnrollmentStatus
{
    Enrolled,
    Waitlisted,
    AlreadyRegistered,
    PrerequisiteNotMet,
    InvalidRequest,
    NotEnrolled,
    Dropped,
    DroppedAndPromoted
}

public sealed record EnrollmentResult(
    EnrollmentStatus Status,
    string Message,
    string? PromotedLearnerId = null);

public sealed class EnrollmentService
{
    private readonly object _gate = new();
    private readonly int _capacity;
    private readonly HashSet<string> _enrolled =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly List<WaitlistEntry> _waitlist = new();

    private sealed class WaitlistEntry
    {
        public string LearnerId { get; }
        public bool HasPrerequisites { get; set; }

        public WaitlistEntry(string learnerId, bool hasPrerequisites)
        {
            LearnerId = learnerId;
            HasPrerequisites = hasPrerequisites;
        }
    }

    public EnrollmentService(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(capacity), "Capacity must be greater than zero.");

        _capacity = capacity;
    }

    public EnrollmentResult Enroll(
        string? learnerId, bool prerequisitesMet)
    {
        if (string.IsNullOrWhiteSpace(learnerId))
        {
            return new(
                EnrollmentStatus.InvalidRequest,
                "Learner ID is required.");
        }

        learnerId = learnerId.Trim();

        lock (_gate)
        {
            if (_enrolled.Contains(learnerId) ||
                _waitlist.Any(x =>
                    x.LearnerId.Equals(
                        learnerId, StringComparison.OrdinalIgnoreCase)))
            {
                return new(
                    EnrollmentStatus.AlreadyRegistered,
                    "Learner is already enrolled or waitlisted.");
            }

            if (!prerequisitesMet)
            {
                return new(
                    EnrollmentStatus.PrerequisiteNotMet,
                    "Required prerequisites have not been met.");
            }

            if (_enrolled.Count < _capacity)
            {
                _enrolled.Add(learnerId);
                return new(
                    EnrollmentStatus.Enrolled,
                    "Enrollment successful.");
            }

            _waitlist.Add(new WaitlistEntry(learnerId, true));

            return new(
                EnrollmentStatus.Waitlisted,
                "Course is full. Learner added to the waitlist.");
        }
    }

    public EnrollmentResult Drop(string? learnerId)
    {
        if (string.IsNullOrWhiteSpace(learnerId))
        {
            return new(
                EnrollmentStatus.InvalidRequest,
                "Learner ID is required.");
        }

        learnerId = learnerId.Trim();

        lock (_gate)
        {
            if (!_enrolled.Remove(learnerId))
            {
                return new(
                    EnrollmentStatus.NotEnrolled,
                    "Learner is not currently enrolled.");
            }

            string? promotedLearnerId = PromoteEarliestEligible();

            if (promotedLearnerId is not null)
            {
                return new(
                    EnrollmentStatus.DroppedAndPromoted,
                    $"Learner dropped. {promotedLearnerId} was promoted.",
                    promotedLearnerId);
            }

            return new(
                EnrollmentStatus.Dropped,
                "Learner dropped successfully. No eligible learner was promoted.");
        }
    }

    // Models a prerequisite-status change after a learner joins the waitlist.
    public bool UpdateWaitlistedPrerequisites(
        string? learnerId, bool prerequisitesMet)
    {
        if (string.IsNullOrWhiteSpace(learnerId))
            return false;

        lock (_gate)
        {
            var entry = _waitlist.FirstOrDefault(x =>
                x.LearnerId.Equals(
                    learnerId.Trim(), StringComparison.OrdinalIgnoreCase));

            if (entry is null)
                return false;

            entry.HasPrerequisites = prerequisitesMet;
            return true;
        }
    }

    private string? PromoteEarliestEligible()
    {
        // Keep ineligible learners waiting, but skip them for promotion.
        int index = _waitlist.FindIndex(x => x.HasPrerequisites);

        if (index < 0)
            return null;

        WaitlistEntry entry = _waitlist[index];
        _waitlist.RemoveAt(index);
        _enrolled.Add(entry.LearnerId);

        return entry.LearnerId;
    }

    public IReadOnlyCollection<string> GetEnrolledLearners()
    {
        lock (_gate)
            return _enrolled.ToArray();
    }

    public IReadOnlyList<string> GetWaitlistedLearners()
    {
        lock (_gate)
            return _waitlist.Select(x => x.LearnerId).ToArray();
    }
}
