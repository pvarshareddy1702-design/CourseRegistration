
using System.Linq;
using System.Threading.Tasks;
using CourseRegistration.Core.Services;
using Xunit;

namespace CourseRegistration.Tests;

public class EnrollmentServiceTests
{
    [Fact]
    public void Enroll_WhenSeatAvailable_EnrollsLearner()
    {
        var service = new EnrollmentService(capacity: 2);

        var result = service.Enroll("Alice", prerequisitesMet: true);

        Assert.Equal(EnrollmentStatus.Enrolled, result.Status);
        Assert.Contains("Alice", service.GetEnrolledLearners());
    }

    [Fact]
    public void Enroll_WhenFull_AddsEligibleLearnerToWaitlist()
    {
        var service = new EnrollmentService(capacity: 1);
        service.Enroll("Alice", true);

        var result = service.Enroll("Bob", true);

        Assert.Equal(EnrollmentStatus.Waitlisted, result.Status);
        Assert.Equal(new[] { "Bob" }, service.GetWaitlistedLearners());
    }

    [Fact]
    public void Enroll_WhenLearnerAlreadyRegistered_RejectsDuplicate()
    {
        var service = new EnrollmentService(capacity: 1);
        service.Enroll("Alice", true);
        service.Enroll("Bob", true);

        var result = service.Enroll("bob", true);

        Assert.Equal(EnrollmentStatus.AlreadyRegistered, result.Status);
        Assert.Single(service.GetWaitlistedLearners());
    }

    [Fact]
    public void Drop_WhenSeatOpens_PromotesEarliestEligibleLearner()
    {
        var service = new EnrollmentService(capacity: 1);
        service.Enroll("Alice", true);
        service.Enroll("Bob", true);
        service.Enroll("Charlie", true);

        var result = service.Drop("Alice");

        Assert.Equal(EnrollmentStatus.DroppedAndPromoted, result.Status);
        Assert.Equal("Bob", result.PromotedLearnerId);
        Assert.Contains("Bob", service.GetEnrolledLearners());
        Assert.Equal(new[] { "Charlie" }, service.GetWaitlistedLearners());
    }

    [Fact]
    public void Enroll_WhenPrerequisitesNotMet_RejectsLearner()
    {
        var service = new EnrollmentService(capacity: 2);

        var result = service.Enroll("Alice", prerequisitesMet: false);

        Assert.Equal(EnrollmentStatus.PrerequisiteNotMet, result.Status);
        Assert.Empty(service.GetEnrolledLearners());
        Assert.Empty(service.GetWaitlistedLearners());
    }

    [Fact]
    public void Drop_WhenLearnerIsNotEnrolled_ReturnsClearResult()
    {
        var service = new EnrollmentService(capacity: 1);

        var result = service.Drop("Unknown");

        Assert.Equal(EnrollmentStatus.NotEnrolled, result.Status);
        Assert.Empty(service.GetEnrolledLearners());
    }

    [Fact]
    public void Drop_WhenRepeated_DoesNotPromoteTwice()
    {
        var service = new EnrollmentService(capacity: 1);
        service.Enroll("Alice", true);
        service.Enroll("Bob", true);

        var firstDrop = service.Drop("Alice");
        var secondDrop = service.Drop("Alice");

        Assert.Equal(
            EnrollmentStatus.DroppedAndPromoted, firstDrop.Status);
        Assert.Equal("Bob", firstDrop.PromotedLearnerId);
        Assert.Equal(EnrollmentStatus.NotEnrolled, secondDrop.Status);
        Assert.Contains("Bob", service.GetEnrolledLearners());
        Assert.Single(service.GetEnrolledLearners());
    }

    [Fact]
    public void Drop_SkipsIneligibleLearner_ButKeepsThemWaiting()
    {
        var service = new EnrollmentService(capacity: 1);
        service.Enroll("Alice", true);
        service.Enroll("Bob", true);
        service.Enroll("Charlie", true);

        service.UpdateWaitlistedPrerequisites("Bob", false);

        var result = service.Drop("Alice");

        Assert.Equal("Charlie", result.PromotedLearnerId);
        Assert.Contains("Bob", service.GetWaitlistedLearners());
        Assert.Contains("Charlie", service.GetEnrolledLearners());
    }

    [Fact]
    public void ConcurrentEnrollments_NeverExceedCapacity()
    {
        const int capacity = 5;
        const int learnerCount = 100;
        var service = new EnrollmentService(capacity);

        Parallel.For(0, learnerCount, i =>
        {
            service.Enroll($"Learner{i}", prerequisitesMet: true);
        });

        Assert.Equal(capacity, service.GetEnrolledLearners().Count);
        Assert.Equal(
            learnerCount - capacity,
            service.GetWaitlistedLearners().Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Enroll_WhenLearnerIdIsInvalid_ReturnsInvalidRequest(
        string? learnerId)
    {
        var service = new EnrollmentService(capacity: 1);

        var result = service.Enroll(learnerId, prerequisitesMet: true);

        Assert.Equal(EnrollmentStatus.InvalidRequest, result.Status);
        Assert.Empty(service.GetEnrolledLearners());
    }

    [Fact]
    public void Constructor_WhenCapacityIsInvalid_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new EnrollmentService(capacity: 0));
    }
}
