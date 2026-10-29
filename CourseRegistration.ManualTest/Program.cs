
using CourseRegistration.Core.Services;

var service = new EnrollmentService(capacity: 2);

void Show(string scenario, EnrollmentResult result)
{
    Console.WriteLine($"{scenario}: {result.Status} - {result.Message}");

    if (result.PromotedLearnerId is not null)
        Console.WriteLine($"Promoted learner: {result.PromotedLearnerId}");

    Console.WriteLine(
        $"Enrolled: [{string.Join(", ", service.GetEnrolledLearners())}]");

    Console.WriteLine(
        $"Waitlist: [{string.Join(", ", service.GetWaitlistedLearners())}]");

    Console.WriteLine(new string('-', 60));
}

Console.WriteLine("COURSE REGISTRATION MANUAL TEST");
Console.WriteLine();

Show("Alice enrolls",
    service.Enroll("Alice", prerequisitesMet: true));

Show("Bob enrolls",
    service.Enroll("Bob", prerequisitesMet: true));

Show("Charlie joins full-course waitlist",
    service.Enroll("Charlie", prerequisitesMet: true));

Show("Duplicate Charlie registration",
    service.Enroll("Charlie", prerequisitesMet: true));

Show("Learner without prerequisites",
    service.Enroll("David", prerequisitesMet: false));

Show("Drop Alice - Charlie should be promoted",
    service.Drop("Alice"));

Show("Drop Alice again - should be rejected",
    service.Drop("Alice"));
