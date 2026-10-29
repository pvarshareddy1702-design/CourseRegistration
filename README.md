# Course Registration and Waitlist Service

## Overview

A small C#/.NET service for managing enrollment in a single course offering. It supports capacity-limited enrollment, prerequisite validation, duplicate prevention, dropping enrollment, and FIFO waitlist promotion.

The implementation uses an in-memory store and synchronizes state-changing operations with a lock.

## Features

- Enroll learners when capacity is available.
- Add eligible learners to a waitlist when the course is full.
- Reject learners who have not met prerequisites.
- Prevent duplicate enrollment and waitlist entries.
- Drop an enrolled learner and promote the earliest eligible learner.
- Skip ineligible waitlisted learners during promotion while retaining their waitlist entries.
- Return clear results for invalid requests and repeated drops.
- Protect enrollment state against concurrent requests.

## Requirements

- .NET 10 SDK
- A terminal or Visual Studio Code

## Build and test

From the solution root, run:

```bash
dotnet restore
dotnet build
dotnet test
```

## Project structure

- `CourseRegistration.Core` — enrollment and waitlist business logic.
- `CourseRegistration.Tests` — automated unit and concurrency tests.

## Scope and limitations

This prototype supports one course offering per service instance. State is stored in memory and is lost when the process exits. It has no database, API host, authentication, frontend, or multi-offering support.

See `DESIGN.md` for business rules, invariants, and design tradeoffs. See `AI-USAGE.md` for the AI-assisted engineering activity and verification record.

## Manual testing

The `CourseRegistration.ManualTest` console project demonstrates the enrollment workflow, including successful registration, full-capacity waitlisting, duplicate registration rejection, prerequisite validation, waitlist promotion after a drop, and repeated-drop handling.

Run the manual scenarios from the solution root:

```bash
dotnet run --project CourseRegistration.ManualTest
```

The console prints each operation's result, enrolled learners, and waitlist state so the behavior can be inspected directly.