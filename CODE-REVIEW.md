# Code Review and Verification Record

## Review scope

Reviewed `CourseRegistration.Core/Services/EnrollmentService.cs` against the assessment acceptance criteria.

## Checks performed

| Check | Review question | Evidence |
|---|---|---|
| Capacity | Can concurrent enrollment requests exceed capacity? | Reviewed locking logic; concurrency test result recorded below. |
| Duplicate prevention | Can the same learner register more than once, including with different ID casing? | Inspected ID comparisons and ran duplicate-registration tests. |
| Prerequisites | Can a learner who fails prerequisites enter the course or waitlist? | Inspected validation order and ran prerequisite tests. |
| FIFO promotion | Does the first eligible learner in queue order get promoted? | Inspected promotion logic and ran the FIFO test. |
| Ineligible waitlisted learner | Is the documented skip-and-retain policy applied consistently? | Reviewed eligibility update and promotion behavior; ran the relevant test. |
| Invalid and repeated drops | Do invalid drops avoid changing enrollment state? | Ran invalid/repeated-drop tests and inspected resulting state. |

## Verification results

- Build command: `dotnet build`
- Test command: `dotnet test`
- Manual run: `dotnet run --project CourseRegistration.ManualTest`
- Final test count and result: [Fill in after the final run.]
- Manual scenarios observed: [Record the actual outcomes.]
- Issues discovered and fixes made: [Record any issues; write "None observed" only if accurate.]

## Review notes

The implementation uses in-memory collections and a process-local lock. This does not provide persistence or coordination between multiple service instances. These limitations are documented in `DESIGN.md`.
