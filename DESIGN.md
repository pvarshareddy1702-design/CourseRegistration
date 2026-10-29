# Design Specification

## 1. Service boundary

`EnrollmentService` manages enrollment for one course offering. It owns the offering capacity, enrolled learner IDs, and ordered waitlist. It exposes operations to enroll, drop, update the prerequisite status of a waitlisted learner, and inspect current state.

The service does not implement a frontend, persistence layer, identity system, or course catalog.

## 2. Data and API design

- `Enroll(learnerId, prerequisitesMet)` attempts enrollment and returns an `EnrollmentResult`.
- `Drop(learnerId)` removes an enrolled learner and attempts waitlist promotion.
- `UpdateWaitlistedPrerequisites(learnerId, prerequisitesMet)` updates eligibility for a learner already on the waitlist.
- `GetEnrolledLearners()` returns a snapshot of enrolled learner IDs.
- `GetWaitlistedLearners()` returns a snapshot in waitlist order.

`EnrollmentResult` contains a status, human-readable message, and optional promoted learner ID. Status values distinguish successful enrollment, waitlisting, duplicate registration, unmet prerequisites, invalid requests, unsuccessful drops, and drops that promote another learner.

## 3. Business rules and assumptions

1. Capacity must be greater than zero.
2. Learner IDs must not be null, empty, or whitespace. IDs are trimmed and compared case-insensitively.
3. A learner may be enrolled or waitlisted only once.
4. Prerequisites are checked before either enrollment or waitlisting. A learner who does not meet them is rejected.
5. When capacity is available, an eligible learner is enrolled immediately.
6. When capacity is full, an eligible learner joins the end of the waitlist.
7. On a drop, the service scans the waitlist from the beginning and promotes the earliest learner whose prerequisites are currently satisfied.
8. An ineligible learner is skipped but remains in their original waitlist position. If no eligible learner exists, the seat remains vacant.
9. Repeated drops and drops for learners who are not enrolled return `NotEnrolled` and do not change state.

## 4. Invariants

- The enrolled count never exceeds capacity.
- A learner cannot be both enrolled and waitlisted.
- Duplicate learner IDs are prevented across both collections.
- Waitlist order is preserved except when an eligible learner is promoted or a learner is otherwise removed.
- A promotion occurs only after a seat is released and only for an eligible learner.

## 5. Concurrency

A private lock protects enrollment, dropping, prerequisite updates, promotion, and state snapshots. This makes each operation atomic within a single service instance and prevents concurrent enrollment requests from exceeding capacity.

This synchronization is process-local. It does not coordinate multiple service instances or provide cross-process transactions.

## 6. Persistence and tradeoffs

The prototype uses in-memory collections to keep the implementation focused and easy to run. Data is lost when the process exits. A production implementation would need durable storage, transactional updates, and concurrency control at the persistence layer.

## 7. Limitations and future improvements

- Supports only one offering per service instance.
- No database, web API, authentication, or authorization.
- Prerequisite status is supplied by the caller rather than retrieved from an authoritative system.
- A waitlisted learner whose eligibility changes to satisfied is not promoted until a later drop triggers promotion.
- No cancellation or removal operation for waitlisted learners.
- A production service would need persistence, observability, API validation, and stronger integration tests.
